using System.Collections.Generic;
using UnityEngine;

public class StateMachine {

    private IStateMachine currentState;

    public enum StateName {
        // Add state names here
        NO_CHANGE,
        SETUP,
        PLAY
    }

    private Dictionary<StateName, IStateMachine> _statesDictionary;

    private void AddState(IStateMachine _state, StateName _id) 
    {
        _statesDictionary.Add(_id, _state);
    }

    private void ChangeState(StateName _newStateId) 
    {
        if (currentState != null) {
            currentState.Stop();
        }
        currentState = null;
        if (_statesDictionary.ContainsKey(_newStateId)) {
            currentState = _statesDictionary[_newStateId];
            currentState.Start();
        } else {
            Debug.LogWarning("Invalid State!");
        }
    }

    public StateMachine() 
    {
        _statesDictionary = new Dictionary<StateName, IStateMachine>();

        // Add states here
        AddState(new SetupState(), StateName.SETUP);
        AddState(new PlayState(), StateName.PLAY);
        
    }

    // Call this from your main update loop
    public void Update () 
    {
        if (currentState != null) {
            var newStateId = currentState.Update();
            if(newStateId != StateName.NO_CHANGE) {
                // If new state name is returned, change to that state
                ChangeState(newStateId);
            }
        }
    }
}
