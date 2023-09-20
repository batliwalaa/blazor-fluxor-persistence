using System.Collections.Generic;

namespace Fluxor.Blazor.Persistence.Store;

public class LoadPersistedStateSuccessAction
{
  public IEnumerable<IFeature> Features { get; set; }

  public LoadPersistedStateSuccessAction(IEnumerable<IFeature> features)
  {
    Features = features;
  }
}
