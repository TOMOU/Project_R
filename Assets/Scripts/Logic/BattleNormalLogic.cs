// ==================================================
// BattleNormalLogic.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using System;
using UnityEngine;

namespace Logic
{
    public class BattleNormalLogic : ILogic
    {
        private Transform _unitRoot = null;
        private Transform _bgRoot = null;
        private GameObject _bgBlock = null;
        private Constant.BattlePhase _depth = Constant.BattlePhase.None;
        private int _msgCount = 0;
        private bool _msgReceived = false;
        private int _blueTeamCount = 0;
        private int _redTeamCount = 0;
        private int _curPhase;
        private int _maxPhase;

        private uint[,] _currentTeam;

        private bool _enableInstantKill = true;

        private StageModel.Data _stageData = null;

        private Coroutine _coroutine = null;

        [SerializeField] private float _curTime = 0f;
        [SerializeField] private float _maxTime = 90f;

        [Serializable]
        public class RoundEnemy
        {
            public int round;
            public List<GameObject> objList;
            public List<UnitStatus> statList;
            public List<int> slotList;
            public RoundEnemy(int r)
            {
                round = r;
                objList = new List<GameObject>();
                statList = new List<UnitStatus>();
                slotList = new List<int>();
            }
        }
        [SerializeField] private List<RoundEnemy> _roundEnemyList = new List<RoundEnemy>();

        protected override void Initialize()
        {
            // Caching parameter
            _unitRoot = GameObject.Find("UnitRoot").transform;

            _bgRoot = GameObject.Find("Background").transform;

            _bgRoot.localPosition = new Vector3(2.7f, 0f, 0f);

            if (BattleManager.Singleton.battleType == 1)
            {
                _bgRoot.Find("BG/chapter_1").gameObject.SetActive(BattleManager.Singleton.selectChapter == 1);
                _bgRoot.Find("BG/chapter_2").gameObject.SetActive(BattleManager.Singleton.selectChapter == 2);
                _bgRoot.Find("BG/chapter_3").gameObject.SetActive(BattleManager.Singleton.selectChapter == 3);
                _bgRoot.Find("BG/chapter_4").gameObject.SetActive(BattleManager.Singleton.selectChapter == 4);
            }
            else if (BattleManager.Singleton.battleType == 4)
            {
                _bgRoot.Find("BG/chapter_1").gameObject.SetActive(false);
                _bgRoot.Find("BG/chapter_2").gameObject.SetActive(false);
                _bgRoot.Find("BG/chapter_3").gameObject.SetActive(false);
                _bgRoot.Find("BG/chapter_4").gameObject.SetActive(true);
            }
            else
            {
                _bgRoot.Find("BG/chapter_1").gameObject.SetActive(BattleManager.Singleton.selectStage % 3 == 0);
                _bgRoot.Find("BG/chapter_2").gameObject.SetActive(BattleManager.Singleton.selectStage % 3 == 1);
                _bgRoot.Find("BG/chapter_3").gameObject.SetActive(BattleManager.Singleton.selectStage % 3 == 2);
                _bgRoot.Find("BG/chapter_4").gameObject.SetActive(false);
            }

            _bgBlock = _bgRoot.transform.Find("Background_Block").gameObject;
            _bgBlock.SetActive(false);

            _msgCount = 0;
            _curPhase = 0;

            // 아레나가 아닌 경우
            if (BattleManager.Singleton.battleType != 6)
            {
                var sm = Model.First<StageModel>();
                if (sm == null)
                {
                    Logger.LogError("RoundModel을 가져올 수 없음");
                    return;
                }

                switch (BattleManager.Singleton.battleType)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                        _stageData = sm.Table.Find(e => e.type == BattleManager.Singleton.battleType && e.chapter == BattleManager.Singleton.selectChapter && e.stage == BattleManager.Singleton.selectStage);
                        break;

                    case 3:
                        _stageData = sm.Table.Find(e => e.type == BattleManager.Singleton.battleType && e.grade == BattleManager.Singleton.selectStage);
                        break;
                }

                if (_stageData == null)
                {
                    Logger.LogError("StageModel Data를 가져올 수 없음");
                    return;
                }

                _maxPhase = _stageData.round_count;
            }
            // 아레나의 경우 커스텀
            else
            {
                _maxPhase = 1;
            }

            _blueTeamCount = 0;
            _redTeamCount = 0;

            // AddListener
            Message.AddListener<Battle.Normal.SendUseSkillMsg>(OnSendUseSkill);
            Message.AddListener<Battle.Normal.SendImDyingMsg>(OnSendImDying);
            Message.AddListener<Battle.Normal.MoveToBaseCompleteMsg>(OnMoveToBaseComplete);
            Message.AddListener<Battle.Normal.StartBattleCompleteMsg>(OnStartBattleComplete);
            Message.AddListener<Battle.Normal.MoveToNextCompleteMsg>(OnMoveToNextComplete);
            Message.AddListener<Battle.Normal.MoveToEndCompleteMsg>(OnMoveToEndComplete);
            Message.AddListener<Battle.Normal.Scenario_PandoraMsg>(OnScenario_Pandora);
            Message.AddListener<Battle.Normal.LockInstantKillMsg>(OnLockInstantKill);
            Message.AddListener<Battle.Normal.UnlockInstantKillMsg>(OnUnlockInstantKill);

            // 유닛 소환
            SummonAllyLogic();

            if (BattleManager.Singleton.battleType != 6)
            {
                // 적 라운드 크기별로 소환
                for (int i = 0; i < _maxPhase; i++)
                {
                    SummonEnemyLogic(i + 1);
                }
            }
            else
            {
                SummonEnemy_Arena();
            }

            BlueTeam_Position();
            RedTeam_Position();

            // 현재 Tutorial상태를 확인하고 시나리오 재생할지, 바로 플레이할지를 확인.
            CheckScenario();

            Message.Send<Battle.Normal.AttachStageTimeMsg>(new Battle.Normal.AttachStageTimeMsg(RefreshTimeInUI));
            Message.Send<Battle.Normal.AttachInstantKillMsg>(new Battle.Normal.AttachInstantKillMsg(RefreshInstantKill));

            string battleType = string.Empty;
            string battleInfo = string.Empty;

