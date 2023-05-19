using MediatR;

namespace MediatrBuilder.Interfaces;

public interface ICommand<out TCommandResult> : IRequest<TCommandResult>
{
}