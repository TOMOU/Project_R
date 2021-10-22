using System;
using UnityEngine;

namespace FSM
{
    [RequireComponent (typeof (StateEngine))]
    public class StateBehaviour : MonoBehaviour
    {
        private StateEngine _stateMachine = null;
        public StateEngine stateMachine
        {
            get
            {
                if (_stateMachine == null)
                    _stateMachine = GetComponent<StateEngine> ();

                if (_stateMachine == null)
                    throw new Exception ("StateEngine 컴포넌트를 찾을 수 없습니다.");

                return _stateMachine;
            }
        }

        public Enum GetState ()
        {
            return _stateMachine.GetState ();
        }

        protected void Initialize<T> ()
        {
            stateMachine.Initialize<T> (this);
        }

        protected virtual void ChangeState (Enum newState)
        {
            stateMachine.ChangeState (newState);
        }

        protected virtual void ChangeState (Enum newState, StateTransition transition)
        {
            stateMachine.ChangeState (newState, transition);
        }
    }
}