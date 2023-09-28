﻿using Blazored.LocalStorage;
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

    if (IsValidPersistenceType(persistOptions))
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

  private static bool IsValidPersistenceType(PersistOtions persistOtions)
  {
    if (persistOtions.PersistenceType == PersistenceType.LocalStorage)
    {
      return true;
    }

    throw new InvalidOperationException($"{persistOtions.PersistenceType} is not supported.");
  }
}
