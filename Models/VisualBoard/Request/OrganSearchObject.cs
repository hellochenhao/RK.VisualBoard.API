using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    public class OrganSearchObject
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrganName { get; set; }
    }
}
