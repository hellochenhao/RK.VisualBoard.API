using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class TreeAppMenuObject
    {
        public int id { get; set; }
        public string label { get; set; }
        public IEnumerable<TreeAppMenuChildObject> children { get; set; }
    }
    public class TreeAppMenuChildObject
    {
        public int id { get; set; }
        public string label { get; set; }
    }

}
