using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;
using VisualBoard.Business.Service.PUB;
using VisualBoard.Models.Request;
using VisualBoard_Interface.Common;
namespace VisualBoard_Interface.Controllers
{
    /// <summary>
    /// B2C报表
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class B2CStatementController : ControllerBase
    {
        private readonly IB2CStatementBL B2CStatementBL;

        public B2CStatementController(IB2CStatementBL B2CStatementBL)
        {
            this.B2CStatementBL = B2CStatementBL;
        }


        /// <summary>
        /// 查询B2C报表数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject ShowB2CStatement(B2CStatementObject obj)
        {
            return B2CStatementBL.ShowB2CStatement(obj);
        }



        /// <summary>
        /// 下拉列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject SelectList(string UserID)
        {
            return B2CStatementBL.SelectList(UserID);
        }


        /// <summary>
        /// 项目模糊搜索
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject SelectProject(B2CStatementObject obj)
        {
            return B2CStatementBL.SelectProject(obj);
        }


        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public IActionResult DCComplaintInfo(B2CStatementObject obj)
        {
            try
            {
                string FileName = "B2C报表数据";

                var s = "B2C报表数据";

                var personList = B2CStatementBL.GetList(obj);

                //定义表头

                var heads = new List<string>() { "分公司", "项目","日期","入库单数(单)", "入库件数(件)", "可销售库存数量(件)", "可销售库存SKU数量",
                "残品区库存数量（件）", "残品区库存SKU数量", "存储位库位总量","存储位库位使用量", "库位利用率", "订单接收量（单）", "订单完成量（单）",
                "订单完成件数（件）","单均SKU数", "件单比", "订单完成率", "及时揽收量（单）", "实际发运单量（单）","实际发运SKU数（行）","实际发运件数（件）","揽收及时率",
                 "中通发运单量（单）", "申通发运单量（单）", "圆通发运单量（单）", "韵达发运单量（单）", "邮政发运单量（单）", "顺丰发运单量（单）",
                "活动单订单量（单）", "活动单件数（件）", "虚拟组套订单量（单）", "虚拟组套件数（件）", "MPS操作订单量（单）",
                "MPS操作件数（件）", "边拣边分操作订单量（单）", "边拣边分操作件数（件）", "取消单（单）", "退货实际操单量", "退货实际操件数",
                "自有员工数量", "自有员工出勤总工时", "劳务员工数量", "劳务员工出勤总工时", "支援员工数量", "支援员工出勤总工时","人效","时效"  };

                var excelFilePath = ExcelHelper.CreateExcelFromList(personList, heads, FileName);

                return File(new FileStream(excelFilePath, FileMode.Open), "application/octet-stream", @$"{s}{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
