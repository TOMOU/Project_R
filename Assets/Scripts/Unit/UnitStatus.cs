// ==================================================
// UnitStatus_R.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitStatus
{
    public UnitStatus origin;
    public int level;
    public int grade;

    public uint idx;
    public int code; // 유닛코드
    public string unitName; // 프리팹이름
    public string unitName_Kor; // 한글이름

    public Constant.Position position;

    public int hp; // 체력
    public int hpFull; // 전체 체력
    public int mp; // 마력 (프리코네 TP)
    public int mpFull; // 전체 마력
    public int shieldPhysical; // 물리 실드
    public int shieldMagical; // 마법 실드
    public Constant.BasicAttackType basicProperty; // 기본공격 타입
    public int damage; // 데미지
    public int pDef; // 물리방어력
    public int mDef; // 마법방어력

    public Constant.BasicAttackType attackType; // 공격타입 (0: None, 1: 근거리, 2: 원거리)
    public float attackRange; // 공격가능거리
    public float attackSpeed; // 공격속도
    public float attackKeyFrame; // 평타 키프레임

    public float criticalPercentage; // 치명타확률
    public float criticalMultiple; // 치명타배율

    public float moveSpeed; // 이동속도

    public SkillModel.Skill skill_0;
    public SkillModel.Skill skill_1;
    public SkillModel.Skill skill_2;

    public List<UnitBuff> buffList;

    public IUnitInfo UnitInfo { get { return _unitInfo; } }
    private IUnitInfo _unitInfo;
    private UnitInfo_Normal _normalInfo;
    private int _unitMode;

    public float dirKnockback = 0f;
    public bool isKnockback = false;
    public bool isBlind = false;
    public bool isStun = false;
    public bool isBurn = false;
    public bool isDotHeal = false;
    public bool isAbsorbHP = false;
    public bool isAggroState = false;
    public bool isBleed = false;
    public bool isImmune = false;

    private int _burnCount = 0;
    private float _burnDelta = 0f;
    private float _bleedCount = 0;
    private float _bleedDelta = 0f;
    private int _dotHealCount = 0;
    private float _dotHealDelta = 0;
    public int buffCount = 0;
    public int buffCount_Plus = 0;
    public SwordFSM minionSword;
    public UnitInfo_Normal minionSpear;
    public List<UnitInfo_Normal> j_crossList;
    public List<ShieldFSM> minionShieldList;
    public List<UnitInfo_Normal> minionSpear_AP;
    public int minionShieldCount = 0;

    public UnitStatus(UnitModel.Unit unit, uint idx, int level, int grade)
    {
        this.idx = idx;

        this.level = level;
        this.grade = grade;

        this.code = unit.code;
        this.unitName = unit.unitName;
        this.unitName_Kor = unit.unitName_Kor;

        this.position = unit.position;

        this.hp = BattleCalc.Calculate_Status(unit.hp, level, unit.hpLevel, grade, unit.hpGrade);
        this.hpFull = this.hp;
        this.mp = 0;
        this.mpFull = 1000;
        this.basicProperty = unit.basicProperty;
        this.damage = BattleCalc.Calculate_Status(unit.damage, level, unit.damageLevel, grade, unit.damageGrade);
        this.pDef = BattleCalc.Calculate_Status(unit.physicalDefence, level, unit.pDefLevel, grade, unit.pDefGrade);
        this.mDef = BattleCalc.Calculate_Status(unit.magicalDefence, level, unit.mDefLevel, grade, unit.mDefGrade);

        this.attackType = (Constant.BasicAttackType)unit.attackType;
        this.attackRange = unit.attackRange;
        this.attackSpeed = unit.attackSpeed;
        this.attackKeyFrame = unit.attackKeyFrame;
        this.criticalPercentage = unit.criticalPercentage;
        this.criticalMultiple = unit.criticalMultiple;
        this.moveSpeed = unit.moveSpeed * 1.4f;

        var sm = Model.First<SkillModel>();
        if (sm != null)
        {
            skill_0 = sm.skillTable.Find(e => e.id == unit.skillID_0);
            skill_1 = sm.skillTable.Find(e => e.id == unit.skillID_1);
            skill_2 = sm.skillTable.Find(e => e.id == unit.skillID_2);
        }

        buffList = new List<UnitBuff>();

        origin = this.Copy();
    }

    public void InitUnitInfo(IUnitInfo info)
    {
        _unitInfo = info;
        _normalInfo = _unitInfo as UnitInfo_Normal;
        if (_unitInfo is UnitInfo_Raid)
        {
            _unitMode = 2;
        }
        else
        {
            _unitMode = 0;
        }
    }

    public void Reset()
    {
        hp = hpFull;
    }

    /// <summary>
    /// 버프의 상태 체크 (지속시간)
    /// </summary>
    /// <param name="isLock">업데이트를 일시중지할지?</param>
    public void Refresh(bool isLock)
    {
        if (hp <= 0)
            return;

        else if (_unitInfo.isDie == true)
            return;

        buffCount = buffList.Count;
        buffCount_Plus = buffList.FindAll(e => e.type == Constant.SkillType.Buff).Count;

        if (buffCount_Plus > 0)
            EffectManager.Singleton.OnParticleFollow("buff", _unitInfo.transform, true, _unitInfo.FSM.Skeleton.skeleton.ScaleX, _normalInfo);
        else
            EffectManager.Singleton.OnParticleFollow("buff", _unitInfo.transform, false, _unitInfo.FSM.Skeleton.skeleton.ScaleX, _normalInfo);

        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i].curTime > buffList[i].maxTime)
            {
                //todo 버프를 지워준다.
                buffList[i].Active(false);

                // 스턴이 해제되었으니 idle로
                if (buffList[i].property == Constant.SkillProperty.Stun || buffList[i].property == Constant.SkillProperty.KnockBack)
                {
                    _unitInfo.FSM.Idle();
                }

                buffList.Remove(buffList[i]);

                continue;
            }

            if (isLock == false)
                buffList[i].Refresh();
        }

        if (isLock)
            return;

        CheckDotHeal();
        CheckBleed();
        CheckBurn();
    }

    public UnitStatus Copy()
    {
        return (UnitStatus)this.MemberwiseClone();
    }

    /// <summary>
    /// 현재 활성화되어있는 버프, 디버프를 전부 해제해준다.
    /// </summary>
    public void AllEffectOff()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            buffList[i].Active(false);
        }

        buffList.Clear();

        buffCount = 0;
        buffCount_Plus = 0;

        EffectManager.Singleton.OnParticleFollow("buff", _unitInfo.transform, false, _unitInfo.FSM.Skeleton.skeleton.ScaleX, null);

        minionShieldCount = 0;
        MinionShield_Die();
    }

    public void ApplyBuff(UnitBuff buff)
    {
        UnitInfo_Normal un = _unitInfo as UnitInfo_Normal;

        if (code == 110001 && buff.type == Constant.SkillType.Debuff && un.isRaidLogic == true)
        {
            return;
        }

        if (isImmune == true && buff.type == Constant.SkillType.Debuff)
            return;

        buff.Active(true);
        buffList.Add(buff);

        buffCount = buffList.Count;
        buffCount_Plus = buffList.FindAll(e => e.type == Constant.SkillType.Buff).Count;
    }

    // 초당 퍼뎀으로 체력을 깎는다.
    private void CheckBleed()
    {
        // 출혈상태가 아니면 들어가지 않는다.
        if (isBleed == false)
            return;

        if (hp <= 0)
            return;

        _bleedDelta += Time.deltaTime;
        if (_bleedDelta >= 1f)
        {
            _bleedCount++;
            _bleedDelta = 0f;

            if (UnityEngine.Random.Range(0, 2) == 0)
                EffectManager.Singleton.OnPlayParticle(_unitInfo.transform.localPosition, "j_skill3_1", _unitInfo.FSM.Skeleton.skeleton.ScaleX, _normalInfo.Rend.sortingLayerID, _normalInfo.Rend.sortingOrder);
            else
                EffectManager.Singleton.OnPlayParticle(_unitInfo.transform.localPosition, "j_skill3_2", _unitInfo.FSM.Skeleton.skeleton.ScaleX, _normalInfo.Rend.sortingLayerID, _normalInfo.Rend.sortingOrder);

            // 출혈 퍼뎀을 적용
            UnitBuff b = buffList.Find(e => e.type == Constant.SkillType.Debuff && e.property == Constant.SkillProperty.Bleed);
            if (b != null)
            {
                // 체력을 퍼뎀으로 깎는다.
                Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(_unitInfo, (int)(hpFull * b.value), false));
            }
        }
    }

    private void CheckBurn()
    {
        // 화상 상태가 아니라면 건너뛴다.
        if (isBurn == false)
            return;

        // 체력이 0ㅇ이라면 건너뛴다.
        if (hp <= 0)
            return;

        _burnDelta += Time.deltaTime;

        if (_burnDelta >= 1f)
        {
            _burnCount++;
            _burnDelta = 0f;

            UnitBuff b = buffList.Find(e => e.type == Constant.SkillType.Debuff && e.property == Constant.SkillProperty.Burn);
            if (b != null)
            {
                hp -= (int)b.value;

                if (_unitMode == 2)
                    Message.Send<Battle.Raid.SendDamageMsg>(new Battle.Raid.SendDamageMsg(_unitInfo.transform, (int)b.value, false, false));
                else
                    Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(_unitInfo, (int)b.value, false));


                if (hp <= 0 && _unitInfo.isDie == false)
                {
                    _burnCount = 10;

                    if (_unitMode == 2)
                        Message.Send<Battle.Raid.SendImDyingMsg>(new Battle.Raid.SendImDyingMsg(_unitInfo));
                    else
                        Message.Send<Battle.Normal.SendImDyingMsg>(new Battle.Normal.SendImDyingMsg(_unitInfo));
                }
            }
        }

        if (_burnCount == 10)
        {
            UnitBuff b = buffList.Find(e => e.type == Constant.SkillType.Debuff && e.property == Constant.SkillProperty.Burn);
            if (b != null)
            {
                b.Active(false);
                buffList.Remove(b);
            }
        }
    }

    private void CheckDotHeal()
    {
        // 도트힐상태가 아니라면 건너뛴다.
        if (isDotHeal == false)
            return;

        // 체력이 0이라면 건너뛴다.
        if (hp <= 0)
            return;

        _dotHealDelta += Time.deltaTime;

        if (_dotHealDelta >= 1f)
        {
            _dotHealCount++;
            _dotHealDelta = 0f;
            UnitBuff b = buffList.Find(e => e.property == Constant.SkillProperty.Heal_Tick);
            if (b != null)
            {
                hp += (int)b.value;
                if (hp > hpFull)
                    hp = hpFull;

                if (_unitMode == 2)
                    Message.Send<Battle.Raid.SendDamageMsg>(new Battle.Raid.SendDamageMsg(_unitInfo.transform, (int)b.value, false, true));
                else
                    Message.Send<Battle.Normal.SendHealMsg>(new Battle.Normal.SendHealMsg(_unitInfo, (int)b.value));
            }
        }

        if (_dotHealCount == 10)
        {
            UnitBuff b = buffList.Find(e => e.type == Constant.SkillType.Buff && e.property == Constant.SkillProperty.Heal_Tick);
            if (b != null)
            {
                b.Active(false);
                buffList.Remove(b);
            }
        }
    }

    public void MinionShield_Idle()
    {
        if (minionShieldList == null)
            return;

        for (int i = 0; i < minionShieldList.Count; i++)
        {
            if (minionShieldList[i].shieldEnabled == true)
                minionShieldList[i].Idle();
        }
    }

    public void MinionShield_Run()
    {
        if (minionShieldList == null)
            return;

        for (int i = 0; i < minionShieldList.Count; i++)
        {
            if (minionShieldList[i].shieldEnabled == true)
                minionShieldList[i].Run();
        }
    }

    public void MinionShield_Die()
    {
        if (minionShieldList == null)
            return;

        for (int i = 0; i < minionShieldList.Count; i++)
        {
            if (minionShieldList[i].shieldEnabled == true)
                minionShieldList[i].Die();
        }
    }

    public void MinionShield_Hide(int mode)
    {
        if (minionShieldList == null)
            return;

        for (int i = 0; i < minionShieldList.Count; i++)
        {
            if (minionShieldList[i].shieldEnabled == true)
            {
                var r = minionShieldList[i].GetComponent<Renderer>();

                switch (mode)
                {
                    case 0:
                        r.sortingLayerID = SortingLayer.NameToID("Character");
                        r.sortingOrder = 1;
                        break;

                    case 1:
                        r.sortingLayerID = SortingLayer.NameToID("Skill_Field");
                        r.sortingOrder = 1;
                        break;

                    case 2:
                        r.sortingLayerID = SortingLayer.NameToID("Character_Invisible");
                        r.sortingOrder = 1;
                        break;
                }
            }
        }
    }
}