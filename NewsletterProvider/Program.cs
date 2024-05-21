using Data.Contexts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Use context.Configuration to access the configuration
        services.AddDbContext<DataContext>(x => x.UseSqlServer(context.Configuration.GetConnectionString("SiliconDatabase")));

    })
    .Build();

host.Run();