            switch (BattleManager.Singleton.battleType)
            {
                case 0:
                    battleType = "튜토리얼 전투(극초반)";
                    battleInfo = "";
                    break;

                case 1:
                    battleType = "메인스토리";
                    battleInfo = string.Format("chapter {0}  stage {1}", BattleManager.Singleton.selectChapter, BattleManager.Singleton.selectStage);
                    break;

                case 2:
                    battleType = "대신전";
                    battleInfo = string.Format("chapter {0}  stage {1}", BattleManager.Singleton.selectChapter, BattleManager.Singleton.selectStage);
                    break;

                case 3:
                    battleType = "차원던전";
                    battleInfo = string.Format("난이도 {0}", BattleManager.Singleton.selectStage);
                    break;

                case 4:
                    battleType = "보스전";
                    battleInfo = string.Format("stage {0}", BattleManager.Singleton.selectStage);
                    break;

                case 5:
                    battleType = "바벨타워";
                    battleInfo = string.Format("층수 {0}", BattleManager.Singleton.selectStage);
                    break;

                case 6:
                    battleType = "콜로세움";

                    var am = Model.First<ArenaUserModel>();
                    ArenaUserModel.Data data = am.Table.Find(e => e.idx == BattleManager.Singleton.selectStage);

                    battleInfo = string.Format("전투대상 = {0}", data.userName);
                    break;
            }

            Message.Send<Battle.Normal.AttachStageWaveMsg>(new Battle.Normal.AttachStageWaveMsg(1, _maxPhase));

            Logger.LogFormat("[{0}] {1}", battleType, battleInfo);
        }

        protected override void Release()
        {
            _unitRoot = null;
            _bgRoot = null;
            _bgBlock = null;
            _stageData = null;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (_roundEnemyList != null)
            {
                for (int i = 0; i < _roundEnemyList.Count; i++)
                {
                    _roundEnemyList[i].objList.Clear();
                    _roundEnemyList[i].objList = null;

                    _roundEnemyList[i].statList.Clear();
                    _roundEnemyList[i].statList = null;

                    _roundEnemyList[i].slotList.Clear();
                    _roundEnemyList[i].slotList = null;
                }

                _roundEnemyList.Clear();
                _roundEnemyList = null;
            }

            // RemoveListener
            Message.RemoveListener<Battle.Normal.SendUseSkillMsg>(OnSendUseSkill);
            Message.RemoveListener<Battle.Normal.SendImDyingMsg>(OnSendImDying);
            Message.RemoveListener<Battle.Normal.MoveToBaseCompleteMsg>(OnMoveToBaseComplete);
            Message.RemoveListener<Battle.Normal.StartBattleCompleteMsg>(OnStartBattleComplete);
            Message.RemoveListener<Battle.Normal.MoveToNextCompleteMsg>(OnMoveToNextComplete);
            Message.RemoveListener<Battle.Normal.MoveToEndCompleteMsg>(OnMoveToEndComplete);
            Message.RemoveListener<Battle.Normal.Scenario_PandoraMsg>(OnScenario_Pandora);
            Message.RemoveListener<Battle.Normal.LockInstantKillMsg>(OnLockInstantKill);
            Message.RemoveListener<Battle.Normal.UnlockInstantKillMsg>(OnUnlockInstantKill);
        }

        private void GameStart()
        {
            // 적 활성화

            Dialog.IDialog.RequestDialogEnter<Dialog.NormalMainDialog>();

            MoveToBase();
        }

        #region 유닛 초기화 배치 관련
        private void SummonAllyLogic()
        {
            int cnt = 0;
            Info.Inventory inven = Info.My.Singleton.Inventory;

            switch (BattleManager.Singleton.battleType)
            {
                case 0:
                case 1:
                    _currentTeam = inven.storyTeam;
                    break;

                case 2:
                    _currentTeam = inven.subTeam;
                    break;

                case 3:
                    _currentTeam = inven.dimensionTeam;
                    break;

                case 5:
                    _currentTeam = inven.babelTeam;
                    break;

                case 6:
                    _currentTeam = inven.arenaTeam;
                    break;
            }

            for (int i = 0; i < _currentTeam.GetLength(0); i++)
            {
                for (int j = 0; j < _currentTeam.GetLength(1); j++)
                {
                    Info.Character cha = inven.GetCharacterByIndex(_currentTeam[i, j]);
                    if (cha != null)
                    {
                        var info = SummonAlly(cha.GetStatus(), Constant.Team.Blue, (2 - i) + j * 3);
                        if (info != null)
                        {
                            Message.Send<Battle.Normal.UnitSlotInitMsg>(new Battle.Normal.UnitSlotInitMsg(cnt++, info.Status));

                            BattleManager.Singleton.AddUnit(Constant.Team.Blue, info);

                            info.CheatHP();
                            info.CheatDamage();
                        }
                    }
                }
            }
        }

        private void SummonEnemyLogic(int round)
        {
            int ridx = 0;
            switch (round)
            {
                case 1:
                    ridx = _stageData.round_id_1;
                    break;

                case 2:
                    ridx = _stageData.round_id_2;
                    break;

                case 3:
                    ridx = _stageData.round_id_3;
                    break;
            }

            // RoundModel 로드
            var rm = Model.First<RoundModel>();
            if (rm == null)
            {
                Logger.LogError("RoundModel을 찾을 수 없습니다.");
                return;
            }

            RoundModel.Data r = rm.Table.Find(e => e.index == ridx);
            if (r == null)
            {
                Logger.LogError("RoundData를 찾을 수 없습니다.");
                return;
            }

            var roundEnemy = _roundEnemyList.Find(e => e.round == round);
            if (roundEnemy == null)
            {
                roundEnemy = new RoundEnemy(round);
                _roundEnemyList.Add(roundEnemy);
            }

            var statList = GetEnemyStatusList(r);
            var posList = GetEnemyPosList(r);

            for (int i = 0; i < statList.Count; i++)
            {
                GameObject obj = SummonEnemy(statList[i], Constant.Team.Red, posList[i], round);

                if (obj != null)
                {
                    obj.SetActive(false);

                    roundEnemy.objList.Add(obj);
                    roundEnemy.statList.Add(statList[i]);
                    roundEnemy.slotList.Add(posList[i]);
                }
            }
        }

