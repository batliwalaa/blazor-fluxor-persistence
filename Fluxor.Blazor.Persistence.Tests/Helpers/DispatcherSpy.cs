using FluentAssertions;
using Moq;

namespace Fluxor.Blazor.Persistence.Tests.Helpers;

public class DispatcherSpy : IDispatcher
{
  private readonly IDispatcher _innerDispatcher;
  private int _dispatchMethodTimesCalled = 0;
  private IList<object> _dispatchedActions = new List<object>();

  public DispatcherSpy(IDispatcher dispatcher)
  {  _innerDispatcher = dispatcher; }

  public event EventHandler<ActionDispatchedEventArgs> ActionDispatched
  {
    add { _innerDispatcher.ActionDispatched += value; }
    remove { _innerDispatcher.ActionDispatched -= value; }
  }

  public void Dispatch(object action)
  {
    _innerDispatcher.Dispatch(action);
    _dispatchedActions.Add(action);
    _dispatchMethodTimesCalled++;
  }

  public Times DispatchTimes => Times.Exactly(_dispatchMethodTimesCalled);

  public IList<object> DispatchedActions => _dispatchedActions;
}
