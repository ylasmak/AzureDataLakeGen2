using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Azure.Storage;
using System.IO;
using Azure;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;

namespace Poc_PIM_ADLS.Repositories
{
    public class DataLakeServiceClientRepository: IDataLakeServiceClientRepository
    {

        Settings _settings;
        DataLakeServiceClient _dataLakeServiceClient;
        public DataLakeServiceClientRepository(IOptionsSnapshot<Settings> settings)
        {
            _settings = settings.Value;
            GetDataLakeServiceClient();
        }


        private void  GetDataLakeServiceClient()
        {
            TokenCredential credential = new ClientSecretCredential(
                                 _settings.TenantID, _settings.ClientID,
                                 _settings.ClientSecret, new TokenCredentialOptions());

            string dfsUri = "https://" + _settings.AccountName + ".dfs.core.windows.net";

            _dataLakeServiceClient = new DataLakeServiceClient(new Uri(dfsUri), credential);
        }


        public async Task<DataLakeFileSystemClient> GetFileSystem (string fileSystem)
        {
            return await Task.Run (() => _dataLakeServiceClient.GetFileSystemClient(fileSystem));
        }

        public async Task<IEnumerable<string>> ListFilesInDirectory(string fileSystem, string directory, bool recursive)
        {
            var fileSystemClient = await GetFileSystem(fileSystem);
           return await ListFilesInDirectory(fileSystemClient, directory, recursive);
        }

        public async Task<IEnumerable<string>> ListFilesInDirectory(DataLakeFileSystemClient fileSystemClient,string directory,bool recursive)
        {
            var result = new List<string>();

            IAsyncEnumerator<PathItem> enumerator =
                fileSystemClient.GetPathsAsync(directory,recursive:recursive).GetAsyncEnumerator();

            await enumerator.MoveNextAsync();

            PathItem item = enumerator.Current;

            while (item != null)
            {
                result.Add(item.Name);

                if (!await enumerator.MoveNextAsync())
                {
                    break;
                }

                item = enumerator.Current;
            }

            return result;

        }


        public async Task UploadJsonData(string fileSystem, string directory,string jsonData,string dataId)
        {
            var fileSystemClient = await GetFileSystem(fileSystem);
            await UploadJsonData(fileSystemClient, directory, jsonData, dataId);
        }


        public async Task UploadJsonData(DataLakeFileSystemClient fileSystemClient, string directory,string jsonData, string dataId)
        {
            DataLakeDirectoryClient directoryClient =
                fileSystemClient.GetDirectoryClient(directory);

            DataLakeFileClient fileClient = await directoryClient.CreateFileAsync(dataId+".json");

            byte[] byteArray = Encoding.ASCII.GetBytes(jsonData);
            MemoryStream stream = new MemoryStream(byteArray);

            long fileSize = stream.Length;

            await fileClient.AppendAsync(stream, offset: 0);

            await fileClient.FlushAsync(position: fileSize);

        }

        public async Task<dynamic> DownloadJsonData(string fileSystem, string directory,  string filePath)
        {
            var fileSystemClient = await GetFileSystem(fileSystem);
            return await DownloadJsonData(fileSystemClient, directory, filePath);
        }
        public async Task<dynamic> DownloadJsonData(DataLakeFileSystemClient fileSystemClient, string directory, string filePath)
        {
            DataLakeDirectoryClient directoryClient =
                fileSystemClient.GetDirectoryClient(directory);

            DataLakeFileClient fileClient =
                directoryClient.GetFileClient(filePath);

            Response<FileDownloadInfo> downloadResponse = await fileClient.ReadAsync();
            var streamContent = new StreamContent(downloadResponse.Value.Content);
            var stringContent = await streamContent.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(stringContent);



        }
    }
}
