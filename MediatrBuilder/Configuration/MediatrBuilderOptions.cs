using System.Reflection;

namespace MediatrBuilder.Configuration;

public class MediatrBuilderOptions
{
    internal List<Assembly> AssembliesToRegister { get; private set; } = new();
    internal bool UseFluentValidation { get; private set; }
    
    /// <summary>
    /// Register various handlers from assemblies
    /// </summary>
    /// <param name="assemblies">Assemblies to scan</param>
    /// <returns>MediatrBuilderOptions</returns>
    public MediatrBuilderOptions RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);
        return this;
    }

    /// <summary>
    /// Add a mediatr pipeline with integrated fluent validation 
    /// </summary>
    /// <param name="enable">True to enable, false otherwise</param>
    /// <returns>MediatrBuilderOptions</returns>
    public MediatrBuilderOptions AddFluentValidation(bool enable)
    {
        UseFluentValidation = enable;
        return this;
    }
}