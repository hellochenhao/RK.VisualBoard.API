using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;
using VisualBoard.Models.Request;

namespace VisualBoard.Business.Interface
{
    public interface IUserBL
    {
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        public ResponseObject login(bsc_User _User);
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        public ResponseObject InsterUser(bsc_User _User);
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <returns></returns>
        public ResponseObject UpdateUser(bsc_User _User);
        /// <summary>
        /// 查询一条用户信息
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectOneUser(Userobj _User);

        /// <summary>
        /// 查询用户信息列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectListUser(Userobj _User);

        /// <summary>
        /// 获取App菜单列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetAppMenuList();

        /// <summary>
        /// 获取PC菜单列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetPCMenuList();

        /// <summary>
        /// 获得App菜单权限
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetAppMenuRole(int id);

        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        public ResponseObject InsertRole(bsc_Role _Role);

        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        public ResponseObject UpdateRole(bsc_Role _Role);

        /// <summary>
        /// 查询权限列表
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        public ResponseObject SearchListRole(bsc_Role _Role);

        /// <summary>
        /// 查询一条权限记录
        /// </summary>
        /// <param name="_Role"></param>
        /// <returns></returns>
        public ResponseObject SearchOneRole(bsc_Role _Role);

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <returns></returns>
        public string GetToken();

    }
}
