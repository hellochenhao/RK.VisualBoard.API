using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;
using VisualBoard.Models.Request;

namespace VisualBoard.Business.Interface
{
    public interface IOrganizationBL
    {
        /// <summary>
        /// 新增机构
        /// </summary>
        /// <returns></returns>
        public ResponseObject InsterOrganization(bsc_Organization _Organization);
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <returns></returns>
        public ResponseObject UpdateOrganization(bsc_Organization _Organization);
        /// <summary>
        /// 查询一条用户信息
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectOneOrganization(bsc_Organization _Organization);

        /// <summary>
        /// 查询用户信息列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectListOrganization(OrganSearchObject _Organization);
        /// <summary>
        /// 查询机构树形结构
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetTreeOrganization();
        /// <summary>
        /// 根据机构获取客户列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetCustomerForOrgan(int OrganID);

        public ResponseObject GetTreeOrganAndCustomer();

        /// <summary>
        /// 批量作废机构
        /// </summary>
        /// <returns></returns>
        public ResponseObject BatDelOrganization(int[] IDs);

        public string GetSubOrganization(int pid);
    }
}
