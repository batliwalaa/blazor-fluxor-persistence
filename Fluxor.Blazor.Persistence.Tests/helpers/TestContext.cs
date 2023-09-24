using System.Diagnostics.CodeAnalysis;

namespace Fluxor.Blazor.Persistence.Tests.helpers;

public abstract class TestContext : IDisposable
{
  private bool disposed;

  public TestServiceProvider Services { get; }

  public TestContext()
  {
    Services = new TestServiceProvider();
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  [SuppressMessage("TestContext", "CA2012", Justification = "Explicitly suppressing message")]
  protected virtual void Dispose(bool disposing)
  {
    if (disposed || !disposing)
      return;

    disposed = true;

    _ = Services.DisposeAsync();

    Services.Dispose();
  }
}
