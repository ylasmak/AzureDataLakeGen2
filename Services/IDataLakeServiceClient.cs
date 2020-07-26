using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poc_PIM_ADLS.Services
{
   public interface IDataLakeServiceClient
    {
        Task<IEnumerable<string>> ListFilesInDirectory(string fileSystem, string directory, bool recursive);
        Task UploadJsonData(string fileSystem, string directory, string stringContent, string id);
        Task HandleEventTypeBlobCreated(EventGridTopic @event);
        Task<string> HandleEventSubscriptionValidation(EventGridTopic @event);
    }
}
