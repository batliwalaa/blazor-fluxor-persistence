using Fluxor.Blazor.Persistence.BrowserStorage;
using System;
using System.Text.Json;
using System.Threading.Tasks;

//[assembly: InternalsVisibleTo("Fluxor.Blazor.Persistence.Tests")]
namespace Fluxor.Blazor.Persistence
{
  internal sealed class LocalStoragePersistenceService : ILocalStoragePersistenceService
  {
    private readonly IBrowserStorage _browserStorage;
    private readonly PersistOtions _persistOptions;

    public LocalStoragePersistenceService(
      IBrowserStorage browserStorage,
      PersistOtions persistOptions) =>
        (_browserStorage, _persistOptions) = (browserStorage, persistOptions);

    public async Task SaveAsync<T>(string key, T state)
    {
      await _browserStorage.SetItemAsync($"{_persistOptions.PersistenceKey}_{key}", JsonSerializer.Serialize(state, typeof(T)));
    }

    public async Task<object?> LoadAsync(string key, Type featureType)
    {
      string featureState = await _browserStorage.GetItemAsync($"{_persistOptions.PersistenceKey}_{key}");

      if (string.IsNullOrWhiteSpace(featureState))
      {
        return null;
      }

      return JsonSerializer.Deserialize(featureState ?? string.Empty, featureType);
    }
  }
}
