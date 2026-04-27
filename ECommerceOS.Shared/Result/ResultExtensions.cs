using System.Linq.Expressions;

namespace ECommerceOS.Shared.Result;

public static class ResultExtensions
{
    extension<TIn>(Result<TIn> result)
    {
        public Result<TOut> Bind<TOut>(Func<TIn, Result<TOut>> bind)
        {
            return result.IsSuccess ?
                bind(result.Value!) :
                Result<TOut>.Failure(result.Error!);
        }

        public Result<TIn> Tap(Action<TIn> action)
        {
            if (result.IsSuccess)
            {
                action(result.Value!);
            }

            return result;
        }

        public async Task<Result<TOut>> BindAsync<TOut>(Func<TIn, Task<Result<TOut>>> bind)
        {
            return result.IsSuccess ?
                await bind(result.Value!) :
                Result<TOut>.Failure(result.Error!); 
        }
        

        public async Task<Result<TIn>> TapAsync(Func<TIn, Task> action)
        {
            if (result.IsSuccess)
            {
                await action.Invoke(result.Value!);
            }

            return result;
        }
        
        public Result<TOut> TryCatch<TOut>(Func<TIn, TOut> func, Error error)
        {
            try
            {
                return result.IsSuccess ?
                    Result<TOut>.Success(func(result.Value!)) :
                    Result<TOut>.Failure(result.Error!);
            }
            catch
            {
                return Result<TOut>.Failure(error);
            } 
        }
    }

    extension<TIn>(Task<Result<TIn>> task)
    {
        public async Task<Result<TIn>> Tap(Action<TIn> action)
        {
            var result = await task;
            return result.Tap(action);
        }

        public async Task<Result<TIn>> TapAsync(Func<TIn, Task> action)
        {
            var result = await task;
            if (result.IsSuccess)
            {
                await action(result.Value!);
            }
            
            return result;
        }

        public async Task<Result<TOut>> Bind<TOut>(Func<TIn, Result<TOut>> func)
        {
            var result = await task;
            return result.Bind(func);
        }

        public async Task<Result<TOut>> BindAsync<TOut>(Func<TIn, Task<Result<TOut>>> func)
        {
            var result = await task;
            return result.IsSuccess ?
                Result<TOut>.Failure(result.Error!) :
                await func(result.Value!);
        }
    }

    extension(Result result)
    {
        public IResult ToProblemDetails()
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException();
            }

            return Results.Problem(
                statusCode: GetStatus(result.Error!.Type),
                title: GetTitle(result.Error.Type),
                type: GetErrorType(result.Error.Type),
                extensions: new Dictionary<string, object?>
                {
                    ["errors"] = new[] { result.Error }
                }
            );

            static int GetStatus(ErrorType errorType) =>
                errorType switch
                {
                    ErrorType.Validation => StatusCodes.Status400BadRequest,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status500InternalServerError
                };

            static string GetTitle(ErrorType errorType) =>
                errorType switch
                {
                    ErrorType.Validation => "Bad Request",
                    ErrorType.NotFound => "Not Found",
                    ErrorType.Conflict => "Conflict",
                    _ => "Server Error"
                };

            static string GetErrorType(ErrorType errorType) =>
                errorType switch
                {
                    ErrorType.Validation => Problems.BadRequest,
                    ErrorType.NotFound => Problems.NotFound,
                    ErrorType.Conflict => Problems.Conflict,
                    _ => Problems.ServerError
                };
        }
    }
}