using System.Threading;
using System.Threading.Tasks;

namespace Fluxor.Blazor.Persistence.BrowserStorage
{
  internal interface IBrowserStorage
  {
    ValueTask ClearAsync(CancellationToken cancellationToken = default);
    ValueTask<string> GetItemAsync(string key, CancellationToken cancellationToken = default);
    ValueTask SetItemAsync(string key, string data, CancellationToken cancellationToken = default);
  }
}
