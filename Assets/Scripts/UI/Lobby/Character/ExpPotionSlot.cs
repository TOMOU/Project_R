// ==================================================
// ExpPotionSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class ExpPotionSlot : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private int _idx;
    [SerializeField] private Text _addExpText;
    [SerializeField] private Image _potionIconImage;
    [SerializeField] private int _potionCountValue;
    [SerializeField] private Text _potionCountText;
    private System.Action<int, int> _callback;
    private ItemModel.Data _data;

    public void Init(System.Action<int, int> callback, ItemModel.Data data)
    {
        _button.onClick.AddListener(OnClick);

        _data = data;

        _addExpText.text = string.Format("EXP +{0:##,##0}", _data.value1);

        _callback = callback;
    }

    public void Release()
    {
        _button.onClick.RemoveAllListeners();
        _button = null;
        _addExpText = null;
        _potionIconImage = null;
        _potionCountText = null;
        _callback = null;
    }

    public void Refresh()
    {
        GetPotionCount();
    }

    private void OnClick()
    {
        // 예외처리용 한번 더 개수 체크
        GetPotionCount();

        if (_potionCountValue == 0)
            return;

        if (_callback != null)
        {
            _callback(_data.value1, 1);
        }
    }

    private void GetPotionCount()
    {
        _potionCountValue = Info.My.Singleton.Inventory.potion[_idx];

        _potionIconImage.SetGrayScaleWithChild(_potionCountValue == 0);
        _potionCountText.text = string.Format("{0:##,##0}", _potionCountValue);
    }

    public void OnClick_Tutorial()
    {
        OnClick();
    }
}
