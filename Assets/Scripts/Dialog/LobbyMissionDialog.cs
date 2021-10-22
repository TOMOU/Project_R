// ==================================================
// LobbyMissionDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class LobbyMissionDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        [Header("- Temp")]
        [SerializeField] private Button _button;

        protected override void OnLoad()
        {
            base.OnLoad();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _backButton.onClick.RemoveAllListeners();
            _homeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            CheckScenario();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyMissionDialog>();
                RequestDialogEnter<LobbyMainDialog>();
            }));
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void OnClickBack()
        {
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClickHome()
        {
            Message.Send<Global.ReturnToHomeMsg>(new Global.ReturnToHomeMsg());
        }

        private void OnClickClose()
        {
            //? 임시로 back키 할당.
            //? 나중에 기능이 확정되면 커스텀.
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClick()
        {

        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 36)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(36, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_button.transform, () =>
                    {
                        OnClick();
                        Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(37, () =>
                        {
                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
                        }));
                    }));
                }));
            }
        }
    }
}