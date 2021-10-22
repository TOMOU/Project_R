// ==================================================
// Skill_J.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class Skill_J : SkillBase
{
    protected override void Prepare()
    {
        base.Prepare();

        if (_unit.SyncVertList == null)
        {
            _unit.SyncVertList = new List<bool>();
            _unit.SyncVertList.Add(false);  // J는 스킬이 범위X
            _unit.SyncVertList.Add(false);  // J는 스킬이 범위X
            _unit.SyncVertList.Add(false);   // J는 스킬이 범위X
        }

        // Load Effect
        EffectManager.Singleton.OnCreateParticle("j_skill1");

        // 상대방 5명분
        for (int i = 0; i < 5; i++)
        {
            EffectManager.Singleton.OnCreateParticle("j_skill3_1");
            EffectManager.Singleton.OnCreateParticle("j_skill3_2");
        }

        // if (_unit.Status.j_crossList == null)
        // {
        //     _unit.Status.j_crossList = new List<UnitInfo_Normal>();
        //     for (int i = 0; i < 5; i++)
        //     {
        //         var cross = SummonUnit("j_cross", new Vector3(30f, 0));
        //         cross.isSummonUnit = true;
        //         cross.owner = _unit;
        //         cross.isSummonUnit = true;
        //         cross.owner = _unit;
        //         cross.Status.hp = 0;
        //         cross.isDie = true;
        //         cross.IsLock = true;
        //         cross.FSM.Die();

        //         _unit.Status.j_crossList.Add(cross);
        //         BattleManager.Singleton.AddUnit(Constant.Team.None, cross);
        //     }
        // }
    }

    public override IEnumerator coSkillNormal()
    {
        // 근거리 공격 로직 실행
        yield return new WaitForSpineEvent(_skel, "attack");

        SoundManager.Singleton.PlayAttackSound(false);

        EffectManager.Singleton.OnPlayParticle(_unit.Target.transform.position + new Vector3(0f, 2.5f), "j_attack", _unit.FSM.Skeleton.skeleton.ScaleX, _unit.Rend.sortingLayerID, _unit.Rend.sortingOrder);

        // 데미지 총합 계산 후 전달
        SendDamage();

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_1()
    {
        // 디버프 대기
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출 (버프)
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

        EffectManager.Singleton.OnPlayParticle(Vector3.zero, "j_skill1", _unit.FSM.Skeleton.skeleton.ScaleX, _unit.Rend.sortingLayerID, _unit.Rend.sortingOrder);

        var list = _unit.Team == Constant.Team.Blue ? BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();

        for (int i = 0; i < list.Count; i++)
        {
            // 5초동안 실명상태
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Debuff, Constant.SkillProperty.Blind, 1f, 5f));

            // // ! TEST
            // list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Debuff, Constant.SkillProperty.Blind, 1f, 30f));
        }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_2()
    {
        yield return null;

        base.AddActivePoint();

        // // ! TEST
        // _unit.Status.mp = 1000;
    }

    public override IEnumerator coSkill_3()
    {
        // 디버프 대기
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출 (버프)
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

        var list = _unit.Team == Constant.Team.Blue ? BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();

        for (int i = 0; i < list.Count; i++)
        {
            // 5초동안 출혈상태(초당 퍼뎀)
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Debuff, Constant.SkillProperty.Bleed, 0.05f, 5f));
        }
    }

    private UnitInfo_Normal SummonUnit(string unitName, Vector3 summonPos)
    {
        var um = Model.First<UnitModel>();
        if (um == null)
        {
            Logger.LogError("Can't load UnitModel");
            return null;
        }

        var unit = um.unitTable.Find(e => e.unitName == unitName);

        GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", unitName));
        if (prefab == null)
        {
            Logger.LogErrorFormat("Can't load prefab in Character/{0}", unitName);
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab);

        obj.SetActive(false);

        obj.transform.SetParent(_unit.transform.parent);
        obj.transform.localPosition = summonPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one * 0.15f;

        obj.SetActive(true);

        SkeletonAnimation skel = obj.GetComponent<SkeletonAnimation>();
        if (skel == null)
        {
            Logger.LogErrorFormat("Can't find <SkeletonAnimation> Component in {0} object.", obj.name);
            return null;
        }

        if (_unit.Team == Constant.Team.Red)
        {
            skel.skeleton.ScaleX = -1f;
        }

        // Add UnitFSM
        UnitFSM fsm = obj.AddComponent<UnitFSM>();
        fsm.Init(skel, 0);

        UnitStatus status = new UnitStatus(unit, 0, 1, 1);

        // Add UnitInfo
        UnitInfo_Normal info = obj.AddComponent<UnitInfo_Normal>();
        info.Init(_unit.Team, status, fsm);
        info.slotIndex = _unit.slotIndex;
        info.isSummonUnit = true;

        info.isRaidLogic = _unit.isRaidLogic;

        SkillBase skill = obj.GetComponent<SkillBase>();
        skill.Init(info, skel);
        fsm.skill = skill;
        info.useSkill = skill.UseSkill;

        return info;
    }
}
