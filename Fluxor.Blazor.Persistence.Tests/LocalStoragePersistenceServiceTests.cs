using Blazored.LocalStorage;
using FluentAssertions;
using Fluxor.Blazor.Persistence.Store;
using Moq;

namespace Fluxor.Blazor.Persistence.Tests;

public class LocalStoragePersistenceServiceTests
{
  private Mock<ILocalStorageService> _mockLocalStorageService;
  private ILocalStoragePersistenceService _sut;
  private PersistOtions _persistOtions = new();

  public LocalStoragePersistenceServiceTests()
  {
    _mockLocalStorageService = new Mock<ILocalStorageService>();
    _sut = new LocalStoragePersistenceService(
      _mockLocalStorageService.Object,
      _persistOtions);
  }

  [Fact]
  public async Task SaveAsync()
  {
    // Arrange.
    var state = new { };
    var key = "Cart";
    _mockLocalStorageService
      .Setup(x =>
        x.SetItemAsync(
          $"Fluxor.Blazor.Persistence_{key}",
          It.IsAny<object>(),
          CancellationToken.None))
      .Verifiable();

    // Act.
    await _sut.SaveAsync(key, state);

    // Assert.
    _mockLocalStorageService
      .Verify(
        x => x.SetItemAsync(
          $"Fluxor.Blazor.Persistence_{key}",
          It.IsAny<object>(),
          CancellationToken.None),
        Times.Once);
  }

  [Fact]
  public async Task LoadAsync_Return_Null()
  {
    // Arrange.
    var key = "Cart";
    _mockLocalStorageService
      .Setup(x =>
        x.GetItemAsync<string>(
          $"Fluxor.Blazor.Persistence_{key}",
          CancellationToken.None))
      .ReturnsAsync(string.Empty)
      .Verifiable();

    // Act.
    var resultState =
      await _sut.LoadAsync(key, typeof(StatePersistenceFailureState));

    // Assert.
    _mockLocalStorageService
      .Verify(x =>
        x.GetItemAsync<string>(
          $"Fluxor.Blazor.Persistence_{key}",
          CancellationToken.None),
        Times.Once);
    resultState.Should().BeNull();
  }

  [Fact]
  public async Task LoadAsync_Return_State()
  {
    // Arrange.
    var serializedState = "{\u0022Errors\u0022:[]}";
    var key = "StatePersistence";
    _mockLocalStorageService
      .Setup(x =>
        x.GetItemAsync<string>(
          $"Fluxor.Blazor.Persistence_{key}",
          CancellationToken.None))
      .ReturnsAsync(serializedState)
      .Verifiable();

    // Act.
    var resultState =
      await _sut.LoadAsync(key, typeof(StatePersistenceFailureState));

    // Assert.
    _mockLocalStorageService
      .Verify(x =>
        x.GetItemAsync<string>(
          $"Fluxor.Blazor.Persistence_{key}",
          CancellationToken.None),
        Times.Once);
    resultState.Should().NotBeNull();
    resultState.Should().BeOfType<StatePersistenceFailureState>();
    (resultState as StatePersistenceFailureState)?.Errors.Count.Should().Be(0);
  }
}
