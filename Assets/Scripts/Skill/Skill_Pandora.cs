// ==================================================
// Skill_Pandora.cs
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

public class Skill_Pandora : SkillBase
{
    public List<MissileInfo> skill3_missiles;

    protected override void Prepare()
    {
        base.Prepare();

        if (missileInfo == null)
        {
            // 빈 오브젝트 하위에 붙이기
            GameObject obj = new GameObject();
            obj.transform.SetParent(EffectManager.Singleton.transform);

            // 해당 오브젝트에 스크립트 붙여주고 캐싱
            missileInfo = obj.AddComponent<MissileInfo>();
            missileInfo.Init(1030311);

            EffectManager.Singleton.missileList.Add(obj);
        }

        skill3_missiles = new List<MissileInfo>();

        for (int i = 0; i < 9; i++)
        {
            // 빈 오브젝트 하위에 붙이기
            GameObject obj = new GameObject();
            obj.transform.SetParent(EffectManager.Singleton.transform);

            // 해당 오브젝트에 스크립트 붙여주고 캐싱
            skill3_missiles.Add(obj.AddComponent<MissileInfo>());
            skill3_missiles[i].Init(1030313);

            EffectManager.Singleton.missileList.Add(obj);
        }

        if (_unit.SyncVertList == null)
        {
            _unit.SyncVertList = new List<bool>();
            _unit.SyncVertList.Add(false);  // 전체
            _unit.SyncVertList.Add(false);  // 전체
            _unit.SyncVertList.Add(false);  // 전체
        }

        // Load Effect
        EffectManager.Singleton.OnCreateParticle("pandora_attack");
        EffectManager.Singleton.OnCreateParticle("pandora_skill1");
        EffectManager.Singleton.OnCreateParticle("pandora_skill2");
        EffectManager.Singleton.OnCreateParticle("pandora_skill3_1");
        EffectManager.Singleton.OnCreateParticle("pandora_skill3_2");
    }

    public override IEnumerator coSkillNormal()
    {
        // 원거리 평타 공격
        yield return new WaitForSpineEvent(_skel, "attack");

        if (missileInfo != null)
        {
            if (_unit.isRaidLogic == true)
                missileInfo.Line(_unit, _unit.Target.PartInfo.socketList[_unit.slotIndex].position, () =>
                {
                    //todo 사운드 연출
                    SoundManager.Singleton.PlayAttackSound(false);

                    SendDamage();
                });
            else
                missileInfo.Line(_unit, _unit.Target, () =>
                {
                    //todo 사운드 연출
                    SoundManager.Singleton.PlayAttackSound(false);

                    SendDamage();
                });
        }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_1()
    {
        yield return new WaitForSpineEvent(_skel, "skill2");
        PlayEffect("pandora_skill2", new Vector3(-0.6f, 1.7f));

        yield return new WaitForSpineEvent(_skel, "skill1");
        PlayEffect("pandora_skill1", new Vector3(3f, 1.5f));

        // 전체 물마공 버프
        yield return new WaitForSpineEvent(_skel, "buff");

        //todo 사운드 연출 (버프)
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

        var list = _unit.Team == Constant.Team.Blue ? BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Buff, Constant.SkillProperty.PhysicalDamage, 10f, 10f));
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Buff, Constant.SkillProperty.MagicalDamage, 10f, 10f));
        }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_2()
    {
        // 전체 마방 증가, 모상저100
        yield return new WaitForSpineEvent(_skel, "buff");

        //todo 사운드 연출 (버프)
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

        PlayEffect("pandora_skill2", new Vector3(0f, 4.2f));

        var list = _unit.Team == Constant.Team.Blue ? BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Buff, Constant.SkillProperty.MagicalDefence, 10f, 10f));
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Buff, Constant.SkillProperty.Immune, 10f, 10f));
        }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_3()
    {
        yield return new WaitForSeconds(0.1f);
        PlayEffect("pandora_skill3_1", new Vector3(0.5f, 3f));

        var list = (_unit.Team == Constant.Team.Blue ? BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>()).FindAll(e => e.isDie == false);
        Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg(_unit, true, list, null));

        // Skill3 애니 끝날때까지 대기
        while (_skel.AnimationState.GetCurrent(0).IsComplete == false)
            yield return null;

        // Skill3_2 재생
        _skel.AnimationState.SetAnimation(0, "skill3_2", true);

        for (int i = 0; i < skill3_missiles.Count; i++)
        {
            if (i + 1 <= list.Count)
            {
                if (list[i].Status.hp <= 0)
                    continue;

                int idx = i;
                skill3_missiles[i].Fixed(_unit, list[i].transform.position, () =>
                {
                    //todo 사운드 연출 (버프)
                    Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

                    Message.Send<Battle.Normal.SendHealMsg>(new Battle.Normal.SendHealMsg(list[idx], 300));
                    list[idx].Status.ApplyBuff(new UnitBuff(list[idx].Status, Constant.SkillType.Buff, Constant.SkillProperty.TP, 300f, 10f));
                });

                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return new WaitForSeconds(2f);

        // for (int i = 0; i < skill3_missiles.Count; i++)
        // {
        //     if (skill3_missiles[i].IsComplete == false)
        //         yield return null;
        // }

        base.AddActivePoint();
    }
}
