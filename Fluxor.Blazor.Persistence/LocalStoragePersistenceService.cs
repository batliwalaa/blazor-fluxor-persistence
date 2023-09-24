using Blazored.LocalStorage;
using System.Text.Json;

namespace Fluxor.Blazor.Persistence;

internal sealed class LocalStoragePersistenceService
{
  private readonly ILocalStorageService _localStorageService;
  private readonly PersistOtions _persistOptions;

  public LocalStoragePersistenceService(
    ILocalStorageService localStorageService,
    PersistOtions persistOptions) =>
      (_localStorageService, _persistOptions) = (localStorageService, persistOptions);

  public async Task SaveAsync<T>(string key, T state)
  {
    await _localStorageService.SetItemAsync($"{_persistOptions.PersistenceKey}_{key}", JsonSerializer.Serialize(state, typeof(T)));
  }

  public async Task<object?> LoadAsync(string key, Type featureType)
  {
    string featureState = await _localStorageService.GetItemAsync<string>($"{_persistOptions.PersistenceKey}_{key}");

    if (string.IsNullOrWhiteSpace(featureState))
    {
      return null;
    }

    return JsonSerializer.Deserialize(featureState ?? string.Empty, featureType);
  }
}
