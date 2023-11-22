using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Transfer;
using ConsoleApp1.BusinessLogic;
using ConsoleApp1.Integrations;
using Microsoft.Extensions.Logging;

using ExpectedPathOutput = (
    string ExternalPath, 
    string LocalFilePath,
    string BucketName, 
    string RelativePath, 
    System.Threading.CancellationToken CancellationToken
);

namespace ConsoleApp1.Tests.Integrations;

public class S3UploadFileServiceTests
{
    [Fact]
    public async Task UploadFile_WhenGivenExternalPath_CorrectlyParsesBucketName()
    {
        var (sut, transferUtilityMock) = SetupServiceUnderTest();
        var expected = SetupTestData();

        await sut.UploadFile(new UploadFileParams
        {
            LocalFilePath = String.Empty,
            ExternalPath = expected.ExternalPath
        }, expected.CancellationToken);

        transferUtilityMock.Verify(a =>
            a.UploadAsync(
                It.Is<TransferUtilityUploadRequest>(
                    r => r.BucketName.Equals(expected.BucketName)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadFile_WhenGivenExternalPath_CorrectlyParsesFileKey()
    {
        var (sut, transferUtilityMock) = SetupServiceUnderTest();
        var expected = SetupTestData();

        await sut.UploadFile(new UploadFileParams
        {
            LocalFilePath = String.Empty,
            ExternalPath = expected.ExternalPath
        }, expected.CancellationToken);

        transferUtilityMock.Verify(a =>
            a.UploadAsync(
                It.Is<TransferUtilityUploadRequest>(
                    r => r.Key.Equals(expected.RelativePath)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadFile_WhenGivenLocalFilePath_CorrectlyMapsToRequest()
    {
        var (sut, transferUtilityMock) = SetupServiceUnderTest();
        var expected = SetupTestData();

        await sut.UploadFile(new UploadFileParams
        {
            LocalFilePath = expected.LocalFilePath,
            ExternalPath = String.Empty
        }, expected.CancellationToken);

        transferUtilityMock.Verify(a =>
            a.UploadAsync(
                It.Is<TransferUtilityUploadRequest>(
                    r => r.FilePath.Equals(expected.LocalFilePath)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadFile_WhenGivenCorrectParamsAndS3UploadSucceeds_ReturnsTrue()
    {
        var (sut, _) = SetupServiceUnderTest();
        var expected = SetupTestData();

        var result = await sut.UploadFile(new UploadFileParams
        {
            LocalFilePath = expected.LocalFilePath,
            ExternalPath = expected.ExternalPath
        }, expected.CancellationToken);

        Assert.True(result);
    }

    [Fact]
    public async Task UploadFile_WhenGivenCorrectParamsButS3UploadFails_ReturnsFalse()
    {
        var (sut, transferUtilityMock) = SetupServiceUnderTest();
        transferUtilityMock.Setup(
            a=>a.UploadAsync(
                It.IsAny<TransferUtilityUploadRequest>(), 
                It.IsAny<CancellationToken>())
        ).ThrowsAsync(new AmazonS3Exception("test error"));
            
        var expected = SetupTestData();

        var result = await sut.UploadFile(new UploadFileParams
        {
            LocalFilePath = expected.LocalFilePath,
            ExternalPath = expected.ExternalPath
        }, expected.CancellationToken);

        Assert.False(result);
    }

    private static ExpectedPathOutput SetupTestData() {
        var bucketName = "jm-learnings";
        var relativePath =  "s3uploadtest/helloworld-123.txt";

        return new ExpectedPathOutput {
            BucketName = bucketName,
            LocalFilePath = "C://HelloWorld.txt",
            RelativePath = relativePath,
            ExternalPath = $"{bucketName}/{relativePath}",
            CancellationToken = CancellationToken.None
        };
    }

    private static (S3UploadFileService, Mock<ITransferUtility>) SetupServiceUnderTest()
    {
        var transferUtilityMock = new Mock<ITransferUtility>();
        var loggerStub = new Mock<ILogger<S3UploadFileService>>();

        var sut = new S3UploadFileService(
            transferUtilityMock.Object,
            loggerStub.Object
        );

        return (sut, transferUtilityMock);
    }
}