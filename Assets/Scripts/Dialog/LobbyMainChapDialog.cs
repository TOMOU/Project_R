// ==================================================
// LobbyMainChapDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Dialog
{
    public class LobbyMainChapDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _titleLabel;

        [Header("- Chapter")]
        [SerializeField] private GameObject _chapterPanel;
        [SerializeField] private List<Button> _chapterLevelList;
        [SerializeField] private List<MainChapterSlot> _chapterSlotList;

        [Header("- Stage")]
        [SerializeField] private GameObject _stagePanel;
        [SerializeField] private Image _stageBackground;
        [SerializeField] private List<MainStageSlot> _stageSlotList;

        [Serializable]
        private class StagePos
        {
            public int chapter;
            public List<Vector2> posList;
        }
        [SerializeField] private List<StagePos> _posList;

        [SerializeField] private int _selectChapter = 1;
        [SerializeField] private int _selectStage = 0;

        private List<StageModel.Data> _stageTable;
        [SerializeField] private int _chapterCount;
        [SerializeField] private int _stageCount;


        protected override void OnLoad()
        {
            base.OnLoad();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            var sm = Model.First<StageModel>();
            if (sm == null)
            {
                Logger.LogError("StageModel 로드 실패");
                return;
            }

            _stageTable = sm.Table.FindAll(e => e.type == 1);
            if (_stageTable == null)
            {
                Logger.LogError("StageTable 로드 실패");
                return;
            }

            _chapterCount = 0;
            for (int i = 0; i < _stageTable.Count; i++)
            {
                if (_stageTable[i].chapter > _chapterCount)
                    _chapterCount = _stageTable[i].chapter;
            }
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
                RequestDialogExit<LobbyMainChapDialog>();
                RequestDialogEnter<LobbyContentDialog>();
            }));

            _chapterPanel.SetActive(true);
            _stagePanel.SetActive(false);

            // Maximum 클리어한 스테이지로 챕터 고정표시
            if (BattleManager.Singleton.battleType == 1)
            {
                _selectChapter = BattleManager.Singleton.selectChapter;
                _selectStage = BattleManager.Singleton.selectStage;
            }
            else
            {
                _selectChapter = Info.My.Singleton.User.maxClearedMainstory / 4 + 1;
                _selectStage = Info.My.Singleton.User.maxClearedMainstory % 4 + 1;
            }

            if (_selectChapter > 4)
                _selectChapter = 4;
            if (_selectChapter == 4 && _selectStage > 1)
                _selectStage = 1;

            RefreshChapterList();

            OpenFirst();
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

                case Constant.TutorialCallbackType.MainChapter:
                    SelectChapterSlot(_selectChapter);

                    if (BattleManager.Singleton.inLobby == true)
                        SelectStageSlot(_selectStage);

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


        private void RefreshChapterList()
        {
            for (int i = 0; i < _chapterCount; i++)
            {
                _chapterSlotList[i].Init(i + 1, LocalizeManager.Singleton.GetString(8002 + i), SelectChapterSlot);
            }
        }

        private void RefreshStageList()
        {
            int mapid = 0;
            if (_selectChapter > 3)
                mapid = 3;
            else
                mapid = _selectChapter;

            _stageBackground.sprite = Resources.Load<Sprite>(string.Format("Texture/MainChapter/BG/stage_background_{0}", mapid));

            int stageCount = _stageTable.FindAll(e => e.chapter == _selectChapter).Count;

            StagePos pos = _posList.Find(e => e.chapter == _selectChapter);

            for (int i = 0; i < _stageSlotList.Count; i++)
            {
                if (i < stageCount)
                {
                    _stageSlotList[i].Init(_selectChapter, i + 1, SelectStageSlot);
                    if (i + 1 < stageCount)
                        _stageSlotList[i].SetPosition(pos.posList[i], pos.posList[i + 1]);
                    else
                        _stageSlotList[i].SetPosition(pos.posList[i]);
                }
                else
                    _stageSlotList[i].Init();
            }
        }

        private void SelectChapterSlot(int chapter)
        {
            _selectChapter = chapter;

            _chapterPanel.SetActive(false);
            _stagePanel.SetActive(true);

            RefreshStageList();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                _chapterPanel.SetActive(true);
                _stagePanel.SetActive(false);
            }));
        }

        private void SelectStageSlot(int stage)
        {
            _selectStage = stage;

            BattleManager.Singleton.battleType = 1;
            BattleManager.Singleton.selectChapter = _selectChapter;
            BattleManager.Singleton.selectStage = _selectStage;

            Dialog.IDialog.RequestDialogEnter<Dialog.LobbyStageInfoDialog>();
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 10)
                PlayScenario_10();
            //? else if (sidx == 13)
            //? {
            //?     Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, OnClickStartBattle));
            //? }
            // else if (sidx == 22)
            //     PlayScenario_22();
            else if (sidx == 31)
                PlayScenario_31();
            else if (sidx == 32)
                PlayScenario_32();
            // else if (sidx == 34)
            //     PlayScenario_34();
            else if (sidx == 38)
                PlayScenario_38();
            else if (sidx == 40)
                PlayScenario_40();
            else if (sidx == 44)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(44, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[0].transform, () =>
                    {
                        // 1스테이지 입장
                        SelectStageSlot(1);
                    }));
                }));
            }
            else if (sidx == 46)
            {

            }
            else if (sidx == 47)
            {

            }
            else if (sidx == 50)
                PlayScenario_50();
            else if (sidx == 58)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[1].transform, () =>
                {
                    // 2챕터 입장
                    SelectChapterSlot(2);
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[1].transform, () =>
                    {
                        // 2스테이지 입장
                        SelectStageSlot(2);
                    }));
                }));
            }
            else if (sidx == 60)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[2].transform, () =>
                {
                    // 3스테이지 입장
                    SelectStageSlot(3);
                }));
            }
            else if (sidx == 62)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(62, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[3].transform, () =>
                {
                    // 4스테이지 입장
                    SelectStageSlot(4);
                }));
                }));
            }
            else if (sidx == 65)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                {
                    OnClickBack();
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                    {
                        OnClickBack();
                    }));
                }));
            }
            else if (sidx == 72)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[2].transform, () =>
                {
                    // 3챕터 입장
                    SelectChapterSlot(3);
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[0].transform, () =>
                    {
                        // 1스테이지 입장
                        SelectStageSlot(1);
                    }));
                }));
            }
            else if (sidx == 75 && (Info.My.Singleton.User.maxClearedMainstory == 9 || Info.My.Singleton.User.maxClearedMainstory == 10))
            {
                int idx = Info.My.Singleton.User.maxClearedMainstory == 9 ? 1 : 2;

                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[idx].transform, () =>
                    {
                        // 2스테이지 입장
                        SelectStageSlot(idx + 1);

                    }));
            }
            else if (sidx == 82)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[2].transform, () =>
                {
                    // 3챕터 입장
                    SelectChapterSlot(3);
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[3].transform, () =>
                    {
                        // 4스테이지 입장
                        SelectStageSlot(4);
                    }));
                }));
            }
            else if (sidx == 88)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[3].transform, () =>
                {
                    // 4챕터 입장
                    SelectChapterSlot(4);
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[0].transform, () =>
                    {
                        // 1스테이지 입장
                        SelectStageSlot(1);
                    }));
                }));
            }
        }

        private void PlayScenario_10()
        {
            // scene 10. scenario -> touching worldmap button, open mainstory stage
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(10, () =>
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[0].transform, () =>
                {
                    // 1챕터 입장
                    SelectChapterSlot(1);
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[0].transform, () =>
                    {
                        // 1스테이지 입장
                        SelectStageSlot(1);
                    }));
                }));
            }));
        }

        private void PlayScenario_22()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_homeButton.transform, () =>
            {
                OnClickHome();
            }));
        }

        private void PlayScenario_31()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[0].transform, () =>
            {
                // 1챕터 입장
                SelectChapterSlot(1);

                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[1].transform, () =>
                {
                    // 2스테이지 입장
                    SelectStageSlot(2);
                }));
            }));
        }

        private void PlayScenario_32()
        {

        }

        private void PlayScenario_34()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
            {
                OnClickBack();
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                {
                    OnClickBack();
                }));
            }));
        }

        private void PlayScenario_38()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[0].transform, () =>
            {
                // 1챕터 입장
                SelectChapterSlot(1);
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[2].transform, () =>
                {
                    // 3스테이지 입장
                    SelectStageSlot(3);
                }));
            }));
        }

        private void PlayScenario_40()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[3].transform, () =>
                {
                    // 4스테이지 입장
                    SelectStageSlot(4);
                }));
        }

        private void PlayScenario_50()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
            {
                OnClickBack();
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                {
                    OnClickBack();
                }));
            }));
        }
    }
}