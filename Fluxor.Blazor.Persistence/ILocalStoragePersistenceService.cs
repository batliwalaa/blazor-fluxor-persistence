using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Fluxor.Blazor.Persistence;

internal interface ILocalStoragePersistenceService
{
  Task SaveAsync<T>(string key, T state);
  Task<object?> LoadAsync(string key, Type featureType);
}
