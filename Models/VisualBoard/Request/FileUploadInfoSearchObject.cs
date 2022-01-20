using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    public class FileUploadInfoSearchObject: FileUploadInfo
    {
        /// <summary>
        /// 创建开始时间
        /// </summary>
        public DateTime? CreateTimeStart { get; set; }
        /// <summary>
        /// 创建结束时间
        /// </summary>
        public DateTime? CreateTimeEnd { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int pagenum { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int pagecount { get; set; }
    }
}
