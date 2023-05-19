using MediatR;

namespace MediatrBuilder;

public abstract class BuilderRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected abstract Task<TResponse> PreProcess(TRequest request, CancellationToken cancellationToken = default);

    protected abstract Task<TResponse> Process(TRequest request, CancellationToken cancellationToken = default);

    protected abstract Task PostProcess(TRequest request, TResponse response, CancellationToken cancellationToken = default);
    
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        var result = await PreProcess(request, cancellationToken);
        if (result != null) return result;

        result = await Process(request, cancellationToken);
        await PostProcess(request, result, cancellationToken);

        return result;
    }
}