using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    public class B2CStatementObject
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 分公司
        /// </summary>
        public string[] Filiale { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string[] Project { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int pagenum { get; set; }
        /// <summary>
        /// 条数
        /// </summary>
        public int pagecount { get; set; }
    }
}