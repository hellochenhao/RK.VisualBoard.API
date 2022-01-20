using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;
using VisualBoard.Models.Request;

namespace VisualBoard.Business.Interface
{
    public interface ILoginBL
    {

        /// <summary>
        /// 小程序用户登录
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        public ResponseObject LoginsUser(bsc_User _User);

        /// <summary>
        /// 临时用户添加信息
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        public ResponseObject AddUserInfo(bsc_User _User);
    }
}
