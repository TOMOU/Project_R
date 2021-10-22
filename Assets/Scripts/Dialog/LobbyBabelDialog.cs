// ==================================================
// LobbyBabelDialog.cs
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

namespace Dialog
{
    public class LobbyBabelDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        [Header("- Main"), Tooltip("바벨탑의 기본 화면"), Space(20)]
        [SerializeField] private GameObject _objFloor;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private ContentSizeFitter _fitter;
        [SerializeField] private List<BabelFloorSlot> _stageList;

        [Header("- Main Button")]
        [SerializeField] private Button _guideButton_Main;
        [SerializeField] private Button _enterLaboButton_Main;
        [SerializeField] private Button _upgradeLaboButton_Main;
        [SerializeField] private Button _collectButton_Main;

        [Header("- Dispatch"), Tooltip("캐릭터 파견"), Space(20)]
        [SerializeField] private GameObject _objDispatch;
        [SerializeField] private Image _selectUnitPortrait;
        [SerializeField] private Button _resizeGridButton;
        [SerializeField] private GameObject[] _resizeGridIconObj;
        [SerializeField] private Button _sortTypeButton;
        [SerializeField] private Text _sortTypeText;
        [SerializeField] private Button _sortDirectionButton;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private List<UnitCardSlot> _listCardSlots;
        [SerializeField] private Button _dispatchConfirmBtn;

        [Header("- Development"), Tooltip("DNA 연구소"), Space(20)]
        [SerializeField] private GameObject _objDevelopment;
        [SerializeField] private Text _developmentLevelText;
        [SerializeField] private Image _developmentLevelSlider;
        [SerializeField] private List<BabelDevelopmentSlot> _developmentSlotList;

        [Header("- Development Button")]
        [SerializeField] private Button _guideButton_Dev;
        [SerializeField] private Button _enterLaboButton_Dev;
        [SerializeField] private Button _upgradeLaboButton_Dev;
        [SerializeField] private Button _collectButton_Dev;

        [Header("- Production Popup"), Tooltip("DNA 재료 투입"), Space(20)]
        [SerializeField] private GameObject _objProduction;
        [SerializeField] private List<BabelFlaskSlot> _productionSlotList;
        [SerializeField] private Button _productionConfirmBtn;


        // Main Parameter
        private int _selectFloor;
        private int _maxFloor;

        //Dispacth Parameter
        private int _gridSizeIdx = 0;
        private int _sortTypeIndex = 0;
        private int _sortDirectionIndex = 0;
        private uint _selectUnitIdx = 0;
        private int _selectUnitCode = 0;
        private Info.Inventory _inventory;

        // Dev Parameter
        private int _devLevel;

        // Product Parameter
        private int _devSlotIdx;


