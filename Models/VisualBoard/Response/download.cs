using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class download
    {
        /// <summary>
        /// id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 报表名称
        /// </summary>
        public string ReportName { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public string Cstate { get; set; }
        /// <summary>
        /// 文件状态
        /// </summary>
        public string WJstate { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string WJsize { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string OverTime { get; set; }
        /// <summary>
        /// 错误日志
        /// </summary>
        public string ErrorRZ { get; set; }
        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; }
    }
}
