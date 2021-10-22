using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

// namespace Nexelon.StateMachine
namespace FSM
{
    public class StateEngine : MonoBehaviour
    {
        public event Action<System.Enum> changed;

        private StateMapping _curState;
        private StateMapping _destState;

        private Dictionary<Enum, StateMapping> _stateLookup;
        private Dictionary<string, Delegate> _methodLookup;

        private readonly string[] _ignoredNames = new [] { "add", "remove", "get", "set" };

        private bool _isInTransition = false;
        public bool IsInTransition { get { return _isInTransition; } }
        private IEnumerator _curTransition;
        private IEnumerator _enterRoutine;
        private IEnumerator _exitRoutine;
        private IEnumerator _queuedChange;

        public Enum GetState ()
        {
            if (_curState != null)
                return _curState.state;

            return null;
        }

        public void Initialize<T> (MonoBehaviour entity)
        {
            // State 초기화
            var values = Enum.GetValues (typeof (T));
            _stateLookup = new Dictionary<Enum, StateMapping> ();
            for (int i = 0; i < values.Length; i++)
            {
                var mapping = new StateMapping ((Enum) values.GetValue (i));
                _stateLookup.Add (mapping.state, mapping);
            }

            // 메소드 정보 파싱
            var methods = entity.GetType ().GetMethods (BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
                BindingFlags.NonPublic);

            // State에 메소드 연결
            var separator = "_".ToCharArray ();
            for (int i = 0; i < methods.Length; i++)
            {
                var names = methods[i].Name.Split (separator);

                // (형식에 따르지 않는) 언더바 없는 메소드 무시
                if (names.Length <= 1)
                {
                    continue;
                }

                Enum key;
                try
                {
                    key = (Enum) Enum.Parse (typeof (T), names[0]);
                }
                catch (ArgumentException)
                {
                    // 무시목록 건너뛰기
                    for (int j = 0; j < _ignoredNames.Length; j++)
                    {
                        if (names[0] == _ignoredNames[j])
                        {
                            goto SkipWarning;
                        }
                    }

                    Logger.LogWarningFormat ("{0} 메소드에 맞는 State가 없습니다.", names[0]);
                    continue;

                    SkipWarning:
                        continue;
                }

                var targetState = _stateLookup[key];

                switch (names[1])
                {
                    case "Enter":
                        if (methods[i].ReturnType == typeof (IEnumerator))
                        {
                            targetState.enter = CreateDelegate<Func<IEnumerator>> (methods[i], entity);
                        }
                        else
                        {
                            var action = CreateDelegate<Action> (methods[i], entity);
                            targetState.enter = () => { action (); return null; };
                        }
                        break;
                    case "Exit":
                        if (methods[i].ReturnType == typeof (IEnumerator))
                        {
                            targetState.exit = CreateDelegate<Func<IEnumerator>> (methods[i], entity);
                        }
                        else
                        {
                            var action = CreateDelegate<Action> (methods[i], entity);
                            targetState.exit = () => { action (); return null; };
                        }
                        break;
                    case "Finally":
                        targetState.final = CreateDelegate<Action> (methods[i], entity);
                        break;
                    case "Update":
                        targetState.update = CreateDelegate<Action> (methods[i], entity);
                        break;
                    case "LateUpdate":
                        targetState.lateUpdate = CreateDelegate<Action> (methods[i], entity);
                        break;
                    case "FixedUpdate":
                        targetState.fixedUpdate = CreateDelegate<Action> (methods[i], entity);
                        break;
                }
            }
        }

        private V CreateDelegate<V> (MethodInfo method, Object target) where V : class
        {
            // 대리자 생성
            var ret = (Delegate.CreateDelegate (typeof (V), target, method) as V);

            if (ret == null)
            {
                throw new ArgumentException (string.Format ("{0} 메서드에 대한 대리자 생성에 실패했습니다."));
            }

            return ret;
        }

