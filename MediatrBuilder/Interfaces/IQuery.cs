using MediatR;

namespace MediatrBuilder.Interfaces;

public interface IQuery<out TIQueryResult> : IRequest<TIQueryResult>
{
}