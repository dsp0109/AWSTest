using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS_ServiceModel.MasterdataXMLModel
{
    public class CatelogXML : IHeaderBase
    {
        public string? SRN { get; set; }
        public string? Name { get; set; }
        public string? Number { get; set; }
        public string? Address { get; set; }
    }
}
