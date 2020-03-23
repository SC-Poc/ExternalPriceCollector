using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ExternalPriceCollector.Azure
{
    public interface IBlobStorage
    {
        Task<string> SaveBlobAsync(string container, string key, Stream bloblStream, bool anonymousAccess = false);

        Task SaveBlobAsync(string container, string key, byte[] blob);

        Task SaveBlobAsync(string container, string key, byte[] blob, IReadOnlyDictionary<string, string> metadata);

        Task<bool> HasBlobAsync(string container, string key);

        Task<bool> CreateContainerIfNotExistsAsync(string container);

        Task<DateTime> GetBlobsLastModifiedAsync(string container);

        Task<Stream> GetAsync(string container, string key);
        Task<string> GetAsTextAsync(string container, string key);

        string GetBlobUrl(string container, string key);

        Task DeleteBlobsByPrefixAsync(string container, string prefix);

        Task<IEnumerable<string>> GetListOfBlobKeysByPrefixAsync(string container, string prefix,
            int? maxResultsCount = null);


        Task<IEnumerable<string>> GetListOfBlobsAsync(string container);
        Task<IEnumerable<string>> GetListOfBlobKeysAsync(string container, int? maxResultsCount = null);

        Task DelBlobAsync(string container, string key);

        Stream this[string container, string key] { get; }

        Task<string> GetMetadataAsync(string container, string key, string metaDataKey);
        Task<IDictionary<string, string>> GetMetadataAsync(string container, string key);

        Task<List<string>> ListBlobsAsync(string container, string path);


        Task<BlobProperties> GetPropertiesAsync(string container, string key);

        Task<string> AcquireLeaseAsync(string container, string key, TimeSpan? leaseTime, string proposedLeaseId = null);

        Task ReleaseLeaseAsync(string container, string key, string leaseId);

        Task RenewLeaseAsync(string container, string key, string leaseId);
    }
}
