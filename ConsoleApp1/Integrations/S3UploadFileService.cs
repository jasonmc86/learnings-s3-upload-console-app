using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ConsoleApp1.BusinessLogic;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Integrations;

public class S3UploadFileService (
    ITransferUtility transferUtility,
    ILogger<S3UploadFileService> logger
): IUploadFileService {
    public async Task<bool> UploadFile(UploadFileParams uploadFileParams, CancellationToken ct)
    {
        var externalPath = uploadFileParams.ExternalPath;
        var pathPeices = externalPath.Split("/", 2);
        var bucketName = pathPeices.First();
        var subDirectory = pathPeices.Last();

        var request = new TransferUtilityUploadRequest {
            BucketName = bucketName,
            FilePath = uploadFileParams.LocalFilePath,
            Key = subDirectory
        };

        try
        {
            await transferUtility.UploadAsync(request, cancellationToken: ct);
        }
        catch (AmazonS3Exception s3Ex)
        {
            logger.LogError(s3Ex, $"Could not upload {request.Key} from {request.FilePath}");
            return false;
        }
        catch (TaskCanceledException tcEx) {
            logger.LogError(tcEx, $"Could not upload {request.Key} from {request.FilePath} because task was cancelled");
            return false;
        }
         
        return true;
    }
}