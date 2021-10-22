// ==================================================
// Skill_Alexander.cs
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

public class Skill_Alexander : SkillBase
{
    public Transform laserRootBone;
    public Transform boosterRootBone;

    protected override void Prepare()
    {
        base.Prepare();

        if (_unit.SyncVertList == null)
        {
            _unit.SyncVertList = new List<bool>();
            _unit.SyncVertList.Add(false);  // 어그로
            _unit.SyncVertList.Add(true);  // 넉백
            _unit.SyncVertList.Add(false);  // 전체
        }

        // Load Effect
        EffectManager.Singleton.OnCreateParticle("alexander_attack1");      // 평타

        EffectManager.Singleton.OnCreateParticle("alexander_skill1");       // 스킬1 어그로

        EffectManager.Singleton.OnCreateParticle("alexander_booster");      // 스킬3 비행
        EffectManager.Singleton.OnCreateParticle("alexander_skill3");       // 스킬3 레이저
    }

    public override IEnumerator coSkillNormal()
    {
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(true);

        //todo 스킬이펙트 연출
        PlayEffect("alexander_attack1", new Vector3(1f, 3f));

        int cnt = 0;

        while (cnt < 2)
        {
            // 데미지 총합 계산 후 전달
            SendDamage();

            cnt++;

            if (cnt < 2)
                yield return new WaitForSpineEvent(_skel, "attack");
        }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_1()
    {
        // 버프 실행
        yield return new WaitForSpineEvent(_skel, "buff");

        //todo 사운드 연출 (버프)
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

        //todo 스킬이펙트 연출
        PlayEffect("alexander_skill1", new Vector3(0f, 2f));

        _unit.Status.ApplyBuff(new UnitBuff(_unit.Status, Constant.SkillType.Buff, Constant.SkillProperty.Aggro, 1f, 10f));

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_2()
    {
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(true);

        List<UnitInfo_Normal> list = null;
        if (NoRange == false)
            list = BattleManager.Singleton.FindUnitByHorizontal_Limit<UnitInfo_Normal>(_unit.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue, _unit.CurrentPosition, 12f, _unit.FSM.Skeleton.initialFlipX);
        else
        {
            list = new List<UnitInfo_Normal>();
            list.Add(_unit.Target);
        }

        for (int i = 0; i < list.Count; i++)
        {
            // 데미지 총합 후 전달
            SendDamage(list[i]);

            // 타겟에 넉백 전달
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Debuff, Constant.SkillProperty.KnockBack, _skel.skeleton.ScaleX, 1f));
        }

        // int cnt = 0;

        // while (cnt < 3)
        // {
        //     // 공격 실행 (돌진공격)
        //     yield return new WaitForSpineEvent(_skel, "attack");
        //     Logger.LogFormat("알렉산더 스킬2 - 직선 돌진{0}", cnt);
        //     cnt++;

        //     yield return null;
        // }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_3()
    {
        var list = (_unit.Team == Constant.Team.Red ? BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>()).FindAll(e => e.isDie == false);
        Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg(_unit, true, list, null));

        yield return new WaitForSpineEvent(_skel, "start1");

        //todo 스킬이펙트 연출
        EffectManager.Singleton.OnParticleAttach("alexander_booster", boosterRootBone, true, 2f, _unit.FSM.Skeleton.skeleton.ScaleX, _unit.Rend.sortingLayerID, _unit.Rend.sortingOrder);

        Vector3 p1 = _unit.transform.localPosition;
        Vector3 p2 = p1 + new Vector3(0, 20f, 20f);
        Vector3 p3 = new Vector3(_unit.Team == Constant.Team.Blue ? -22f : 22f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(_unit.Team == Constant.Team.Blue ? -13f : 13f, 1.5f, 1.5f);

        float t = 0f;

        // 상단으로 이동
        while (t < 1f)
        {
            t += Time.deltaTime * 2.5f;
            _unit.transform.localPosition = Vector3.Lerp(p1, p2, t);
            yield return null;
        }

        _unit.FSM.Skeleton.skeleton.ScaleX = _unit.Team == Constant.Team.Red ? -1f : 1f;

        EffectManager.Singleton.OnParticleAttach("alexander_booster", boosterRootBone, false, 2f, _unit.FSM.Skeleton.skeleton.ScaleX, _unit.Rend.sortingLayerID, _unit.Rend.sortingOrder);

        // 화면 좌측에서 등장
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2.5f;
            _unit.transform.localPosition = Vector3.Lerp(p3, p4, t);
            yield return null;
        }

        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_2));

        //todo 스킬이펙트 연출
        EffectManager.Singleton.OnParticleAttach("alexander_skill3", laserRootBone, true, 1f, _unit.FSM.Skeleton.skeleton.ScaleX, _unit.Rend.sortingLayerID, _unit.Rend.sortingOrder);

        yield return new WaitForSeconds(1.2f);

        for (int i = 0; i < list.Count; i++)
        {
            // 데미지 총합 후 전달
            SendDamage(list[i]);
        }

        yield return new WaitForSpineEvent(_skel, "start2");

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            _unit.transform.localPosition = Vector3.Lerp(p4, p1, t);
            yield return null;
        }

        base.AddActivePoint();
    }
}
