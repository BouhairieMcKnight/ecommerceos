using MediatR;
using ECommerceOS.Shared.Result;

namespace ECommerceOS.Shared.DTOs.Query;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;