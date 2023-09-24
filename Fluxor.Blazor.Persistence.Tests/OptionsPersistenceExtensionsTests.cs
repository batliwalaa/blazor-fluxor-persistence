using Blazored.LocalStorage;
using FluentAssertions;
using Fluxor.Blazor.Persistence.Tests.helpers;
using Fluxor.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;

namespace Fluxor.Blazor.Persistence.Tests;

public class OptionsPersistenceExtensionsTests : TestContext
{

  public class PersistenceWithLocalStorage : OptionsPersistenceExtensionsTests
  {
    [Fact]
    public void When_UsePersistence_Called_On_ServiceCollection()
    {
      // Arrange.
      FluxorOptions options = new(Services);
      Mock<IJSRuntime> mockJsRuntime = new();
      Services.Add(new ServiceDescriptor(typeof(IJSRuntime), mockJsRuntime.Object));

      // Act.
      options.UsePersistence();

      // Assert.
      Services.GetRequiredService<PersistOtions>().Should().NotBeNull();
      Services.GetRequiredService<PersistenceMiddleware>().Should().NotBeNull();
      Services.GetRequiredService<ILocalStorageService>().Should().NotBeNull();
      Services.GetRequiredService<LocalStoragePersistenceService>().Should().NotBeNull();
    }
  }
}
