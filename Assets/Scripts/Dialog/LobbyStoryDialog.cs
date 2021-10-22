// ==================================================
// LobbyStoryDialog.cs
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
    public class LobbyStoryDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _titleLabel;

        [Header("- Character")]
        [SerializeField] private GameObject _charPanel;
        [SerializeField] private List<StoryCharacterSlot> _charSlotList;

        [Header("- Chapter")]
        [SerializeField] private GameObject _chapterPanel;
        [SerializeField] private List<StoryCharacterSlot> _chapterSlotList;

        [Header("- Stage")]
        [SerializeField] private GameObject _stagePanel;
        [SerializeField] private Image _stageBackground;
        [SerializeField] private List<MainStageSlot> _stageSlotList;

        [Serializable]
        private class StagePos
        {
            public int cha_id;
            public int chapter;
            public List<Vector2> posList;
        }
        [SerializeField] private List<StagePos> _posList;

        private List<StageModel.Data> _stageTable;
        private int _charCount;
        private int _selectCharacter;
        private int _chapCount;
        private int _selectChapter;
        private int _selectStage;

        private int[] cArr = new int[5] { 101031, 100211, 101131, 102831, 103031 };


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

            _stageTable = sm.Table.FindAll(e => e.type == 2);
            if (_stageTable == null)
            {
                Logger.LogError("StageTable 로드 실패");
                return;
            }

            List<int> charid = new List<int>();
            for (int i = 0; i < _stageTable.Count; i++)
            {
                if (charid.Contains(_stageTable[i].cha_id) == false)
                    charid.Add(_stageTable[i].cha_id);

                if (_stageTable[i].chapter > _chapCount)
                    _chapCount = _stageTable[i].chapter;
            }

            _charCount = charid.Count;

            charid.Clear();
            charid = null;
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

            if (_charSlotList != null)
            {
                _charSlotList.Clear();
                _charSlotList = null;
            }

            if (_chapterSlotList != null)
            {
                _chapterSlotList.Clear();
                _chapterSlotList = null;
            }

            if (_stageSlotList != null)
            {
                _stageSlotList.Clear();
                _stageSlotList = null;
            }
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyStoryDialog>();
                RequestDialogEnter<LobbyContentDialog>();
            }));

            _selectCharacter = 101031;
            _selectChapter = 1;
            _selectStage = Info.My.Singleton.User.maxClearedStory + 1;
            if (_selectStage > 4)
                _selectStage = 4;

            _charPanel.SetActive(true);
            _chapterPanel.SetActive(false);
            _stagePanel.SetActive(false);

            RefreshCharacterPanel();

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

                case Constant.TutorialCallbackType.Story:
                    SelectCharacterSlot(_selectCharacter);
                    SelectChapterSlot(_selectChapter);
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

        private void OnClickOpenChapter()
        {
            // _titleLabel.text = "치우 스토리";
            _charPanel.SetActive(false);
            _chapterPanel.SetActive(true);
            _stagePanel.SetActive(false);

            RefreshChapterPanel();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                // _titleLabel.text = "대신전";
                _charPanel.SetActive(true);
                _chapterPanel.SetActive(false);
                _stagePanel.SetActive(false);
            }));
        }

        private void OnClickOpenStage()
        {
            // _titleLabel.text = "치우 스토리 - Chapter.1";
            _charPanel.SetActive(false);
            _chapterPanel.SetActive(false);
            _stagePanel.SetActive(true);

            RefreshStagePanel();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                // _titleLabel.text = "치우 스토리";
                _charPanel.SetActive(false);
                _chapterPanel.SetActive(true);
                _stagePanel.SetActive(false);
            }));
        }

        private void RefreshCharacterPanel()
        {
            for (int i = 0; i < _chapterSlotList.Count; i++)
            {
                if (i < _charCount)
                    _charSlotList[i].InitCharacter(cArr[i], SelectCharacterSlot);
                else
                    _charSlotList[i].InitCharacter(cArr[i], SelectLockContent);
            }
        }

        private void RefreshChapterPanel()
        {
            for (int i = 0; i < _chapterSlotList.Count; i++)
            {
                if (i < _chapCount)
                    _chapterSlotList[i].InitChapter(101031, i + 1, SelectChapterSlot);
                else
                    _chapterSlotList[i].InitChapter(101031, i + 1, SelectLockContent);
            }
        }

        private void RefreshStagePanel()
        {
            _stageBackground.sprite = Resources.Load<Sprite>("Texture/MainChapter/BG/stage_background_1");

            int stageCount = _stageTable.FindAll(e => e.chapter == _selectChapter).Count;
            StagePos pos = _posList.Find(e => e.cha_id == _selectCharacter && e.chapter == _selectChapter);

            for (int i = 0; i < _stageSlotList.Count; i++)
            {
                if (i < stageCount)
                {
                    _stageSlotList[i].Init(1, i + 1, SelectStageSlot);
                    _stageSlotList[i].SetPosition(pos.posList[i]);
                }
                else
                    _stageSlotList[i].Init();
            }
        }

        private void SelectCharacterSlot(int charIdx)
        {
            _selectCharacter = charIdx;

            OnClickOpenChapter();
        }

        private void SelectChapterSlot(int chapter)
        {
            _selectChapter = chapter;

            OnClickOpenStage();
        }

        private void SelectLockContent(int idx)
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(12), null, true));
        }

        private void SelectStageSlot(int stage)
        {
            _selectStage = stage;

            BattleManager.Singleton.battleType = 2;
            BattleManager.Singleton.selectChapter = _selectChapter;
            BattleManager.Singleton.selectStage = _selectStage;
            BattleManager.Singleton.selectCharacter = _selectCharacter;

            Dialog.IDialog.RequestDialogEnter<Dialog.LobbyStageInfoDialog>();
        }

        private void CheckScenario()
        {
            if (TutorialManager.Singleton.curTutorialIndex > 55)
                return;

            if (Info.My.Singleton.User.maxClearedStory == 0)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_charSlotList[0].transform, () =>
                {
                    SelectCharacterSlot(101031);
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_chapterSlotList[0].transform, () =>
                    {
                        SelectChapterSlot(1);
                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[0].transform, () =>
                        {
                            SelectStageSlot(1);
                        }));
                    }));
                }));
            }
            else if (Info.My.Singleton.User.maxClearedStory < 4)
            {
                int idx = Info.My.Singleton.User.maxClearedStory;
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageSlotList[idx].transform, () =>
                {
                    SelectStageSlot(idx + 1);
                }));
            }
            else
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                {
                    OnClickBack();
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                    {
                        OnClickBack();
                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                        {
                            OnClickBack();
                        }));
                    }));
                }));
            }
        }
    }
}