        private void SummonEnemy_Arena()
        {
            var am = Model.First<ArenaUserModel>();
            ArenaUserModel.Data data = am.Table.Find(e => e.idx == BattleManager.Singleton.selectStage);

            var um = Model.First<UnitModel>();
            var table = um.unitTable;

            List<UnitStatus> statList = new List<UnitStatus>();

            List<int> unitList = new List<int>();
            List<int> slotList = new List<int>();
            List<int> levelList = new List<int>();
            List<int> gradeList = new List<int>();
            if (data.unit_id_1 > 0)
            {
                unitList.Add((int)data.unit_id_1);
                slotList.Add(data.unit_pos_1);
                levelList.Add(data.unit_lv_1);
                gradeList.Add(data.unit_g_1);
            }
            if (data.unit_id_2 > 0)
            {
                unitList.Add((int)data.unit_id_2);
                slotList.Add(data.unit_pos_2);
                levelList.Add(data.unit_lv_2);
                gradeList.Add(data.unit_g_2);
            }
            if (data.unit_id_3 > 0)
            {
                unitList.Add((int)data.unit_id_3);
                slotList.Add(data.unit_pos_3);
                levelList.Add(data.unit_lv_3);
                gradeList.Add(data.unit_g_3);
            }
            if (data.unit_id_4 > 0)
            {
                unitList.Add((int)data.unit_id_4);
                slotList.Add(data.unit_pos_4);
                levelList.Add(data.unit_lv_4);
                gradeList.Add(data.unit_g_4);
            }
            if (data.unit_id_5 > 0)
            {
                unitList.Add((int)data.unit_id_5);
                slotList.Add(data.unit_pos_5);
                levelList.Add(data.unit_lv_5);
                gradeList.Add(data.unit_g_5);
            }

            for (int i = 0; i < unitList.Count; i++)
            {
                var unit = table.Find(e => e.code == unitList[i]);
                statList.Add(new UnitStatus(unit, 0, levelList[i], gradeList[i]));
            }

            var roundEnemy = _roundEnemyList.Find(e => e.round == 1);
            if (roundEnemy == null)
            {
                roundEnemy = new RoundEnemy(1);
                _roundEnemyList.Add(roundEnemy);
            }

            for (int i = 0; i < statList.Count; i++)
            {
                GameObject obj = SummonEnemy(statList[i], Constant.Team.Red, i, 1);

                if (obj != null)
                {
                    obj.SetActive(false);

                    roundEnemy.objList.Add(obj);
                    roundEnemy.statList.Add(statList[i]);
                    roundEnemy.slotList.Add(slotList[i]);
                }
            }
        }

        private List<GameObject> GetEnemyObjList(RoundModel.Data data)
        {
            List<GameObject> list = new List<GameObject>();



            return list;
        }

