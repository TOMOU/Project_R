// ==================================================
// Skill_Minion_Spear.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;

public class Skill_Minion_Spear : SkillBase
{
    protected override void Prepare()
    {
        base.Prepare();

        if (_unit.SyncVertList == null)
        {
            _unit.SyncVertList = new List<bool>();
            _unit.SyncVertList.Add(false);
            _unit.SyncVertList.Add(false);
            _unit.SyncVertList.Add(false);
        }

        EffectManager.Singleton.OnCreateParticle("jinshi_spear");
    }

    public override IEnumerator coSkillNormal()
    {
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(true);

        PlayEffect("jinshi_spear", new Vector3(0.5f, 0.7f));

        // 데미지 총합 계산 후 전달
        SendDamage();
    }

    public override IEnumerator coSkill_1()
    {
        yield return new WaitForSpineEvent(_skel, "buff");

        _unit.Status.ApplyBuff(new UnitBuff(_unit.Status, Constant.SkillType.Buff, Constant.SkillProperty.Speed, 3f, 10f));
    }
}
