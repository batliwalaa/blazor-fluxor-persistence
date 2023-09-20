using Blazored.LocalStorage;
using System.Text.Json;

namespace Fluxor.Blazor.Persistence;

public sealed class LocalStoragePersistenceService
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

  public async Task<string> LoadAsync(string key)
  {
    return await _localStorageService.GetItemAsync<string>($"{_persistOptions.PersistenceKey}_{key}");
  }
}
