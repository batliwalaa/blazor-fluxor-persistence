namespace Fluxor.Blazor.Persistence.Store;

public class LoadPersistedStateFailureAction
{
  public string ActionType => "Load";
  public string FeatureName { get; private set; } = string.Empty;
  public Exception Exception { get; private set; }

  public LoadPersistedStateFailureAction(
    string featureName,
    Exception exception
   ) => (FeatureName, Exception) = (featureName, exception);
}
