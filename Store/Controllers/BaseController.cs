using Microsoft.AspNetCore.Mvc;
using Store.Core.Unit;
using Store.DTO.Result;
using Store.Middlewares;
using Store.Models.Authenitication;
using System.Security.Claims;

namespace Store.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {

        }
        protected long? CurrentUserId
        {
            get
            {
                var Success = long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId);
                if (Success)
                    return userId;
                return null; ;
            }
        }

        /// <summary>
        /// Get Service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetService<T>() where T : class
        {
            return HttpContext.RequestServices.GetService<T>();
        }


        public Account _currentAccount { get; set; }

        public Account CurrentAccount
        {
            get
            {
                _currentAccount = _currentAccount ?? GetService<IUnitOfWork>()
                    .AccountRepository.Get(x => x.Id == CurrentUserId).Result;
                //.GetUserAsync(User).GetAwaiter().GetResult();
                return _currentAccount;
            }
        }

        /// <summary>
        /// return error response from model state
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected BaseAPIResult ErrorResponseFromModelState()
        {
            return new BaseAPIResult
            {
                IsSuccess = false,
                Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors?.FirstOrDefault()?.ErrorMessage ?? "Un Expected Error"
            };
        }

        protected BaseAPIResult BaseSuccessResponse(string message = null)
        {
            return new BaseAPIResult
            {
                IsSuccess = true,
                Message = message ?? "OPERATION SUCCESS"
            };
        }

        //protected BaseAPIResult SendErrorResponse(string message)
        //{
        //    throw new MetaErrorResponseException(message, 400);
        //}



        protected BaseAPIResult SendErrorResponse(int code, string message)
        {
            throw new HusseinErrorResponseException(message, code);
        }

        protected APIResult<T> SuccessResponse<T>(T data, string message = null)
        {
            return new APIResult<T>
            {
                IsSuccess = true,
                Message = message ?? "Task Completed Successfully",
                Data = data
            };
        }


        protected string CurrentRequestURL
        {
            get
            {
                return Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);
            }
        }
    }
}
