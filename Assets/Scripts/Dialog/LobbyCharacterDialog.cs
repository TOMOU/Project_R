// ==================================================
// LobbyCharacterDialog.cs
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
    public class LobbyCharacterDialog : IDialog
    {
        [Header("- Global")]
        [SerializeField] private ToggleGroup _tabGroup;
        [SerializeField] private Toggle[] _tabToggle;
        [SerializeField] private GameObject[] _tabPanel;

        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        [Header("- Left")]
        [SerializeField] private Image _characterImage;
        [SerializeField] private Text _characterName;
        [SerializeField] private Text _characterLevel;
        [SerializeField] private GameObject[] _characterGrade;
        [SerializeField] private Image _characterExpSlider;
        [SerializeField] private Text _characterExpText;

        [Header("- Bottom")]
        [SerializeField] private Button _openBottomButton;
        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private Transform _contentBottom;
        [SerializeField] private List<UnitCardSlot> _bottomCardSlots;
        private bool _isOpenBottom = false;

        [Header("- List")]
        [SerializeField] private CharacterManage_List _listPanel;
        [SerializeField] private List<UnitCardSlot> _listCardSlots;

        [Header("- Info")]
        [SerializeField] private CharacterManage_Info _infoPanel;

        [Header("- Growth")]
        [SerializeField] private CharacterManage_Growth _growthPanel;

        [Header("- Evolve")]
        [SerializeField] private CharacterManage_Evolve _evolvePanel;

        [Header("- Specialize")]
        [SerializeField] private CharacterManage_Specialize _specializePanel;

        [Header("- Skill")]
        [SerializeField] private Str_CharacterManage_Skill _skillPanel;

        private int _gridSizeIdx = 0;
        private int _sortTypeIndex = 0;
        private int _sortDirectionIndex = 0;
        private uint _selectUnitIdx = 0;
        private bool _isEnterFirst = false;

        private Info.Inventory _inventory;
        private Texture2D _tex;

        protected override void OnLoad()
        {
            _inventory = Info.My.Singleton.Inventory;

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _infoPanel.skeleton.UpdateComplete += UpdateComplete;

            Init_Toggle();
            Init_SortType();
            Init_Panel();

            _listPanel.resizeGridButton.onClick.AddListener(OnClickResizeGrid);
            _listPanel.sortTypeButton.onClick.AddListener(OnClickChangeSortType);
            _listPanel.sortDirectionButton.onClick.AddListener(OnClickChangeSortDirection);

            _openBottomButton.onClick.AddListener(OnClickOpenBottomPanel);
        }

        protected override void OnUnload()
        {
            _backButton.onClick.RemoveAllListeners();
            _homeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();

            _infoPanel.skeleton.UpdateComplete -= UpdateComplete;

            _listPanel.Release();
            _infoPanel.Release();
            _growthPanel.Release();
            _evolvePanel.Release();
            _specializePanel.Release();
            _skillPanel.Release();

            _tex = null;

            _openBottomButton.onClick.RemoveAllListeners();

            Release_Toggle();
        }

        void UpdateComplete(Spine.Unity.ISkeletonAnimation anim)
        {
            if (_tex == null)
                return;

            if (_infoPanel.skeleton.OverrideTexture != _tex)
            {
                _infoPanel.skeleton.OverrideTexture = _tex;
            }
        }

        protected override void OnEnter()
        {
            if (_isOpenBottom == true)
                OnClickOpenBottomPanel();

            _tabGroup.SetAllTogglesOff();
            _tabToggle[0].isOn = true;

            _listPanel.sortTypeText.text = LocalizeManager.Singleton.GetString(3037 + _sortTypeIndex);

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyCharacterDialog>();
                RequestDialogEnter<LobbyMainDialog>();
            }));

            Message.AddListener<Lobby.RefreshCharacterDialogMsg>(OnRefreshCharacterDialog);

            CheckScenario();
        }

        protected override void OnExit()
        {
            _isEnterFirst = false;

            Message.RemoveListener<Lobby.RefreshCharacterDialogMsg>(OnRefreshCharacterDialog);
        }

        private void Init_Toggle()
        {
            // 최상단 토글 Init
            _tabToggle[0].onValueChanged.AddListener(OpenListPanel);
            _tabToggle[1].onValueChanged.AddListener(OpenInfoPanel);
            _tabToggle[2].onValueChanged.AddListener(OpenGrowthPanel);
            _tabToggle[3].onValueChanged.AddListener(OpenEvolvePanel);
            _tabToggle[4].onValueChanged.AddListener(OpenSpecializePanel);
            _tabToggle[5].onValueChanged.AddListener(OpenSkillPanel);

            // 정보 탭의 토글 Init
            _infoPanel.toggleGroup.SetAllTogglesOff();
            _infoPanel.toggles[0].isOn = true;
            _infoPanel.toggles[0].onValueChanged.AddListener(isOn =>
            {
                _infoPanel.detailPanelObj[0].SetActive(isOn);
                RefreshInfoPanel();
            });
            _infoPanel.toggles[1].onValueChanged.AddListener(isOn =>
            {
                _infoPanel.detailPanelObj[1].SetActive(isOn);
                RefreshInfoPanel();
            });
        }

        private void Release_Toggle()
        {
            // 최상단 토글 Init
            _tabToggle[0].onValueChanged.RemoveAllListeners();
            _tabToggle[1].onValueChanged.RemoveAllListeners();
            _tabToggle[2].onValueChanged.RemoveAllListeners();
            _tabToggle[3].onValueChanged.RemoveAllListeners();
            _tabToggle[4].onValueChanged.RemoveAllListeners();
            _tabToggle[5].onValueChanged.RemoveAllListeners();
        }

        private void Init_Panel()
        {
            // Panel 전부 Off
            for (int i = 0; i < _tabPanel.Length; i++)
            {
                _tabPanel[i].SetActive(false);
            }

            for (int i = 0; i < _infoPanel.detailPanelObj.Length; i++)
            {
                _infoPanel.detailPanelObj[i].SetActive(false);
            }

            // Growth Panel
            var im = Model.First<ItemModel>();
            if (im == null)
            {
                Logger.LogError("Can't load ItemModel");
                return;
            }

            ItemModel.Data data = null;

            for (int i = 0; i < _growthPanel.expPotionSlots.Length; i++)
            {
                data = im.Table.Find(e => e.id == i + 7);
                _growthPanel.expPotionSlots[i].Init(SelectPotionSlot, data);
            }

            // Evolve Panel
            _evolvePanel.evolveButton.onClick.AddListener(OnClickEvolveCharacter);
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
                if (_listPanel.gridLayoutGroup.transform.childCount < i + 1)
                {
                    _listCardSlots.Add(CreateCardSlot(_listPanel.gridLayoutGroup.transform));
                }
            }

            // Bottom - 유닛카드가 부족하다면 새로 생성
            if (_bottomCardSlots == null)
                _bottomCardSlots = new List<UnitCardSlot>();
            for (int i = 0; i < unitList.Count; i++)
            {
                if (_contentBottom.childCount < i + 1)
                {
                    _bottomCardSlots.Add(CreateCardSlot(_contentBottom));
                }
            }

            // 카드 갱신
            for (int i = 0; i < _listCardSlots.Count; i++)
            {
                _listCardSlots[i].Init(unitList[i]);
                _listCardSlots[i].AddCallback(SelectCardSlot);

                _bottomCardSlots[i].Init(unitList[i]);
                _bottomCardSlots[i].AddCallback(SelectCardSlot);
            }
        }

        private UnitCardSlot CreateCardSlot(Transform parent)
        {
            GameObject prefab = Resources.Load<GameObject>("UI/Lobby/CharacterCardSlot");
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

        private void SelectCardSlot(uint idx)
        {
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == idx);
            if (character != null)
            {
                UnitStatus status = character.GetStatus();

                _selectUnitIdx = idx;

                _characterImage.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/1_portrait_full/{0}", status.code));
                _characterName.text = status.unitName_Kor;
                _characterLevel.text = string.Format("Lv.{0}", status.level);
                for (int i = 0; i < _characterGrade.Length; i++)
                {
                    _characterGrade[i].SetActive(i < status.grade);
                }

                _characterExpSlider.fillAmount = character.exp / (float)character.nexp;
                _characterExpText.text = string.Format("{0:##,##0} / {1:##,##0}", character.exp, character.nexp);

                RefreshBottomPanel();

                if (_tabPanel[0].activeSelf == true)
                    RefreshListPanel();
                else if (_tabPanel[1].activeSelf == true)
                    RefreshInfoPanel();
                else if (_tabPanel[2].activeSelf == true)
                    RefreshGrowthPanel();
                else if (_tabPanel[3].activeSelf == true)
                    RefreshEvolvePanel();
                else if (_tabPanel[4].activeSelf == true)
                    RefreshSpecializePanel();
                else if (_tabPanel[5].activeSelf == true)
                    RefreshSkillPanel();
            }
        }

        private void SelectPotionSlot(int addExp, int count)
        {
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == _selectUnitIdx);
            if (character == null)
            {
                Logger.LogWarning("캐릭터의 인덱스를 찾을 수 없음");
                return;
            }

            if (character.level >= Info.My.Singleton.User.level && character.exp >= character.nexp)
            {
                Logger.LogWarning("캐릭터의 레벨은 유저의 레벨을 넘어갈 수 없음");
                return;
            }

            Message.Send<Lobby.UseExpPotionMsg>(new Lobby.UseExpPotionMsg(_selectUnitIdx, addExp, count));
        }

        private void OpenListPanel(bool isOn)
        {
            if (isOn == false)
            {
                _tabPanel[0].SetActive(false);
                return;
            }

            _tabPanel[0].SetActive(true);

            RefreshListPanel();

            // 최초 입장 시에만 첫번째 카드 클릭
            if (_isEnterFirst == false)
            {
                _isEnterFirst = true;
                _listCardSlots[0].OnClick();
            }
        }

        private void OpenInfoPanel(bool isOn)
        {
            if (isOn == false)
            {
                _tabPanel[1].SetActive(false);
                return;
            }

            _tabPanel[1].SetActive(true);

            _infoPanel.toggleGroup.SetAllTogglesOff();
            _infoPanel.toggles[0].isOn = true;

            RefreshInfoPanel();
        }

        private void OpenGrowthPanel(bool isOn)
        {
            if (isOn == false)
            {
                _tabPanel[2].SetActive(false);
                return;
            }

            _tabPanel[2].SetActive(true);

            RefreshGrowthPanel();
        }

        private void OpenEvolvePanel(bool isOn)
        {
            if (isOn == false)
            {
                _tabPanel[3].SetActive(false);
                return;
            }

            _tabPanel[3].SetActive(true);

            RefreshEvolvePanel();
        }

        private void OpenSpecializePanel(bool isOn)
        {
            if (isOn == false)
            {
                _tabPanel[4].SetActive(false);
                return;
            }

            _tabPanel[4].SetActive(true);

            RefreshSpecializePanel();
        }

        private void OpenSkillPanel(bool isOn)
        {
            if (isOn == false)
            {
                _tabPanel[5].SetActive(false);
                return;
            }

            _tabPanel[5].SetActive(true);

            RefreshSkillPanel();
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
                if (_listPanel.gridLayoutGroup.transform.childCount < i + 1)
                {
                    _listCardSlots.Add(CreateCardSlot(_listPanel.gridLayoutGroup.transform));
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

        private void RefreshInfoPanel()
        {
            // 유닛정보 가져오기
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == _selectUnitIdx);
            if (character == null)
            {
                Logger.LogErrorFormat("{0} 인덱스의 캐릭터를 가져오는 데 실패하였습니다.", _selectUnitIdx);
                return;
            }

            UnitStatus status = character.GetStatus();

            // 능력치 탭 활성화
            if (_infoPanel.detailPanelObj[0].activeSelf == true)
            {
                _infoPanel.statusTexts[0].text = LocalizeManager.Singleton.GetString(100000000 + status.code);
                _infoPanel.statusTexts[1].text = string.Format("{0:##,##0}", BattleCalc.Calculate_BattlePower(status));
                _infoPanel.statusTexts[2].text = string.Format("{0:##,##0}", 0);
                _infoPanel.statusTexts[3].text = string.Format("{0:##,##0}", status.damage);
                _infoPanel.statusTexts[4].text = string.Format("{0:##,##0}", status.damage);
                _infoPanel.statusTexts[5].text = string.Format("{0:##,##0}", status.pDef);
                _infoPanel.statusTexts[6].text = string.Format("{0:##,##0}", status.mDef);
                _infoPanel.statusTexts[7].text = string.Format("{0:##,##0}", status.hpFull);
                _infoPanel.statusTexts[8].text = string.Format("{0:##,##0}", 0);
                _infoPanel.statusTexts[9].text = string.Format("{0:##,##0}", 0);
                _infoPanel.statusTexts[10].text = string.Format("{0:##,##0}", status.criticalPercentage);
                _infoPanel.statusTexts[11].text = string.Format("{0:##,##0}", status.criticalMultiple);
                _infoPanel.statusTexts[12].text = string.Format("{0:##,##0}", 0);
                _infoPanel.statusTexts[13].text = string.Format("{0:##,##0}", 0);
                _infoPanel.statusTexts[14].text = string.Format("{0:##,##0}", 0);
            }
            // 프로필 탭 활성화
            else
            {
                _infoPanel.profileTexts[0].text = LocalizeManager.Singleton.GetString(101000000 + status.code);
                _infoPanel.profileTexts[1].text = LocalizeManager.Singleton.GetString(102000000 + status.code);
                _infoPanel.profileTexts[2].text = LocalizeManager.Singleton.GetString(103000000 + status.code);
                _infoPanel.profileTexts[3].text = LocalizeManager.Singleton.GetString(104000000 + status.code);
                _infoPanel.profileTexts[4].text = LocalizeManager.Singleton.GetString(105000000 + status.code);
                _infoPanel.profileTexts[5].text = LocalizeManager.Singleton.GetString(106000000 + status.code);
                _infoPanel.profileTexts[6].text = LocalizeManager.Singleton.GetString(107000000 + status.code);
                _infoPanel.profileTexts[7].text = LocalizeManager.Singleton.GetString(108000000 + status.code);

                _infoPanel.skeleton.skeletonDataAsset = SpineManager.Singleton.GetSkeletonDataAsset(status.unitName);
                // SpineManager.Singleton.spineAssetDic.TryGetValue(status.unitName, out _infoPanel.skeleton.skeletonDataAsset);

                // _infoPanel.skeleton.skeletonDataAsset = Resources.Load<Spine.Unity.SkeletonDataAsset>(string.Format("Character/SpineData/{0}/{0}_SkeletonData", status.unitName));
                _infoPanel.skeleton.Initialize(true);

                _tex = null;
                if (SpineManager.Singleton.spineTextureDic.TryGetValue(status.unitName, out _tex) == true)
                {

                }
            }
        }

        private void RefreshGrowthPanel()
        {
            // 유닛정보 가져오기
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == _selectUnitIdx);
            if (character == null)
            {
                Logger.LogErrorFormat("{0} 인덱스의 캐릭터를 가져오는 데 실패하였습니다.", _selectUnitIdx);
                return;
            }

            UnitStatus status = character.GetStatus();

            _growthPanel.statusTexts[0].text = string.Format("{0:##,##0}", status.level);
            _growthPanel.statusTexts[1].text = string.Format("{0:##,##0}", BattleCalc.Calculate_BattlePower(status));
            _growthPanel.statusTexts[2].text = string.Format("{0:##,##0}", BattleCalc.Calculate_BattlePower(status));
            _growthPanel.statusTexts[3].text = string.Format("{0:##,##0}", 0);
            _growthPanel.statusTexts[4].text = string.Format("{0:##,##0}", status.damage);
            _growthPanel.statusTexts[5].text = string.Format("{0:##,##0}", status.damage);
            _growthPanel.statusTexts[6].text = string.Format("{0:##,##0}", status.pDef);
            _growthPanel.statusTexts[7].text = string.Format("{0:##,##0}", status.mDef);
            _growthPanel.statusTexts[8].text = string.Format("{0:##,##0}", status.hpFull);
            _growthPanel.statusTexts[9].text = string.Format("{0:##,##0}", 0);
            _growthPanel.statusTexts[10].text = string.Format("{0:##,##0}", 0);
            _growthPanel.statusTexts[11].text = string.Format("{0:##,##0}", status.criticalPercentage);

            // 경험치포션 갱신
            for (int i = 0; i < _growthPanel.expPotionSlots.Length; i++)
            {
                _growthPanel.expPotionSlots[i].Refresh();
            }
        }

        private void RefreshEvolvePanel()
        {
            // 유닛정보 가져오기
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == _selectUnitIdx);
            if (character == null)
            {
                Logger.LogErrorFormat("{0} 인덱스의 캐릭터를 가져오는 데 실패하였습니다.", _selectUnitIdx);
                return;
            }

            Info.Inventory inven = Info.My.Singleton.Inventory;

            // 재료 아이콘
            // for(int i=0;i<_evolvePanel.materialIcons.Length;i++)
            // {
            //     _evolvePanel.materialIcons[i].sprite = 
            // }

            // 재료 수량
            int[] matCnt = new int[3] { inven.material_1, inven.material_2, inven.material_3 };
            _evolvePanel.materialCountTexts[0].text = string.Format("{2}{0:##,##0}{3} / {1:##,##0}", matCnt[0], 15, matCnt[0] < 15 ? "<color=#ff0000ff>" : "", matCnt[0] < 15 ? "</color>" : "");
            _evolvePanel.materialCountTexts[1].text = string.Format("{2}{0:##,##0}{3} / {1:##,##0}", matCnt[1], 20, matCnt[1] < 20 ? "<color=#ff0000ff>" : "", matCnt[1] < 20 ? "</color>" : "");
            _evolvePanel.materialCountTexts[2].text = string.Format("{2}{0:##,##0}{3} / {1:##,##0}", matCnt[2], 50, matCnt[2] < 50 ? "<color=#ff0000ff>" : "", matCnt[2] < 50 ? "</color>" : "");

            for (int i = 0; i < _evolvePanel.gradeIcons.Length; i++)
            {
                _evolvePanel.gradeIcons[i].gameObject.SetActive(i < character.grade);
            }

            _evolvePanel.priceText.text = string.Format("{0:##,##0}", 50000);

            _evolvePanel.evolveButton.SetGrayScaleWithChild(
                character.grade >= 6 ||
                inven.material_1 < 15 || inven.material_2 < 20 || inven.material_3 < 50 ||
                Info.My.Singleton.User.gold < 50000);
        }

        private void RefreshSpecializePanel()
        {

        }

        private void RefreshSkillPanel()
        {

        }

        private void RefreshBottomPanel()
        {
            // 소지한 유닛 리스트 로드
            List<UnitStatus> unitList = SortList();

            // List - 유닛카드가 부족하다면 새로 생성
            if (_bottomCardSlots == null)
                _bottomCardSlots = new List<UnitCardSlot>();

            for (int i = 0; i < unitList.Count; i++)
            {
                if (_contentBottom.childCount < i + 1)
                {
                    _bottomCardSlots.Add(CreateCardSlot(_contentBottom));
                }
            }

            // 카드 갱신
            for (int i = 0; i < _bottomCardSlots.Count; i++)
            {
                _bottomCardSlots[i].Init(unitList[i]);
                _bottomCardSlots[i].AddCallback(SelectCardSlot);

                if (_bottomCardSlots[i].idx == _selectUnitIdx)
                {
                    _bottomCardSlots[i].OnSelect();
                }
                else
                {
                    _bottomCardSlots[i].OnDeselect();
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

        private void OnClickResizeGrid()
        {
            _gridSizeIdx = _gridSizeIdx == 0 ? 1 : 0;

            _listPanel.resizeGridIconObj[0].SetActive(_gridSizeIdx == 0);
            _listPanel.resizeGridIconObj[1].SetActive(_gridSizeIdx == 1);

            if (_gridSizeIdx == 0)
            {
                _listPanel.gridLayoutGroup.cellSize = new Vector2(225, 250);
                _listPanel.gridLayoutGroup.spacing = new Vector2(15, 20);
                _listPanel.gridLayoutGroup.constraintCount = 5;
            }
            else
            {
                _listPanel.gridLayoutGroup.cellSize *= 1.25f;
                _listPanel.gridLayoutGroup.spacing *= 1.25f;
                _listPanel.gridLayoutGroup.constraintCount = 4;
            }
        }

        private void Init_SortType()
        {
            _sortTypeIndex = 0;
            _listPanel.sortTypeText.text = LocalizeManager.Singleton.GetString(3037 + _sortTypeIndex);
        }

        private void OnClickChangeSortType()
        {
            _sortTypeIndex++;
            if (_sortTypeIndex > 2)
                _sortTypeIndex = 0;

            // 버튼 텍스트 변경
            _listPanel.sortTypeText.text = LocalizeManager.Singleton.GetString(3037 + _sortTypeIndex);

            // List 패널 활성화 시에만 갱신
            if (_tabPanel[0].activeSelf == true)
                RefreshListPanel();
            if (_isOpenBottom == true)
                RefreshBottomPanel();
        }

        private void OnClickChangeSortDirection()
        {
            _sortDirectionIndex = (_sortDirectionIndex == 0) ? 1 : 0;

            // List 패널 활성화 시에만 갱신
            if (_tabPanel[0].activeSelf == true)
                RefreshListPanel();
            if (_isOpenBottom == true)
                RefreshBottomPanel();
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

        private void OnClickOpenBottomPanel()
        {
            _isOpenBottom = !_isOpenBottom;

            if (_isOpenBottom == true)
            {
                _bottomPanel.anchoredPosition = new Vector2(0f, 0f);
                _openBottomButton.transform.localScale = Vector3.one;

                RefreshBottomPanel();
            }
            else
            {
                _bottomPanel.anchoredPosition = new Vector2(0f, -255f);
                _openBottomButton.transform.localScale = new Vector3(1, -1, 1);
            }
        }

        private void OnClickEvolveCharacter()
        {
            Message.Send<Lobby.UseEvolveMaterialMsg>(new Lobby.UseEvolveMaterialMsg(_selectUnitIdx));
        }

        private void OnRefreshCharacterDialog(Lobby.RefreshCharacterDialogMsg msg)
        {
            SelectCardSlot(_selectUnitIdx);
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 24)
                PlayScenario_24();
            else if (sidx == 34)
                PlayScenario_34();
        }

        private void PlayScenario_24()
        {
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(24, () =>
            {
                // 치우를 선택해준다.
                UnitCardSlot slot = _listCardSlots.Find(e => e.code == 101031);
                _listPanel.gridLayoutGroup.enabled = false;

                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
                {
                    _listPanel.gridLayoutGroup.enabled = true;
                    slot.OnClick();

                    HorizontalLayoutGroup horizontal = _tabGroup.GetComponent<HorizontalLayoutGroup>();
                    if (horizontal != null)
                        horizontal.enabled = false;

                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_tabToggle[2].transform, () =>
                    {
                        if (horizontal != null)
                            horizontal.enabled = false;

                        _tabToggle[2].isOn = true;
                        Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(25, () =>
                        {
                            // 경험치물약 마지막꺼 버튼가이드
                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_growthPanel.expPotionSlots[3].transform, () =>
                            {
                                _growthPanel.expPotionSlots[3].OnClick_Tutorial();

                                // 장비장착으로 보낸다
                                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_homeButton.transform, OnClickHome));
                            }));
                        }));
                    }));
                }));
            }));
        }

        private void PlayScenario_34()
        {
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(34, () =>
            {
                // 치우를 선택해준다.
                UnitCardSlot slot = _listCardSlots.Find(e => e.code == 101031);

                _listPanel.gridLayoutGroup.enabled = false;

                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
                {
                    _listPanel.gridLayoutGroup.enabled = true;
                    slot.OnClick();

                    HorizontalLayoutGroup horizontal = _tabGroup.GetComponent<HorizontalLayoutGroup>();
                    if (horizontal != null)
                        horizontal.enabled = false;
                    _listPanel.gridLayoutGroup.enabled = false;

                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_tabToggle[3].transform, () =>
                    {
                        _tabToggle[3].isOn = true;
                        if (horizontal != null)
                            horizontal.enabled = true;

                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_evolvePanel.evolveButton.transform, () =>
                        {
                            OnClickEvolveCharacter();
                            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(35, () =>
                            {
                                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
                            }));
                        }));
                    }));
                }));
            }));
        }
    }

    #region Struct
    [System.Serializable]
    public class CharacterManage_List
    {
        public Button resizeGridButton;
        public GameObject[] resizeGridIconObj;
        public Button sortTypeButton;
        public Text sortTypeText;
        public Button sortDirectionButton;
        public GridLayoutGroup gridLayoutGroup;

        public void Release()
        {
            resizeGridIconObj = null;
            resizeGridButton.onClick.RemoveAllListeners();
            resizeGridButton = null;
            sortTypeButton.onClick.RemoveAllListeners();
            sortTypeButton = null;
            sortTypeText = null;
            sortDirectionButton.onClick.RemoveAllListeners();
            sortDirectionButton = null;
            gridLayoutGroup = null;
        }
    }

    [System.Serializable]
    public class CharacterManage_Info
    {
        public ToggleGroup toggleGroup;
        public Toggle[] toggles;
        public GameObject[] detailPanelObj;
        public Text[] statusTexts;
        public Text[] profileTexts;
        public Spine.Unity.SkeletonGraphic skeleton;
        public Texture2D tex;

        public void Release()
        {
            toggleGroup = null;

            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].onValueChanged.RemoveAllListeners();
            }

            toggles = null;
            detailPanelObj = null;
            statusTexts = null;
            profileTexts = null;
            skeleton = null;
            tex = null;
        }
    }

    [System.Serializable]
    public class CharacterManage_Growth
    {
        public Text[] statusTexts;
        public ExpPotionSlot[] expPotionSlots;

        public void Release()
        {
            statusTexts = null;
            for (int i = 0; i < expPotionSlots.Length; i++)
            {
                expPotionSlots[i].Release();
            }
            expPotionSlots = null;
        }
    }

    [System.Serializable]
    public class CharacterManage_Evolve
    {
        public Image[] materialIcons;
        public Text[] materialCountTexts;
        public Image[] gradeIcons;
        public Text priceText;
        public Button evolveButton;

        public void Release()
        {
            materialIcons = null;
            materialCountTexts = null;
            gradeIcons = null;
            priceText = null;
            evolveButton.onClick.RemoveAllListeners();
            evolveButton = null;
        }
    }

    [System.Serializable]
    public class CharacterManage_Specialize
    {
        public void Release()
        {

        }
    }

    [System.Serializable]
    public struct Str_CharacterManage_Skill
    {
        public void Release()
        {

        }
    }
    #endregion
}