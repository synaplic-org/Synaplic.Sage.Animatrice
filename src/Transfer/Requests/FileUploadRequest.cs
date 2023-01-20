namespace Uni.Scan.Transfer.Requests
{
    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Folder { get; set; }
        public byte[] Data { get; set; }
    }
}