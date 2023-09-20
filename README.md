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
    .UsePersistence()
    .UseReduxDevTools();
  });
```
