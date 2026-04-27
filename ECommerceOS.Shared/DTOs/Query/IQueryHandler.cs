using MediatR;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.Shared.DTOs.Query;

public interface IQueryHandler<in TQuery, TResponse> :
    IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>;