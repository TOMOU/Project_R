// ==================================================
// RewardSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Text _name;
    [SerializeField] private Text _value;

    public void Init(int idx, int value)
    {
        var im = Model.First<ItemModel>();
        ItemModel.Data data = im.Table.Find(e => e.id == idx);

        _icon.sprite = Resources.Load<Sprite>(string.Format("Texture/Item/{0}", idx));
        _name.text = data.name;

        if (value > 1)
            _value.text = string.Format("{0:##,##0}", value);
        else
            _value.text = "";
    }
}
