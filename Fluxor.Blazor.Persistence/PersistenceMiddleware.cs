using Fluxor.Blazor.Persistence.Store;
using Fluxor.Blazor.Web.Middlewares.Routing;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fluxor.Blazor.Persistence.Tests")]
namespace Fluxor.Blazor.Persistence;

internal sealed class PersistenceMiddleware : Middleware
{
  private IStore? _store;
  private readonly ILocalStoragePersistenceService _localStoragePersistenceService;
  private readonly PersistOtions _persistOtions;
  private IDispatcher? _dispatcher;
  private readonly object _syncRoot = new();

  public PersistenceMiddleware(
    ILocalStoragePersistenceService localStoragePersistenceService,
    PersistOtions persistOtions)
  {
    _localStoragePersistenceService = localStoragePersistenceService;
    _persistOtions = persistOtions;
  }

  public override async Task InitializeAsync(IDispatcher dispatcher, IStore store)
  {
    _store = store;
    _dispatcher = dispatcher;

    IEnumerable<IFeature> features = _store.Features.Values;

    foreach (IFeature feature in features.OrderBy(x => x.GetName()))
    {
      try
      {
        string featureName = feature.GetName().ToLower();

        if (featureName != "@routing" || (featureName == "@routing" && _persistOtions.PersistRoutes))
        {
          feature.StateChanged += Feature_StateChanged;
          var featureState = await _localStoragePersistenceService.LoadAsync(
          feature.GetName(),
          feature.GetStateType()
          ).ConfigureAwait(false);

          if (featureState != null)
          {
            feature.RestoreState(featureState);
          }

          if (feature.GetName().ToLower() == "@routing")
          {
            string? routeUri = (featureState as RoutingState)?.Uri;

            if (!string.IsNullOrWhiteSpace(routeUri))
            {
              _dispatcher.Dispatch(new GoAction(routeUri));
            }
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
          string featureName = stateChangeFeature.GetName().ToLower();
          if (featureName != "@statepersistence")
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
