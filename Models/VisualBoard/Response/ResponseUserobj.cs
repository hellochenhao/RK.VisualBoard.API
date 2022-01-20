using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class ResponseUserobj: bsc_User
    {
        public string OrganName { get; set; }
    }
}
