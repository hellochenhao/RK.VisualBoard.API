using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    /// <summary>
    /// 首页查询条件
    /// </summary>
    public class IndexQueryObject
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
        /// 登录机构
        /// </summary>
        public string OrganizationID { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        public string CustomerID { get; set; }
    }
}
