using UnityEngine;

public class PlayState : IStateMachine {

    public void Start()
    {
    }

    public void Stop() 
    {
    }

    public StateMachine.StateName Update()
    {
        return StateMachine.StateName.NO_CHANGE;
    }

}
