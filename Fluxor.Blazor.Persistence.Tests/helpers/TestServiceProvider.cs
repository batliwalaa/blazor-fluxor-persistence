using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Fluxor.Blazor.Persistence.Tests.helpers;

public sealed class TestServiceProvider : IServiceCollection, IServiceProvider, IDisposable, IAsyncDisposable
{
  private readonly IServiceCollection _serviceCollection;
  private readonly IServiceProvider _serviceProvider;
  private readonly IServiceScope? _serviceScope;
  private readonly IServiceProvider? _rootServiceProvider;

  public int Count => _serviceCollection.Count;

  public bool IsReadOnly => true;

  public ServiceDescriptor this[int index]
  {
    get => _serviceCollection[index];
    set => _serviceCollection[index] = value;
  }

  public TestServiceProvider()
  {
    _serviceCollection = new ServiceCollection();
    _rootServiceProvider = _serviceCollection.BuildServiceProvider();
    _serviceScope = _rootServiceProvider.CreateScope();
    _serviceProvider = _serviceScope.ServiceProvider;
  }

  public T GetService<T>() => (T?)GetService(typeof(T))!;

  public object? GetService(Type serviceType)
  {
    return _serviceProvider.GetService(serviceType);
  }

  public void Dispose()
  {
    if (_serviceScope is IDisposable serviceScope)
    {
      serviceScope.Dispose();
    }

    if (_rootServiceProvider is IDisposable rootServiceProvider)
    {
      rootServiceProvider.Dispose();
    }
  }

  public async ValueTask DisposeAsync()
  {
    if (_serviceScope is IAsyncDisposable serviceScope) await serviceScope.DisposeAsync();

    if (_rootServiceProvider is IAsyncDisposable rootServiceProvider) await rootServiceProvider.DisposeAsync();
  }

  public int IndexOf(ServiceDescriptor item) => _serviceCollection.IndexOf(item);

  public void Insert(int index, ServiceDescriptor item) => _serviceCollection.Insert(index, item);

  public void RemoveAt(int index) => _serviceCollection.RemoveAt(index);

  public void Add(ServiceDescriptor item) => _serviceCollection.Add(item);

  public void Clear() => _serviceCollection.Clear();

  public bool Contains(ServiceDescriptor item) => _serviceCollection.Contains(item);

  public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _serviceCollection.CopyTo(array, arrayIndex);

  public bool Remove(ServiceDescriptor item) => _serviceCollection.Remove(item);

  public IEnumerator<ServiceDescriptor> GetEnumerator() => _serviceCollection.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}