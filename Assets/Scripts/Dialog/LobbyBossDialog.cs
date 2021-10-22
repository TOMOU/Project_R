// ==================================================
// LobbyBossDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class LobbyBossDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _titleLabel;

        [Header("- Boss Board")]
        [SerializeField] private Animator _anim;
        [SerializeField] private Button _openInfoButton;
        [SerializeField] private Button _closeDialog;

        [Header("Warning Text")]
        [SerializeField] private RectTransform[] _warningRect;

        private float _curValue;
        private Coroutine _coroutine;

        protected override void OnLoad()
        {
            base.OnLoad();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _openInfoButton.onClick.AddListener(OnClickOpenInfoPopup);
            _closeDialog.onClick.AddListener(OnClickBack);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _openInfoButton.onClick.RemoveAllListeners();
            _closeDialog.onClick.RemoveAllListeners();

            _backButton.onClick.RemoveAllListeners();
            _homeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyBossDialog>();
                RequestDialogEnter<LobbyContentDialog>();
            }));

            _titleLabel.text = LocalizeManager.Singleton.GetString(13001);

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coEnter());
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

        private void OnClickOpenInfoPopup()
        {
            BattleManager.Singleton.battleType = 4;
            BattleManager.Singleton.selectChapter = 0;
            BattleManager.Singleton.selectStage = 0;

            Info.My.Singleton.Inventory.bossTeam[0] = 000001;
            Info.My.Singleton.Inventory.bossTeam[1] = 000002;
            Info.My.Singleton.Inventory.bossTeam[2] = 000003;
            Info.My.Singleton.Inventory.bossTeam[3] = 000004;
            Info.My.Singleton.Inventory.bossTeam[4] = 000005;

            Dialog.IDialog.RequestDialogEnter<Dialog.LobbyStageInfoDialog>();
        }

        private void OnClickStartBattle()
        {
            BattleManager.Singleton.battleType = 4;
            BattleManager.Singleton.selectChapter = 0;
            BattleManager.Singleton.selectStage = 0;

            BattleManager.Singleton.inLobby = false;

            TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.ContentMap;

            Scene.IScene.LoadScene<Scene.BattleRaidScene>();
        }

        private void OnClickCancelbutton()
        {
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClickOpenFormationButton()
        {
            BattleManager.Singleton.battleType = 4;
            BattleManager.Singleton.selectChapter = 0;
            BattleManager.Singleton.selectStage = 0;

            BattleManager.Singleton.inLobby = true;

            TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Boss;

            // 팀 배치 UI 연다.
            RequestDialogExit<LobbyBossDialog>();
            RequestDialogEnter<LobbyFormationDialog>();
        }

        private IEnumerator coEnter()
        {
            yield return new WaitForEndOfFrame();

            while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;

            Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());

            if (TutorialManager.Singleton.curTutorialIndex == 65)
            {
                yield return new WaitForSeconds(0.5f);

                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(65, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_openInfoButton.transform, () =>
                    {
                        OnClickOpenInfoPopup();
                    }));
                }));
            }
        }
    }
}