using Newtonsoft.Json;
using SQS_ServiceModel;
using SQS_ServiceModel.MasterdataXMLModel;
using System.Data;

namespace SQS_ServiceLib.Handler
{
    public class CatelogFileHandler1 : IMasterdataFileHandler
    {
        public static MasterDataType Type => MasterDataType.CATELOG;

        public static List<string> CronScheduler => new List<string> { "*/2 * * * *", "0 0 * * 0" };

        public static List<long> ScheduleInMinutes => new List<long> { 2, 10080 };

        public static string JobId => MasterDataType.CATELOG.ToString();

        public async Task HandleAsync()
        {
            // To Do -
            // 1. Featch data from DB
            // 2. Do Some mssaging on the data
            // 3. Convert data to XML
            // 4. create and upload XML file on s3 blob
            GenerateFile();
            await Task.CompletedTask;
        }

        public void GenerateFile()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name"));
            dt.Columns.Add(new DataColumn("Number"));
            dt.Columns.Add(new DataColumn("Address"));

            dt.Rows.Add("DP", 12, "251-1A CA");
            dt.Rows.Add("DP1", 122, "251-1A CA1");

            //var serializer = dt.AsEnumerable().ToList().Select(x => new 
            //{ 
            //    Header = new 
            //        { 
            //            Name = x.Field<string?>("Name"), 
            //            Number = x.Field<string?>("Number")
            //        }, 
            //    Address = x.Field<string?>("Address") 
            //}).ToList();
            var serializer = dt.AsEnumerable().ToList().Select(x => new CatelogXML
            {
                SRN = "12562",
                Name = x.Field<string?>("Name"),
                Number = x.Field<string?>("Number"),
                Address = x.Field<string?>("Address")
            });

            foreach (var itm in serializer)
            {
                var xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(itm), "Envelope", true);
                xmlDocument.Save($"CATELOG1_{itm.SRN}.xml");
                xmlDocument = null;
            }
        }
    }
}
