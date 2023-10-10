using Microsoft.JSInterop;

namespace Fluxor.Blazor.Persistence.BrowserStorage
{
  internal class SessionBrowserStorage : BrowserStorage
  {
    public SessionBrowserStorage(IJSRuntime jSRuntime)
      : base("sessionStorage", jSRuntime)
    {
    }
  }
}
