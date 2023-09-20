using System.Text.Json;

namespace Fluxor.Blazor.Persistence.Store;

public class LoadPersistedStateEffects
{
  private readonly LocalStoragePersistenceService _localStoragePersistenceService;

  public LoadPersistedStateEffects(LocalStoragePersistenceService localStoragePersistenceService)
  {
    _localStoragePersistenceService = localStoragePersistenceService;
  }

  [EffectMethod]
  public async Task LoadPersistedState(LoadPersistedStateAction action, IDispatcher _)
  {
    foreach (IFeature feature in action.Features.Values.OrderBy(x => x.GetName()))
    {
      string featureState = await _localStoragePersistenceService.LoadAsync(feature.GetName());
      if (!string.IsNullOrWhiteSpace(featureState))
      {
        var state = JsonSerializer.Deserialize(featureState, feature.GetStateType());
        feature.RestoreState(state);
      }
    }
  }
}
