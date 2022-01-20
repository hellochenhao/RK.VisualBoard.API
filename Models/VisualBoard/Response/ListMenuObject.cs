using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class ListMenuObject : bsc_menus
    {
        public List<bsc_menus> Subs { get; set; }
    }
}
