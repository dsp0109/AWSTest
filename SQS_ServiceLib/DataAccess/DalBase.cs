using System.Data;
using System.Data.SqlClient;

namespace SQS_ServiceLib.DataAccess
{
    public class DalBase : IDalBase
    {
        public string _connectionString => "";

        public DataSet GetDataSet(string sqlQuery, CommandType commandType, IEnumerable<SqlParameter>? sqlParameters = null)
        {
            var resultDataSet = new DataSet();
            using var con = new SqlConnection(_connectionString);
            var command = new SqlCommand(sqlQuery, con);
            command.CommandType = commandType;
            
            if(sqlParameters != null)
            {
                command.Parameters.AddRange(sqlParameters.ToArray());
            }

            var sqlDataAdapter = new SqlDataAdapter(command);
            sqlDataAdapter.Fill(resultDataSet);

            return resultDataSet;
        }
    }
}
