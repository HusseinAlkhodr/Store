using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Store.DTO.Result;
using System.Net;

namespace Store.Middlewares
{
    public class HusseinErrorResponseException : Exception
    {
        public string Title { get; set; }
        public int ErrorCode { get; set; }
        public HusseinErrorResponseException(string title, int errorCode = StatusCodes.Status404NotFound) : base(title)
        {
            ErrorCode = errorCode;
            Title = title;
        }
    }
    public class ExceptionHandlerMiddleWare
    {
        private readonly IServiceProvider serviceProvider;
        private readonly RequestDelegate _next;

        private readonly IWebHostEnvironment _env;
        public ILogger Logger { get; }

        public ExceptionHandlerMiddleWare(IServiceProvider serviceProvider, RequestDelegate next,
            IWebHostEnvironment env, ILogger<ExceptionHandlerMiddleWare> Logger/*,IHttpContextAccessor httpContextAccessor*/)
        {
            this.serviceProvider = serviceProvider;
            this._next = next;
            this._env = env;
            this.Logger = Logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {

                await _next.Invoke(context);
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    var result = new BaseAPIResult
                    {
                        IsSuccess = false,
                        Message = "unauthenticated User",
                        Code = (int)HttpStatusCode.Unauthorized
                    };

                    await SendErrorResponse(context, result);
                }
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    var result = new BaseAPIResult
                    {
                        IsSuccess = false,
                        Message = "Access Denided",
                        Code = StatusCodes.Status403Forbidden
                    };

                    await SendErrorResponse(context, result);
                }
            }
            catch (HusseinErrorResponseException ex)
            {
                var result = new BaseAPIResult
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Code = ex.ErrorCode

                };
                await SendErrorResponse(context, result);
            }
            catch (ApplicationException ex)
            {
                Logger.LogCritical(ex?.Message + ex?.StackTrace);

                //error response
                var result = new BaseAPIResult
                {
                    Code = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    Message = ex?.Message ?? "Un expected error !!"
                };
                await SendErrorResponse(context, result);
            }
            catch (Exception ex)
            {

                if (this._env.EnvironmentName == Environments.Development)
                    throw;
                Logger?.LogCritical(ex?.Message + ex?.StackTrace);

                var result = new BaseAPIResult
                {
                    Code = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    Message = ex?.Message + ex?.InnerException?.Message + ex?.StackTrace ?? (this._env.EnvironmentName == Environments.Development ? ex.Message + ex.StackTrace : "Un expected error")
                };
                await SendErrorResponse(context, result);
            }
        }

        private async Task SendErrorResponse(HttpContext context, BaseAPIResult result)
        {

            context.Response.OnStarting((state) =>
            {
                context.Response.StatusCode = result.Code;
                context.Response.ContentType = "application/json";
                return Task.FromResult(0);
            }, null);
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result, serializerSettings));
            await Task.CompletedTask;
        }
    }
}
