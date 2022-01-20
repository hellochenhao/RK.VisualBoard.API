using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    /// <summary>
    /// 折线图
    /// </summary>
    public class MixedLineAndBarObject
    { 
        /// <summary>
        /// 中文版
        /// </summary>
        public string target_cn { get; set; }
  
        /// <summary>
        /// X轴日期
        /// </summary>
        public string[] day { get; set; }

        /// <summary>
        /// Y轴数值
        /// </summary>
        public decimal[] Value { get; set; }
         
    }
}
