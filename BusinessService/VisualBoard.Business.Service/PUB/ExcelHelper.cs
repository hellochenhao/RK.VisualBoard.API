using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VisualBoard.Business.Service.PUB
{
    public class ExcelHelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">数据</param>
        /// <param name="headers">表头</param>
        /// <returns></returns>
        public static string CreateExcelFromList<T>(List<T> dataList, List<string> headers, string FileName)
        {
            //指定EPPlus使用非商业证书
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string sWebRootFolder = Path.Combine("../Excel");

            //Directory.Delete("../Excel", true);

            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }
            string sFileName = $@"{FileName}{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(@$"{FileName}");
                worksheet.Cells.LoadFromCollection(dataList, true);
                //表头字段
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                worksheet.Column(3).Style.Numberformat.Format = "yyyy-MM-dd";

                package.Save();
            }
            return path;
        }
    }
}
