namespace FootballWebApp.Platform.BlobStorage;

public interface IBlobStorage
{
    Task<bool> ContainsFileByNameAsync(string fileName);
    Task CreateFileAsync(string fileName);

    Task<List<int>> GetAllFilesByNameAsync(Guid championshipId);

    Task<Stream> GetFileByNameAsync(string fileName);
    Task DeleteFileAsync(string fileName);
    Task UpdateFileAsync(string fileName, Stream content);
}