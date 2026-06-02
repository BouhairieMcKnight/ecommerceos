using MediatR;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.Shared.DTOs.Command;

public interface ICommand : IRequest<Result.Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;