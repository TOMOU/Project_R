// ==================================================
// SkillBase.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SkillBase : MonoBehaviour
{
    public bool UseSkill { get; private set; }
    public bool NoRange { get; private set; }
    protected UnitInfo_Normal _unit;
    protected SkeletonAnimation _skel;
    public MissileInfo missileInfo;

    public void Init(IUnitInfo info, SkeletonAnimation skeleton)
    {
        _unit = info as UnitInfo_Normal;
        _skel = skeleton;

        UseSkill = (
            this is Skill_Monster == false &&
            this is Skill_Minion_Shield == false &&
            this is Skill_Minion_Spear == false &&
            this is Skill_Minion_Sword == false);

        Prepare();
    }

    public void SetUnlimitedRange()
    {
        NoRange = true;
    }

    public virtual IEnumerator coSkillNormal()
    {
        yield return null;
    }

    public virtual IEnumerator coSkill_1()
    {
        yield return null;
    }

    public virtual IEnumerator coSkill_2()
    {
        yield return null;
    }

    public virtual IEnumerator coSkill_3()
    {
        yield return null;
    }

    protected void AddActivePoint()
    {
        _unit.Status.mp += 100;
    }

    protected virtual void Prepare()
    {
        if (_unit.isSummonUnit == true)
            return;

        // Load Global Effect
        for (int i = 0; i < 3; i++)
        {
            EffectManager.Singleton.OnCreateParticle("hit");
        }

        EffectManager.Singleton.OnCreateParticle("buff");
    }

    protected void SendDamage()
    {
        if (_unit.Target == null)
            return;

        // 데미지 연산
        int damage = BattleCalc.Calculate_Damage(_unit.Status.damage, _unit.Target.Status.pDef, Random.Range(0.95f, 1.05f));

        // 크리티컬 계산
        bool isCritical = BattleCalc.Calculate_ActiveCritical(_unit.Status.criticalPercentage, _unit.Status.level, _unit.Target.Status.level);

        // 실명상태 적용
        if (_unit.Status.isBlind == true)
            damage = 0;
        else if (_unit.isSummonUnit == true && _unit.owner != null && _unit.owner.Status.isBlind)
            damage = 0;

        // 타겟에 데미지 전달
        if (_unit.isRaidLogic == true)
            Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(_unit.Target, damage, isCritical, _unit.slotIndex));
        else
            Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(_unit.Target, damage, isCritical));
    }

    protected void SendDamage(IUnitInfo target)
    {
        if (target == null)
            return;

        // 데미지 연산
        int damage = BattleCalc.Calculate_Damage(_unit.Status.damage, target.Status.pDef);

        // 크리티컬 계산
        bool isCritical = BattleCalc.Calculate_ActiveCritical(_unit.Status.criticalPercentage, _unit.Status.level, target.Status.level);

        // 실명상태 적용
        if (_unit.Status.isBlind == true)
            damage = 0;
        else if (_unit.isSummonUnit == true && _unit.owner != null && _unit.owner.Status.isBlind)
            damage = 0;

        // 타겟에 데미지 전달
        if (_unit.isRaidLogic == true)
            Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(target, damage, isCritical, _unit.slotIndex));
        else
            Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(target, damage, isCritical));
    }

    protected void PlayEffect(string key, Vector3 offset)
    {
        if (_skel.skeleton.ScaleX != 1f)
            offset.x *= -1f;
        EffectManager.Singleton.OnPlayParticle(_unit.transform.localPosition + offset, key, _unit.FSM.Skeleton.skeleton.ScaleX, _unit.Rend.sortingLayerID, _unit.Rend.sortingOrder);
    }
}
