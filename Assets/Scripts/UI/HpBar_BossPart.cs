// ==================================================
// HpBar_BossPart.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class HpBar_BossPart : MonoBehaviour
{
    [SerializeField] private int _idx;
    [SerializeField] private UnitInfo_Normal _bossInfo;
    [SerializeField] private Image _hpValue;

    private int _curHP;

    public void Init(UnitInfo_Normal info)
    {
        _bossInfo = info;

        _curHP = _bossInfo.breakGauge[_idx];
    }

    private void Update()
    {
        if (_bossInfo == null)
            return;

        if (_curHP > _bossInfo.breakGauge[_idx])
        {
            _curHP -= 10;
        }
        else if (_curHP < _bossInfo.breakGauge[_idx])
        {
            _curHP = _bossInfo.breakGauge[_idx];
        }

        _hpValue.fillAmount = _curHP / 1000f;
    }
}
