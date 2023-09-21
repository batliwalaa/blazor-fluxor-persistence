namespace Fluxor.Blazor.Persistence.Store;

public class StatePersistenceFailureFeature : Feature<StatePersistenceFailureState>
{
  public override string GetName() => "@StatePersistence";

  protected override StatePersistenceFailureState GetInitialState()
  {
    return new StatePersistenceFailureState();
  }
}
