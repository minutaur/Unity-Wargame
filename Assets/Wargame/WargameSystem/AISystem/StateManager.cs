using System;
using UnityEngine;

namespace Wargame.AISystem
{
    public class StateManager : MonoBehaviour
    {
        private State _currentState;

        private void Update()
        {
            RunStateMachine();
        }

        void RunStateMachine()
        {
            State nextState = _currentState?.Run() ?? new IdleState();

            if (nextState)
            {
                SwitchState(nextState);
            }
        }

        void SwitchState(State nextState)
        {
            _currentState = nextState;
        }
    }
}