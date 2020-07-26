using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poc_PIM_ADLS
{
    
    public class EventGridTopic
    {
        public string Topic { get; set; }
        public string Subject { get; set; }

        public string EventType { get; set; }

        public string Id { get; set; }
        public string DataVersion { get; set; }
        public string MetadataVersion { get; set; }
        public DateTime EventTime { get; set; }

        public dynamic Data { get; set; }
    }

    public class BlobCreatedData 
    {
        public string Api { get; set; }
        public string ClientRequestId { get; set; }
        public string RequestId { get; set; }
        public string ETag { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public string BlobType { get; set; }
        public string Url { get; set; }
        public string Sequencer { get; set; }   

    }

    public class SubscriptionValidationEventData
    {
        public string ValidationCode { get; set; }
        public string ValidationUrl { get; set; }

    }



    public class Const
    {
        public static readonly string EventTypeBlobCreated = "Microsoft.Storage.BlobCreated";
        public static readonly string EventSubscriptionValidation = "Microsoft.EventGrid.SubscriptionValidationEvent";
    }


}
