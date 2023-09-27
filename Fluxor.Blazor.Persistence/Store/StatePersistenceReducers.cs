namespace Fluxor.Blazor.Persistence.Store;

public static class StatePersistenceReducers
{
  [ReducerMethod]
  public static StatePersistenceFailureState OnLoadPersistedStateFailure(
    StatePersistenceFailureState state,
    LoadPersistedStateFailureAction action)
  {
    state.Errors.Add(new StatePersistenceExceptionItem 
    { 
      ActionType = action.ActionType,
      FeatureName = action.FeatureName,
      Exception = action.Exception
    });

    return state;
  }

  [ReducerMethod]
  public static StatePersistenceFailureState OnSavePersistedStateFailure(
    StatePersistenceFailureState state,
    SavePersistedStateFailureAction action)
  {
    state.Errors.Add(new StatePersistenceExceptionItem 
    {
      ActionType = action.ActionType,
      FeatureName = action.FeatureName,
      Exception = action.Exception
    });

    return state;
  }

  [ReducerMethod(typeof(ClearStatePersistenceFailureStateAction))]
  public static StatePersistenceFailureState OnClearStatePersistenceFailureState(StatePersistenceFailureState state)
  {
    return state with { Errors = new() };
  }
}
