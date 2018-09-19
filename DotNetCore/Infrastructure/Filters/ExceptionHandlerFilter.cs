using System;
using DotNetCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace DotNetCore.Infrastructure.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = GetActionResult((dynamic)context.Exception);
        }

        private IActionResult GetActionResult(ValidationException validationException)
        {
            ModelStateDictionary modelStateDictionary = new ModelStateDictionary();

            foreach (var memberName in validationException.ValidationResult.MemberNames)
            {
                modelStateDictionary.AddModelError(memberName, validationException.Message);
            }

            return new BadRequestObjectResult(modelStateDictionary);
        }

        private IActionResult GetActionResult(EntityNotFoundException entityNotFoundException)
        {
            return new NotFoundObjectResult(entityNotFoundException.Message);
        }

        private IActionResult GetActionResult(Exception exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}