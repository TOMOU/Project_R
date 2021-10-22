using UnityEngine.UI;

namespace Dialog
{
    public class RaidPauseDialog : IDialog
    {
        private Button _backButton = null;
        private Button _settingButton = null;
        private Button _giveUpButton = null;

        protected override void OnLoad()
        {
            _backButton = _dialogView.transform.Find("Popup/Button/Back").GetComponent<Button>();
            _settingButton = _dialogView.transform.Find("Popup/Button/Setting").GetComponent<Button>();
            _giveUpButton = _dialogView.transform.Find("Popup/Button/GiveUp").GetComponent<Button>();

            _backButton.onClick.AddListener(OnClickBack);
            _settingButton.onClick.AddListener(OnClickSetting);
            _giveUpButton.onClick.AddListener(OnClickGiveUp);
        }

        protected override void OnUnload()
        {
            _backButton.onClick.RemoveAllListeners();
            _backButton = null;
            _settingButton.onClick.RemoveAllListeners();
            _settingButton = null;
            _giveUpButton.onClick.RemoveAllListeners();
            _giveUpButton = null;
        }

        protected override void OnEnter()
        {
            Message.Send<Global.EnablePauseMsg>(new Global.EnablePauseMsg());
            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                if (_dialogView != null)
                {
                    _dialogView.SetActive(false);
                }

                Message.Send<Global.DisablePauseMsg>(new Global.DisablePauseMsg());
            }));

            _giveUpButton.SetGrayScaleWithChild(TutorialManager.Singleton.curTutorialIndex < 93);
        }

        protected override void OnExit()
        {
            Message.Send<Global.DisablePauseMsg>(new Global.DisablePauseMsg());
            Message.Send<Global.RemoveEscapeActionMsg>(new Global.RemoveEscapeActionMsg());
        }

        private void OnClickBack()
        {
            // Message.Send<Global.PlaySoundMsg> (new Global.PlaySoundMsg (Constant.SoundName.button_click));
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClickSetting()
        {
            // Message.Send<Global.PlaySoundMsg> (new Global.PlaySoundMsg (Constant.SoundName.button_click));
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(12), null, true));
        }

        private void OnClickGiveUp()
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(11), LobbySceneLoad, null));
        }

        private void LobbySceneLoad()
        {
            Message.Send<Battle.Normal.ForceIdleMsg>(new Battle.Normal.ForceIdleMsg());
            Scene.IScene.LoadScene<Scene.LobbyScene>();
        }
    }
}