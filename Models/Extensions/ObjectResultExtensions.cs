using Microsoft.AspNetCore.Mvc;
using Shared.Api.Result;

namespace Server.Models.Extensions
{
    public static class ObjectResultExtensions
    {
        public static ObjectResult ToObjectResult (this PlainResult result) => new(result) { StatusCode = result.StatusCode };
    }
}
