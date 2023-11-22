namespace ConsoleApp1.BusinessLogic {
    public record UploadFileParams {
        public required string LocalFilePath  { get; init; }
        public required string ExternalPath { get; init; }
    }
}

