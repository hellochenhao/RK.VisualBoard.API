using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;
using VisualBoard_Interface.Common;

namespace VisualBoard_Interface.Controllers
{

    /// <summary>
    /// 小程序用户登录控制器
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginBL loginBL;

        public LoginController(ILoginBL loginBL)
        {
            this.loginBL = loginBL;
        }


        /// <summary>
        /// 小程序用户登录
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public ResponseObject LoginsUser(bsc_User _User)
        {
            return loginBL.LoginsUser(_User);
        }

        /// <summary>
        /// 临时用户添加信息
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject AddUserInfo(bsc_User _User)
        {
            return loginBL.AddUserInfo(_User);
        }
    }
}
