using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class ErrorController : Controller
    {

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            };


            switch (statusCode)
            {
                case 403:
                    errorViewModel.Message = "You do not have permission to access this page.";
                    return View("AccessDenied", errorViewModel);
                case 404:
                    errorViewModel.Message = "Page Not Found";
                    return View("NotFound", errorViewModel);
                default:
                    errorViewModel.Message = "An unexpected error has occurred.";
                    return View("Error", errorViewModel);
            }

        }


        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var errorViewModel = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                Message = exceptionHandlerPathFeature?.Error.Message,
                StackTrace = exceptionHandlerPathFeature?.Error.StackTrace
            };

            return View("InternalError", errorViewModel);
        }

   
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
