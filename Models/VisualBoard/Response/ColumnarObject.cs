using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class ColumnarObject
    {
        /// <summary>
        /// X轴数据
        /// </summary>
        public string[] Name { get; set; }

        /// <summary>
        /// Y轴数据
        /// </summary>
        public decimal[] Data { get; set; }
    }
}
