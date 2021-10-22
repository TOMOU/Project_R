// ==================================================
// Skill_Monster.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using Spine.Unity;
using System.Collections;

public class Skill_Monster : SkillBase
{
    protected override void Prepare()
    {
        base.Prepare();

        if (_unit.Status.code == 104001)
        {
            if (missileInfo == null)
            {
                // 빈 오브젝트 하위에 붙이기
                GameObject obj = new GameObject();
                obj.transform.SetParent(EffectManager.Singleton.transform);

                // 해당 오브젝트에 스크립트 붙여주고 캐싱
                missileInfo = obj.AddComponent<MissileInfo>();
                missileInfo.Init(1040011);

                EffectManager.Singleton.missileList.Add(obj);
            }
        }
    }

    public override IEnumerator coSkillNormal()
    {
        yield return new WaitForSpineEvent(_skel, "attack");

        if (_unit == null)
            Logger.LogError("_unit is null");

        if (_unit.Target == null)
            Logger.LogError("_unit.Target is null");

        if (_unit.Status.code == 104001)
        {
            missileInfo.Line(_unit, _unit.Target, () =>
            {
                SoundManager.Singleton.PlayAttackSound(false);
                SendDamage();
            });
        }
        else
        {
            SoundManager.Singleton.PlayAttackSound(true);

            // 데미지 총합 계산 후 전달
            SendDamage();
        }

        base.AddActivePoint();
    }
}
