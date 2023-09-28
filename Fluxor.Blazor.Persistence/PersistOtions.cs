namespace Fluxor.Blazor.Persistence;

public class PersistOtions
{
  public PersistenceType PersistenceType { get; set; } = PersistenceType.LocalStorage;
  public string PersistenceKey { get; set; } = "Fluxor.Blazor.Persistence";
  public bool PersistRoutes { get; set; } = true;
}
