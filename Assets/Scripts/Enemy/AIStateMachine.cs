using UnityEngine;
using System.Collections.Generic;

namespace EnemyAI
{
    public class AIStateMachine
    {
        private Dictionary<AIStateType, IAIState> states;
        private IAIState currentState;
        private EnemyAIController controller;

        public AIStateType CurrentStateType => currentState?.StateType ?? AIStateType.Idle;

        public AIStateMachine(EnemyAIController controller)
        {
            this.controller = controller;
            states = new Dictionary<AIStateType, IAIState>();
        }

        public void AddState(IAIState state)
        {
            states[state.StateType] = state;
        }

        public void Start(AIStateType initialState)
        {
            if (states.TryGetValue(initialState, out IAIState state))
            {
                currentState = state;
                currentState.OnEnter(controller);
            }
        }

        public void Update()
        {
            if (currentState == null) return;

            currentState.OnUpdate(controller);

            AIStateType nextState = currentState.CheckTransitions(controller);
            if (nextState != currentState.StateType)
            {
                ChangeState(nextState);
            }
        }

        public void ChangeState(AIStateType newStateType)
        {
            if (!states.TryGetValue(newStateType, out IAIState newState)) return;

            currentState?.OnExit(controller);
            currentState = newState;
            currentState.OnEnter(controller);    
        }
    }
}