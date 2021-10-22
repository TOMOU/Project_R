// ==================================================
// ICardSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class ICardSlot : MonoBehaviour
{
    [Header("Base")]
    protected UnitStatus _status;
    [SerializeField] protected Image _portrait;

    public virtual void Init(UnitStatus status)
    {
        _status = status;

        if (_status == null)
        {
            _portrait.sprite = Resources.Load<Sprite>("CharacterIcon/icon_unit_000000");
        }
        else
        {
            _portrait.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/2_portrait_cardsize/{0}", _status.code));
        }
    }
    public virtual void Release()
    {
        _status = null;
        _portrait = null;
    }
    public virtual void Refresh() { }
}
