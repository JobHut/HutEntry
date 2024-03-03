using HutEntry.Data;
using HutEntry.Models;
using HutEntry.Profiles;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Hosting;

var builder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<UserDbContext>(opts =>
        {
            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            opts.UseSqlServer(connectionString);
        });
        services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<UserDbContext>();
    }).Build();

builder.Run();