        private List<UnitStatus> GetEnemyStatusList(RoundModel.Data data)
        {
            List<UnitStatus> list = new List<UnitStatus>();

            if (data.mob_id_11 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_11, data.mob_lv_11, 1).GetStatus());
            if (data.mob_id_12 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_12, data.mob_lv_12, 1).GetStatus());
            if (data.mob_id_13 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_13, data.mob_lv_13, 1).GetStatus());

            if (data.mob_id_21 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_21, data.mob_lv_21, 1).GetStatus());
            if (data.mob_id_22 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_22, data.mob_lv_22, 1).GetStatus());
            if (data.mob_id_23 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_23, data.mob_lv_23, 1).GetStatus());

            if (data.mob_id_31 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_31, data.mob_lv_31, 1).GetStatus());
            if (data.mob_id_32 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_32, data.mob_lv_32, 1).GetStatus());
            if (data.mob_id_33 > 0)
                list.Add(new Info.Character(0, (int)data.mob_id_33, data.mob_lv_33, 1).GetStatus());

            return list;
        }

        private List<int> GetEnemyPosList(RoundModel.Data data)
        {
            List<int> list = new List<int>();

            if (data.mob_id_11 > 0)
                list.Add(0);
            if (data.mob_id_12 > 0)
                list.Add(3);
            if (data.mob_id_13 > 0)
                list.Add(6);

            if (data.mob_id_21 > 0)
                list.Add(1);
            if (data.mob_id_22 > 0)
                list.Add(4);
            if (data.mob_id_23 > 0)
                list.Add(7);

            if (data.mob_id_31 > 0)
                list.Add(2);
            if (data.mob_id_32 > 0)
                list.Add(5);
            if (data.mob_id_33 > 0)
                list.Add(8);

            return list;
        }

        /// <summary>
        /// 유닛을 소환
        /// </summary>
        /// <param name="status">유닛의 스테이터스 정보</param>
        /// <param name="team">유닛의 팀 번호 (0 = BlueTeam)</param>
        /// <param name="idx">유닛의 배치 번호</param>
        private UnitInfo_Normal SummonAlly(UnitStatus status, Constant.Team team, int teamSlotIndex)
        {
            // Load UnitModel
            var um = Model.First<UnitModel>();
            if (um == null)
            {
                Logger.LogError("Can't load UnitModel");
                return null;
            }

            string unitName = status.unitName;
            int skinIndex = 0;

            if (unitName.Contains("_") == true)
            {
                var split = status.unitName.Split('_');
                unitName = split[0];
                skinIndex = int.Parse(split[1]);
            }

            // Load Prefab based in UnitStatus.
            GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", unitName));
            if (prefab == null)
            {
                Logger.LogErrorFormat("Can't load Prefab in Character/{0}.", unitName);
                return null;
            }

            // Instantiate prefab in scene.
            GameObject obj = GameObject.Instantiate(prefab);

            // Active false before positioning.
            obj.SetActive(false);

            // Set position.
            obj.transform.SetParent(_unitRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one * 0.3f;

            // Active true after positioning.
            obj.SetActive(true);

            // Caching SkeletonAnimation.
            SkeletonAnimation skel = obj.GetComponent<SkeletonAnimation>();
            if (skel == null)
            {
                Logger.LogErrorFormat("Can't find <SkeletonAnimation> Component in {0} object.", obj.name);
                return null;
            }

            // Set Skin
            if (skinIndex > 0)
            {
                skel.skeleton.SetSkin(skel.skeleton.Data.Skins.Items[skinIndex]);
            }

            // Add UnitFSM
            UnitFSM fsm = obj.AddComponent<UnitFSM>();
            fsm.Init(skel, 0);

            // If unit is RedTeam, SkeletonAnimation FlipX.
            if (team == Constant.Team.Red)
            {
                fsm.Skeleton.skeleton.ScaleX = -1f;
            }
            else
            {
                // Add _blueTeamCount
                _blueTeamCount++;
            }

            // Add UnitInfo
            UnitInfo_Normal info = obj.AddComponent<UnitInfo_Normal>();
            info.Init(team, status, fsm);
            info.slotIndex = teamSlotIndex;

            SkillBase skill = obj.GetComponent<SkillBase>();
            skill.Init(info, skel);
            fsm.skill = skill;
            info.useSkill = skill.UseSkill;

            return info;
        }

        private GameObject SummonEnemy(UnitStatus status, Constant.Team team, int teamSlotIndex, int round)
        {
            // Load UnitModel
            var um = Model.First<UnitModel>();
            if (um == null)
            {
                Logger.LogError("Can't load UnitModel");
                return null;
            }

            string unitName = status.unitName;
            int skinIndex = 0;

            if (unitName.Contains("_") == true)
            {
                var split = status.unitName.Split('_');
                unitName = split[0];
                skinIndex = int.Parse(split[1]);
            }

            // Load Prefab based in UnitStatus.
            GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", unitName));
            if (prefab == null)
            {
                Logger.LogErrorFormat("Can't load Prefab in Character/{0}.", unitName);
                return null;
            }

            // Instantiate prefab in scene.
            GameObject obj = GameObject.Instantiate(prefab);

            // Active false before positioning.
            obj.SetActive(false);

            // Set position.
            obj.transform.SetParent(_unitRoot);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one * 0.3f;

            if (status.code == 109001)
                obj.transform.localScale = Vector3.one * 0.15f;

            // Active true after positioning.
            obj.SetActive(true);

            // Caching SkeletonAnimation.
            SkeletonAnimation skel = obj.GetComponent<SkeletonAnimation>();
            if (skel == null)
            {
                Logger.LogErrorFormat("Can't find <SkeletonAnimation> Component in {0} object.", obj.name);
                return null;
            }

            // Set Skin
            if (skinIndex > 0)
            {
                skel.skeleton.SetSkin(skel.skeleton.Data.Skins.Items[skinIndex]);
            }

            // If unit is RedTeam, SkeletonAnimation FlipX.
            if (team == Constant.Team.Red)
            {
                skel.skeleton.ScaleX = -1f;
            }
            else
            {
                // Add _blueTeamCount
                _blueTeamCount++;
            }

            // // Add UnitInfo
            // UnitInfo_Normal info = obj.AddComponent<UnitInfo_Normal>();
            // info.Init(team, status, fsm);
            // info.slotIndex = teamSlotIndex;

            // SkillBase skill = obj.GetComponent<SkillBase>();
            // skill.Init(info, skel);
            // fsm.skill = skill;

            return obj;
        }

        private void ActiveEnemy()
        {
            if (_curPhase > 0)
            {
                var prevList = BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>();
                prevList.ForEach(e =>
                {
                    e.Release();
                    GameObject.Destroy(e.gameObject);
                });

                prevList.Clear();
                prevList = null;

                BattleManager.Singleton.RemoveTeamUnit(Constant.Team.Red);
            }

            RoundEnemy r = _roundEnemyList[_curPhase];

            for (int i = 0; i < r.objList.Count; i++)
            {
                r.objList[i].SetActive(true);

                SkeletonAnimation skel = r.objList[i].GetComponent<SkeletonAnimation>();

                if (r.statList[i].unitName.Contains("_") == true)
                {
                    int skin = int.Parse(r.statList[i].unitName.Split('_')[1]);
                    if (skin > 0)
                    {
                        skel.skeleton.SetSkin(skel.skeleton.Data.Skins.Items[skin]);
                    }
                }

                // Add UnitFSM
                UnitFSM fsm = r.objList[i].AddComponent<UnitFSM>();
                fsm.Init(skel, 0);

                UnitInfo_Normal info = r.objList[i].AddComponent<UnitInfo_Normal>();
                info.Init(Constant.Team.Red, r.statList[i], fsm);
                info.slotIndex = r.slotList[i];

                SkillBase skill = r.objList[i].GetComponent<SkillBase>();
                skill.Init(info, skel);
                fsm.skill = skill;

                info.useSkill = skill.UseSkill;

                if (BattleManager.Singleton.battleType == 6)
                {
                    if (Info.My.Singleton.User.isCompleteArena == true)
                    {
                        info.CheatHP();
                        info.CheatDamage();
                    }
                    // 아직 튜토리얼 아레나 중이니 스텟을 70%정도로 한다.
                    else
                    {
                        info.CheatHP(3);
                        info.CheatDamage(3);
                    }
                }

                BattleManager.Singleton.AddUnit(Constant.Team.Red, info);

                _redTeamCount++;
            }
        }

        /// <summary>
        /// BattleManager에 저장된 유닛 리스트를 기반으로 포지셔닝과 레이어를 세팅한다
        /// </summary>
        /// <param name="team">정렬할 팀</param>
        private void RedTeam_Position()
        {
            var list = BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>();

            foreach (var info in list)
            {
                info.transform.localPosition = new Vector3(
                                 5f + 5f * (info.slotIndex % 3),
                -1.5f - 3f * (info.slotIndex / 3),
                -1.5f - 3f * (info.slotIndex / 3)
                                );

                info.InitBasePosition();
                info.transform.localPosition += Vector3.right * 18f;
            }
        }

        private void BlueTeam_Position()
        {
            var list = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();

            foreach (var info in list)
            {
                info.transform.localPosition = new Vector3(
                                -15f + 5f * (info.slotIndex % 3),
                                -1.5f - 3f * (info.slotIndex / 3),
                                -1.5f - 3f * (info.slotIndex / 3)
                                );

                info.InitBasePosition();
                info.transform.localPosition += Vector3.left * 18f;
            }
        }

        private void ReviveTeam(Constant.Team team, List<UnitInfo_Normal> list)
        {
            if (list == null)
                return;

            list.ForEach(e => e.Revive());

            if (team == Constant.Team.Blue)
                _blueTeamCount = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>().Count;
            else
                _redTeamCount = BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>().Count;
        }
        #endregion

        #region 유닛 통제
        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.S))
            // {
            //     BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>().ForEach(e =>
            //     {
            //         e.Status.mp = e.Status.mpFull;
            //     });
            //     // BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>().ForEach(e =>
            //     // {
            //     //     if (e.Status.code == 103031)
            //     //         e.Status.mp = e.Status.mpFull;
            //     // });
            // }

            // if (Input.GetKeyDown(KeyCode.Keypad1))
            // {
            //     DebugMode_InstantKill();
            // }
            // else if (Input.GetKeyDown(KeyCode.Keypad2))
            // {
            //     Message.Send<Battle.Normal.ForceIdleMsg>(new Battle.Normal.ForceIdleMsg());
            //     _blueTeamCount = 0;
            //     BattleEnd();
            // }
            // else if (Input.GetKeyDown(KeyCode.Keypad3))
            // {
            //     _curTime = _maxTime - 1f;
            // }

            if (_depth == Constant.BattlePhase.StartBattle && _enableInstantKill == true && TutorialManager.Singleton.IsTutorialProcess == false)
            {
                _curTime += Time.deltaTime;
                if (_maxTime - 30f - _curTime <= 30f && _curPhase == 3 && TutorialManager.Singleton.curTutorialIndex == 83)
                {
                    Action_Summon_J();
                }
                else if (_curTime >= _maxTime)
                {
                    _depth = Constant.BattlePhase.TimeUp;

                    Action_TimeUp();
                }
            }
        }

        private void LateUpdate()
        {
            if (_msgReceived == false)
                return;

            ControlUnit();
        }

        private void RefreshTimeInUI(UnityEngine.UI.Text time)
        {
            int remainTime = (int)(_maxTime - _curTime);
            time.text = string.Format("{0:D2}:{1:D2}", remainTime / 60, remainTime % 60);
        }

        private void RefreshInstantKill(UnityEngine.GameObject obj)
        {
            obj.SetActive(_enableInstantKill == true && _depth == Constant.BattlePhase.StartBattle && TutorialManager.Singleton.IsTutorialProcess == false);
        }

        private void ControlUnit()
        {
            if (_msgCount == (_blueTeamCount + _redTeamCount))
            {
                _msgReceived = false;

                switch (_depth)
                {
                    case Constant.BattlePhase.MoveToBase:
                        Action_MoveToBase();
                        break;
                    case Constant.BattlePhase.StartBattle:
                        Action_StartBattle();
                        break;
                    case Constant.BattlePhase.BattleNext:
                        Action_BattleNext();
                        break;
                    case Constant.BattlePhase.BattleEnd:
                        Action_BattleEnd();
                        break;
                }
            }
        }

        /// <summary>
        /// 전투 시작지점으로 이동
        /// </summary>
        private void MoveToBase()
        {
            ActiveEnemy();

            BlueTeam_Position();
            RedTeam_Position();

            _msgCount = 0;
            _msgReceived = false;
            _depth = Constant.BattlePhase.MoveToBase;

            _curPhase++;

            _bgRoot.localPosition = new Vector3(2.7f - (2.7f * (_curPhase - 1)), 0f, 0f);

            Message.Send<Battle.Normal.MoveToBaseMsg>(new Battle.Normal.MoveToBaseMsg());
            Message.Send<Battle.Normal.AttachStageWaveMsg>(new Battle.Normal.AttachStageWaveMsg(_curPhase, _maxPhase));
        }

        private void Action_MoveToBase()
        {
            StartBattle();
        }

        private void OnMoveToBaseComplete(Battle.Normal.MoveToBaseCompleteMsg msg)
        {
            _msgCount++;
            _msgReceived = true;

            // // 모든 살아있는 유닛에게 메세지를 받으면 다음으로 진행.
            // if (_msgCount == (_blueTeamCount + _redTeamCount))
            // {
            //     StartBattle ();
            // }
        }

        /// <summary>
        /// 시작지점으로 이동이 종료되었으니 전투 시작
        /// </summary>
        private void StartBattle()
        {
            _msgCount = 0;
            _msgReceived = false;
            _depth = Constant.BattlePhase.StartBattle;
            Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());

            if (_curPhase == 3 && TutorialManager.Singleton.curTutorialIndex == 59)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(59, () =>
                {
                    Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
                }));
            }
            else if (_curPhase == 3 && TutorialManager.Singleton.curTutorialIndex == 61)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(61, () =>
                {
                    Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
                }));
            }
            else if (_curPhase == 3 && TutorialManager.Singleton.curTutorialIndex == 73)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(73, () =>
                {
                    Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
                }));
            }
            else if (_curPhase == 3 && TutorialManager.Singleton.curTutorialIndex == 83)
            {
                //TODO 적 체력 강제로 10 이하로 안줄어들게 고정.
                Message.Send<Battle.Normal.ImmortalEnemyMsg>(new Battle.Normal.ImmortalEnemyMsg());
                Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
            }
            else if (_curPhase == 1 && TutorialManager.Singleton.curTutorialIndex == 89 && Info.My.Singleton.User.isCompleteArena == true)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(89, () =>
                {
                    Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
                }));
            }
            else if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 1 && _curPhase == 1)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(2, () =>
                {
                    Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
                }, "Chiyou"));
            }
            else
            {
                Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
            }
        }

        private void Action_StartBattle()
        {
            if (_blueTeamCount == 0)
            {
                BattleEnd();
                return;
            }

            // 아직 전투 페이즈가 남았다. 다음 페이즈로 진행.
            if (_curPhase < _maxPhase)
            {
                Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());
                BattleNext();
            }
            // 페이즈가 종료되었으니 결과화면 위치로 이동.
            else
            {
                BattleEnd();
            }
        }

        private void OnStartBattleComplete(Battle.Normal.StartBattleCompleteMsg msg)
        {
            _msgCount++;
            _msgReceived = true;

            // // 모든 살아있는 유닛에게 메세지를 받으면 다음으로 진행.
            // if (_msgCount == (_blueTeamCount + _redTeamCount))
            // {
            //     // 아군이 다 죽었으니 다이렉트로 결과화면으로 간다.
            //     if (_blueTeamCount == 0)
            //     {
            //         BattleEnd ();
            //         return;
            //     }

            //     if (_curPhase < _maxPhase)
            //     {
            //         BattleNext ();
            //     }
            //     else
            //     {
            //         Logger.Log ("Phase End");
            //         BattleEnd ();
            //     }
            // }
        }

        /// <summary>
        /// 현재의 전투가 끝났으니 다음지역으로 이동하라
        /// </summary>
        private void BattleNext()
        {
            _msgCount = 0;
            _msgReceived = false;
            _depth = Constant.BattlePhase.BattleNext;

            Message.Send<Battle.Normal.MoveToNextMsg>(new Battle.Normal.MoveToNextMsg());

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(CameraManager.Singleton.coFadeLoading_Battle(true));
        }

        private void Action_BattleNext()
        {
            MoveToBase();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(CameraManager.Singleton.coFadeLoading_Battle(false));
        }

        private void OnMoveToNextComplete(Battle.Normal.MoveToNextCompleteMsg msg)
        {
            _msgCount++;
            _msgReceived = true;
        }

        /// <summary>
        /// 모든 전투가 종료되었으니 결과 페이즈로 이동하라
        /// </summary>
        private void BattleEnd()
        {
            _msgCount = 0;
            _msgReceived = false;
            _depth = Constant.BattlePhase.BattleEnd;

            // Base Input 설정
            Message.Send<Global.AddBaseEscapeActionMsg>(new Global.AddBaseEscapeActionMsg(null));

            // UI 활성&비활성화
            Dialog.IDialog.RequestDialogExit<Dialog.NormalMainDialog>();
            Dialog.IDialog.RequestDialogEnter<Dialog.NormalResultDialog>();

            // 아군이 이겼다.
            if (_blueTeamCount > 0)
            {
                // 승리팀의 죽은 유닛들을 화면 밖에서부터 이동
                var list = (_blueTeamCount > 0 ? BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>()).FindAll(e => e.isDie == true);
                var team = (_blueTeamCount > 0 ? Constant.Team.Blue : Constant.Team.Red);

                // 부활시킨다
                ReviveTeam(team, list);

                if (BattleManager.Singleton.IsNewStage() == true)
                {
                    switch (BattleManager.Singleton.battleType)
                    {
                        case 1:     // 메인스토리                        
                            Info.My.Singleton.User.maxClearedMainstory++;
                            break;

                        case 2:     // 대신전
                            Info.My.Singleton.User.maxClearedStory++;
                            break;

                        case 3:     // 차원던전
                            Info.My.Singleton.User.maxClearedDimension++;
                            break;

                        case 4:     // 보스전
                            break;

                        case 5:     // 바벨타워                                
                            Info.My.Singleton.User.maxClearedBabel++;
                            break;
                    }
                }

                CheckScenario_End(() =>
                {
                    Message.Send<Battle.Normal.MoveToEndMsg>(new Battle.Normal.MoveToEndMsg());
                });
            }
            // 적이 이겼다. 바로 패배 띄워준다.
            else
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                _coroutine = StartCoroutine(coBattleDefeat());
                // ShowResultDialog();
            }
        }

        private void Action_BattleEnd()
        {
            ShowResultDialog();
        }

        private void Action_Summon_J()
        {
            _depth = Constant.BattlePhase.Summon_J;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coSummon_J());
        }

        private void Action_TimeUp()
        {
            Message.Send<Battle.Normal.ForceIdleMsg>(new Battle.Normal.ForceIdleMsg());

            // Base Input 설정
            Message.Send<Global.AddBaseEscapeActionMsg>(new Global.AddBaseEscapeActionMsg(null));

            Dialog.IDialog.RequestDialogExit<Dialog.NormalMainDialog>();
            Dialog.IDialog.RequestDialogEnter<Dialog.NormalResultDialog>();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coTimeUp());
        }

        private IEnumerator coSummon_J()
        {
            Dialog.IDialog.RequestDialogExit<Dialog.NormalMainDialog>();
            EffectManager.Singleton.AllEffectOff();

            TimeManager.Singleton.IsBattleMode = false;

            Time.timeScale = 1f;

            // 아군적군 정보
            var blue = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();
            var red = BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>();
            var summon = BattleManager.Singleton.GetSummonTeam<UnitInfo_Normal>();

            // 모든 캐릭 전부 강제 idle 초기화
            blue.ForEach(e =>
            {
                e.IsLock = true;
                e.FSM.Skeleton.AnimationState.SetAnimation(0, "idle", true);

                if (e.Status.unitName == "jinshi")
                {
                    e.Status.MinionShield_Idle();
                }
            });
            red.ForEach(e =>
            {
                e.IsLock = true;
                e.FSM.Skeleton.AnimationState.SetAnimation(0, "idle", true);

                if (e.Status.unitName == "jinshi")
                {
                    e.Status.MinionShield_Idle();
                }
            });
            summon.ForEach(e =>
            {
                e.IsLock = true;
                e.FSM.Skeleton.AnimationState.SetAnimation(0, "idle", true);
            });

            bool isComplete = false;

            // 시나리오 83 재생 아군적군 강제 사망
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(83, () =>
            {
                EffectManager.Singleton.OnPlayParticle(Vector3.zero, "j_skill1", 1f, blue[0].Rend.sortingLayerID, blue[0].Rend.sortingOrder);
                isComplete = true;
            }));

            // 시나리오 완료 대기
            yield return new WaitUntil(() => (isComplete == true));
            isComplete = false;
            yield return new WaitForSeconds(0.8f);

            // 아군적군 강제 사망모션
            blue.ForEach(e =>
            {
                e.FSM.Skeleton.AnimationState.SetAnimation(0, "die", false);
                // 2단 연출이 있는 경우
                if (e.FSM.Skeleton.skeleton.Data.Animations.Find(k => k.Name == "die2") != null)
                    e.FSM.Skeleton.AnimationState.AddAnimation(0, "die2", true, 0);

                if (e.Status.unitName == "jinshi")
                {
                    e.Status.MinionShield_Die();
                }
            });

            summon.ForEach(e =>
            {
                if (e.FSM.Skeleton.skeleton.Data.Animations.Find(k => k.Name == "die") != null)
                    e.FSM.Skeleton.AnimationState.SetAnimation(0, "die", false);
                // 2단 연출이 있는 경우
                if (e.FSM.Skeleton.skeleton.Data.Animations.Find(k => k.Name == "die2") != null)
                    e.FSM.Skeleton.AnimationState.AddAnimation(0, "die2", true, 0);
            });

            yield return new WaitForSeconds(2f);

            // 진시황 먼저 찾기
            var jinshi = blue.Find(e => e.Status.unitName == "jinshi");

            // 시나리오 84 재생 후 진시황 강제 기상
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(84, () =>
            {
                jinshi.FSM.Skeleton.AnimationState.SetAnimation(0, "idle", false);
                isComplete = true;
            }));

            // 시나리오 완료 대기
            yield return new WaitUntil(() => (isComplete == true));
            isComplete = false;

            // animation 완료까지 대기
            yield return new WaitUntil(() => jinshi.FSM.Skeleton.AnimationState.GetCurrent(0).IsComplete == true);

            // 시나리오 85 재생 후 진시황 강제 스킬
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(85, () =>
            {
                jinshi.FSM.Skeleton.AnimationState.SetAnimation(0, "skill3", false);
                isComplete = true;
            }));

            yield return new WaitForSpineEvent(jinshi.FSM.Skeleton, "attack");
            EffectManager.Singleton.OnPlayParticle(new Vector3(9f, -5f, 0f), "jinshi_skill3", jinshi.FSM.Skeleton.skeleton.ScaleX, blue[0].Rend.sortingLayerID, blue[0].Rend.sortingOrder);

            red.ForEach(e =>
            {
                e.FSM.Skeleton.AnimationState.SetAnimation(0, "die", false);
                // 2단 연출이 있는 경우
                if (e.FSM.Skeleton.skeleton.Data.Animations.Find(k => k.Name == "die2") != null)
                    e.FSM.Skeleton.AnimationState.AddAnimation(0, "die2", true, 0);

                if (e.Status.unitName == "jinshi")
                {
                    e.Status.MinionShield_Die();
                }
            });

            yield return new WaitForSeconds(3f);

            // 시나리오 완료 대기
            yield return new WaitUntil(() => (isComplete == true));
            isComplete = false;

            // animation 완료까지 대기
            yield return new WaitUntil(() => jinshi.FSM.Skeleton.AnimationState.GetCurrent(0).IsComplete == true);

            // 시나리오 86 재생 후 화면 페이드아웃&인 후 결과화면
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(86, () =>
            {
                Message.Send<Global.ShowScenarioFadeMsg>(new Global.ShowScenarioFadeMsg(true, () =>
                {
                    red.ForEach(e => e.gameObject.SetActive(false));

                    Message.Send<Global.ShowScenarioFadeMsg>(new Global.ShowScenarioFadeMsg(false, () =>
                    {
                        _msgCount = 0;
                        _blueTeamCount = 5;
                        _redTeamCount = 0;

                        blue.ForEach(e =>
                        {
                            e.IsLock = false;
                            e.Revive(false);
                        });
                        red.ForEach(e => e.isDie = true);

                        _depth = Constant.BattlePhase.StartBattle;
                        // BattleEnd();
                    }));
                }));
                isComplete = true;
            }));

            yield return null;
        }

        private IEnumerator coBattleDefeat()
        {
            yield return new WaitForSeconds(2f);

            Message.Send<Battle.Normal.BattleDefeatMsg>(new Battle.Normal.BattleDefeatMsg());
        }

        private void OnMoveToEndComplete(Battle.Normal.MoveToEndCompleteMsg msg)
        {
            _msgCount++;
            _msgReceived = true;

            // // 모든 살아있는 유닛에게 메세지를 받으면 다음으로 진행.
            // if (_msgCount == (_blueTeamCount + _redTeamCount))
            // {
            //     ShowResultDialog ();
            // }
        }

        private void ShowResultDialog()
        {
            if (TutorialManager.Singleton.curTutorialIndex == 7)
            {
                Scene.IScene.LoadScene<Scene.LobbyScene>();
            }
            else
            {
                if (_blueTeamCount > 0)
                {
                    // var list = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();

                    // for (int i = 0; i < list.Count; i++)
                    // {
                    //     list[i].gameObject.SetActive(false);
                    // }

                    Message.Send<Battle.Normal.BattleVictoryMsg>(new Battle.Normal.BattleVictoryMsg());
                }
                else
                    Message.Send<Battle.Normal.BattleDefeatMsg>(new Battle.Normal.BattleDefeatMsg());
            }
        }

        private IEnumerator coTimeUp()
        {
            yield return new WaitForSeconds(2f);

            Message.Send<Battle.Normal.BattleDrawMsg>(new Battle.Normal.BattleDrawMsg());
        }
        #endregion

        /// <summary>
        /// 모든 Dialog Exit.
        /// </summary>
        private void OnExitAllDialog()
        {
            Dialog.IDialog.RequestDialogExit<Dialog.NormalMainDialog>();
            Dialog.IDialog.RequestDialogExit<Dialog.NormalResultDialog>();
        }

        #region Message Listener
        /// <summary>
        /// 어느 캐릭터가 궁스킬을 사용한 후 오는 메세지
        /// </summary>
        /// <param name="msg"></param>
        private void OnSendUseSkill(Battle.Normal.SendUseSkillMsg msg)
        {
            _enableInstantKill = msg.isEnter == false;

            _bgBlock.gameObject.SetActive(msg.isEnter == true);
        }

        /// <summary>
        /// 어느 캐릭터가 죽은 후 오는 메세지
        /// </summary>
        /// <param name="msg"></param>
        private void OnSendImDying(Battle.Normal.SendImDyingMsg msg)
        {
            if (msg.info.Team == Constant.Team.Blue)
                _blueTeamCount--;
            else
                _redTeamCount--;

            // 무승부다. 결과는 일단 띄워주자.
            if (_blueTeamCount == 0 && _redTeamCount == 0)
            {
                _msgReceived = true;
            }
        }
        #endregion

        // #if UNITY_EDITOR
        // private void Update()
        // {

        //     // 30초가 남으면 J 강제 등장
        //     if (Info.My.Singleton.User.MainstoryChapter == 3 && Info.My.Singleton.User.MainstoryStage == 4 && _summon_J == false && _currentTime >= _maxTime - 30f)
        //     {
        //         _summon_J = true;

        //         Action_Summon_J();

        //         return;
        //     }

        //     _currentTime += Time.deltaTime;
        // }

        // private void OnGUI ()
        // {
        //     GUI.Label (new Rect (0, 0, 200, 100), string.Format ("Phase = {0} / {1}\nblue = {2}\nred = {3}\nmsgCount = {4}", _curPhase, _maxPhase, _blueTeamCount, _redTeamCount, _msgCount));
        // }
        // #endif

        #region 프로토타입용 시나리오재생 (하드코딩입니다!!!)
        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;
            // todo 스테이지 넘버와 같이 조합하여 사용하면 될 것같다.
            if (sidx == 4)
            {
                // Fade Out Immediately
                Message.Send<Global.ShowScenarioBackgroundMsg>(new Global.ShowScenarioBackgroundMsg(true));
                // 4번 Scene 시나리오 재생
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(4, () =>
                {
                    // Callback = 5번 Scene 재생
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(5, () =>
                    {
                        // Callback = Fade In
                        Message.Send<Global.ShowScenarioFadeMsg>(new Global.ShowScenarioFadeMsg(false, () =>
                        {
                            // Callback = 6번 Scene 재생 이후 Game Start
                            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(6, GameStart));
                        }));
                    }));
                }));
            }
            else if (sidx == 13)
            {
                // Fade Out Immediately
                Message.Send<Global.ShowScenarioBackgroundMsg>(new Global.ShowScenarioBackgroundMsg(true));
                // 13번 Scene 재생
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(13, () =>
                {
                    // Callback = Fade In 이후 Game Start
                    Message.Send<Global.ShowScenarioFadeMsg>(new Global.ShowScenarioFadeMsg(false, GameStart));
                }));
            }
            else if (sidx == 38)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(38, GameStart));
            }
            else if (sidx == 40)
            {
                Message.Send<Global.ShowScenarioBackgroundMsg>(new Global.ShowScenarioBackgroundMsg(true));
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(40, () =>
                {
                    Message.Send<Global.ShowScenarioFadeMsg>(new Global.ShowScenarioFadeMsg(false, () =>
                    {
                        Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(41, GameStart));
                    }));
                }));
            }
            else if (sidx == 48)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(48, GameStart));
            }
            else if (sidx == 58)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(58, GameStart));
            }
            else if (sidx == 60)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(60, GameStart));
            }
            else if (sidx == 63)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(63, GameStart));
            }
            else if (sidx == 72 && Info.My.Singleton.User.MainstoryChapter == 3 && Info.My.Singleton.User.MainstoryStage == 1)        // ! 3chapter 1stage
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(72, GameStart));
            }
            else if (sidx == 75 && Info.My.Singleton.User.MainstoryChapter == 3 && Info.My.Singleton.User.MainstoryStage == 3)     // ! 3chapter 3stage
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(75, GameStart));
            }
            else if (sidx == 82)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(82, GameStart));
            }
            else if (sidx == 88 && Info.My.Singleton.User.isCompleteArena == true)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(88, GameStart));
            }
            else
            {
                // 치우 시나리오 체크
                if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 1)
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(1, GameStart, "Chiyou"));
                }
                else if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 2)
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(4, GameStart, "Chiyou"));
                }
                else if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 3)
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(5, GameStart, "Chiyou"));
                }
                else if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 4)
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(6, GameStart, "Chiyou"));
                }
                else
                {
                    GameStart();
                }
            }
        }

        private bool CheckScenario_End(System.Action callback)
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 32)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(32, () =>
                {
                    if (callback != null)
                        callback();
                }));
                return true;
            }
            else if (sidx == 39)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(39, () =>
                {
                    if (callback != null)
                        callback();
                }));
                return true;
            }
            else if (sidx == 49)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(49, () =>
                {
                    if (callback != null)
                        callback();
                }));
            }
            else if (sidx == 64)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(64, () =>
                {
                    if (callback != null)
                        callback();
                }));
            }
            else if (sidx == 74)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(74, () =>
                {
                    if (callback != null)
                        callback();
                }));
            }
            else if (sidx == 76 && Info.My.Singleton.User.MainstoryChapter == 3 && Info.My.Singleton.User.MainstoryStage == 4)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(76, () =>
                {
                    if (callback != null)
                        callback();
                }));
            }
            else if (sidx == 90)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(90, () =>
                {
                    if (callback != null)
                        callback();
                }));
            }
            else if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 1)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(3, () =>
                {
                    if (callback != null)
                        callback();
                }, "Chiyou"));
            }
            else if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 4)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(7, () =>
                {
                    if (callback != null)
                        callback();
                }, "Chiyou"));
            }
            else
            {
                if (callback != null)
                    callback();
            }

            return false;
        }

        private void OnLockInstantKill(Battle.Normal.LockInstantKillMsg msg)
        {
            _enableInstantKill = false;
        }

        private void OnUnlockInstantKill(Battle.Normal.UnlockInstantKillMsg msg)
        {
            _enableInstantKill = true;
        }

        private void OnScenario_Pandora(Battle.Normal.Scenario_PandoraMsg msg)
        {
            StartCoroutine(coScenario_Pandora());
        }

        private IEnumerator coScenario_Pandora()
        {
            yield return StartCoroutine(CameraManager.Singleton.coFadeLoading_Battle(true));

            yield return new WaitForSeconds(0.5f);

            // 결과화면 Ui 빼주고
            Dialog.IDialog.RequestDialogExit<Dialog.NormalResultDialog>();

            // 여기서 판도라 강제 추가
            Info.My.Singleton.Inventory.AddCharacter(103031, 1, 3);

            // 나머지 캐릭터 비활성화
            var blueList = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();
            blueList.ForEach(e => e.gameObject.SetActive(false));

            var redList = BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>();
            redList.ForEach(e => e.gameObject.SetActive(false));

            var pandora = Info.My.Singleton.Inventory.characterList.Find(e => e.code == 103031);
            if (pandora != null)
            {
                var info = SummonAlly(pandora.GetStatus(), Constant.Team.Blue, 5);
                BattleManager.Singleton.AddUnit(Constant.Team.Blue, info);

                var trans = info.transform;
                trans.localPosition = new Vector3(0f, -4f, 0f);

                var skel = trans.GetComponent<SkeletonAnimation>();
                if (skel != null)
                {
                    skel.AnimationState.SetAnimation(0, "die", false);
                    skel.AnimationState.GetCurrent(0).TrackTime = skel.AnimationState.GetCurrent(0).TrackEnd * 0.5f;
                }
            }

            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(CameraManager.Singleton.coFadeLoading_Battle(false));

            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(42, () =>
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(43, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;

                    StageModel sm = Model.First<StageModel>();
                    StageModel.Data _stageData = sm.Table.Find(e => e.type == BattleManager.Singleton.battleType && e.chapter == BattleManager.Singleton.selectChapter && e.stage == BattleManager.Singleton.selectStage + 1);
                    if (_stageData == null)
                    {
                        BattleManager.Singleton.selectChapter++;
                        BattleManager.Singleton.selectStage = 1;
                    }
                    else
                    {
                        BattleManager.Singleton.selectStage++;
                    }

                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }));
        }
        #endregion

        private void DebugMode_InstantKill()
        {
            BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>().ForEach(e =>
                {
                    Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(e, 99999, true));
                });
        }

        // // #if UNITY_EDITOR
        // private void OnGUI()
        // {
        //     if (_enableInstantKill == false)
        //         return;

        //     if (_depth != Constant.BattlePhase.StartBattle)
        //         return;

        //     if (TutorialManager.Singleton.IsTutorialProcess == true)
        //         return;

        //     if (_curPhase == 3 && TutorialManager.Singleton.curTutorialIndex == 83)
        //     {
        //         if (GUI.Button(new Rect(Screen.width / 2 - 50, 100, 100, 100), "시나리오"))
        //         {
        //             _curTime = _maxTime - 30f;
        //         }
        //     }
        //     else if (GUI.Button(new Rect(Screen.width / 2 - 50, 100, 100, 100), "몹 제거"))
        //     {
        //         DebugMode_InstantKill();
        //     }
        // }
        // // #endif
    }
}