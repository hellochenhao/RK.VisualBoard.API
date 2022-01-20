using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    /// <summary>
    /// 带行数和页数的ResponseObject
    /// </summary>
    public class ResponseObjectV2 : Rokin.Shared.Model.ResponseObject
    {
        /// <summary>
        ///  总记录条数
        /// </summary>
        public long TotalRows { get; set; }
        /// <summary>
        ///  总页数
        /// </summary>
        public int TotalPages { get; set; }
    }
}
