using SQS_ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS_ServiceLib.Handler
{
    public class CatelogFileHandler : IMasterdataFileHandler
    {
        public static MasterDataType Type => MasterDataType.CATELOG;

        public static List<string> CronScheduler => new List<string> { "*/1 * * * *", "0 0 * * 0" };

        public static string JobId => MasterDataType.CATELOG.ToString();

        public async Task HandleAsync()
        {
            // To Do -
            // 1. Featch data from DB
            // 2. Do Some mssaging on the data
            // 3. Convert data to XML
            // 4. create and upload XML file on s3 blob
            await Task.CompletedTask;
        }
    }
}
