# MediatR.Patterns.Builder

![Build](https://img.shields.io/github/actions/workflow/status/Matozap/MediatR.Patterns.Builder/build.yml?style=for-the-badge&logo=github&color=0D7EBF)
![Commits](https://img.shields.io/github/last-commit/Matozap/MediatR.Patterns.Builder?style=for-the-badge&logo=github&color=0D7EBF)
![Package](https://img.shields.io/nuget/dt/MediatR.Patterns.Builder?style=for-the-badge&logo=nuget&color=0D7EBF)


MediatR.Patterns.Builder Sits on top of MediatR and exposes PreProcess, Process and PostProcess methods to handlers instead of only exposing the Handle method allowing to implement handlers using the builder pattern.


It simplifies common usage of Mediatr integrating interfaces to differentiate between commands and queries and also automatically creating a 
behavior pipeline to apply FluentValidation checks. 

It is as fast as the vanilla version and helps reduce the development process by having a defined pattern within handlers.


------------------------------

### Usage

#### Handlers

Inherit from the abstract class BuilderRequestHandler setting what the request and the response are.

In below example, if the cache has a value, it returns from the `PreProcess` method directly without going to the `Process` method but 
if not (and returns `null`) the `Process` method and then `PostProcess` method will execute in sequence.

```csharp

public class GetAllCountriesHandler : BuilderRequestHandler<GetAllCountries, List<CountryData>>
{
    private readonly ILogger<GetAllCountriesHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllCountriesHandler(ICache cache, ILogger<GetAllCountriesHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    protected override async Task<List<CountryData>> PreProcess(GetAllCountries request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey();

        var cachedValue = await _cache.GetCacheValueAsync<List<CountryData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<List<CountryData>> Process(GetAllCountries request, CancellationToken cancellationToken = default)
    {
        return await GetAllCountries();
    }

    protected override Task PostProcess(GetAllCountries request, List<CountryData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
}

```

#### Differentiate between Query and Command

- For queries instead of using `IRequest` use `IQuery`
- For commands instead of using `IRequest` use `ICommand`

By doing this can further expand your pipeline behaviors and act according to the type of request.

```csharp

public class GetAllCountries : IQuery<List<CountryData>>
{
    ... some properties
}

public class AddCountry : ICommand<List<CountryData>>
{
    ... some properties
}

```

#### Setting FluentValidation

Just create the validations related to the request you will be handling like you normally do:

```csharp

public class GetAllCountries : IQuery<List<CountryData>>
{
    ... some properties
}

public class GetAllCitiesValidator : AbstractValidator<GetAllCountries>
{
    public GetAllCitiesValidator()
    {
        RuleFor(x => x).NotNull();
        
        ... more fluentValidation rules
    }
}

```


### Configuration

#### Dependency Injection 

Just as Mediatr, you can pass the assemblies to scan for handlers and those assemblies can be also used by FluentValidation if `AddFluentValidation`
is set to true so it internally creates a pipeline behavior to validate before going to the handler.

```csharp
services.AddMediatrBuilder(options =>
{
    options.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly(), ... more assemblies)
    options.AddFluentValidation(true);
});
```

###


## Contributing

It is simple, as all things should be:

1. Clone it
2. Improve it
3. Make pull request

## Credits

- Initial development by [Slukad](https://github.com/Slukad)
