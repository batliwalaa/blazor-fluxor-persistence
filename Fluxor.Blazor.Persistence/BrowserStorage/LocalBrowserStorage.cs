using Microsoft.JSInterop;

namespace Fluxor.Blazor.Persistence.BrowserStorage
{
  internal class LocalBrowserStorage : BrowserStorage
  {
    public LocalBrowserStorage(IJSRuntime jSRuntime)
      : base("localStorage", jSRuntime)
    {
    }
  }
}
