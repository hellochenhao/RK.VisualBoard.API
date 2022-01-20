using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System.Data;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Request;
using VisualBoard_Interface.Common;

namespace VisualBoard_Interface.Controllers
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserBL userBL;
        private readonly IDbConnection dbConnection;

        public UserController(IUserBL userBL)
        {
            this.userBL = userBL;
        }

     
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public ResponseObject login(bsc_User _User)
        {
            return userBL.login(_User);
        }
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
  
        public ResponseObject InsterUser(bsc_User _User)
        {
            return userBL.InsterUser(_User);
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
    
        public ResponseObject UpdateUser(bsc_User _User)
        {
            return userBL.UpdateUser(_User);
        }

        /// <summary>
        /// 查询一条用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
    
        public ResponseObject SelectOneUser(Userobj _User)
        {
            return userBL.SelectOneUser(_User);
        }

        /// <summary>
        /// 查询用户信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
    
        public ResponseObject SelectListUser(Userobj _User)
        {
            return userBL.SelectListUser(_User);
        }

        /// <summary>
        /// 获取App菜单列表
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        [HttpPost]
     
        public ResponseObject GetAppMenuList()
        {
            return userBL.GetAppMenuList();
        }

        /// <summary>
        /// 获取PC菜单列表
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        [HttpPost]
    
        public ResponseObject GetPCMenuList()
        {
            return userBL.GetPCMenuList();
        }

        /// <summary>
        /// 获得App菜单权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
    
        public ResponseObject GetAppMenuRole(int RoleID)
        {
            return userBL.GetAppMenuRole(RoleID);
        }

        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        [HttpPost]
    
        public ResponseObject InsertRole(bsc_Role _Role)
        {
            return userBL.InsertRole(_Role);
        }


        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        [HttpPost]
     
        public ResponseObject UpdateRole(bsc_Role _Role)
        {
            return userBL.UpdateRole(_Role);
        }

        /// <summary>
        /// 查询权限列表
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        [HttpPost]
      
        public ResponseObject SearchListRole(bsc_Role _Role)
        {
            return userBL.SearchListRole(_Role);
        }

        /// <summary>
        /// 查询一条权限记录
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        [HttpPost]
   
        public ResponseObject SearchOneRole(bsc_Role _Role)
        {
            return userBL.SearchOneRole(_Role);
        }
    }
}
