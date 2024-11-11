using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using rei.miniurl;
using rei.miniurl.DB;
using rei.miniurl.DBModels;


namespace rei.miniurl.Functions;

public class ShortenUrlFunction
{
    private readonly CosmosDbMongoDbService _cosmosDbService;

    public ShortenUrlFunction(CosmosDbMongoDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [FunctionName("shorturl")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        string url = data?.url;

        if (string.IsNullOrEmpty(url))
        {
            return new BadRequestObjectResult("Please pass a URL in the request body.");
        }

        var urlEntity = new UrlEntity
        {
            Id = Guid.NewGuid().ToString(),
            LongUrl = url,
            ShortCode = GenerateShortCode(url)
        };

        await _cosmosDbService.AddUrlAsync(urlEntity);

        var shortUrl = $"https://rei.url/{urlEntity.ShortCode}";
        return new OkObjectResult(new { ShortUrl = shortUrl });
    }

    [FunctionName("GetLongUrl")]
    public async Task<IActionResult> GetResource(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{id?}")] HttpRequest req,
        string id,
        ILogger log)
    {
        var longUrl = await _cosmosDbService.GetUrlByShortCodeAsync(id);
        return new OkObjectResult(longUrl.LongUrl);
    }

    private string GenerateShortCode(string longUrl)
    {
        // Simple example: use a GUID substring
        return Guid.NewGuid().ToString().Substring(0, 8);
    }
}