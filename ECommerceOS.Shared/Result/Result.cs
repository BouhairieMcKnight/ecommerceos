using System.Runtime.CompilerServices;

namespace ECommerceOS.Shared.Result;

public class Result 
{
   public bool IsSuccess { get; protected set; }
   public Error? Error { get; protected set; }
   protected readonly IEnumerable<Error> _errors = [];
   
   protected Result(bool isSuccess, Error error)
   {
      IsSuccess = isSuccess;
      Error = error;
   }

   protected Result(bool isSuccess, IEnumerable<Error> errors)
   {
      IsSuccess = isSuccess;
      _errors = errors;
   }

   public static Result Success() => new(true, Error.None);
   public static Result Failure(Error error) => new(false, error);
   public static Result Failure(Error[] errors) => new(false, errors);
}

public class Result<T> : Result
{
   public T? Value { get; protected set; }

   private Result(bool isSuccess, T? value, Error error):
      base(isSuccess, error)
   {
      IsSuccess = isSuccess;
      Value = value;
      Error = error;
   }

   private Result(bool isSuccess, T? value, Error[] errors)
      : base(isSuccess, errors)
   {
      IsSuccess = isSuccess;
      Value = value;
   }

   public static Result<T> Success(T value) => new (true, value, Error.None);
   public new static Result<T> Failure(Error error) => new (false, default, error);
   public new static Result<T> Failure(Error[] errors) => new(false, default, errors);
   public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
   {
      return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
   }
}