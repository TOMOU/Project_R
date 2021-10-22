// ==================================================
// DefencePauseDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine.UI;

namespace Dialog
{
    public class DefencePauseDialog : IDialog
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
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg("옵션", "준비중입니다", null, true));
        }

        private void OnClickGiveUp()
        {
            // Message.Send<Global.PlaySoundMsg> (new Global.PlaySoundMsg (Constant.SoundName.button_click));
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg("포기하기", "전투를 포기하고 로비로 이동하시겠습니까?",
                () => Scene.IScene.LoadScene<Scene.LobbyScene>(), null));
        }
    }
}