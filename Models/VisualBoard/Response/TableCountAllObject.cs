using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class TableCountAllObject
    {
        /// <summary>
        ///  中文名
        /// </summary>
        public string target_cn { get; set; }

      /// <summary>
      /// 内容
      /// </summary>
        public TableCountAllDataObject[] table  { get; set; }
    
    }

    public class TableCountAllDataObject
    {
        /// <summary>
        /// 仓库数量
        /// </summary>
        public int? warehousenameNumber { get; set; }


        /// <summary>
        /// 项目数量
        /// </summary>
        public int? ShipperNameNumber { get; set; }

        /// <summary>
        /// 总单量
        /// </summary>
        public int? OrderCount { get; set; }

        /// <summary>
        /// 已发运
        /// </summary>
        public int? Shipping { get; set; }

        /// <summary>
        /// 率值
        /// </summary>
        public decimal? Percent { get; set; }
    }
}
