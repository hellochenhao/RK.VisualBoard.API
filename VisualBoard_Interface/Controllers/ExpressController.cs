using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;
using VisualBoard.Models;
using VisualBoard.Models.Request;
using VisualBoard_Interface.Common;
using static VisualBoard.Models.MoQiuLiObject;
using VisualBoard.Models.Response;
using VisualBoard.Business.Service.PUB;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System.Web;
using System.Threading;
using System.Text;

namespace VisualBoard_Interface.Controllers
{
    /// <summary>
    /// 猪猪快递云控制器
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [NoLogin]
    public class ExpressController : ControllerBase
    {
        private readonly IExpressrBL expressrBL;

        public ExpressController(IExpressrBL expressrBL)
        {
            this.expressrBL = expressrBL;
        }

        /// <summary>
        /// 墨丘利批量轨迹订阅后，接受墨丘利推送消息的回调接口
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<SubscribeCallBack2Service> MoQiuLliSubscribeCallBack(MoQiuLiObject.SubscribeCallBackResult baseResult)
        {
            return await this.expressrBL.MoQiuLliSubscribeCallBack(baseResult);
        }

        [HttpPost]
        public async Task<ResponseObject> MoQiuLliSubscribeFormatXiangXiWuLiu()
        {
            return await this.expressrBL.MoQiuLliSubscribeFormatXiangXiWuLiu();
        }

        [HttpPost]
        public async Task<ResponseObject> ReWriteMoQiuLiTrace()
        {
            return await this.expressrBL.ReWriteMoQiuLiTrace();
        }

        #region 报表相关
        /// <summary>
        /// 每日运作报表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObjectV2 RptRunningDaily(SelectRouteObject query)
        {
            return expressrBL.RptRunningDaily(query);
        }
        /// <summary>
        /// 导出每日运作报表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public string RptRunningDailyExport(SelectRouteObject query)
        {
            return expressrBL.RptRunningDailyExport(query);
        }

        /// <summary>
        /// 2C超时订单预警报表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObjectV2 RptOrder2CTimeOut(SelectRouteObject query)
        {
            return expressrBL.RptOrder2CTimeOut(query);
        }
        /// <summary>
        /// 2C超时订单预警报表-各个Tab状态下的订单数量
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject RptOrder2CTimeOutTabsCount(SelectRouteObject query)
        {
            return expressrBL.RptOrder2CTimeOutTabsCount(query);
        }
        /// <summary>
        /// 导出2C超时订单预警报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public string RptOrder2CTimeOutExport(SelectRouteObject query)
        {
            return expressrBL.RptOrder2CTimeOutExport(query);
        }

        /// <summary>
        /// 2C预警时效维护
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObjectV2 RptOrder2CTimeOutConfig(SelectRouteObject query)
        {
            return expressrBL.RptOrder2CTimeOutConfig(query);
        }
        /// <summary>
        /// 导出2C预警时效维护
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RptOrder2CTimeOutConfigExport(SelectRouteObject query)
        {
            try
            {
                string FileName = "2C预警时效维护";

                var s = "2C预警时效维护";

                List<TwoCTimeOutConfigForExport> dataList = expressrBL.RptOrder2CTimeOutConfigExport(query);

                //定义表头
                var heads = new List<string>() { "超时类型", "货主名称", "省份", "超时时长" };

                var excelFilePath = ExcelHelper.CreateExcelFromList(dataList, heads, FileName);

                return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", @$"{s}{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public class FileUploadAPI
        {
            public IFormFile file { get; set; }
            public string UserID { get; set; }
            public string UserName { get; set; }
        }

        /// <summary>
        /// 上传并导入2C预警时效维护数据(Excel文件格式)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject Upload2CTimeOutConfig([FromForm] FileUploadAPI objFile)
        {
            ResponseObject res = new ResponseObject();
            try
            {
                // 判断是否有上传的文件
                if (objFile.file.Length <= 0)
                {
                    res.code = 1;
                    res.message = "请先选择要上传的文件！";
                    return res;
                }

                #region 将上传的所有的excel文件保存在一个临时文件夹中
                //保存文件到本地
                using (FileStream fileStream = System.IO.File.Create(objFile.file.FileName))
                {
                    objFile.file.CopyTo(fileStream);
                    fileStream.Flush();
                }

                //读取存储的临时文件并保存入库
                Thread t = new Thread(this.Import2CTimeOutConfig);
                t.Start(objFile);
                res = new ResponseObject()
                {
                    code = 0,
                    message = "导入成功"
                };
            }
            catch (Exception ex)
            {
                res = new ResponseObject()
                {
                    code = 1,
                    message = "导入失败"
                };
            }
            #endregion

            return res;
        }

        /// <summary>
        /// 读取2C预警时效维护数据Excel文件并保存数据
        /// </summary>
        /// <param name="filePath"></param>
        private void Import2CTimeOutConfig(object objFile)
        {
            string filePath = string.Empty;

            try
            {
                FileUploadAPI _objFile = objFile as FileUploadAPI;
                filePath = _objFile.file.FileName;

                int userID = 0;
                int.TryParse(_objFile.UserID, out userID);
                string userName = _objFile.UserName;

                XSSFWorkbook workbook;
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    workbook = new XSSFWorkbook(file);
                    file.Close();
                }

                var sheet = workbook.GetSheetAt(0);
                List<TwoCTimeOutConfig> newList = new List<TwoCTimeOutConfig>();

                //获取模板的内容区域
                int rowCount = sheet.LastRowNum;
                TwoCTimeOutConfig model = new TwoCTimeOutConfig();
                //从excel模板的第2行开始读取
                for (int i = 1; i <= rowCount; i++)
                {
                    //获取模板的第三列数据 作为部件名称
                    ICell cell = sheet.GetRow(i).GetCell(0);
                    if (cell == null || string.IsNullOrEmpty(cell.StringCellValue))
                    {
                        break;
                    }

                    string title = null;
                    string companyName = null;
                    string privince = null;
                    double TimeSpan = 0;

                    //超时类型
                    cell = sheet.GetRow(i).GetCell(0);
                    if (cell != null && !string.IsNullOrWhiteSpace(cell.StringCellValue))
                        title = cell.StringCellValue;
                    //货主名称
                    cell = sheet.GetRow(i).GetCell(1);
                    if (cell != null && !string.IsNullOrWhiteSpace(cell.StringCellValue))
                        companyName = cell.StringCellValue;
                    //省份
                    cell = sheet.GetRow(i).GetCell(2);
                    if (cell != null && !string.IsNullOrWhiteSpace(cell.StringCellValue))
                        privince = cell.StringCellValue;
                    //超时时长
                    cell = sheet.GetRow(i).GetCell(3);
                    if (cell != null && cell.NumericCellValue>=0)
                        Double.TryParse(cell.NumericCellValue.ToString(), out TimeSpan);

                    //将读取的模板数据组装成部件信息
                    model = new TwoCTimeOutConfig()
                    {
                        Title = title,
                        CompanyName = companyName,
                        Privince = privince,
                        TimeSpan = (int)TimeSpan,
                        OperateUserID = userID,
                        OperateUserName = userName
                    };
                    newList.Add(model);
                }

                if (newList != null && newList.Count() > 0)
                {
                    int num = expressrBL.SaveTimeOutConfig(newList);
                    if (num < 0)
                    {
                        throw new Exception("保存数据时发生异常错误");
                    }

                    //删除本地文件
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
