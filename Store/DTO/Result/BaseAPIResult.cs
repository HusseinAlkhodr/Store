namespace Store.DTO.Result
{
    public class BaseAPIResult
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public int Code { get; set; } = 200;
    }
    public static class BaseAPIResult_Extensions
    {
        public static async Task<BaseAPIResult> SetAddSuccessMessage(this Task<BaseAPIResult> thread)
        {
            var result = thread.Result;
            result.Message = "Add Done Successfully";
            return result;
        }
        public static async Task<BaseAPIResult> SetUpdateSuccessMessage(this Task<BaseAPIResult> thread)
        {
            var result = thread.Result;
            result.Message = "Update Done Successfully";
            return result;
        }
    }
}
