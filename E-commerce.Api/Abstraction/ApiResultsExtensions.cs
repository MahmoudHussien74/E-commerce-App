namespace E_commerce.Api.Abstraction;

public static class ResultsExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot create a problem result from a successful operation.");

        var problem = Results.Problem(statusCode: result.Error.StatusCode, title: result.Error.Code, detail: result.Error.Message);
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
        problemDetails.Status = result.Error.StatusCode;
        return new ObjectResult(problemDetails);
    }
}
