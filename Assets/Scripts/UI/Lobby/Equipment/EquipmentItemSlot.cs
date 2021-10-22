// ==================================================
// EquipmentItemSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class EquipmentItemSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _isEquipObj;
    [SerializeField] private GameObject _isSelect;
    [SerializeField] private Button _button;

    public Info.Equipment Data { get { return _data; } }
    private uint _idx;
    Info.Equipment _data;
    private System.Action<uint> _callback;

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
            _callback(_idx);
    }

    public void Init(Info.Equipment data, System.Action<uint> callback)
    {
        _data = data;
        _callback = callback;
        _idx = _data.idx;

        _icon.sprite = Resources.Load<Sprite>(string.Format("Texture/Item/{0}", data.code));
        _isEquipObj.SetActive(data.characterIdx > 0);
    }

    public void SetSelected(uint selected)
    {
        if (_data == null)
            return;

        _isSelect.SetActive(_idx == selected);
    }
}
