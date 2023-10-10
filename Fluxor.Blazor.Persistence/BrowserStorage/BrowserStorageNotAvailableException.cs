using System;

namespace Fluxor.Blazor.Persistence.BrowserStorage
{
  public class BrowserStorageNotAvailableException : Exception
  {
    public BrowserStorageNotAvailableException() { }

    public BrowserStorageNotAvailableException(string message) : base(message) { }

    public BrowserStorageNotAvailableException(string message, Exception innerException) : base(message, innerException) { }
  }
}
