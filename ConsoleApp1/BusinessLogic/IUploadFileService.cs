namespace ConsoleApp1.BusinessLogic {
    public interface IUploadFileService {
        Task<bool> UploadFile(UploadFileParams uploadFileParams, CancellationToken ct);
    }
}

