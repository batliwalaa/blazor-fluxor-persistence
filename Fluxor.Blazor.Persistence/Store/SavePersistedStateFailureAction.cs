using System;

namespace Fluxor.Blazor.Persistence.Store
{
  public class SavePersistedStateFailureAction
  {
    public string ActionType => "Save";
    public string FeatureName { get; private set; } = string.Empty;
    public Exception Exception { get; private set; }

    public SavePersistedStateFailureAction(
      string featureName,
      Exception exception
     ) => (FeatureName, Exception) = (featureName, exception);
  }
}
