namespace MedicalPoint.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string ActionPath { get; set; }
        public string ControllerPath { get; set; }
        public string Id { get; set; }
        public string ErrorMessage { get; set; }
    }
}