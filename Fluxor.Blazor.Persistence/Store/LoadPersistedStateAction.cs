namespace Fluxor.Blazor.Persistence.Store;

public class LoadPersistedStateAction
{
  public IReadOnlyDictionary<string, IFeature> Features { get; set; }

  public LoadPersistedStateAction(IReadOnlyDictionary<string, IFeature> features)
  {
    Features = features;
  }
}