        protected override void OnLoad()
        {
            base.OnLoad();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            for (int i = 0; i < _developmentSlotList.Count; i++)
            {
                _developmentSlotList[i].AddOpenProduction(OnClickOpenProduction);
            }

            var sm = Model.First<StageModel>();
            if (sm == null)
            {
                Logger.LogError("StageModel 로드 실패");
                return;
            }

            List<StageModel.Data> data = sm.Table.FindAll(e => e.type == 5);
            if (data == null)
            {
                Logger.LogError("StageData 로드 실패");
                return;
            }

            _maxFloor = data.Count;

            // _guideButton_Main.onClick.AddListener();
            _enterLaboButton_Main.onClick.AddListener(OnClickOpenLabo);
            _upgradeLaboButton_Main.onClick.AddListener(OnClickUndevelopmentContent);
            _collectButton_Main.onClick.AddListener(OnClickCollectMaterial);

            _dispatchConfirmBtn.onClick.AddListener(OnClickConfirmDispatch);

            _guideButton_Dev.onClick.AddListener(OnClickOpenMain);
            // _enterLaboButton_Dev.onClick.AddListener();
            _upgradeLaboButton_Dev.onClick.AddListener(OnClickUndevelopmentContent);
            _collectButton_Dev.onClick.AddListener(OnClickCollectDNA);

            _productionConfirmBtn.onClick.AddListener(OnClickProductionConfirm);
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

            _guideButton_Main.onClick.RemoveAllListeners();
            _enterLaboButton_Main.onClick.RemoveAllListeners();
            _upgradeLaboButton_Main.onClick.RemoveAllListeners();
            _collectButton_Main.onClick.RemoveAllListeners();

            _guideButton_Dev.onClick.RemoveAllListeners();
            _enterLaboButton_Dev.onClick.RemoveAllListeners();
            _upgradeLaboButton_Dev.onClick.RemoveAllListeners();
            _collectButton_Dev.onClick.RemoveAllListeners();

            _productionConfirmBtn.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyBabelDialog>();
                RequestDialogEnter<LobbyContentDialog>();
            }));

            if (BattleManager.Singleton.battleType == 5)
            {
                _selectFloor = BattleManager.Singleton.selectStage;
                // _scrollRect.verticalNormalizedPosition = _selectFloor / 10f;
            }
            else
            {
                _selectFloor = Info.My.Singleton.User.maxClearedBabel + 1;
                // _scrollRect.verticalNormalizedPosition = _selectFloor / 10f;
            }

            _objFloor.SetActive(true);
            _objDispatch.SetActive(false);
            _objDevelopment.SetActive(false);
            _objProduction.SetActive(false);

            _selectUnitIdx = 0;
            _selectUnitCode = 0;
            _inventory = Info.My.Singleton.Inventory;

            RefreshMain();
            RefreshDispatch();
            RefreshDevelopment();

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
                case Constant.TutorialCallbackType.Babel:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    if (BattleManager.Singleton.inLobby == true)
                    {
                        SelectBabelSlot(_selectFloor);
                    }
                    break;

                default:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    break;
            }

            CheckScenario();
        }

        private void Update()
        {
            if (_dialogView.activeSelf == false)
                return;

            if (_objDevelopment.activeSelf == true)
            {
                for (int i = 0; i < _developmentSlotList.Count; i++)
                {
                    _developmentSlotList[i].Refresh();
                }
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

        private void OnClickUndevelopmentContent()
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(12), null, true));
        }

        private void OnClickOpenMain()
        {
            _objFloor.SetActive(true);
            _objDispatch.SetActive(false);
            _objDevelopment.SetActive(false);
            _objProduction.SetActive(false);

            RefreshMain();
        }

        private void OnClickOpenDispatch()
        {
            _objDispatch.SetActive(true);

            _selectUnitIdx = 0;
            _selectUnitCode = 0;

            RefreshDispatch();
            RefreshListPanel();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                _objDispatch.SetActive(false);

                RefreshMain();
            }));
        }

        private void OnClickOpenLabo()
        {
            _objFloor.SetActive(false);
            _objDispatch.SetActive(false);
            _objDevelopment.SetActive(true);
            _objProduction.SetActive(false);

            RefreshDevelopment();
        }

        private void OnClickOpenProduction(int idx)
        {
            _devSlotIdx = idx;

            _objProduction.SetActive(true);
            RefreshProduction();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RefreshDevelopment();
                _objProduction.SetActive(false);
            }));
        }

        private void OnClickProductionConfirm()
        {
            for (int i = 0; i < _productionSlotList.Count; i++)
            {
                // i
                // _productionSlotList[i].MaterialCount;
            }

            //todo 유저 재료 깎기

            // UI 닫으면서 생산 시작
            OnClickBack();
            _developmentSlotList[_devSlotIdx].ConfirmCraft();
        }

        private void OnClickCollectMaterial()
        {
            var list = Info.My.Singleton.User.babelDispatchSlotList;
            for (int i = 0; i < _stageList.Count; i++)
            {
                if (list[i].character > 0 && list[i].state > 1)
                {
                    // list[i].character = 0;
                    // list[i].state = 0;
                    SelectDispatchSlot(i + 1);
                }
            }
        }

        private void OnClickCollectDNA()
        {
            var list = Info.My.Singleton.User.babelDevSlotList;
            for (int i = 0; i < _developmentSlotList.Count; i++)
            {
                if (list[i].state == 3)
                    _developmentSlotList[i].OnClick();
            }
        }

        private void OnClickConfirmDispatch()
        {
            if (_selectUnitIdx == 0)
                return;

            // _selectFloor-1에 캐릭터 배치
            Info.User user = Info.My.Singleton.User;
            var info = user.babelDispatchSlotList.Find(e => e.idx == _selectFloor - 1);
            if (info == null)
            {
                user.babelDispatchSlotList.Add(new Info.User.BabelDispatchState(_selectFloor - 1, 1, _selectUnitIdx, System.DateTime.Now.AddHours(UnityEngine.Random.Range(1, 5)).AddMinutes(UnityEngine.Random.Range(0, 6) * 10)));
            }
            else
            {
                info.character = _selectUnitIdx;
                info.state = 1;
                info.endTime = System.DateTime.Now.AddHours(UnityEngine.Random.Range(1, 5)).AddMinutes(UnityEngine.Random.Range(0, 6) * 10);
            }

            OnClickBack();
        }

        private void RefreshMain()
        {
            for (int i = 0; i < _stageList.Count; i++)
            {
                if (i < _maxFloor)
                    _stageList[i].Init(i + 1, SelectBabelSlot, SelectDispatchSlot);
                else
                    _stageList[i].Init();
            }
        }

        private void RefreshDispatch()
        {
            if (_selectUnitIdx == 0)
                _selectUnitPortrait.gameObject.SetActive(false);
            else
            {
                _selectUnitPortrait.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/1_portrait_full/{0}", _selectUnitCode));
                _selectUnitPortrait.gameObject.SetActive(true);
            }
        }

        private void RefreshDevelopment()
        {
            _devLevel = Info.My.Singleton.User.babelDevelopmentLevel;

            _developmentLevelText.text = string.Format(LocalizeManager.Singleton.GetString(11024), _devLevel);
            _developmentLevelSlider.fillAmount = _devLevel / 10f;

            var list = Info.My.Singleton.User.babelDevSlotList;

            for (int i = 0; i < _developmentSlotList.Count; i++)
            {
                _developmentSlotList[i].Init(list[i]);
            }
        }

        private void RefreshProduction()
        {
            for (int i = 0; i < _productionSlotList.Count; i++)
            {
                _productionSlotList[i].Init(i);
            }
        }

        private void SelectBabelSlot(int floor)
        {
            _selectFloor = floor;

            BattleManager.Singleton.battleType = 5;
            BattleManager.Singleton.selectChapter = 0;
            BattleManager.Singleton.selectStage = _selectFloor;

            BattleManager.Singleton.inLobby = false;

            Dialog.IDialog.RequestDialogEnter<Dialog.LobbyStageInfoDialog>();
        }

        private void SelectDispatchSlot(int floor)
        {
            _selectFloor = floor;

            var info = Info.My.Singleton.User.babelDispatchSlotList[floor - 1];

            // 캐릭터가 배치되어 있다.
            if (info.character > 0)
            {
                // 아직 시간이 남아있다.
                if ((info.endTime - System.DateTime.Now).TotalMilliseconds > 0)
                {
                    info.endTime = System.DateTime.Now;
                }
                // 시간이 종료되었다.
                else
                {
                    //todo 보상을 받는다.

                    // info.character = _selectUnitIdx;
                    info.state = 1;
                    info.endTime = System.DateTime.Now.AddHours(UnityEngine.Random.Range(1, 5)).AddMinutes(UnityEngine.Random.Range(0, 6) * 10);

                    // 해당슬롯 갱신
                    _stageList[_selectFloor - 1].Refresh();

                    //! 보상 DNA아이템 생성
                    List<int> reward_id = new List<int>();
                    List<int> reward_value = new List<int>();

                    reward_id.Add(10001);
                    reward_id.Add(10002);
                    reward_id.Add(10003);
                    reward_id.Add(10004);
                    reward_value.Add(300);
                    reward_value.Add(150);
                    reward_value.Add(200);
                    reward_value.Add(100);

                    // 보상UI 출력 후 바로 장착
                    Dialog.IDialog.RequestDialogEnter<GlobalRewardDialog>();
                    Message.Send<Global.ShowRewardMsg>(new Global.ShowRewardMsg(reward_id, reward_value, () =>
                    {

                    }));
                }
            }
            // 캐릭터가 비어있다.
            else
            {
                OnClickOpenDispatch();
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
                if (i < unitList.Count)
                {
                    _listCardSlots[i].gameObject.SetActive(true);
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
                else
                {
                    _listCardSlots[i].gameObject.SetActive(false);
                }
            }
        }

        private void SelectCardSlot(uint idx)
        {
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == idx);
            if (character != null)
            {
                UnitStatus status = character.GetStatus();

                _selectUnitIdx = idx;
                _selectUnitCode = character.code;

                RefreshListPanel();
                RefreshDispatch();
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
                if (Info.My.Singleton.User.IsExistInDispatch(_inventory.characterList[i].idx) == true)
                    continue;

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
                if (Info.My.Singleton.User.IsExistInDispatch(_inventory.characterList[i].idx) == true)
                    continue;

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
                if (Info.My.Singleton.User.IsExistInDispatch(_inventory.characterList[i].idx) == true)
                    continue;

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

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 69)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(69, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageList[0].transform, _gridLayout, () =>
                     {
                         SelectBabelSlot(1);
                     }));
                }));
            }
            else if (sidx == 70)
            {
                if (Info.My.Singleton.User.maxClearedBabel == 1)
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageList[1].transform, _gridLayout, () =>
                     {
                         SelectBabelSlot(2);
                     }));
                }
                else if (Info.My.Singleton.User.maxClearedBabel == 2)
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageList[2].transform, _gridLayout, () =>
                     {
                         SelectBabelSlot(3);
                     }));
                }
                else if (Info.My.Singleton.User.maxClearedBabel == 3)
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(70, () =>
                    {
                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_stageList[0].transform, _gridLayout, () =>
                         {
                             // 1층 선택
                             SelectDispatchSlot(1);

                             // CardList에서 캐릭터 찾기 (치우)
                             var chiyou = _listCardSlots.Find(e => e.code == 101031);

                             Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(chiyou.transform, _gridLayoutGroup, () =>
                              {
                                  SelectCardSlot(chiyou.idx);

                                  Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_dispatchConfirmBtn.transform, () =>
                                  {
                                      OnClickConfirmDispatch();

                                      Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(71, () =>
                                      {
                                          Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
                                      }));
                                  }));
                              }));
                         }));
                    }));
                }
            }
            else if (sidx == 78)
            {
                Info.My.Singleton.User.babelDispatchSlotList[0].endTime = System.DateTime.Now;

                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(78, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_collectButton_Main.transform, () =>
                    {
                        // 재료회수
                        OnClickCollectMaterial();

                        //! 보상 DNA아이템 생성
                        List<int> reward_id = new List<int>();
                        List<int> reward_value = new List<int>();

                        reward_id.Add(10001);
                        reward_id.Add(10002);
                        reward_id.Add(10003);
                        reward_id.Add(10004);
                        reward_value.Add(300);
                        reward_value.Add(150);
                        reward_value.Add(200);
                        reward_value.Add(100);

                        // 보상UI 출력 후 바로 장착
                        Dialog.IDialog.RequestDialogEnter<GlobalRewardDialog>();
                        Message.Send<Global.ShowRewardMsg>(new Global.ShowRewardMsg(reward_id, reward_value, () =>
                        {
                            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(79, () =>
                                                    {
                                                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_enterLaboButton_Main.transform, () =>
                                                        {
                                                            // 연구소 입장
                                                            OnClickOpenLabo();
                                                            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(80, () =>
                                                            {
                                                                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_developmentSlotList[0].transform, () =>
                                                                {
                                                                    // DNA제조 메뉴 열기
                                                                    _developmentSlotList[0].OnClick();

                                                                    //todo DNA 생성재료 투입
                                                                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_productionConfirmBtn.transform, () =>
                                                                                                {
                                                                                                    OnClickProductionConfirm();

                                                                                                    // DNA 시간 클리어
                                                                                                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_developmentSlotList[0].transform, () =>
                                                                                                                        {
                                                                                                                            _developmentSlotList[0].OnClick();

                                                                                                                            // DNA 바로 회수
                                                                                                                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_developmentSlotList[0].transform, () =>
                                                                                                                        {
                                                                                                                            _developmentSlotList[0].OnClick();

                                                                                                                            //! 보상 DNA아이템 생성
                                                                                                                            List<int> reward_dna_id = new List<int>();
                                                                                                                            List<int> reward_dna_value = new List<int>();

                                                                                                                            reward_dna_id.Add(46);
                                                                                                                            reward_dna_value.Add(1);

                                                                                                                            // 인벤에 꽃아주기
                                                                                                                            Info.My.Singleton.Inventory.AddItem(46, 1);

                                                                                                                            // 보상UI 출력 후 바로 장착
                                                                                                                            Dialog.IDialog.RequestDialogEnter<GlobalRewardDialog>();
                                                                                                                            Message.Send<Global.ShowRewardMsg>(new Global.ShowRewardMsg(reward_dna_id, reward_dna_value, () =>
                                                                                {
                                                                                    // 실제 진시황에 DNA 장착 고정
                                                                                    var jinshi = Info.My.Singleton.Inventory.characterList.Find(e => e.code == 101131);
                                                                                    var dna = Info.My.Singleton.Inventory.equipmentList.Find(e => e.code == 46);

                                                                                    jinshi.equip[6] = dna.idx;
                                                                                    dna.characterIdx = jinshi.idx;

                                                                                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(81, () =>
                            {
                                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
{
    OnClickBack();
    // Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
    // {
    //     OnClickBack();
    // }));
}));
                            }));
                                                                                }));
                                                                                                                        }));
                                                                                                                        }));
                                                                                                }));
                                                                }));
                                                            }));

                                                        }));
                                                    }));
                        }));
                    }));
                }));
            }
        }

        private void ShowMessage(string title, string message, System.Action callback)
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(title, message, callback, true));
        }
    }
}