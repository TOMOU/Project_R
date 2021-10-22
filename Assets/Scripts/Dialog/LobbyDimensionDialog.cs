// ==================================================
// LobbyDimensionDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dialog
{
    public class LobbyDimensionDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        [Header("- Panel")]
        [SerializeField] private List<DimensionSlot> _dimensionSlotList;


        private int _selectLevel;

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
            _backButton = null;
            _homeButton.onClick.RemoveAllListeners();
            _homeButton = null;
            _closeButton.onClick.RemoveAllListeners();
            _closeButton = null;
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyDimensionDialog>();
                RequestDialogEnter<LobbyContentDialog>();
            }));

            RefreshDimensionSlot();

            // Maximum 클리어한 스테이지로 챕터 고정표시
            if (BattleManager.Singleton.battleType == 3)
            {
                _selectLevel = BattleManager.Singleton.selectStage;
            }
            else
            {
                _selectLevel = Info.My.Singleton.User.maxClearedDimension + 1;
            }

            OpenFirst();
            CheckScenario();
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void OpenFirst()
        {
            switch (TutorialManager.Singleton.openFirst)
            {
                case Constant.TutorialCallbackType.None:
                    break;

                case Constant.TutorialCallbackType.Dimension:
                    if (BattleManager.Singleton.inLobby == true)
                        SelectDimensionSlot(_selectLevel);

                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    break;
            }

            CheckScenario();
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

        private void RefreshDimensionSlot()
        {
            for (int i = 0; i < _dimensionSlotList.Count; i++)
            {
                _dimensionSlotList[i].Init(i + 1, SelectDimensionSlot);
            }
        }

        private void SelectDimensionSlot(int level)
        {
            _selectLevel = level;
            BattleManager.Singleton.battleType = 3;
            BattleManager.Singleton.selectChapter = 0;
            BattleManager.Singleton.selectStage = _selectLevel;

            Dialog.IDialog.RequestDialogEnter<Dialog.LobbyStageInfoDialog>();
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 51)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(51, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_dimensionSlotList[0].transform, () =>
                    {
                        SelectDimensionSlot(1);
                    }));
                }));
            }
            // else if (sidx == 53)
            // {
            //     Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
            // }
        }
    }
}