using FluentAssertions;
using Moq;

namespace Fluxor.Blazor.Persistence.Tests.Helpers;

public class StoreSpy : IStore
{
  private readonly IStore _innerStore;
  private int _featuresCalledTimesCount = 0;

  public StoreSpy(IStore store)
  {
    _innerStore = store ?? throw new ArgumentNullException(nameof(store));    
  }

  public IReadOnlyDictionary<string, IFeature> Features
  {
    get 
    {
      _featuresCalledTimesCount++;
      return _innerStore.Features;
    }
  }

  public Task Initialized => _innerStore.Initialized;

  public event EventHandler<Exceptions.UnhandledExceptionEventArgs> UnhandledException
  {
    add { _innerStore.UnhandledException += value; }
    remove { _innerStore.UnhandledException -= value; }
  }

  public void AddEffect(IEffect effect) => _innerStore.AddEffect(effect);

  public void AddFeature(IFeature feature) => _innerStore.AddFeature(feature);

  public void AddMiddleware(IMiddleware middleware) => _innerStore.AddMiddleware(middleware);

  public IDisposable BeginInternalMiddlewareChange() => _innerStore.BeginInternalMiddlewareChange();

  public IDisposable GetActionUnsubscriberAsIDisposable(object subscriber) => 
    _innerStore.GetActionUnsubscriberAsIDisposable(subscriber);

  public IEnumerable<IMiddleware> GetMiddlewares() => _innerStore.GetMiddlewares();

  public Task InitializeAsync() => _innerStore.InitializeAsync();

  public void SubscribeToAction<TAction>(object subscriber, Action<TAction> callback) =>
    _innerStore.SubscribeToAction(subscriber, callback);

  public void UnsubscribeFromAllActions(object subscriber) => _innerStore?.UnsubscribeFromAllActions(subscriber);

  public Times FeaturesTimes => Times.Exactly(_featuresCalledTimesCount);
}
