// ==================================================
// Skill_Jinshi.cs
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

public class Skill_Jinshi : SkillBase
{
    protected override void Prepare()
    {
        base.Prepare();

        if (_unit.SyncVertList == null)
        {
            _unit.SyncVertList = new List<bool>();
            _unit.SyncVertList.Add(false);  // 소환
            _unit.SyncVertList.Add(false);  // 소환
            _unit.SyncVertList.Add(true);  // 범위석화
        }

        // 평타용 검마용 캐싱
        if (_unit.Status.minionSword == null)
        {
            _unit.Status.minionSword = SummonSword(new Vector3(-30f, 0f));
        }

        // 스킬1 창마용
        if (_unit.Status.minionSpear == null)
        {
            _unit.Status.minionSpear = SummonUnit("jinshi_minion_spear", new Vector3(-30f, 0f));
            _unit.Status.minionSpear.isSummonUnit = true;
            _unit.Status.minionSpear.owner = _unit;

            // 미리 죽은상태로 설정
            _unit.Status.minionSpear.Status.hp = 0;
            _unit.Status.minionSpear.isDie = true;
            _unit.Status.minionSpear.IsLock = true;
            _unit.Status.minionSpear.FSM.Die();

            BattleManager.Singleton.AddUnit(Constant.Team.None, _unit.Status.minionSpear);
        }

        // 스킬2 방마용
        if (_unit.Status.minionShieldList == null)
            _unit.Status.minionShieldList = new List<ShieldFSM>();

        if (_unit.Status.minionSpear_AP == null)
        {
            _unit.Status.minionSpear_AP = new List<UnitInfo_Normal>();
            for (int i = 0; i < 5; i++)
            {
                var minion = SummonUnit("jinshi_minion_spear", new Vector3(-30f, 0f));
                minion.isSummonUnit = true;
                minion.owner = _unit;
                minion.Status.hp = 0;
                minion.isDie = true;
                minion.IsLock = true;
                minion.FSM.Die();

                _unit.Status.minionSpear_AP.Add(minion);
                BattleManager.Singleton.AddUnit(Constant.Team.None, minion);
            }
        }

        // 방패병 3마리 먼저 소환
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = Vector3.zero;

            switch (i + 1)
            {
                case 1:
                    pos = new Vector3(_unit.FSM.Skeleton.skeleton.ScaleX > 0 ? 8f : -8f, 0f, -0.1f);
                    break;

                case 2:
                    pos = new Vector3(_unit.FSM.Skeleton.skeleton.ScaleX > 0 ? 6f : -6f, 3f, 0.1f);
                    break;

                case 3:
                    pos = new Vector3(_unit.FSM.Skeleton.skeleton.ScaleX > 0 ? 5f : -5f, 0f, -0.2f);
                    break;
            }

            var minion = SummonShield(pos);
            _unit.Status.minionShieldList.Add(minion);
        }

