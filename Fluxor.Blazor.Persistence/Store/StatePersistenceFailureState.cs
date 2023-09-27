namespace Fluxor.Blazor.Persistence.Store;

public class StatePersistenceExceptionItem
{
  public string ActionType { get; set; } = string.Empty;
  public string FeatureName { get; set; } = string.Empty;
  public Exception? Exception { get; set; }
}

public record StatePersistenceFailureState
{
  public List<StatePersistenceExceptionItem> Errors { get; set; } = new();
}
