using UnityEngine;

public class SetupState : IStateMachine 
{

    private float countDown;
    
    public void Start()
    {
        // Setup here
        countDown = 0.5f;
    }

    public void Stop() 
    {
    }

    public StateMachine.StateName Update() 
    {
        countDown -= Time.deltaTime;
        if(countDown <= 0.0f) {
            return StateMachine.StateName.PLAY;
        }
        return StateMachine.StateName.NO_CHANGE;
    }

}