        // Load Effect
        EffectManager.Singleton.OnCreateParticle("jinshi_smoke");
        EffectManager.Singleton.OnCreateParticle("jinshi_skill3");
    }

    public override IEnumerator coSkillNormal()
    {
        // 원거리 공격 로직 실행
        yield return new WaitForSpineEvent(_skel, "minion");

        // 타겟과 나 사이의 앞에 검마용 소환
        _unit.Status.minionSword.Attack(_unit, _unit.Target);

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_1()
    {
        // 창마용 소환
        yield return new WaitForSpineEvent(_skel, "minion");

        _unit.Status.ApplyBuff(new UnitBuff(_unit.Status, Constant.SkillType.Buff, Constant.SkillProperty.MinionSpear, 1f, 100f));

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_2()
    {
        // 방마용 소환
        yield return new WaitForSpineEvent(_skel, "minion");

        _unit.Status.ApplyBuff(new UnitBuff(_unit.Status, Constant.SkillType.Buff, Constant.SkillProperty.MinionShield, 1f, 100f));

        base.AddActivePoint();
    }

    public override IEnumerator coSkill_3()
    {
        var list = BattleManager.Singleton.FindUnitByArea<UnitInfo_Normal>(_unit.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue, _unit.Target.CurrentPosition, 5f);
        Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg(_unit, true, list, null));

        // 석화 & 검마용 소환
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Magic_Skill_2));

        EffectManager.Singleton.OnPlayParticle(_unit.Target.transform.position, "jinshi_skill3", _unit.FSM.Skeleton.skeleton.ScaleX, _unit.Rend.sortingLayerID, _unit.Rend.sortingOrder);

        for (int i = 0; i < list.Count; i++)
        {
            list[i].Status.ApplyBuff(new UnitBuff(list[i].Status, Constant.SkillType.Debuff, Constant.SkillProperty.Stun, 1f, 10f));

            if (list[i].Status.isStun == true)
            {
                var minion = _unit.Status.minionSpear_AP[i];
                if (minion != null)
                {
                    minion.transform.localPosition = list[i].transform.localPosition + new Vector3(list[i].FSM.Skeleton.skeleton.ScaleX > 0 ? 3f : -3f, 0f, 0f);
                    minion.Status.hp = minion.Status.hpFull;
                    minion.followAggro = true;
                    minion.Target = list[i];
                    minion.isDie = false;
                    minion.FSM.Idle();

                    var r = minion.GetComponent<Renderer>();
                    r.sortingLayerID = SortingLayer.NameToID("Skill_Field");
                    r.sortingOrder = 1;
                }
            }
        }

        yield return new WaitForSeconds(1f);

        base.AddActivePoint();
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

    private ShieldFSM SummonShield(Vector3 summonPos)
    {
        var um = Model.First<UnitModel>();
        if (um == null)
        {
            Logger.LogError("Can't load UnitModel");
            return null;
        }

        string unitName = "jinshi_minion_shield";

        var unit = um.unitTable.Find(e => e.unitName == unitName);

        GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", unitName));
        if (prefab == null)
        {
            Logger.LogErrorFormat("Can't load prefab in Character/{0}", unitName);
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab);

        obj.SetActive(false);

        obj.transform.SetParent(_unit.transform);
        obj.transform.localPosition = summonPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one * 0.5f;

        // obj.SetActive(true);

        SkeletonAnimation skel = obj.GetComponent<SkeletonAnimation>();
        if (skel == null)
        {
            Logger.LogErrorFormat("Can't find <SkeletonAnimation> Component in {0} object.", obj.name);
            return null;
        }

        if (_unit.Team == Constant.Team.Red)
        {
            skel.skeleton.ScaleX = _unit.FSM.Skeleton.skeleton.ScaleX;
        }

        // Add UnitFSM
        ShieldFSM fsm = obj.AddComponent<ShieldFSM>();
        fsm.Init(skel);

        return fsm;
    }

    private SwordFSM SummonSword(Vector3 summonPos)
    {
        var um = Model.First<UnitModel>();
        if (um == null)
        {
            Logger.LogError("Can't load UnitModel");
            return null;
        }

        string unitName = "jinshi_minion_sword";

        var unit = um.unitTable.Find(e => e.unitName == unitName);

        GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", unitName));
        if (prefab == null)
        {
            Logger.LogErrorFormat("Can't load prefab in Character/{0}", unitName);
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab);

        obj.SetActive(false);

        obj.transform.SetParent(_unit.transform);
        obj.transform.localPosition = summonPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one * 0.5f;

        // obj.SetActive(true);

        SkeletonAnimation skel = obj.GetComponent<SkeletonAnimation>();
        if (skel == null)
        {
            Logger.LogErrorFormat("Can't find <SkeletonAnimation> Component in {0} object.", obj.name);
            return null;
        }

        if (_unit.Team == Constant.Team.Red)
        {
            skel.skeleton.ScaleX = _unit.FSM.Skeleton.skeleton.ScaleX;
        }

        // Add UnitFSM
        SwordFSM fsm = obj.AddComponent<SwordFSM>();
        fsm.Init(skel);

        return fsm;
    }
}
