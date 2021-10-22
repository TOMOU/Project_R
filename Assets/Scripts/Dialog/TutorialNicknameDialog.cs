// ==================================================
// TutorialNicknameDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class TutorialNicknameDialog : IDialog
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _confirmButton;

        protected override void OnLoad()
        {
            _confirmButton.onClick.AddListener(OnClickConfirm);
        }

        protected override void OnUnload()
        {

        }

        protected override void OnEnter()
        {
            SoundManager.Singleton.PlayScenarioBGM(true);
        }

        protected override void OnExit()
        {

        }

        private void OnClickConfirm()
        {
            if (string.IsNullOrEmpty(_inputField.text))
            {
                _inputField.text = string.Empty;
                Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(15), null, true));
                return;
            }

            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), string.Format(LocalizeManager.Singleton.GetString(16), _inputField.text), () =>
            {
                Info.My.Singleton.User.nickName = _inputField.text;
                MoveToFirstBattle();
            }, null));
        }

        private void MoveToFirstBattle()
        {
            if (_dialogView != null)
                _dialogView.SetActive(false);

            OnExit();

            // 닉네임 생성이 완료되었으니 첫 전투로 이동한다.
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(3, () =>
            {
                Scene.IScene.LoadScene<Scene.BattleNormalScene>();
            }));
        }
    }
}