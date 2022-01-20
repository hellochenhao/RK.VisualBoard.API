using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class bi_wmsTableObject
    {
        /// <summary>
        ///  中文名
        /// </summary>
        public string target_cn { get; set; }

      /// <summary>
      /// 内容
      /// </summary>
        public bi_wmsTableDataObject[] table  { get; set; }
    
    }

    public class bi_wmsTableDataObject
    {
        public string WarehouseName { get; set; }

        public string OrderType { get; set; }
        /// <summary>
        /// 率值
        /// </summary>
        public decimal? Percent { get; set; }

        public int? OrderCount { get; set; }
        public int? WaitDistribution { get; set; }
        public int? WavePicking { get; set; }

        public int? WaitPick { get; set; }
        public int? EndPick { get; set; }
        public int? WaitPackage { get; set; }

        public int? EndReview { get; set; }

        public int? EndPresell { get; set; }

        public int? OrderWeight { get; set; }

        public int? Shipping { get; set; }

        public int? Cancel { get; set; }
        public int? Exception { get; set; }

         public int? DataFrom { get; set; }

        public DateTime? OperateTime { get; set; }

        /// <summary>
        /// 快递揽收条数
        /// </summary>
        public int? Collect { get; set; }

    }

    public class bi_Wms_V2: bi_Wms
    {
        /// <summary>
        ///  快递揽收数量
        /// </summary>
        public int? Collect { get; set; }

    }
}
