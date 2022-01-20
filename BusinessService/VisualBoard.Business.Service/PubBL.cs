using Dapper;
using Rokin.Common.Tools;
using Rokin.Dapper;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VisualBoard.Business.Interface;

namespace VisualBoard.Business.Service
{
    public class PubBL : IPubBL
    {
        private readonly _WMS_VisualboardContext wMS_Visualboard;
        private readonly IDbConnection connection;
        public PubBL(_WMS_VisualboardContext _WMS_Visualboard, IDbConnection _connection)
        {
            wMS_Visualboard = _WMS_Visualboard;
            connection = _connection;
        }

        public ResponseObject GetIDreplaceList(pub_Idreplace _Idreplace)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result = connection.Query<pub_Idreplace>(LambdaHelper.CreateWhere(_Idreplace));
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }
    }
}
