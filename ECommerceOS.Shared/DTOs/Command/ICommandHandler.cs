using MediatR;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.Shared.DTOs.Command;

public interface ICommandHandler<in TCommand, TResponse> 
    : IRequestHandler<TCommand, Result<TResponse>> where TCommand : ICommand<TResponse>;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result.Result> where TCommand 
    : ICommand;