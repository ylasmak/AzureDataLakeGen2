using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Poc_PIM_ADLS.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Poc_PIM_ADLS.Services
{
    public class DataLakeServiceClient: IDataLakeServiceClient
    {

        private readonly IDataLakeServiceClientRepository _dataLakeServiceClientRepository;
        private readonly ILogger<DataLakeServiceClient> _logger;
        public DataLakeServiceClient(ILogger<DataLakeServiceClient> logger,
                               IDataLakeServiceClientRepository dataLakeServiceClientRepository)
        {
            _logger = logger;
            _dataLakeServiceClientRepository = dataLakeServiceClientRepository;

        }

        public async Task<IEnumerable<string>>  ListFilesInDirectory(string fileSystem,string directory,bool recursive)
        {
           return await _dataLakeServiceClientRepository.ListFilesInDirectory(fileSystem, directory, true);
        }

        public async Task UploadJsonData(string fileSystem, string directory,string  stringContent, string id)
        {
            await _dataLakeServiceClientRepository.UploadJsonData(fileSystem, directory, stringContent, id);
        }

        public async Task HandleEventTypeBlobCreated(EventGridTopic @event)
        {
            var eventData = DynamicToObject<BlobCreatedData>(@event.Data);

            if (eventData.ContentLength > 0 | eventData.Api == "RenameFile")
            {
                var url = new Uri(!string.IsNullOrEmpty(eventData.Url) ? eventData.Url : eventData.DestinationUrl);
                var localPath = url.LocalPath;
                var fileName = Path.GetFileName(localPath);
                var folderPath = Path.GetDirectoryName(localPath).Split(Path.DirectorySeparatorChar);

                var fileStream = string.Empty;
                var directoryPath = string.Empty;


                foreach (var item in folderPath)
                {
                    if (String.IsNullOrEmpty(item)) continue;
                    if (string.IsNullOrEmpty(fileStream))
                    {
                        fileStream = item;
                        continue;
                    }
                    directoryPath = Path.Combine(directoryPath, item);
                }


                var data = await _dataLakeServiceClientRepository.DownloadJsonData(fileStream, directoryPath, fileName);

                //do something
                //....
                //End do something

                var id = "out_product" + "_" + Guid.NewGuid().ToString();
                await _dataLakeServiceClientRepository.UploadJsonData("outputfs", "output", data.ToString(), id);
            }

        }

        public async Task<string> HandleEventSubscriptionValidation(EventGridTopic @event)
        {
           return  await Task.Run(() =>
                    {
                        var eventData = DynamicToObject<SubscriptionValidationEventData>(@event.Data);
                        dynamic eventResponse = new System.Dynamic.ExpandoObject();
                        eventResponse.validationResponse = eventData.ValidationCode;
                        return JsonConvert.SerializeObject(eventResponse);


                    });
        }

        private T DynamicToObject<T>(dynamic dynamic) where T : class
        {
            return JsonConvert.DeserializeObject<T>(dynamic.ToString()) as T;
        }
    }
}
