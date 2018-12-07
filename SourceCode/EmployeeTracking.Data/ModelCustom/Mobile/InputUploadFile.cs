namespace EmployeeTracking.Data.ModelCustom.Mobile
{
    public class InputUploadFile
    {
        public int FileID { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public byte[] FileContent { get; set; }
        public byte[] ThumbContent { get; set; }
        public byte[] MiddleContent { get; set; }
        public string Description { get; set; }
    }
}
