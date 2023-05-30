using Newtonsoft.Json;
using SQS_ServiceModel;
using SQS_ServiceModel.MasterdataXMLModel;
using System.Data;
using System.Xml.Serialization;

namespace SQS_ServiceLib.Handler
{
    public class CatelogFileHandler : IMasterdataFileHandler
    {
        public static MasterDataType Type => MasterDataType.CATELOG;

        public static List<string> CronScheduler => new List<string> { "*/1 * * * *", "0 0 * * 0" };

        public static List<long> ScheduleInMinutes => new List<long> { 1, 10080 };

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

            dt.Rows.Add("DP",12, "251-1A CA");
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
            var serializer = dt.AsEnumerable().ToList().Select(x => new CatelogXML {
                SRN = "12562",
                Name = x.Field<string?>("Name"),
                Number = x.Field<string?>("Number"),
                Address = x.Field<string?>("Address"),
                SmallModels = new List<SmallModel>
                {
                    new SmallModel
                    {
                        SmallName = "ABC"
                    }
                }
            });

            foreach(var itm in serializer)
            {
                var xmlDocument = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(itm), "Envelope", true);
                var xmlstring = SerializeToXml(itm);
                xmlDocument.Save($"CATELOG_{itm.SRN}.xml");
                xmlDocument = null;
            }
        }

        public string SerializeToXml(object input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType(), null, null, new XmlRootAttribute("Envelope"), null);
            string result = string.Empty;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, input);

                memStm.Position = 0;
                result = new StreamReader(memStm).ReadToEnd();
            }

            return result;
        }
    }
}
