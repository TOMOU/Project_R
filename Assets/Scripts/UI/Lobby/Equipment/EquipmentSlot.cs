// ==================================================
// EquipmentSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] private Image _equipmentIcon;
    [SerializeField] private GameObject _removeObj;
    [SerializeField] private Button _button;

    private int _slotIndex;
    private Info.Equipment _data;
    private System.Action<int> _callback;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OnClick()
    {
        if (_callback != null)
            _callback(_slotIndex);
    }

    public void Init(int slotIndex, uint idx, System.Action<int> callback)
    {
        _slotIndex = slotIndex;
        _callback = callback;

        if (idx == 0)
            _data = null;
        else
        {
            _data = Info.My.Singleton.Inventory.GetEquipmentByIndex(idx);
        }

        if (_data != null)
        {
            _equipmentIcon.gameObject.SetActive(true);
            _equipmentIcon.sprite = Resources.Load<Sprite>(string.Format("Texture/Item/{0}", _data.code));
        }
        else
        {
            _equipmentIcon.gameObject.SetActive(false);
        }
    }

    public void SetRemove(bool active)
    {
        if (_data == null)
        {
            if (_removeObj.activeSelf == true)
                _removeObj.SetActive(false);
            return;
        }

        _removeObj.SetActive(active);
    }
}
