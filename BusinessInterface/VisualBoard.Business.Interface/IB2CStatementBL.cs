using System;
using System.Collections.Generic;
using System.Text;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;

namespace VisualBoard.Business.Interface
{
    public interface IB2CStatementBL
    {
        public ResponseObject ShowB2CStatement(B2CStatementObject obj);

        public ResponseObject SelectList(string UserID);

        public ResponseObject SelectProject(B2CStatementObject obj);

        public List<B2CExcelObject> GetList(B2CStatementObject obj);
    }
}