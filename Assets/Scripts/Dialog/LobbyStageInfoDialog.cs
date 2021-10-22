// ==================================================
// LobbyStageInfoDialog.cs
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
    public class LobbyStageInfoDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _titleLabel;

        [Header("- Popup")]
        [SerializeField] private GameObject _stageInfoPopup;
        [SerializeField] private List<StageInfoMonsterSlot> _monsterList;
        [SerializeField] private List<FormationSlot> _teamSlotList;

        [SerializeField] private Text _stageNameText;

        [SerializeField] private Button _stageEnterButton;
        [SerializeField] private Button _stageCancelButton;
        [SerializeField] private Button _stageSkipButton;
        [SerializeField] private Button _stageFormationButton;

        protected override void OnLoad()
        {
            base.OnLoad();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _stageEnterButton.onClick.AddListener(OnClickStartBattle);
            _stageCancelButton.onClick.AddListener(OnClickCancelbutton);
            _stageSkipButton.onClick.AddListener(OnClickSkipButtton);
            _stageFormationButton.onClick.AddListener(OnClickOpenFormationButton);
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

            _stageEnterButton.onClick.RemoveAllListeners();
            _stageCancelButton.onClick.RemoveAllListeners();
            _stageSkipButton.onClick.RemoveAllListeners();
            _stageFormationButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            SetTitleName();
            SetBoardInfo();
            SetTeamBoard();

            _stageSkipButton.gameObject.SetActive(BattleManager.Singleton.battleType == 1);

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                Dialog.IDialog.RequestDialogExit<LobbyStageInfoDialog>();
            }));

            CheckScenario();
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void SetTitleName()
        {
            int chapter = BattleManager.Singleton.selectChapter;
            int stage = BattleManager.Singleton.selectStage;

            switch (BattleManager.Singleton.battleType)
            {
                case 1:
                    _stageNameText.text = string.Format("{0}  {1} - {2}", LocalizeManager.Singleton.GetString(8002 + chapter - 1), chapter, stage);
                    break;

                case 2:
                    _stageNameText.text = string.Format("{0}  {1} - {2}", LocalizeManager.Singleton.GetString(10002), chapter, stage);
                    break;

                case 3:
                    _stageNameText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(9001), LocalizeManager.Singleton.GetString(9002 + stage - 1));
                    break;

                case 4:
                    _stageNameText.text = LocalizeManager.Singleton.GetString(13001);
                    break;

                case 5:
                    _stageNameText.text = string.Format(LocalizeManager.Singleton.GetString(11033), stage);
                    break;
            }
        }

        private void SetBoardInfo()
        {
            int type = BattleManager.Singleton.battleType;
            int chapter = BattleManager.Singleton.selectChapter;
            int stage = BattleManager.Singleton.selectStage;

            StageModel sm = Model.First<StageModel>();
            RoundModel rm = Model.First<RoundModel>();

            StageModel.Data sData = null;
            RoundModel.Data rData = null;

            List<int> round = new List<int>();
            List<uint> monster = new List<uint>();

            if (type != 3)
            {
                sData = sm.Table.Find(e => e.type == type && e.chapter == chapter && e.stage == stage);
            }
            else
            {
                sData = sm.Table.Find(e => e.type == type && e.grade == stage);
            }

            if (sData.round_id_1 > 0)
                round.Add(sData.round_id_1);
            if (sData.round_id_2 > 0)
                round.Add(sData.round_id_2);
            if (sData.round_id_3 > 0)
                round.Add(sData.round_id_3);

            for (int i = 0; i < round.Count; i++)
            {
                rData = rm.Table.Find(e => e.index == round[i]);
                if (rData.mob_id_11 > 0 && monster.Contains(rData.mob_id_11) == false)
                    monster.Add(rData.mob_id_11);
                if (rData.mob_id_12 > 0 && monster.Contains(rData.mob_id_12) == false)
                    monster.Add(rData.mob_id_12);
                if (rData.mob_id_13 > 0 && monster.Contains(rData.mob_id_13) == false)
                    monster.Add(rData.mob_id_13);
                if (rData.mob_id_21 > 0 && monster.Contains(rData.mob_id_21) == false)
                    monster.Add(rData.mob_id_21);
                if (rData.mob_id_22 > 0 && monster.Contains(rData.mob_id_22) == false)
                    monster.Add(rData.mob_id_22);
                if (rData.mob_id_23 > 0 && monster.Contains(rData.mob_id_23) == false)
                    monster.Add(rData.mob_id_23);
                if (rData.mob_id_31 > 0 && monster.Contains(rData.mob_id_31) == false)
                    monster.Add(rData.mob_id_31);
                if (rData.mob_id_32 > 0 && monster.Contains(rData.mob_id_32) == false)
                    monster.Add(rData.mob_id_32);
                if (rData.mob_id_33 > 0 && monster.Contains(rData.mob_id_33) == false)
                    monster.Add(rData.mob_id_33);
            }

            for (int i = 0; i < _monsterList.Count - 1; i++)
            {
                if (i < monster.Count)
                {
                    _monsterList[i].gameObject.SetActive(true);
                    _monsterList[i].Init(monster[i]);
                }
                else
                {
                    _monsterList[i].gameObject.SetActive(false);
                }
            }
        }

        private void SetTeamBoard()
        {
            Info.Inventory inven = Info.My.Singleton.Inventory;
            uint[,] _contentTeam = new uint[3, 3];

            switch (BattleManager.Singleton.battleType)
            {
                case 0:
                case 1:
                case 4:
                    _contentTeam = inven.storyTeam;
                    break;

                case 2:
                    _contentTeam = inven.subTeam;
                    break;

                case 3:
                    _contentTeam = inven.dimensionTeam;
                    break;

                case 5:
                    _contentTeam = inven.babelTeam;
                    break;

                case 6:
                    _contentTeam = inven.arenaTeam;
                    break;
            }

            for (int i = 0; i < _teamSlotList.Count; i++)
            {
                _teamSlotList[i].Init(i, inven.GetCharacterByIndex(_contentTeam[i / 3, i % 3]));
            }
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

        private void OnClickCancelbutton()
        {
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClickStartBattle()
        {
            if (CanStartGame() == false)
            {
                Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(16011), null, true));
                return;
            }

            switch (BattleManager.Singleton.battleType)
            {
                case 1:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;
                    break;

                case 2:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Story;
                    break;

                case 3:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Dimension;
                    break;

                case 4:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.ContentMap;
                    break;

                case 5:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Babel;
                    break;

                case 6:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.ContentMap;
                    break;
            }

            BattleManager.Singleton.inLobby = false;

            // 보스전, 혹은 메인스토리 4-1의 경우 레이드씬으로 보낸다.
            if (BattleManager.Singleton.battleType == 4 || (BattleManager.Singleton.battleType == 1 && BattleManager.Singleton.selectChapter == 4 && BattleManager.Singleton.selectStage == 1))
                Scene.IScene.LoadScene<Scene.BattleRaidScene>();
            else
                Scene.IScene.LoadScene<Scene.BattleNormalScene>();
        }

        private void OnClickSkipButtton()
        {

        }

        private void OnClickOpenFormationButton()
        {
            BattleManager.Singleton.inLobby = true;

            // 팀 배치 UI 연다.
            RequestDialogExit<LobbyStageInfoDialog>();
            RequestDialogEnter<LobbyFormationDialog>();
        }

        private bool CanStartGame()
        {
            int cnt = 0;

            if (BattleManager.Singleton.battleType != 4)
            {
                uint[,] contentTeam = new uint[3, 3];
                switch (BattleManager.Singleton.battleType)
                {
                    case 1:
                        contentTeam = Info.My.Singleton.Inventory.storyTeam;
                        break;

                    case 2:
                        contentTeam = Info.My.Singleton.Inventory.subTeam;
                        break;

                    case 3:
                        contentTeam = Info.My.Singleton.Inventory.dimensionTeam;
                        break;

                    case 5:
                        contentTeam = Info.My.Singleton.Inventory.babelTeam;
                        break;

                    case 6:
                        contentTeam = Info.My.Singleton.Inventory.arenaTeam;
                        break;
                }

                for (int i = 0; i < contentTeam.GetLength(0); i++)
                {
                    for (int j = 0; j < contentTeam.GetLength(1); j++)
                    {
                        if (contentTeam[i, j] > 0)
                            cnt++;
                    }
                }
            }
            else
            {
                uint[] contentTeam = Info.My.Singleton.Inventory.bossTeam;
                for (int i = 0; i < contentTeam.Length; i++)
                {
                    if (contentTeam[i] > 0)
                        cnt++;
                }
            }

            return cnt > 0;
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 11)
            {
                // 편성화면 보여주기
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageFormationButton.transform, () =>
                {
                    OnClickOpenFormationButton();
                }));
            }
            else if (sidx == 13)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, OnClickStartBattle));
            }
            else if (sidx == 31)
            {
                // 진시황 편성
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageFormationButton.transform, OnClickOpenFormationButton));
            }
            else if (sidx == 32)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 38)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 40)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 45)
            {
                // 편성에서 판도라
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageFormationButton.transform, OnClickOpenFormationButton));
            }
            else if (sidx == 46)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 47)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(47, OnClickStartBattle));
                }));
            }
            else if (sidx == 52)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, OnClickStartBattle));
            }
            else if (sidx == 55)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 58)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 60)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 63)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 66)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                        {
                            OnClickStartBattle();
                        }));
            }
            else if (sidx == 70)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, OnClickStartBattle));
            }
            else if (sidx == 72)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 75 && (Info.My.Singleton.User.maxClearedMainstory == 9 || Info.My.Singleton.User.maxClearedMainstory == 10))
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 82)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
            else if (sidx == 88)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageEnterButton.transform, () =>
                {
                    OnClickStartBattle();
                }));
            }
        }
    }
}