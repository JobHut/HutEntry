using HutEntry.Data;
using HutEntry.Models;
using HutEntry.Profiles;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HutEntry
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddApplicationInsightsTelemetryWorkerService();
            builder.Services.ConfigureFunctionsApplicationInsights();
            builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

            builder.Services.AddDbContext<UserDbContext>(opts =>
            {
                var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
                opts.UseSqlServer(connectionString);
            });

            builder.Services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<UserDbContext>();
        }
    }
}
