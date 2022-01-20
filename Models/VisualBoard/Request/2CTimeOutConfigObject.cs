using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    /// <summary>
    /// 2C预警时效维护
    /// </summary>
    public class TwoCTimeOutConfig
    {
        /// <summary>
        /// 行号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 超时类型
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 货主名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Privince { get; set; }
        /// <summary>
        /// 超时时长
        /// </summary>
        public int TimeSpan { get; set; }
        /// <summary>
        /// 维护人工号
        /// </summary>
        public int? OperateUserID { get; set; }
        /// <summary>
        /// 维护时间
        /// </summary>
        public string OperateTime { get; set; }
        /// <summary>
        /// 维护人姓名
        /// </summary>
        public string OperateUserName { get; set; }
    }

    public class TwoCTimeOutConfigForExport
    {
        /// <summary>
        /// 超时类型
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 货主名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Privince { get; set; }
        /// <summary>
        /// 超时时长
        /// </summary>
        public int TimeSpan { get; set; }
    }
}