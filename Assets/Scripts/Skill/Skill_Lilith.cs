// ==================================================
// Skill_Lilith.cs
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

public class Skill_Lilith : SkillBase
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
            missileInfo.Init(1002111);

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
            skill3_missiles[i].Init(1002113);

            EffectManager.Singleton.missileList.Add(obj);
        }

        if (_unit.SyncVertList == null)
        {
            _unit.SyncVertList = new List<bool>();
            _unit.SyncVertList.Add(false);  // 버프
            _unit.SyncVertList.Add(true);  // 직선
            _unit.SyncVertList.Add(false);  // 범위
        }

        // Load Effect
        EffectManager.Singleton.OnCreateParticle("lillith_skull");
        EffectManager.Singleton.OnCreateParticle("lillith_swing");
        EffectManager.Singleton.OnCreateParticle("lillith_skill2");
        EffectManager.Singleton.OnCreateParticle("lillith_skill3_1");

        for (int i = 0; i < 9; i++)
        {
            EffectManager.Singleton.OnCreateParticle("lillith_skill3_2");
        }
    }

    public override IEnumerator coSkillNormal()
    {
        yield return new WaitForSpineEvent(_skel, "attack");

        if (missileInfo != null)
        {
            PlayEffect("lillith_swing", new Vector3(0f, 2f));

            if (_unit.isRaidLogic == true)
                missileInfo.Line(_unit, _unit.Target.PartInfo.socketList[_unit.slotIndex].position, () =>
                {
                    //todo 사운드 연출
                    SoundManager.Singleton.PlayAttackSound(false);

                    SendDamage();
                });
            else
                missileInfo.Line(_unit, _unit.Target, SendDamage);
        }

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_1()
    {
        yield return new WaitForSpineEvent(_skel, "buff");

        //todo 사운드 연출 (버프)
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_1));

        _unit.Status.ApplyBuff(new UnitBuff(_unit.Status, Constant.SkillType.Buff, Constant.SkillProperty.MagicalDamage, 10f, 10f));

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_2()
    {
        // 공격 실행 (직선 레이저)
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(false);

        PlayEffect("lillith_skill2", new Vector3(2f, 2f));

        // 자기 앞쪽의 직선상 캐릭터의 리스트 생성
        List<UnitInfo_Normal> list = null;
        if (NoRange == false)
            list = BattleManager.Singleton.FindUnitByHorizontal<UnitInfo_Normal>(_unit.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue, _unit.CurrentPosition, _unit.FSM.Skeleton.initialFlipX);
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

    public override IEnumerator coSkill_3()
    {
        var showList = (_unit.Team == Constant.Team.Blue ? BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>()).FindAll(e => e.isDie == false);
        Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg(_unit, true, showList, null));

        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(false);

        PlayEffect("lillith_skill3_1", new Vector3(0f, 0f));

        yield return new WaitForSeconds(1f);

        // Target 중점으로 9cell 범위 구분
        Vector3 vec = _unit.Target.CurrentPosition;
        if (_unit.isRaidLogic == true)
        {
            vec = _unit.Target.PartInfo.socketList[_unit.slotIndex].position;
        }
        List<Vector2> vList = new List<Vector2>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                vList.Add(new Vector2(vec.x - 6f * (i - 1), vec.y - 3f * (j - 1)));
            }
        }
        Vector2 sSize = new Vector2(6f, 3f);

        // 공격 실행 (범위형 스킬)
        for (int i = 0; i < 9; i++)
        {
            Vector2 ranVec = vList[Random.Range(0, vList.Count)];
            vList.Remove(ranVec);

            // 해당 위치의 유닛 가져오기
            var list = BattleManager.Singleton.FindUnitByArea_Square<UnitInfo_Normal>(_unit.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue, ranVec, sSize);

            // 설치형 미사일 발사
            skill3_missiles[i].Fixed(_unit, ranVec, () =>
            {
                // for (int j = 0; i < list.Count; j++)
                // {
                //     // 데미지 총합 후 전달
                //     SendDamage(list[j]);

                //     Logger.LogWarningFormat("{0}", list[j].Status.unitName_Kor);
                // }

                //todo 사운드 연출
                SoundManager.Singleton.PlayAttackSound(false);

                list.ForEach(e =>
                {
                    SendDamage(e);
                });
            });

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(2f);

        base.AddActivePoint();
    }
}
