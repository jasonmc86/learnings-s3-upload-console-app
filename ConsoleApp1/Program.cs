using Amazon.S3;
using Amazon.S3.Transfer;
using ConsoleApp1.BusinessLogic;
using ConsoleApp1.Integrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var orchestrator = host.Services.GetRequiredService<Orchestrator>();
        
        await orchestrator.ExecuteAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAWSService<IAmazonS3>();

                services.AddTransient<ITransferUtility, TransferUtility>();

                services.AddTransient<IUploadFileService, S3UploadFileService>();

                services.AddTransient<Orchestrator>();
            });
}