// ==================================================
// LobbyEquipDialog.cs
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
    public class LobbyEquipDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        [Header("- Left (CharacterInfo)")]
        [SerializeField] private Image _characterImage;
        [SerializeField] private Text _characterName;
        [SerializeField] private Text _characterBP;
        [SerializeField] private List<EquipmentSlot> _characterEquipList;

        [SerializeField] private Button _removeEquipButton;
        [SerializeField] private Button _removeEquipAllButton;

        [Header("- Right (Inventory)")]
        [SerializeField] private Transform _content;
        [SerializeField] private GridLayoutGroup _gridLayout_Inventory;
        [SerializeField] private List<EquipmentItemSlot> _itemSlotList;

        [Header("- Popup (Equipment Info)")]
        [SerializeField] private GameObject _equipmentInfoObj;
        [SerializeField] private Text _equipmentName;
        [SerializeField] private Button _strengthenButton;
        [SerializeField] private Button overpowerButton;
        [SerializeField] private Button _unequipButton;
        [SerializeField] private Button _closeInfoButton;


        [Header("- Bottom")]
        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private Transform _contentBottom;
        [SerializeField] private GridLayoutGroup _gridLayout_Character;
        [SerializeField] private List<UnitCardSlot> _bottomCardSlots;

        private int _sortTypeIndex = 0;
        private uint _selectUnitIdx = 0;
        private uint _selectItemIdx = 0;
        private int _sortDirectionIndex = 0;
        private Info.Inventory _inventory;
        private bool _isEnterFirst = false;
        private bool _isRemoveMode = false;

        protected override void OnLoad()
        {
            base.OnLoad();

            _inventory = Info.My.Singleton.Inventory;

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _removeEquipButton.onClick.AddListener(OnClickRemoveEquip);
            _removeEquipAllButton.onClick.AddListener(OnClickRemoveEquipAll);

            _closeInfoButton.onClick.AddListener(OnClickClose);
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

            _removeEquipButton.onClick.RemoveAllListeners();
            _removeEquipAllButton.onClick.RemoveAllListeners();

            _closeInfoButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            _isRemoveMode = false;
            _equipmentInfoObj.SetActive(false);

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                Dialog.IDialog.RequestDialogExit<LobbyEquipDialog>();
                Dialog.IDialog.RequestDialogEnter<LobbyMainDialog>();
            }));

            RefreshEquipInfo();
            RefreshItemList();
            RefreshBottomPanel();

            // 최초 입장 시에만 첫번째 카드 클릭
            if (_isEnterFirst == false)
            {
                _isEnterFirst = true;
                _bottomCardSlots[0].OnClick();
            }

            CheckScenario();
        }

        protected override void OnExit()
        {
            base.OnExit();

            _isEnterFirst = false;
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

        private void OnClickRemoveEquip()
        {
            _isRemoveMode = true;

            if (_equipmentInfoObj.activeSelf == true)
            {
                // 장비정보창 떠있다면 비활성화
                OnClickBack();
            }

            _selectItemIdx = 0;

            for (int i = 0; i < _characterEquipList.Count; i++)
            {
                _characterEquipList[i].SetRemove(_isRemoveMode);
            }
        }

        private void OnClickRemoveEquipAll()
        {
            // 캐릭터정보 가져옴
            Info.Character character = Info.My.Singleton.Inventory.GetCharacterByIndex(_selectUnitIdx);
            if (character != null)
            {
                for (int i = 0; i < character.equip.Length; i++)
                {
                    if (character.equip[i] == 0)
                        continue;

                    Info.Equipment equip = Info.My.Singleton.Inventory.GetEquipmentByIndex(character.equip[i]);
                    if (equip != null)
                    {
                        character.equip[i] = 0;
                        equip.characterIdx = 0;
                    }
                }

                RefreshEquipInfo();
                RefreshItemList();
                RefreshBottomPanel();
            }
        }

        private void RefreshEquipInfo()
        {
            // _selectUnitIdx;
            Info.Character character = Info.My.Singleton.Inventory.GetCharacterByIndex(_selectUnitIdx);

            for (int i = 0; i < _characterEquipList.Count; i++)
            {
                if (character != null)
                {
                    _characterEquipList[i].Init(i, character.equip[i], SelectEquipSlot);
                    _characterEquipList[i].SetRemove(_isRemoveMode);
                }
                else
                {
                    _characterEquipList[i].Init(i, 0, SelectEquipSlot);
                    _characterEquipList[i].SetRemove(_isRemoveMode);
                }
            }
        }

        private void RefreshItemList()
        {
            var list = Info.My.Singleton.Inventory.equipmentList;

            if (list.Count > _itemSlotList.Count)
            {
                int count = list.Count - _itemSlotList.Count;
                for (int i = 0; i < count; i++)
                {
                    // 부족분만큼 생성
                    GameObject prefab = Resources.Load<GameObject>("UI/Lobby/ItemCardSlot");

                    GameObject obj = GameObject.Instantiate(prefab);
                    obj.transform.SetParent(_content);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localScale = Vector3.one;

                    EquipmentItemSlot slot = obj.GetComponent<EquipmentItemSlot>();
                    _itemSlotList.Add(slot);
                }
            }

            for (int i = 0; i < _itemSlotList.Count; i++)
            {
                if (i < list.Count)
                {
                    _itemSlotList[i].gameObject.SetActive(true);
                    _itemSlotList[i].Init(list[i], SelectItemSlot);
                    _itemSlotList[i].SetSelected(_selectItemIdx);
                }
                else
                {
                    _itemSlotList[i].gameObject.SetActive(false);
                }
            }
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

        private void SelectCardSlot(uint idx)
        {
            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == idx);
            if (character != null)
            {
                UnitStatus status = character.GetStatus();

                _selectUnitIdx = idx;

                _characterImage.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/1_portrait_full/{0}", status.code));
                _characterName.text = string.Format("<size=45>Lv. {0}</size>  {1}", status.level, status.unitName_Kor);
                _characterBP.text = string.Format("{0:##,##0}", BattleCalc.Calculate_BattlePower(status));

                RefreshEquipInfo();
                RefreshItemList();
                RefreshBottomPanel();
            }
        }

        private void SelectEquipSlot(int idx)
        {
            if (_selectUnitIdx == 0)
                return;

            // 아이템정보를 가져와서 슬롯과 맞지 않으면 건너뛴다.
            Info.Equipment itemInfo = Info.My.Singleton.Inventory.GetEquipmentByIndex(_selectItemIdx);
            if (itemInfo == null)
                return;

            var im = Model.First<ItemModel>();
            var info = im.Table.Find(e => e.id == itemInfo.code);
            if (idx != info.equip - 1)
                return;

            Info.Character character = Info.My.Singleton.Inventory.characterList.Find(e => e.idx == _selectUnitIdx);

            // 제거모드가 아니라면
            if (_selectItemIdx > 0)
            {
                if (character != null)
                {
                    // 이전에 껴있던 아이템 확인 후 제거
                    if (character.equip[idx] > 0)
                    {
                        Info.Equipment prevItem = Info.My.Singleton.Inventory.GetEquipmentByIndex(character.equip[idx]);
                        Info.Equipment newItem = Info.My.Singleton.Inventory.GetEquipmentByIndex(_selectItemIdx);

                        if (prevItem != null)
                        {
                            prevItem.characterIdx = 0;
                            character.equip[idx] = 0;
                        }

                        if (newItem != null)
                        {
                            newItem.characterIdx = _selectUnitIdx;
                            character.equip[idx] = _selectItemIdx;

                            _selectItemIdx = 0;
                        }

                        // 장비정보팝업 끄기
                        OnClickBack();
                    }
                    else
                    {
                        Info.Equipment newItem = Info.My.Singleton.Inventory.GetEquipmentByIndex(_selectItemIdx);

                        if (newItem != null)
                        {
                            newItem.characterIdx = _selectUnitIdx;
                            character.equip[idx] = _selectItemIdx;

                            _selectItemIdx = 0;
                        }

                        // 장비정보팝업 끄기
                        OnClickBack();
                    }
                }
            }
            // 제거모드(선택 안한상태)에서 슬롯 지우기
            else
            {
                if (_isRemoveMode == true)
                {
                    Info.Equipment item = Info.My.Singleton.Inventory.GetEquipmentByIndex(character.equip[idx]);
                    if (item != null)
                    {
                        item.characterIdx = 0;
                        character.equip[idx] = 0;

                        _selectItemIdx = 0;
                    }

                    _isRemoveMode = false;
                }
            }

            RefreshEquipInfo();
            RefreshItemList();
            RefreshBottomPanel();
        }

        private void SelectItemSlot(uint idx)
        {
            Info.Equipment item = Info.My.Singleton.Inventory.GetEquipmentByIndex(idx);
            if (item != null)
            {
                _isRemoveMode = false;

                _selectItemIdx = idx;

                OpenEquipmentInfo();

                RefreshEquipInfo();
                RefreshItemList();
                RefreshBottomPanel();
            }
        }

        private void OpenEquipmentInfo()
        {
            if (_equipmentInfoObj.activeSelf == false)
            {
                Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
                {
                    _equipmentInfoObj.SetActive(false);
                    _selectItemIdx = 0;

                    RefreshItemList();
                }));
            }

            // 장비정보 가져오기
            Info.Equipment item = Info.My.Singleton.Inventory.GetEquipmentByIndex(_selectItemIdx);

            var im = Model.First<ItemModel>();
            string name = im.Table.Find(e => e.id == item.code).name;

            _equipmentInfoObj.SetActive(true);

            _equipmentName.text = name;
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

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 26)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(26, () =>
                {
                    // 치우 슬롯 선택
                    var chiyou = _bottomCardSlots.Find(e => e.code == 101031);

                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(chiyou.transform, _gridLayout_Character, () =>
                        {
                            SelectCardSlot(chiyou.idx);

                            // 장비 선택 (무기)
                            var weapon = _itemSlotList.Find(e => e.Data.GetData().equip == 1);

                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(weapon.transform, _gridLayout_Inventory, () =>
                            {
                                SelectItemSlot(weapon.Data.idx);

                                // 무기 장비슬롯에 장착
                                var slot = _characterEquipList[0];
                                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
                                {
                                    SelectEquipSlot(0);

                                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(27, () =>
                                    {
                                        Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(28, () =>
                                        {
                                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, () =>
                                            {
                                                OnClickBack();
                                            }));
                                        }));
                                    }));
                                }));
                            }));
                        }));
                }));
            }
            if (sidx == 53)     // 치우의 장비강화 후 대신전(캐릭터스토리) 이동
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(53, () =>
                {
                    // 치우 슬롯 선택
                    var chiyou = _bottomCardSlots.Find(e => e.code == 101031);

                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(chiyou.transform, _gridLayout_Character, () =>
                    {
                        SelectCardSlot(chiyou.idx);

                        // 장비 선택 (무기)
                        var weapon = _itemSlotList.Find(e => e.Data.GetData().equip == 1);

                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(weapon.transform, _gridLayout_Inventory, () =>
                        {
                            SelectItemSlot(weapon.Data.idx);

                            // 장비강화버튼
                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_strengthenButton.transform, () =>
                            {
                                OnClickBack();

                                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(54, () =>
                                {
                                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
                                }));
                            }));
                        }));
                    }));
                }));
            }
        }
    }
}