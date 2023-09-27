# blazor-fluxor-persistence

Fluxor state persistence for web blazor application using LocalStorage.
Currently supports LocalStorage only.

## Usage
In Program.cs file Add UsePersistence to FluxorOptions

Install Fluxor.Blazor.Persistence

Add below code to the program.cs file

```C#
builder.Services
  .AddFluxor(o =>
  {
    o.ScanAssemblies(typeof(Program).Assembly)
    .UseRouting()
    .UsePersistence();
  });
```

### Exceptions
Any exception thrown when loading/saving the persist state are added to `StatePersistenceFailureState'
This errors can be retrieved from state

This exceptions are cleared on browser refresh or close.
The exceptions can also be cleared by dispatching action

e.g.
Razor component 'StateErrors.razor'

```C#
@page "/persist-state-errors"
@inherits FluxorComponent
@using Fluxor.Blazor.Persistence.Store
@inject IState<StatePersistenceFailureState> State
@inject IDispatcher Dispatcher;

<h3>Errors</h3>

@if (State.Value.Errors.Count == 0)
{
  <div>No Errors</div>
}
else 
{
  @foreach(var e in State.Value.Errors)
  {
    <div class="row">
      <span class="col1">@e.ActionType</span>
      <span class="col2">@e.FeatureName</span>
      <span class="col3">@e.Exception?.Message</span>
    </div>
  }

  <button class="btn btn-outline" @onclick="ClearErrors">Clear Errors</button>
}

@code {
  protected override void OnInitialized()
  {
    base.OnInitialized();
  }

  public void ClearErrors() 
  {
    Dispatcher.Dispatch(new ClearStatePersistenceFailureStateAction());
  }
}
```
