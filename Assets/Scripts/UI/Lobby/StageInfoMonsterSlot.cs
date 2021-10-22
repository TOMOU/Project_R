// ==================================================
// StageInfoMonsterSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class StageInfoMonsterSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;

    public void Init(uint id)
    {
        _icon.sprite = Resources.Load<Sprite>(string.Format("CharacterIcon/{0}", id));
    }
}
