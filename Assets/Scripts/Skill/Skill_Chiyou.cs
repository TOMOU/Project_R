// ==================================================
// Skill_Chiyou.cs
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

public class Skill_Chiyou : SkillBase
{
    protected override void Prepare()
    {
        base.Prepare();

        if (_unit.SyncVertList == null)
        {
            _unit.SyncVertList = new List<bool>();
            _unit.SyncVertList.Add(true);  // 근접범위
            _unit.SyncVertList.Add(false);  // 버서커
            _unit.SyncVertList.Add(true);  // 광역
        }

        // Load Effect
        EffectManager.Singleton.OnCreateParticle("chiyou_attack");      // 평타
        EffectManager.Singleton.OnCreateParticle("chiyou_skill1");      // 스킬1 근접범위공격
        EffectManager.Singleton.OnCreateParticle("chiyou_skill2");      // 스킬2 버프
        EffectManager.Singleton.OnCreateParticle("chiyou_skill3");      // 스킬3 범위난무
    }

    public override IEnumerator coSkillNormal()
    {
        // 근거리 공격 로직 실행
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(true);

        PlayEffect("chiyou_attack", new Vector3(4f, 2f));

        // 데미지 총합 계산 후 전달
        SendDamage();

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_1()
    {
        // 공격 실행 (근접 범위기술)
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(true);

        PlayEffect("chiyou_skill1", new Vector3(4f, 2f));

        List<UnitInfo_Normal> list = null;
        if (NoRange == false)
            list = BattleManager.Singleton.FindUnitByHorizontal_Limit<UnitInfo_Normal>(_unit.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue, _unit.CurrentPosition, 6f, _unit.FSM.Skeleton.initialFlipX);
        else
        {
            list = new List<UnitInfo_Normal>();
            list.Add(_unit.Target);
        }

        for (int i = 0; i < list.Count; i++)
        {
            // 데미지 총합 후 전달
            SendDamage(list[i]);
        }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_2()
    {
        // 버프 실행 (방어하락, 공격증가)
        yield return new WaitForSpineEvent(_skel, "buff");

        //todo 사운드 연출 (버프)
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

        PlayEffect("chiyou_skill2", new Vector3(0f, 2f));

        _unit.Status.ApplyBuff(new UnitBuff(_unit.Status, Constant.SkillType.Debuff, Constant.SkillProperty.PhysicalDefence, -10f, 10f));
        _unit.Status.ApplyBuff(new UnitBuff(_unit.Status, Constant.SkillType.Buff, Constant.SkillProperty.PhysicalDamage, 20f, 10f));
        base.AddActivePoint();
    }

    public override IEnumerator coSkill_3()
    {
        List<UnitInfo_Normal> list = null;
        if (NoRange == false)
            list = BattleManager.Singleton.FindUnitByHorizontal_Limit<UnitInfo_Normal>(_unit.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue, _unit.CurrentPosition, 10f, _unit.FSM.Skeleton.initialFlipX);
        else
        {
            list = new List<UnitInfo_Normal>();
            list.Add(_unit.Target);
        }
        Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg(_unit, true, list, null));

        int cnt = 0;
        bool isEffectPlayed = false;

        while (cnt < 4)
        {
            yield return new WaitForSpineEvent(_skel, "attack");

            //todo 사운드 연출
            SoundManager.Singleton.PlayAttackSound(true);

            if (isEffectPlayed == false)
            {
                isEffectPlayed = true;
                PlayEffect("chiyou_skill3", new Vector3(4f, 2f));
            }

            for (int i = 0; i < list.Count; i++)
            {
                // 데미지 총합 후 전달
                SendDamage(list[i]);
            }

            cnt++;
        }

        base.AddActivePoint();
    }
}
