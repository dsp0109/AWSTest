using Amazon.SQS.Model;
using SQS_ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS_ServiceLib.DataAccess
{
    public class OutboundMessageDal : IOutboundMessageDal
    {
        private readonly IDalBase _dalBase;
        
        public OutboundMessageDal(IDalBase dalBase) 
        {
            _dalBase = dalBase;
        }

        public IEnumerable<OutboundMessage> GetOutboundMessages(string filterText)
        {
            throw new NotImplementedException();
        }
    }
}
