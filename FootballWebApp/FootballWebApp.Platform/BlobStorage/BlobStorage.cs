using Azure.Storage.Blobs;

namespace FootballWebApp.Platform.BlobStorage;

public class BlobStorage : IBlobStorage
{
    private readonly BlobConfiguration _configuration;
    private readonly BlobServiceClient _client;
    public BlobStorage(BlobConfiguration configuration)
    {
        _configuration = configuration;
        _client = new BlobServiceClient(configuration.ConnectionString);
    }
    public async Task<bool> ContainsFileByNameAsync(string fileName)
    {
        return await _client
            .GetBlobContainerClient(_configuration.ContainerName)
            .GetBlobClient(fileName)
            .ExistsAsync();
    }

    public async Task CreateFileAsync(string fileName)
    {
        await _client
            .GetBlobContainerClient(_configuration.ContainerName)
            .GetBlobClient(fileName)
            .UploadAsync(new MemoryStream());
    }

    public async Task<List<int>> GetAllFilesByNameAsync(Guid championshipId)
    {
        var users = _client.GetBlobContainerClient(_configuration.ContainerName)
            .GetBlobs(prefix: championshipId.ToString("N"))
            .AsPages(default, 10000)
            .SelectMany(dt => dt.Values).Select(fn => int.Parse(fn.Name.Split('_').Last())).ToList();
        return users;
    }

    public async Task<Stream> GetFileByNameAsync(string fileName)
    {
        var blobClient = _client
            .GetBlobContainerClient(_configuration.ContainerName)
            .GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"File {fileName} not found in blob storage.");
        }

        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var blobClient = _client
            .GetBlobContainerClient(_configuration.ContainerName)
            .GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"File {fileName} not found in blob storage.");
        }

        await blobClient.DeleteAsync();
    }

    public async Task UpdateFileAsync(string fileName, Stream content)
    {
        var blobClient = _client
            .GetBlobContainerClient(_configuration.ContainerName)
            .GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"File {fileName} not found in blob storage.");
        }

        await blobClient.UploadAsync(content, overwrite: true);
    }
}