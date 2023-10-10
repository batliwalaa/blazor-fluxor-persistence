using Fluxor.Blazor.Persistence.BrowserStorage;
using Fluxor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fluxor.Blazor.Persistence
{
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

      if (IsValidPersistenceType(persistOptions))
      {
        options.Services.Add(new ServiceDescriptor(
            typeof(IBrowserStorage),
            GetStorageType(persistOptions.PersistenceType),
            ServiceLifetime.Singleton));

        options.Services.Add(new ServiceDescriptor(
          typeof(ILocalStoragePersistenceService),
          typeof(LocalStoragePersistenceService),
          ServiceLifetime.Singleton));
      }

      return options;
    }

    private static Type GetStorageType(PersistenceType persistenceType)
    {
      if (persistenceType == PersistenceType.SessionStorage)
      {
        return typeof(SessionBrowserStorage);
      }

      return typeof(LocalBrowserStorage);
    }

    private static bool IsValidPersistenceType(PersistOtions persistOtions)
    {
      if (persistOtions.PersistenceType == PersistenceType.LocalStorage || persistOtions.PersistenceType == PersistenceType.SessionStorage)
      {
        return true;
      }

      throw new InvalidOperationException($"{persistOtions.PersistenceType} is not supported.");
    }
  }
}
