// ==================================================
// LobbyFormationDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Dialog
{
    public class LobbyFormationDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Text _titleText;

        [SerializeField] private List<FormationSlot> _formationList;

        [Header("- Right Cardslot List")]
        [SerializeField] private Button _resizeGridButton;
        [SerializeField] private GameObject[] _resizeGridIconObj;
        [SerializeField] private Button _sortTypeButton;
        [SerializeField] private Text _sortTypeText;
        [SerializeField] private Button _sortDirectionButton;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private List<UnitCardSlot> _listCardSlots;
        private int _gridSizeIdx = 0;
        private int _sortTypeIndex = 0;
        private int _sortDirectionIndex = 0;
        private uint _selectUnitIdx = 0;
        private uint[,] _contentTeam;
        private Info.Inventory _inventory;
        [SerializeField] private Button _confirmFormation;

        protected override void OnLoad()
        {
            base.OnLoad();

            _inventory = Info.My.Singleton.Inventory;

            Init_SortType();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _resizeGridButton.onClick.AddListener(OnClickResizeGrid);
            _sortTypeButton.onClick.AddListener(OnClickChangeSortType);
            _sortDirectionButton.onClick.AddListener(OnClickChangeSortDirection);

            _confirmFormation.onClick.AddListener(OnClickConform);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _resizeGridButton.onClick.RemoveAllListeners();
            _resizeGridButton = null;
            _resizeGridIconObj = null;
            _sortTypeButton.onClick.RemoveAllListeners();
            _sortTypeButton = null;
            _sortTypeText = null;
            _sortDirectionButton.onClick.RemoveAllListeners();
            _sortDirectionButton = null;
            _gridLayoutGroup = null;
            _inventory = null;
            _confirmFormation.onClick.RemoveAllListeners();
            _confirmFormation = null;

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

            switch (BattleManager.Singleton.battleType)
            {
                case 1:
                    _titleText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(8001), LocalizeManager.Singleton.GetString(16001));
                    _contentTeam = Info.My.Singleton.Inventory.storyTeam;
                    break;

                case 2:
                    _titleText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(10001), LocalizeManager.Singleton.GetString(16001));
                    _contentTeam = Info.My.Singleton.Inventory.subTeam;
                    break;

                case 3:
                    _titleText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(9001), LocalizeManager.Singleton.GetString(16001));
                    _contentTeam = Info.My.Singleton.Inventory.dimensionTeam;
                    break;

                case 4:
                    _titleText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(13001), LocalizeManager.Singleton.GetString(16001));
                    _contentTeam = Info.My.Singleton.Inventory.storyTeam;
                    break;

                case 5:
                    _titleText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(11001), LocalizeManager.Singleton.GetString(16001));
                    _contentTeam = Info.My.Singleton.Inventory.babelTeam;
                    break;

                case 6:
                    _titleText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(12001), LocalizeManager.Singleton.GetString(16001));
                    _contentTeam = Info.My.Singleton.Inventory.arenaTeam;
                    break;
            }

            CheckTutorial();

            if (BattleManager.Singleton.battleType != 6)
            {
                Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
                                            {
                                                RequestDialogExit<LobbyFormationDialog>();
                                                RequestDialogEnter<LobbyStageInfoDialog>();
                                            }));
            }
            else
            {
                Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
                            {
                                RequestDialogExit<LobbyFormationDialog>();
                                RequestDialogEnter<LobbyArenaDialog>();
                            }));
            }



            _selectUnitIdx = 0;
            RefreshFormationPanel();
            RefreshListPanel();
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void RefreshFormationPanel()
        {
            for (int i = 0; i < _formationList.Count; i++)
            {
                _formationList[i].Init(i, _inventory.GetCharacterByIndex(_contentTeam[i / 3, i % 3]));
                _formationList[i].AddCallback(SelectFormationSlot);
            }
        }

        private void RefreshListPanel()
        {
            // 소지한 유닛 리스트 로드
            List<UnitStatus> unitList = SortList();

            // List - 유닛카드가 부족하다면 새로 생성
            if (_listCardSlots == null)
                _listCardSlots = new List<UnitCardSlot>();

            for (int i = 0; i < unitList.Count; i++)
            {
                if (_gridLayoutGroup.transform.childCount < i + 1)
                {
                    _listCardSlots.Add(CreateCardSlot(_gridLayoutGroup.transform));
                }
            }

            // 카드 갱신
            for (int i = 0; i < _listCardSlots.Count; i++)
            {
                _listCardSlots[i].Init(unitList[i]);
                _listCardSlots[i].AddCallback(SelectCardSlot);

                if (_listCardSlots[i].idx == _selectUnitIdx)
                {
                    _listCardSlots[i].OnSelect();
                }
                else
                {
                    _listCardSlots[i].OnDeselect();
                }
            }
        }

        private List<UnitStatus> SortList()
        {
            var characterList = Info.My.Singleton.Inventory.characterList;

            if (_sortTypeIndex == 0)
                return SortByGrade();
            else if (_sortTypeIndex == 1)
                return SortByLevel();
            else if (_sortTypeIndex == 2)
                return SortByObtain();
            else
                return null;
        }

        private List<UnitStatus> SortByLevel()
        {
            List<UnitStatus> result = new List<UnitStatus>();
            for (int i = 0; i < _inventory.characterList.Count; i++)
            {
                result.Add(_inventory.characterList[i].GetStatus());
            }

            if (_sortDirectionIndex == 1)
            {
                result = result
                .OrderBy(e => e.level)
                .ThenBy(e => e.grade)
                .ThenBy(e => e.idx).ToList();
            }
            else
            {
                result = result
                .OrderByDescending(e => e.level)
                .ThenByDescending(e => e.grade)
                .ThenByDescending(e => e.idx).ToList();
            }

            return result;
        }

        private List<UnitStatus> SortByGrade()
        {
            List<UnitStatus> result = new List<UnitStatus>();
            for (int i = 0; i < _inventory.characterList.Count; i++)
            {
                result.Add(_inventory.characterList[i].GetStatus());
            }

            if (_sortDirectionIndex == 1)
            {
                result = result
                .OrderBy(e => e.grade)
                .ThenBy(e => e.level)
                .ThenBy(e => e.idx).ToList();
            }
            else
            {
                result = result
                .OrderByDescending(e => e.grade)
                .ThenByDescending(e => e.level)
                .ThenByDescending(e => e.idx).ToList();
            }

            return result;
        }

        private List<UnitStatus> SortByObtain()
        {
            List<UnitStatus> result = new List<UnitStatus>();
            for (int i = 0; i < _inventory.characterList.Count; i++)
            {
                result.Add(_inventory.characterList[i].GetStatus());
            }

            if (_sortDirectionIndex == 1)
            {
                result = result
                .OrderBy(e => e.idx).ToList();
            }
            else
            {
                result = result
                .OrderByDescending(e => e.idx).ToList();
            }

            return result;
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

        private void OnClickResizeGrid()
        {
            _gridSizeIdx = _gridSizeIdx == 0 ? 1 : 0;

            _resizeGridIconObj[0].SetActive(_gridSizeIdx == 0);
            _resizeGridIconObj[1].SetActive(_gridSizeIdx == 1);

            if (_gridSizeIdx == 0)
            {
                _gridLayoutGroup.cellSize = new Vector2(256, 256);
                _gridLayoutGroup.spacing = new Vector2(15, 20);
                _gridLayoutGroup.constraintCount = 5;
            }
            else
            {
                _gridLayoutGroup.cellSize *= 1.25f;
                _gridLayoutGroup.spacing *= 1.25f;
                _gridLayoutGroup.constraintCount = 4;
            }
        }

        private void Init_SortType()
        {
            _sortTypeIndex = 0;
            _sortTypeText.text = LocalizeManager.Singleton.GetString(16005 + _sortTypeIndex);
        }

        private void OnClickChangeSortType()
        {
            _sortTypeIndex++;
            if (_sortTypeIndex > 2)
                _sortTypeIndex = 0;

            // 버튼 텍스트 변경
            _sortTypeText.text = LocalizeManager.Singleton.GetString(16005 + _sortTypeIndex);

            RefreshListPanel();
        }

        private void OnClickChangeSortDirection()
        {
            _sortDirectionIndex = (_sortDirectionIndex == 0) ? 1 : 0;

            RefreshListPanel();
        }

        private void OnClickConform()
        {
            switch (TutorialManager.Singleton.openFirst)
            {
                case Constant.TutorialCallbackType.MainChapter:
                    _inventory.storyTeam = _contentTeam;
                    break;

                case Constant.TutorialCallbackType.Dimension:
                    _inventory.storyTeam = _contentTeam;
                    break;

                case Constant.TutorialCallbackType.Boss:
                    _inventory.storyTeam = _contentTeam;
                    break;

                case Constant.TutorialCallbackType.Babel:
                    _inventory.storyTeam = _contentTeam;
                    break;

                case Constant.TutorialCallbackType.Story:
                    _inventory.storyTeam = _contentTeam;
                    break;
            }

            OnClickBack();
        }

        private void InitCardSlot()
        {
            // 소지한 유닛 리스트 로드
            List<UnitStatus> unitList = SortList();

            // List - 유닛카드가 부족하다면 새로 생성
            if (_listCardSlots == null)
                _listCardSlots = new List<UnitCardSlot>();
            for (int i = 0; i < unitList.Count; i++)
            {
                if (_gridLayoutGroup.transform.childCount < i + 1)
                {
                    _listCardSlots.Add(CreateCardSlot(_gridLayoutGroup.transform));
                }
            }

            // 카드 갱신
            for (int i = 0; i < _listCardSlots.Count; i++)
            {
                _listCardSlots[i].Init(unitList[i]);
                _listCardSlots[i].AddCallback(SelectCardSlot);
            }
        }

        private UnitCardSlot CreateCardSlot(Transform parent)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/Lobby/FormationCardSlot");
            if (prefab == null)
            {
                Logger.LogErrorFormat("캐릭터관리UI에 사용하는 프리팹이 없거나 이름을 잘못 적었습니다.");
                return null;
            }

            GameObject obj = GameObject.Instantiate(prefab);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            return obj.GetComponent<UnitCardSlot>();
        }

        private void SelectFormationSlot(uint idx, FormationSlot slot)
        {
            // 선택 카드가 없는 경우 1차로 거름.
            // 만약 캐릭터카드가 꼽혀있는 위치라면 카드제거.
            if (_selectUnitIdx == 0)
            {
                if (idx > 0)
                {
                    _contentTeam[slot.slotNumber / 3, slot.slotNumber % 3] = 0;
                }
            }
            // 여기서부터는 선택한 캐릭터카드가 있는 상태.
            else
            {
                // 캐릭터정보 가져오기
                Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == _selectUnitIdx);
                if (character == null)
                    return;

                // 캐릭터 선택 상태에서 빈자리 선택
                if (idx == 0)
                {
                    FormationSlot prev = _formationList.Find(e => e.idx == _selectUnitIdx);

                    // 다른자리에 같은 캐릭터가 배치되었는지 확인
                    if (prev != null)
                    {
                        _contentTeam[prev.slotNumber / 3, prev.slotNumber % 3] = 0;
                        _contentTeam[slot.slotNumber / 3, slot.slotNumber % 3] = _selectUnitIdx;
                    }
                    // 다른자리에 캐릭터가 없다. 그자리에 배치하자.
                    else
                    {
                        // 일단 5명 전원 배치했는지 체크
                        if (IsContentTeamFull() == true)
                        {
                            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13),
                            LocalizeManager.Singleton.GetString(16012), null, true));
                            return;
                        }

                        _contentTeam[slot.slotNumber / 3, slot.slotNumber % 3] = _selectUnitIdx;
                    }
                }

                // 캐릭터 선택 상태에서 같은자리 선택
                else if (_selectUnitIdx == idx)
                {
                    _contentTeam[slot.slotNumber / 3, slot.slotNumber % 3] = 0;
                }

                // 캐릭터 선택 상태에서 다른캐릭터자리 선택
                else if (_selectUnitIdx != idx)
                {
                    // 일단 5명 전원 배치했는지 체크
                    if (IsContentTeamFull() == true)
                    {
                        Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13),
                        LocalizeManager.Singleton.GetString(16012), null, true));
                        return;
                    }

                    // 이미 다른자리에 배치하려는 캐릭터가 있는지?
                    FormationSlot prev = _formationList.Find(e => e.idx == _selectUnitIdx);
                    if (prev != null)
                    {
                        _contentTeam[prev.slotNumber / 3, prev.slotNumber % 3] = 0;
                        _contentTeam[slot.slotNumber / 3, slot.slotNumber % 3] = _selectUnitIdx;
                    }
                    else
                    {
                        _contentTeam[slot.slotNumber / 3, slot.slotNumber % 3] = _selectUnitIdx;
                    }
                }
            }

            _selectUnitIdx = 0;
            RefreshFormationPanel();
            RefreshListPanel();
        }

        private void SelectCardSlot(uint idx)
        {
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == idx);
            if (character != null)
            {
                UnitStatus status = character.GetStatus();

                _selectUnitIdx = idx;

                RefreshFormationPanel();
                RefreshListPanel();
            }
        }

        private bool IsContentTeamFull()
        {
            int cnt = 0;

            for (int i = 0; i < _contentTeam.GetLength(0); i++)
            {
                for (int j = 0; j < _contentTeam.GetLength(1); j++)
                {
                    if (_contentTeam[i, j] > 0)
                        cnt++;
                }
            }

            return cnt == 5;
        }

        private void CheckTutorial()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;
            if (sidx == 11)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(11, () => StartCoroutine(TutorialFormationComplete())));
            }
            else if (sidx == 31)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(31, () => StartCoroutine(TutorialJinshiComplete())));
            }
            else if (sidx == 38)
                PlayScenario_38();
            else if (sidx == 40)
                PlayScenario_40();
            else if (sidx == 45)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(45, () => StartCoroutine(TutorialPandoraComplete())));
            }
            else if (sidx == 52 || sidx == 58 || sidx == 60 || sidx == 63 || sidx == 70 || sidx == 72 || sidx == 75 || sidx == 82 || sidx == 88 || sidx == 55)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_confirmFormation.transform, () =>
                {
                    OnClickConform();

                    // 팀설정 동기화
                    Info.My.Singleton.Inventory.dimensionTeam = Info.My.Singleton.Inventory.storyTeam;
                    Info.My.Singleton.Inventory.subTeam = Info.My.Singleton.Inventory.storyTeam;
                    Info.My.Singleton.Inventory.babelTeam = Info.My.Singleton.Inventory.storyTeam;
                    Info.My.Singleton.Inventory.arenaTeam = Info.My.Singleton.Inventory.storyTeam;
                }));
            }
        }

        private IEnumerator TutorialFormationComplete()
        {
            // 슬롯 비활성화하기 전 Grid Layout Off
            _gridLayoutGroup.enabled = false;

            // 치우 배치
            bool isComplete = false;
            var slot = _listCardSlots.Find(e => e.code == 101031);
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
            {
                SelectCardSlot(slot.idx);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // 배치
            isComplete = false;
            var formation = _formationList[2];
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(formation.FormationButton.transform, () =>
            {
                SelectFormationSlot(formation.idx, formation);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // 릴리스 배치
            isComplete = false;
            slot = _listCardSlots.Find(e => e.code == 100211);
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
            {
                SelectCardSlot(slot.idx);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // 배치
            isComplete = false;
            formation = _formationList[4];
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(formation.FormationButton.transform, () =>
            {
                SelectFormationSlot(formation.idx, formation);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // 알렉산더 배치
            isComplete = false;
            slot = _listCardSlots.Find(e => e.code == 102831);
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
            {
                SelectCardSlot(slot.idx);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // 배치
            isComplete = false;
            formation = _formationList[0];
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(formation.FormationButton.transform, () =>
            {
                SelectFormationSlot(formation.idx, formation);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // Grid Layout 다시 활성화
            _gridLayoutGroup.enabled = true;

            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(12, () =>
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_confirmFormation.transform, () =>
                {
                    OnClickConform();

                    // 팀설정 동기화
                    Info.My.Singleton.Inventory.dimensionTeam = Info.My.Singleton.Inventory.storyTeam;
                    Info.My.Singleton.Inventory.subTeam = Info.My.Singleton.Inventory.storyTeam;
                    Info.My.Singleton.Inventory.babelTeam = Info.My.Singleton.Inventory.storyTeam;
                    Info.My.Singleton.Inventory.arenaTeam = Info.My.Singleton.Inventory.storyTeam;
                }));
            }));

            yield return null;
        }

        private IEnumerator TutorialJinshiComplete()
        {
            // 슬롯 비활성화하기 전 Grid Layout Off
            _gridLayoutGroup.enabled = false;

            // 진시황 배치
            bool isComplete = false;
            var slot = _listCardSlots.Find(e => e.code == 101131);
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
            {
                SelectCardSlot(slot.idx);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // 배치
            isComplete = false;
            var formation = _formationList[6];
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(formation.FormationButton.transform, () =>
            {
                SelectFormationSlot(formation.idx, formation);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_confirmFormation.transform, () =>
            {
                OnClickConform();

                // 팀설정 동기화
                Info.My.Singleton.Inventory.dimensionTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.subTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.babelTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.arenaTeam = Info.My.Singleton.Inventory.storyTeam;
            }));
        }

        private void PlayScenario_38()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_confirmFormation.transform, () =>
            {
                OnClickConform();

                // 팀설정 동기화
                Info.My.Singleton.Inventory.dimensionTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.subTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.babelTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.arenaTeam = Info.My.Singleton.Inventory.storyTeam;
            }));
        }

        private void PlayScenario_40()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_confirmFormation.transform, () =>
            {
                OnClickConform();

                // 팀설정 동기화
                Info.My.Singleton.Inventory.dimensionTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.subTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.babelTeam = Info.My.Singleton.Inventory.storyTeam;
                Info.My.Singleton.Inventory.arenaTeam = Info.My.Singleton.Inventory.storyTeam;
            }));

        }

        private IEnumerator TutorialPandoraComplete()
        {
            // 슬롯 비활성화하기 전 Grid Layout Off
            _gridLayoutGroup.enabled = false;

            // 판도라 배치
            bool isComplete = false;
            var slot = _listCardSlots.Find(e => e.code == 103031);
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
            {
                SelectCardSlot(slot.idx);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            // 배치
            isComplete = false;
            var formation = _formationList[8];
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(formation.FormationButton.transform, () =>
            {
                SelectFormationSlot(formation.idx, formation);
                isComplete = true;
            }));
            yield return new WaitUntil(() => (isComplete == true));

            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(46, () =>
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_confirmFormation.transform, () =>
                {
                    OnClickConform();
                }));
            }));
        }
    }
}