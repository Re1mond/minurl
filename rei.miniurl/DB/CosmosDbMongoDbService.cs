using System.Threading.Tasks;
using MongoDB.Driver;
using rei.miniurl.Models;

namespace rei.miniurl.DB;

public class CosmosDbMongoDbService
{
    private readonly IMongoCollection<UrlEntity> _urlsCollection;

    public CosmosDbMongoDbService(string connectionString, string databaseName, string collectionName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _urlsCollection = database.GetCollection<UrlEntity>(collectionName);
    }

    public async Task AddUrlAsync(UrlEntity url)
    {
        await _urlsCollection.InsertOneAsync(url);
    }

    public async Task<UrlEntity> GetUrlByShortCodeAsync(string shortCode)
    {
        return await _urlsCollection.Find(url => url.ShortCode == shortCode).FirstOrDefaultAsync();
    }
}