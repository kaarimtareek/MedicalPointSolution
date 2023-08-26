namespace MedicalPoint.Common
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public static OperationResult Failed(string message)
        {
            return new OperationResult { Success = false, Message = message };
        }
        public static OperationResult Succeeded(string message = "")
        {
            return new OperationResult { Success = true, Message = message };
        }
    }
    public class OperationResult<T> : OperationResult where T : class
    {
        public T Data { get; set; }
        public static OperationResult<T> Failed(string message)
        {
            return new OperationResult<T> { Success = false, Message = message };
        }
        public static OperationResult<T> Succeeded<T> (T data,string message = "") where T : class
        {
            return new OperationResult<T> { Success = true, Message = message, Data = data };
        }
    }
}
