using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fluxor.Blazor.Persistence.BrowserStorage
{
  internal abstract class BrowserStorage : IBrowserStorage
  {
    private readonly string _browserStorageNotAvailable =
      "Unable to access browser storage, check browser settings";
    private readonly IJSRuntime _jSRuntime;
    private readonly string _browserStorageKey;

    public BrowserStorage(string browserStorageKey, IJSRuntime jSRuntime)
    {
      _jSRuntime = jSRuntime;
      _browserStorageKey = browserStorageKey;
    }

    public async ValueTask ClearAsync(CancellationToken cancellationToken = default)
    {
      await ExecuteAsync(() => _jSRuntime.InvokeVoidAsync($"{_browserStorageKey}.clear", cancellationToken));
    }

    public async ValueTask<string> GetItemAsync(string key, CancellationToken cancellationToken = default)
    {
      return await ExecuteAsync(() => _jSRuntime.InvokeAsync<string>($"{_browserStorageKey}.getItem", cancellationToken, key));
    }

    public async ValueTask SetItemAsync(string key, string data, CancellationToken cancellationToken = default)
    {
      await ExecuteAsync(() => _jSRuntime.InvokeVoidAsync($"{_browserStorageKey}.setItem", cancellationToken, key, data));
    }

    private bool IsBrowserStorageNotAvailableException(Exception ex) =>
      ex.Message.ToLower().Contains($"failed to read the '{_browserStorageKey}'");

    private async ValueTask<T> ExecuteAsync<T>(Func<ValueTask<T>> func)
    {
      try
      {
        return await func();
      }
      catch (Exception ex)
      {
        if (IsBrowserStorageNotAvailableException(ex))
        {
          throw new BrowserStorageNotAvailableException(_browserStorageNotAvailable, ex);
        }
        throw;
      }
    }

    private async ValueTask ExecuteAsync(Func<ValueTask> func)
    {
      try
      {
        await func();
      }
      catch (Exception ex)
      {
        if (IsBrowserStorageNotAvailableException(ex))
        {
          throw new BrowserStorageNotAvailableException(_browserStorageNotAvailable, ex);
        }
        throw;
      }
    }
  }
}
