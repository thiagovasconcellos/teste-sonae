using Microsoft.AspNetCore.Mvc;
using TestMcSonae.DTOs;

namespace TestMcSonae.Extensions
{
    public static class ControllerExtensions
    {
        public static ActionResult<ApiResponse<T>> ApiResponse<T>(this ControllerBase controller, ApiResponse<T> response)
        {
            return controller.StatusCode(response.StatusCode, response);
        }
    }
}