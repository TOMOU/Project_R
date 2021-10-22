// ==================================================
// LobbyArenaDialog.cs
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
    public class LobbyArenaDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        [Header("- Left")]
        [SerializeField] private List<ArenaTeamSlot> _myTeamSlotList;
        [SerializeField] private Button _formationButton;
        [SerializeField] private Button _challengeButton;
        [SerializeField] private Button _challengeRandomButton;

        [Header("- Right (Ranker)")]
        [SerializeField] private GameObject _rankerObj;
        [SerializeField] private List<ArenaRankSlot> _rankerSlotList;

        [Header("- Right (Enemy)")]
        [SerializeField] private GameObject _userListObj;
        [SerializeField] private List<ArenaUserSlot> _enemySlotList;

        [Header("- Popup (BattleInfo)")]
        [SerializeField] private GameObject _popupObj;
        [SerializeField] private GameObject _objHiddenPopup;
        [SerializeField] private List<FormationSlot> _blueSlotList;
        [SerializeField] private List<FormationSlot> _redSlotList;
        [SerializeField] private Button _formationButton_Popup;
        [SerializeField] private Button _startButton;


        private List<ArenaUserModel.Data> _userTable;
        private ArenaUserModel.Data _selectUserData;
        private bool _isPopupEntered = false;

        protected override void OnLoad()
        {
            base.OnLoad();

            var am = Model.First<ArenaUserModel>();
            if (am != null)
            {
                _userTable = am.Table;
            }

            _isPopupEntered = false;

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _formationButton.onClick.AddListener(OnClickFormation);
            _challengeButton.onClick.AddListener(OnClickChallenge);
            _challengeRandomButton.onClick.AddListener(OnClickChallengeRandom);

            _formationButton_Popup.onClick.AddListener(OnClickFormation);

            _startButton.onClick.AddListener(OnClickStartBattle);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _backButton.onClick.RemoveAllListeners();
            _homeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();

            _formationButton.onClick.RemoveAllListeners();
            _challengeButton.onClick.RemoveAllListeners();
            _challengeRandomButton.onClick.RemoveAllListeners();

            _formationButton_Popup.onClick.RemoveAllListeners();

            _startButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyArenaDialog>();
                RequestDialogEnter<LobbyContentDialog>();
            }));

            if (_isPopupEntered == true)
            {
                _isPopupEntered = false;
                RefreshBattleFormation();
                _objHiddenPopup.SetActive(true);
                return;
            }

            _rankerObj.SetActive(true);
            _userListObj.SetActive(false);
            _popupObj.SetActive(false);

            RefreshMain();

            OpenFirst();

            CheckScenario();
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

        private void OnClickFormation()
        {
            BattleManager.Singleton.battleType = 6;

            if (_popupObj.activeSelf == true)
            {
                _isPopupEntered = true;
                _objHiddenPopup.SetActive(false);
            }

            // 팀 배치 UI 연다.
            // RequestDialogExit<LobbyArenaDialog>();
            RequestDialogEnter<LobbyFormationDialog>();
        }

        private void OnClickChallenge()
        {
            _rankerObj.SetActive(false);
            _userListObj.SetActive(true);
            _popupObj.SetActive(false);

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                _rankerObj.SetActive(true);
                _userListObj.SetActive(false);
                _popupObj.SetActive(false);
            }));

            RefreshChallenge();
        }

        private void OnClickChallengeRandom()
        {
            int ridx = Random.Range(1, _userTable.Count);
            _selectUserData = _userTable.Find(e => e.idx == ridx);

            _rankerObj.SetActive(false);
            _userListObj.SetActive(false);
            _popupObj.SetActive(true);

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                _rankerObj.SetActive(true);
                _userListObj.SetActive(false);
                _popupObj.SetActive(false);
            }));

            RefreshBattleFormation();
        }

        private void OnClickStartBattle()
        {
            BattleManager.Singleton.battleType = 6;
            BattleManager.Singleton.selectChapter = 0;
            BattleManager.Singleton.selectStage = _selectUserData.idx;

            TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Arena;

            Scene.IScene.LoadScene<Scene.BattleNormalScene>();
        }

        private void SelectUserSlotBtn(ArenaUserModel.Data data)
        {
            _selectUserData = data;

            _rankerObj.SetActive(false);
            _userListObj.SetActive(false);
            _popupObj.SetActive(true);

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                _rankerObj.SetActive(false);
                _userListObj.SetActive(true);
                _popupObj.SetActive(false);
            }));

            RefreshBattleFormation();
        }

        private void RefreshMain()
        {
            var list = Info.My.Singleton.Inventory.GetArenaCharacter();

            for (int i = 0; i < _myTeamSlotList.Count; i++)
            {
                if (i < list.Count)
                    _myTeamSlotList[i].Init(list[i]);
                else
                    _myTeamSlotList[i].Init(null);
            }

            for (int i = 0; i < _rankerSlotList.Count; i++)
            {
                _rankerSlotList[i].Init(_userTable[i]);
            }
        }

        private void RefreshChallenge()
        {
            int sIdx = Random.Range(1, _userTable.Count - 5);
            for (int i = 0; i < _enemySlotList.Count; i++)
            {
                _enemySlotList[i].Init(_userTable[i + sIdx], SelectUserSlotBtn);
            }
        }

        private void RefreshBattleFormation()
        {
            _objHiddenPopup.SetActive(true);

            Info.Inventory inven = Info.My.Singleton.Inventory;

            for (int i = 0; i < _blueSlotList.Count; i++)
            {
                _blueSlotList[i].Init(i, inven.GetCharacterByIndex(inven.arenaTeam[i / 3, i % 3]));
            }

            var list = GetCharacterInUserData();
            for (int i = 0; i < _redSlotList.Count; i++)
            {
                _redSlotList[i].Init(i, list[i]);
            }
        }

        private List<Info.Character> GetCharacterInUserData()
        {
            if (_selectUserData == null)
                return null;

            uint[,] team = new uint[3, 3];
            int[,] lv = new int[3, 3];

            if (_selectUserData.unit_id_1 > 0)
            {
                team[_selectUserData.unit_pos_1 % 3, _selectUserData.unit_pos_1 / 3] = _selectUserData.unit_id_1;
                lv[_selectUserData.unit_pos_1 % 3, _selectUserData.unit_pos_1 / 3] = _selectUserData.unit_lv_1;
                // result[_selectUserData.unit_pos_1] = new Info.Character(0, (int)_selectUserData.unit_id_1, _selectUserData.unit_lv_1, 1);
            }

            if (_selectUserData.unit_id_2 > 0)
            {
                team[_selectUserData.unit_pos_2 % 3, _selectUserData.unit_pos_2 / 3] = _selectUserData.unit_id_2;
                lv[_selectUserData.unit_pos_2 % 3, _selectUserData.unit_pos_2 / 3] = _selectUserData.unit_lv_2;
                // result[_selectUserData.unit_pos_2] = new Info.Character(0, (int)_selectUserData.unit_id_2, _selectUserData.unit_lv_2, 1);
            }

            if (_selectUserData.unit_id_3 > 0)
            {
                team[_selectUserData.unit_pos_3 % 3, _selectUserData.unit_pos_3 / 3] = _selectUserData.unit_id_3;
                lv[_selectUserData.unit_pos_3 % 3, _selectUserData.unit_pos_3 / 3] = _selectUserData.unit_lv_3;
                // result[_selectUserData.unit_pos_3] = new Info.Character(0, (int)_selectUserData.unit_id_3, _selectUserData.unit_lv_3, 1);
            }

            if (_selectUserData.unit_id_4 > 0)
            {
                team[_selectUserData.unit_pos_4 % 3, _selectUserData.unit_pos_4 / 3] = _selectUserData.unit_id_4;
                lv[_selectUserData.unit_pos_4 % 3, _selectUserData.unit_pos_4 / 3] = _selectUserData.unit_lv_4;
                // result[_selectUserData.unit_pos_4] = new Info.Character(0, (int)_selectUserData.unit_id_4, _selectUserData.unit_lv_4, 1);
            }

            if (_selectUserData.unit_id_5 > 0)
            {
                team[_selectUserData.unit_pos_5 % 3, _selectUserData.unit_pos_5 / 3] = _selectUserData.unit_id_5;
                lv[_selectUserData.unit_pos_5 % 3, _selectUserData.unit_pos_5 / 3] = _selectUserData.unit_lv_5;
                // result[_selectUserData.unit_pos_5] = new Info.Character(0, (int)_selectUserData.unit_id_5, _selectUserData.unit_lv_5, 1);
            }

            List<Info.Character> result = new List<Info.Character>();
            for (int i = 0; i < team.GetLength(0); i++)
            {
                for (int j = 0; j < team.GetLength(1); j++)
                {
                    if (team[i, j] > 0)
                    {
                        result.Add(new Info.Character(0, (int)team[i, j], lv[i, j], 1));
                    }
                    else
                    {
                        result.Add(null);
                    }
                }
            }

            return result;
        }

        private void OpenFirst()
        {
            switch (TutorialManager.Singleton.openFirst)
            {
                case Constant.TutorialCallbackType.Arena:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    break;
            }
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 88)
            {
                if (Info.My.Singleton.User.isCompleteArena == false)
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_challengeButton.transform, () =>
                    {
                        OnClickChallenge();

                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_enemySlotList[0].ChallengeButton.transform, () =>
                        {
                            SelectUserSlotBtn(_enemySlotList[0].Data);

                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_startButton.transform, OnClickStartBattle));
                        }));
                    }));
                }
                else
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
                }
            }
        }
    }
}