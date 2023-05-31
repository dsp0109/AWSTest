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
            using var command = new SqlCommand(sqlQuery, con)
            {
                CommandType = commandType
            };

            if (sqlParameters != null)
            {
                command.Parameters.AddRange(sqlParameters.ToArray());
            }

            var sqlDataAdapter = new SqlDataAdapter(command);
            sqlDataAdapter.Fill(resultDataSet);

            return resultDataSet;
        }

        public int ExecuteNonQuery(string sqlQuery, CommandType commandType, IEnumerable<SqlParameter>? sqlParameters = null)
        {
            using SqlConnection con = new(_connectionString);
            using SqlCommand cmd = new(sqlQuery, con);
            cmd.CommandType = commandType;

            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters.ToArray());
            }
            con.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
