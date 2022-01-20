using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Request;
using VisualBoard_Interface.Common;

namespace VisualBoard_Interface.Controllers
{
    /// <summary>
    /// 机构
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationBL organizationBL;

        public OrganizationController(IOrganizationBL organizationBL)
        {
            this.organizationBL = organizationBL;
        }

        /// <summary>
        /// 新增机构
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject InsterOrganization(bsc_Organization _Organization)
        {
            return organizationBL.InsterOrganization(_Organization);
        }

        /// <summary>
        /// 修改机构信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject UpdateOrganization(bsc_Organization _Organization)
        {
            return organizationBL.UpdateOrganization(_Organization);
        }

        /// <summary>
        /// 批量作废机构
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject BatDelOrganization(DelOrganObject delOrgan)
        {
            return organizationBL.BatDelOrganization(delOrgan.IDs);
        }

        /// <summary>
        /// 查询一条机构信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject SelectOneOrganization(bsc_Organization _Organization)
        {
            return organizationBL.SelectOneOrganization(_Organization);
        }

        /// <summary>
        /// 查询机构信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject SelectListOrganization(OrganSearchObject _Organization)
        {
            return organizationBL.SelectListOrganization(_Organization);
        }


        /// <summary>
        /// 查询机构树形结构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject GetTreeOrganization()
        {
            return organizationBL.GetTreeOrganization();
        }


        /// <summary>
        /// 查询机构和客户树形结构
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject GetTreeOrganAndCustomer()
        {
            return organizationBL.GetTreeOrganAndCustomer();
        }

        /// <summary>
        /// 根据机构获取客户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject GetCustomerForOrgan(int OrganID)
        {
            return organizationBL.GetCustomerForOrgan(OrganID);
        }
        
    }
}
