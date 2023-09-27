using Blazored.LocalStorage;
using Fluxor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Fluxor.Blazor.Persistence;

public static class OptionsPersistenceExtensions
{
  public static FluxorOptions UsePersistence(
    this FluxorOptions options,
    Action<PersistOtions>? configurePersistOptions = null)
  {
    PersistOtions persistOptions = new();
    configurePersistOptions?.Invoke(persistOptions);

    options.AddMiddleware<PersistenceMiddleware>();
    options.Services.Add(new ServiceDescriptor(
        typeof(PersistOtions), persistOptions));

    if (persistOptions.PersistenceType == PersistenceType.LocalStorage)
    {
      options.Services.AddBlazoredLocalStorageAsSingleton(options =>
        options.JsonSerializerOptions.WriteIndented = true);

      options.Services.Add(new ServiceDescriptor(
        typeof(ILocalStoragePersistenceService),
        typeof(LocalStoragePersistenceService),
        ServiceLifetime.Singleton));
    }

    return options;
  }
}
