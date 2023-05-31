using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS_ServiceLib.DataAccess
{
    public interface IDalBase
    {
        DataSet GetDataSet(string sqlQuery, CommandType commandType, IEnumerable<SqlParameter>? sqlParameters = null);

        int ExecuteNonQuery(string sqlQuery, CommandType commandType, IEnumerable<SqlParameter>? sqlParameters = null);
    }
}
