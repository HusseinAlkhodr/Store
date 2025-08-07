using Microsoft.Extensions.Localization;
using Store.Middlewares;

namespace Store.Extensions
{
    public static class QueryExtension
    {
        public static async Task<T> ValidateNotFound<T>(this Task<T> QueryTask, string ErrorMessage = null)
        {
            var entity = await QueryTask;
            if (entity == null)
                throw new HusseinErrorResponseException(ErrorMessage == null ? $"{typeof(T).Name} Not Found" : ErrorMessage, 404);
            return entity;
        }
        public static async Task<T> ValidateNotFound<T>(this Task<T> QueryTask, IStringLocalizer stringLocalizer, string EntityName)
        {
            var entity = await QueryTask;
            if (entity == null)
                throw new HusseinErrorResponseException(stringLocalizer.GetString("NOT FOUND", EntityName), 404);
            return entity;
        }
        public static TSource Validate_Null<TSource>(this TSource source, string ErrorMessage = null)
        {
            if (source == null)
                throw new HusseinErrorResponseException(ErrorMessage == null ? $"{typeof(TSource).Name} Is Null" : ErrorMessage);
            return source;
        }
    }
}
