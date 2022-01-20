using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;

namespace VisualBoard_Interface.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PubController : ControllerBase
    {
        private readonly IPubBL pubBL;

        public PubController(IPubBL pubBL)
        {
            this.pubBL = pubBL;
        }

        /// <summary>
        /// 获取映射关系表
        /// </summary>
        /// <param name="_Idreplace"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetIDreplaceList(pub_Idreplace _Idreplace)
        {
            return pubBL.GetIDreplaceList(_Idreplace);
        }
    }
}
