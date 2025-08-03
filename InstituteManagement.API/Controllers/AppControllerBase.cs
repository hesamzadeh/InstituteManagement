using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InstituteManagement.API.Controllers
{
    public abstract class AppControllerBase : ControllerBase
    {
        protected IActionResult ValidationError(string key, string message)
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError(key, message);
            return ValidationProblem(modelState);
        }
    }
}
