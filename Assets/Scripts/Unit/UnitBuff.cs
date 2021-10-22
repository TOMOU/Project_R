// ==================================================
// UnitBuff_R.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitBuff
{
    private UnitStatus status;
    /// <summary>
    /// 버프인지, 디버프인지
    /// </summary>
    /// <value></value>
    public Constant.SkillType type { get; private set; }
    /// <summary>
    /// 버프의 종류 (물리데미지 증감,....)
    /// </summary>
    /// <value></value>
    public Constant.SkillProperty property { get; private set; }
    /// <summary>
    /// 적용시킬 값
    /// </summary>
    /// <value></value>
    public float value { get; private set; }
    /// <summary>
    /// 지속된 시간
    /// </summary>
    /// <value></value>
    public float curTime { get; private set; }
    /// <summary>
    /// 최대 지속시간
    /// </summary>
    /// <value></value>
    public float maxTime { get; private set; }

    public UnitBuff(UnitStatus status, Constant.SkillType type, Constant.SkillProperty property, float value, float maxTime)
    {
        this.status = status;
        this.type = type;
        this.property = property;
        this.value = value;
        this.curTime = 0f;
        this.maxTime = maxTime;
    }

    public void Active(bool active)
    {
        if (property == Constant.SkillProperty.Critical)
        {
            status.criticalPercentage += (active == true ? (int)value : -(int)value);
        }
        else if (property == Constant.SkillProperty.PhysicalDamage)
        {
            if (status.basicProperty == Constant.BasicAttackType.Physics)
                status.damage += (active == true ? (int)value : -(int)value);
        }
        else if (property == Constant.SkillProperty.PhysicalDefence)
        {
            status.pDef += (active == true ? (int)value : -(int)value);
        }
        else if (property == Constant.SkillProperty.PhysicalShield)
        {
            status.shieldPhysical = (active == true ? (int)value : 0);
        }
        else if (property == Constant.SkillProperty.MagicalDamage)
        {
            if (status.basicProperty == Constant.BasicAttackType.Magic)
                status.damage += (active == true ? (int)value : -(int)value);
        }
        else if (property == Constant.SkillProperty.MagicalDefence)
        {
            status.mDef += (active == true ? (int)value : -(int)value);
        }
        else if (property == Constant.SkillProperty.MagicalShield)
        {
            status.shieldMagical = (active == true ? (int)value : 0);
        }
        else if (property == Constant.SkillProperty.TP)
        {
            if (active == false)
                return;

            // 바로 종료시켜 준다.
            curTime = maxTime;
            status.mp += (int)value;
        }
        else if (property == Constant.SkillProperty.Speed)
        {
            status.attackSpeed = (active == true ? status.origin.attackSpeed / value : status.origin.attackSpeed);
        }
        else if (property == Constant.SkillProperty.Heal_Tick)
        {
            status.isDotHeal = active;
            maxTime = 100f;
        }
        else if (property == Constant.SkillProperty.Blind)
        {
            status.isBlind = active;
        }
        else if (property == Constant.SkillProperty.Burn)
        {
            status.isBurn = active;
            maxTime = 100f;
        }
        else if (property == Constant.SkillProperty.KnockBack)
        {
            status.dirKnockback = value;
            status.isKnockback = active;
        }
        else if (property == Constant.SkillProperty.Stun)
        {
            status.isStun = active;
        }
        else if (property == Constant.SkillProperty.AbsorbHP)
        {
            status.isAbsorbHP = active;
        }
        else if (property == Constant.SkillProperty.Aggro)
        {
            status.isAggroState = active;

            if (active == false)
            {
                var list = status.UnitInfo.Team == Constant.Team.Blue ? BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Target == status.UnitInfo)
                    {
                        list[i].Search_Force();
                    }
                }
            }
        }
        else if (property == Constant.SkillProperty.Bleed)
        {
            status.isBleed = active;
        }
        else if (property == Constant.SkillProperty.Immune)
        {
            status.isImmune = active;
        }
        else if (property == Constant.SkillProperty.MinionSpear)
        {
            var minion = status.minionSpear;

            if (minion == null)
                return;

            if (active)
            {
                if (minion.isDie == true)
                {
                    minion.transform.localPosition = status.UnitInfo.transform.localPosition + new Vector3(status.UnitInfo.FSM.Skeleton.skeleton.ScaleX > 0 ? 5f : -5f, 0f);

                    EffectManager.Singleton.OnPlayParticle(minion.transform.localPosition, "jinshi_smoke", 1f, minion.Rend.sortingLayerID, minion.Rend.sortingOrder);

                    minion.Status.hp = minion.Status.hpFull;
                    minion.isDie = false;
                    minion.IsLock = false;
                    minion.FSM.Idle();
                }
                else
                {
                    minion.FSM.Skill1(true);
                }
            }
            else
            {
                minion.Status.hp = 0;
                minion.isDie = true;
                minion.IsLock = true;
                minion.FSM.Die();

                minion.Status.AllEffectOff();
            }
        }
        else if (property == Constant.SkillProperty.MinionShield)
        {
            if (active == true)
            {
                status.minionShieldCount++;
                var minion = status.minionShieldList.Find(e => e.shieldEnabled == false);
                if (minion != null)
                {
                    EffectManager.Singleton.OnPlayParticle(minion.transform.position, "jinshi_smoke", 1f, minion.Rend.sortingLayerID, minion.Rend.sortingOrder);
                    minion.Idle();
                }
            }
        }
    }

    public void Refresh()
    {
        curTime += Time.deltaTime;
    }
}