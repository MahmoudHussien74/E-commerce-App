using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_commerce.Core.Common;

namespace E_commerce.Api.Abstraction;

public static class ResultsExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot create a problem result from a successful operation.");

        var problem = Results.Problem(statusCode: result.Error.statusCode);

        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
               "errors",
                new[]
                {
                    new
                    {
                        result.Error.Code,
                        result.Error.Message
                    }
                }
            }
        };
        return new ObjectResult(problemDetails);
    }
}
