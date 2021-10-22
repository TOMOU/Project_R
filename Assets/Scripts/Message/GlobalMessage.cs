namespace Global
{
    // ================================================================================
    // SceneLoad 관련 메세지
    // ================================================================================
    #region SceneLoad
    /// <summary>
    /// 씬 전환 호출 메세지
    /// </summary>
    public class ChangeSceneMsg : Message
    {
        public System.Type Scene { get; private set; }
        public ChangeSceneMsg(System.Type scene)
        {
            this.Scene = scene;
        }
    }

    /// <summary>
    /// 오브젝트 타입에 따른 부모 연결 메세지
    /// </summary>
    public class TransformAttachMsg : Message
    {
        /// <summary>
        /// 오브젝트 타입
        /// </summary>
        /// <value></value>
        public Constant.BehaviourType Type { get; private set; }
        /// <summary>
        /// 해당 오브젝트의 Transform
        /// </summary>
        /// <value></value>
        public UnityEngine.Transform Transform { get; private set; }

        public TransformAttachMsg(Constant.BehaviourType type, UnityEngine.Transform transform)
        {
            this.Type = type;
            this.Transform = transform;
        }
    }

    /// <summary>
    /// 메인 카메라 갱신 메세지
    /// </summary>
    public class InitMainCameraMsg : Message
    {
        public UnityEngine.Camera Camera { get; private set; }
        public InitMainCameraMsg(UnityEngine.Camera camera)
        {
            this.Camera = camera;
        }
    }

    /// <summary>
    /// UI 카메라 등록 메세지
    /// </summary>
    public class InitUICameraMsg : Message
    {
        public UnityEngine.Camera Camera { get; private set; }
        public InitUICameraMsg(UnityEngine.Camera camera)
        {
            this.Camera = camera;
        }
    }

    /// <summary>
    /// Global 카메라 등록 메세지
    /// </summary>
    public class InitGlobalCameraMsg : Message
    {
        public UnityEngine.Camera Camera { get; private set; }
        public InitGlobalCameraMsg(UnityEngine.Camera camera)
        {
            this.Camera = camera;
        }
    }
    #endregion

    // ================================================================================
    // 사운드 관련 메세지
    // ================================================================================
    #region Sound
    public class PlaySoundMsg : Message
    {
        public Constant.SoundName soundName { get; private set; }
        public PlaySoundMsg(Constant.SoundName soundName)
        {
            this.soundName = soundName;
        }
    }

    public class StopSoundMsg : Message
    {
        public Constant.SoundName soundName { get; private set; }
        public StopSoundMsg(Constant.SoundName soundName)
        {
            this.soundName = soundName;
        }
    }

    public class StopAllSoundMsg : Message
    {
        public StopAllSoundMsg() { }
    }
    #endregion

    // ================================================================================
    // 속도 관련 메세지
    // ================================================================================
    #region TimeScale
    /// <summary>
    /// 배속모드 활성화
    /// </summary>
    public class FastSpeedMsg : Message { }
    /// <summary>
    /// 배속모드 중단
    /// </summary>
    public class NormalSpeedMsg : Message { }
    /// <summary>
    /// 기존에 저장해놨던 배속모드 활성화
    /// </summary>
    public class ReloadSpeedMsg : Message { }
    /// <summary>
    /// 일시정지 활성화
    /// </summary>
    public class EnablePauseMsg : Message { }
    /// <summary>
    /// 일시정지 비활성화
    /// </summary>
    public class DisablePauseMsg : Message { }
    #endregion

    // ================================================================================
    // Input 관련 메세지
    // ================================================================================
    #region Input
    /// <summary>
    /// 기본 Escape 키 동작 초기화
    /// </summary>
    public class AddBaseEscapeActionMsg : Message
    {
        public System.Action Action { get; private set; }
        public AddBaseEscapeActionMsg(System.Action action)
        {
            this.Action = action;
        }
    }

    /// <summary>
    /// Escape 키 동작 추가
    /// </summary>
    public class AddEscapeActionMsg : Message
    {
        public System.Action Action { get; private set; }
        public AddEscapeActionMsg(System.Action action)
        {
            this.Action = action;
        }
    }

    /// <summary>
    /// 마지막에 넣은 Escape 키 동작 POP
    /// <para>POP: 동작을 실행 시키면서 제거</para>
    /// </summary>
    public class PopEscapeActionMsg : Message { }

    /// <summary>
    /// 마지막에 넣은 Escape 키 동작 제거
    /// </summary>
    public class RemoveEscapeActionMsg : Message { }

    /// <summary>
    /// 모든 Escape 키 동작 제거
    /// </summary>
    public class RemoveEscapeActionAllMsg : Message { }

    public class ReturnToHomeMsg : Message { }

    /// <summary>
    /// 입력 잠금 활성화
    /// </summary>
    public class InputLockMsg : Message { }

    /// <summary>
    /// 입력 잠금 비활성화
    /// </summary>
    public class InputUnlockMsg : Message { }
    public class EventSystemLockMsg : Message { }
    public class EventSystemUnlockMsg : Message { }
    public class EscapeLockMsg : Message { }
    public class EscapeUnlockMsg : Message { }
    public class EnableAutoModeMsg : Message { }
    public class DisableAutoModeMsg : Message { }
    #endregion

    // ================================================================================
    // 기타 메세지
    // ================================================================================
    #region ETC
    /// <summary>
    /// 메세지 박스 호출 메세지
    /// </summary>
    public class MessageBoxMsg : Message
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public System.Action Cancel { get; private set; }
        public System.Action Confirm { get; private set; }
        public bool OnlyConfirm { get; private set; }
        public MessageBoxMsg(string title, string message, System.Action confirm, System.Action cancel)
        {
            this.Title = title;
            this.Message = message;
            this.Confirm = confirm;
            this.Cancel = cancel;
            this.OnlyConfirm = false;
        }
        public MessageBoxMsg(string title, string message, System.Action confirm, bool onlyConfirm)
        {
            this.Title = title;
            this.Message = message;
            this.Confirm = confirm;
            this.Cancel = null;
            this.OnlyConfirm = true;
        }
    }
    #endregion

    public class ShowScenarioTextMsg : Message
    {
        public string scenarioName { get; private set; }
        public int sceneIndex { get; private set; }
        public System.Action callback { get; private set; }
        public ShowScenarioTextMsg(int sceneIndex, System.Action callback, string scenarioName = "Main")
        {
            this.sceneIndex = sceneIndex;
            this.callback = callback;
            this.scenarioName = scenarioName;
        }
    }

    public class ShowScenarioBackgroundMsg : Message
    {
        public bool isEnabled { get; private set; }
        public ShowScenarioBackgroundMsg(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }
    }

    public class ShowScenarioFadeMsg : Message
    {
        public bool isFade { get; private set; }
        public System.Action Callback { get; private set; }
        public ShowScenarioFadeMsg(bool isFade, System.Action callback = null)
        {
            this.isFade = isFade;
            this.Callback = callback;
        }
    }
    public class ShowImageGuideMsg : Message { }
    public class ForceFocusGuideMsg : Message
    {
        public UnityEngine.Transform Target { get; private set; }
        public System.Action Callback { get; private set; }
        public bool ShowAnimation { get; private set; }

        public ForceFocusGuideMsg(UnityEngine.Transform target, System.Action callback, bool showAnimation = true)
        {
            this.Target = target;
            this.Callback = callback;
            this.ShowAnimation = showAnimation;
        }
    }
    public class ForceButtonGuideMsg : Message
    {
        public UnityEngine.Transform Target { get; private set; }
        public UnityEngine.UI.GridLayoutGroup GridLayout { get; private set; }
        public System.Action Callback { get; private set; }

        public ForceButtonGuideMsg(UnityEngine.Transform target, System.Action callback)
        {
            this.Target = target;
            this.Callback = callback;
            this.GridLayout = null;
        }

        public ForceButtonGuideMsg(UnityEngine.Transform target, UnityEngine.UI.GridLayoutGroup gridLayout, System.Action callback)
        {
            this.Target = target;
            this.Callback = callback;
            this.GridLayout = gridLayout;
        }
    }

    public class LoadingCountAddMsg : Message
    {
        public string sender { get; private set; }
        public LoadingCountAddMsg(string s)
        {
            sender = s;
        }
    }

    public class MaxLoadingCountMsg : Message
    {
        public int Max { get; private set; }
        public MaxLoadingCountMsg(int max)
        {
            this.Max = max;
        }
    }

    public class ShowRewardMsg : Message
    {
        public System.Collections.Generic.List<int> Items;
        public System.Collections.Generic.List<int> Values;
        public System.Action Callback;
        public ShowRewardMsg(System.Collections.Generic.List<int> items, System.Collections.Generic.List<int> values, System.Action callback)
        {
            Items = items;
            Values = values;
            Callback = callback;
        }
    }

    public class ChangeLocaleMsg : Message { }
}