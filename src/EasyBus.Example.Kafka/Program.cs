﻿using EasyBus.Example.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostBuilder();
builder.ConfigureServices(services =>
{
    services.AddTransient<IConfiguration>(_ =>
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        return builder.Build();
    });
    services.AddLogging(x => x.AddConsole());
    services.AddTestMessageQueue();
    services.AddHostedService<LocalRunner>();
});
var app = builder.Build();

await app.RunAsync();

Console.WriteLine("End");
Console.ReadKey();