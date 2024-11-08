using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using rei.miniurl;
using rei.miniurl.DB;

[assembly: FunctionsStartup(typeof(rei.miniurl.FunctionStartup))]

namespace rei.miniurl;

public class FunctionStartup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var connectionString = Environment.GetEnvironmentVariable("MongoDb:ConnectionString");
        var dbName = Environment.GetEnvironmentVariable("MongoDb:DatabaseName");
        var collectionName = Environment.GetEnvironmentVariable("MongoDb:CollectionName");

        builder.Services.AddSingleton<CosmosDbMongoDbService>(serviceProvider =>
        {
            return new CosmosDbMongoDbService(connectionString, dbName, collectionName);
        });
    }
}