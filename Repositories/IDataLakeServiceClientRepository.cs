using Azure.Storage.Files.DataLake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poc_PIM_ADLS.Repositories
{
    public interface IDataLakeServiceClientRepository
    {
        Task<DataLakeFileSystemClient> GetFileSystem(string fileSystem);
        Task<IEnumerable<string>> ListFilesInDirectory(string fileSystem, string directory, bool recursive);
        Task<IEnumerable<string>> ListFilesInDirectory(DataLakeFileSystemClient fileSystemClient, string directory, bool recursive);
        Task UploadJsonData(string fileSystem, string directory, string jsonData, string dataId);
        Task UploadJsonData(DataLakeFileSystemClient fileSystemClient, string directory, string jsonData, string dataId);
        Task<dynamic> DownloadJsonData(string fileSystem, string directory, string filePath);
        Task<dynamic> DownloadJsonData(DataLakeFileSystemClient fileSystemClient, string directory, string filePath);


    }
}
