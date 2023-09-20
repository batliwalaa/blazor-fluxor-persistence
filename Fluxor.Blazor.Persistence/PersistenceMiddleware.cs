using Fluxor.Blazor.Web.Middlewares.Routing;
using System.Text.Json;

namespace Fluxor.Blazor.Persistence;

internal sealed class PersistenceMiddleware : Middleware
{
  private IStore? _store;
  private readonly LocalStoragePersistenceService _localStoragePersistenceService;
  private IDispatcher? _dispatcher;
  private readonly object SyncRoot = new();

  public PersistenceMiddleware(
    LocalStoragePersistenceService localStoragePersistenceService)
  {
    _localStoragePersistenceService = localStoragePersistenceService;
  }

  public override async Task InitializeAsync(IDispatcher dispatcher, IStore store)
  {
    _store = store;
    _dispatcher = dispatcher;

    foreach (IFeature feature in _store.Features.Values.OrderBy(x => x.GetName()))
    {
      string featureState = await _localStoragePersistenceService.LoadAsync(feature.GetName()).ConfigureAwait(false);
      if (!string.IsNullOrWhiteSpace(featureState))
      {
        var state = JsonSerializer.Deserialize(featureState, feature.GetStateType());
        feature.RestoreState(state);

        if (feature.GetName() == "@routing")
        {
          string? routeUri = (state as RoutingState)?.Uri;

          if (!string.IsNullOrWhiteSpace(routeUri))
          {
            _dispatcher.Dispatch(new GoAction(routeUri));
          }
        }
      }
    }

    await Task.CompletedTask;
  }

  public override void AfterDispatch(object action)
  {
    lock (SyncRoot)
    {
      if (_store != null)
      {
        foreach (IFeature feature in _store.Features.Values.OrderBy(x => x.GetName()))
        {
          _localStoragePersistenceService.SaveAsync(
            feature.GetName(), feature.GetState()).ConfigureAwait(false);
        }
      }
    }
  }
}
