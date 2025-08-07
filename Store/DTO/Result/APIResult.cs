namespace Store.DTO.Result
{
    public class APIResult<T> : BaseAPIResult
    {
        public T Data { get; set; }
    }
}