        public void ChangeState (Enum newState, StateTransition transition = StateTransition.Safe)
        {
            if (_stateLookup == null)
            {
                throw new Exception ("State가 정의되지 않았습니다. 먼저 상태를 초기화 해주세요.");
            }

            if (!_stateLookup.ContainsKey (newState))
            {
                throw new Exception (string.Format ("이름이 {0}인 State를 찾을 수 없습니다. 초기화가 제대로 되었는지 확인해 주세요.", newState));
            }

            var nextState = _stateLookup[newState];

            // 현재 State와 동일하다면 건너뛴다.
            if (_curState == nextState)
                return;

            if (_queuedChange != null)
            {
                StopCoroutine (_queuedChange);
                _queuedChange = null;
            }

            switch (transition)
            {
                case StateTransition.Safe:
                    if (_isInTransition)
                    {
                        // 이전 상태가 아직 종료되지 않았다.
                        if (_exitRoutine != null)
                        {
                            // 새로운 State로 덮어 씌우기
                            _destState = nextState;
                            return;
                        }

                        // 아직 이전상태가 시작도 안했다면 이전상태가 완전히 종료할 때까지 기다린다.
                        if (_enterRoutine != null)
                        {
                            _queuedChange = WaitForPreviousTransition (nextState);
                            StartCoroutine (_queuedChange);
                            return;
                        }
                    }
                    break;
                case StateTransition.Overwrite:
                    if (_curTransition != null)
                        StopCoroutine (_curTransition);

                    if (_exitRoutine != null)
                        StopCoroutine (_exitRoutine);

                    if (_enterRoutine != null)
                        StopCoroutine (_enterRoutine);

                    if (_curState != null)
                        _curState.final ();

                    _curState = null;
                    break;
            }

            _isInTransition = true;
            _curTransition = ChangeToNewStateRoutine (nextState);
            StartCoroutine (_curTransition);
        }

        private IEnumerator ChangeToNewStateRoutine (StateMapping newState)
        {
            _destState = newState;

            if (_curState != null)
            {
                _exitRoutine = _curState.exit ();

                if (_exitRoutine != null)
                {
                    yield return StartCoroutine (_exitRoutine);
                }

                _exitRoutine = null;

                _curState.final ();
            }

            _curState = _destState;

            if (_curState != null)
            {
                _enterRoutine = _curState.enter ();

                if (_enterRoutine != null)
                {
                    yield return StartCoroutine (_enterRoutine);
                }

                _enterRoutine = null;

                if (changed != null)
                {
                    changed (_curState.state);
                }
            }

            _isInTransition = false;
        }

        IEnumerator WaitForPreviousTransition (StateMapping nextState)
        {
            while (_isInTransition)
            {
                yield return null;
            }

            ChangeState (nextState.state);
        }

        void FixedUpdate ()
        {
            if (_curState != null)
            {
                _curState.fixedUpdate ();
            }
        }

        void Update ()
        {
            if (_curState != null && !IsInTransition)
            {
                _curState.update ();
            }
        }

        void LateUpdate ()
        {
            if (_curState != null && !IsInTransition)
            {
                _curState.lateUpdate ();
            }
        }

        public static void DoNothing () { }

        public static void DoNothingCollider (Collider other) { }

        public static void DoNothingCollision (Collision other) { }

        public static IEnumerator DoNothingCoroutine () { yield break; }
    }

    public enum StateTransition
    {
        Overwrite,
        Safe
    }

    public class StateMapping
    {
        public Enum state { get; private set; }

        public Func<IEnumerator> enter = StateEngine.DoNothingCoroutine;
        public Func<IEnumerator> exit = StateEngine.DoNothingCoroutine;
        public Action update = StateEngine.DoNothing;
        public Action lateUpdate = StateEngine.DoNothing;
        public Action fixedUpdate = StateEngine.DoNothing;
        public Action final = StateEngine.DoNothing;

        public StateMapping (Enum state)
        {
            this.state = state;
        }
    }
}