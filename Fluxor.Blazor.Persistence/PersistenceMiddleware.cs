using Fluxor.Blazor.Persistence.Store;
using Fluxor.Blazor.Web.Middlewares.Routing;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fluxor.Blazor.Persistence.Tests")]
namespace Fluxor.Blazor.Persistence;

internal sealed class PersistenceMiddleware : Middleware
{
  private IStore? _store;
  private readonly ILocalStoragePersistenceService _localStoragePersistenceService;
  private IDispatcher? _dispatcher;
  private readonly object _syncRoot = new();

  public PersistenceMiddleware(
    ILocalStoragePersistenceService localStoragePersistenceService)
  {
    _localStoragePersistenceService = localStoragePersistenceService;
  }

  public override async Task InitializeAsync(IDispatcher dispatcher, IStore store)
  {
    _store = store;
    _dispatcher = dispatcher;

    IEnumerable<IFeature> features = _store.Features.Values;

    foreach (IFeature feature in features.OrderBy(x => x.GetName()))
    {
      feature.StateChanged += Feature_StateChanged;
      try
      {
        var featureState = await _localStoragePersistenceService.LoadAsync(
          feature.GetName(),
          feature.GetStateType()
          ).ConfigureAwait(false);
        feature.RestoreState(featureState);

        if (feature.GetName() == "@routing")
        {
          string? routeUri = (featureState as RoutingState)?.Uri;

          if (!string.IsNullOrWhiteSpace(routeUri))
          {
            _dispatcher.Dispatch(new GoAction(routeUri));
          }
        }
      }
      catch (Exception ex)
      {
        _dispatcher.Dispatch(new LoadPersistedStateFailureAction(feature.GetName(), ex));
      }
    }

    await Task.CompletedTask;
  }

  private void Feature_StateChanged(object? feature, EventArgs e)
  {
    if (feature != null)
    {
      IFeature stateChangeFeature = (IFeature)feature;
      lock (_syncRoot)
      {
        try
        {
          if (stateChangeFeature.GetName() != "@StatePersistence")
          {
            _localStoragePersistenceService.SaveAsync(
              stateChangeFeature.GetName(), stateChangeFeature.GetState()).ConfigureAwait(false);
          }
        }
        catch (Exception ex)
        {
          _dispatcher?.Dispatch(new SavePersistedStateFailureAction(stateChangeFeature.GetName(), ex));
        }
      }
    }
  }
}
