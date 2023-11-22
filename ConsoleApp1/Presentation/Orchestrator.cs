using ConsoleApp1.BusinessLogic;
using Microsoft.Extensions.Logging;

public class Orchestrator (
    IUploadFileService uploadFileService,
    ILogger<Orchestrator> logger
) {
    public async Task ExecuteAsync() {
        Random r = new Random();
        int rInt = r.Next(0, 10000);
        var currentDirectory = Directory.GetCurrentDirectory();
        var uploadFileParams = new UploadFileParams {
            ExternalPath = $"jm-learnings/s3uploadtest/helloworld-{rInt}.txt",
            LocalFilePath = Path.Combine(currentDirectory, "HelloWorld.txt")
        };
        var ct = new CancellationTokenSource(1000); // example to show case how cancellation token's work
        var success = await uploadFileService.UploadFile(uploadFileParams, ct.Token);
        var successMessage = success ? "was uploaded successfully": "FAILED TO UPLOAD!!!";
        logger.LogInformation($"{uploadFileParams.ExternalPath} {successMessage}");
    }
}