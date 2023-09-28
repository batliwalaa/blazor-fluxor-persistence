using Blazored.LocalStorage;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using FluentAssertions.Execution;
using Fluxor.Blazor.Persistence.Store;
using Fluxor.Blazor.Persistence.Tests.Helpers;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Fluxor.Blazor.Persistence.Tests;

public class PersistenceMiddlewareTests : TestContext
{
  private IStore? _store;
  private IDispatcher? _dispatcher;
  private readonly Mock<ILocalStoragePersistenceService> _mockLocalStoragePersistenceService;

  public PersistenceMiddlewareTests()
  {
    Services.Add(new ServiceDescriptor(typeof(ILocalStorageService), new Mock<ILocalStorageService>().Object));
    Services.AddSingleton<FakeNavigationManager>();
    Services.AddSingleton<NavigationManager>(s => s.GetRequiredService<FakeNavigationManager>());
    _mockLocalStoragePersistenceService = new Mock<ILocalStoragePersistenceService>();
  }

  [Fact]
  public async Task Routing_Feature()
  {
    // Arrange.
    Services.AddFluxor(o =>
      o.ScanAssemblies(typeof(StatePersistenceFailureState).Assembly)
        .UseRouting());
    _store = Services.GetRequiredService<IStore>();
    _dispatcher = Services.GetRequiredService<IDispatcher>();
    _store.InitializeAsync().Wait();

    _mockLocalStoragePersistenceService.Setup(x =>
        x.LoadAsync(It.IsAny<string>(), It.IsAny<Type>()))
      .ReturnsAsync(new RoutingState("https://localhost/route"));
    _mockLocalStoragePersistenceService.Setup(x =>
        x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()));

    PersistenceMiddleware sut = new (_mockLocalStoragePersistenceService.Object, new PersistOtions());
    var dispatcherSpy = new DispatcherSpy(_dispatcher);
    var storeSpy = new StoreSpy(_store);
    var navigationManager = Services.GetRequiredService<NavigationManager>();
    // Act.
    await sut.InitializeAsync(dispatcherSpy, storeSpy);

    // Assert.
    using (new AssertionScope())
    {
      dispatcherSpy.DispatchTimes.Should().Be(Times.Once());
      storeSpy.FeaturesTimes.Should().Be(Times.Once());

      _mockLocalStoragePersistenceService.Verify(x =>
        x.LoadAsync(It.IsAny<string>(), It.IsAny<Type>()), Times.Once);
      _mockLocalStoragePersistenceService.Verify(x =>
        x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
      navigationManager.Uri.Should().Be("https://localhost/route");
    }
  }

  [Fact]
  public async Task LoadAsync_Failure()
  {
    // Arrange.
    Services.AddFluxor(o =>
      o.ScanAssemblies(typeof(StatePersistenceFailureState).Assembly));

    _store = Services.GetRequiredService<IStore>();
    _store.AddFeature(new StatePersistenceFailureFeature());
    _dispatcher = Services.GetRequiredService<IDispatcher>();
    _store.InitializeAsync().Wait();

    _mockLocalStoragePersistenceService.Setup(x =>
        x.LoadAsync(It.IsAny<string>(), It.IsAny<Type>()))
      .ThrowsAsync(new Exception());
    _mockLocalStoragePersistenceService.Setup(x =>
        x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()));

    PersistenceMiddleware sut = new(_mockLocalStoragePersistenceService.Object, new PersistOtions());
    var dispatcherSpy = new DispatcherSpy(_dispatcher);
    var storeSpy = new StoreSpy(_store);
    var navigationManager = Services.GetRequiredService<NavigationManager>();

    // Act.
    await sut.InitializeAsync(dispatcherSpy, storeSpy);

    // Assert.
    using (new AssertionScope())
    {
      dispatcherSpy.DispatchTimes.Should().Be(Times.Once());
      dispatcherSpy.DispatchedActions.Single().Should().BeOfType(typeof(LoadPersistedStateFailureAction));
      dispatcherSpy.DispatchedActions.Single()
        .As<LoadPersistedStateFailureAction>().FeatureName
        .Should()
        .Be("@StatePersistence");
      storeSpy.FeaturesTimes.Should().Be(Times.Once());

      _mockLocalStoragePersistenceService.Verify(x =>
        x.LoadAsync(It.IsAny<string>(), It.IsAny<Type>()), Times.Once);
    }
  }

  [Fact]
  public async Task SaveAsync_Failure()
  {
    // Arrange.
    Services.AddFluxor(o =>
      o.ScanAssemblies(typeof(StatePersistenceFailureState).Assembly)
        .UseRouting());

    _store = Services.GetRequiredService<IStore>();
    _store.AddFeature(new StatePersistenceFailureFeature());
    _dispatcher = Services.GetRequiredService<IDispatcher>();
    _store.InitializeAsync().Wait();

    _mockLocalStoragePersistenceService.Setup(x =>
        x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()))
      .Throws(new Exception());

    PersistenceMiddleware sut = new(_mockLocalStoragePersistenceService.Object, new PersistOtions());
    var dispatcherSpy = new DispatcherSpy(_dispatcher);
    var storeSpy = new StoreSpy(_store);
    var navigationManager = Services.GetRequiredService<NavigationManager>();

    // Act.
    await sut.InitializeAsync(dispatcherSpy, storeSpy);

    // Assert.
    using (new AssertionScope())
    {
      dispatcherSpy.Should().NotBeNull();
      dispatcherSpy.DispatchTimes.Should().Be(Times.Once());
      dispatcherSpy.DispatchedActions
        .Single(x => x is SavePersistedStateFailureAction)
        .Should()
        .NotBeNull();
      dispatcherSpy.DispatchedActions
        .Single(x => x is SavePersistedStateFailureAction)
        .As<SavePersistedStateFailureAction>().FeatureName
        .Should()
        .Be("@routing");
      storeSpy.FeaturesTimes.Should().Be(Times.Once());

      _mockLocalStoragePersistenceService.Verify(x =>
        x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }
  }

  [Fact]
  public async Task Routing_Feature_With_PersistRoutes_False()
  {
    // Arrange.
    Services.AddFluxor(o =>
      o.ScanAssemblies(typeof(StatePersistenceFailureState).Assembly)
        .UseRouting());
    _store = Services.GetRequiredService<IStore>();
    _dispatcher = Services.GetRequiredService<IDispatcher>();
    _store.InitializeAsync().Wait();

    _mockLocalStoragePersistenceService.Setup(x =>
        x.LoadAsync(It.IsAny<string>(), It.IsAny<Type>()))
      .ReturnsAsync(new RoutingState("https://localhost/route"));
    _mockLocalStoragePersistenceService.Setup(x =>
        x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()));

    PersistenceMiddleware sut = new(_mockLocalStoragePersistenceService.Object, new PersistOtions() { PersistRoutes = false });
    var dispatcherSpy = new DispatcherSpy(_dispatcher);
    var storeSpy = new StoreSpy(_store);
    // Act.
    await sut.InitializeAsync(dispatcherSpy, storeSpy);

    // Assert.
    using (new AssertionScope())
    {
      dispatcherSpy.DispatchTimes.Should().Be(Times.Never());
      storeSpy.FeaturesTimes.Should().Be(Times.Once());

      _mockLocalStoragePersistenceService.Verify(x =>
        x.LoadAsync(It.IsAny<string>(), It.IsAny<Type>()), Times.Never);
      _mockLocalStoragePersistenceService.Verify(x =>
        x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
    }
  }
}
