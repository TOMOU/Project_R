// ==================================================
// UnitFSM_R.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System;
using System.Collections;
using FSM;
using Spine.Unity;
using UnityEngine;

public class UnitFSM : StateBehaviour
{
    private SkeletonAnimation _skel;
    public SkeletonAnimation Skeleton { get { return _skel; } }
    public Spine.AnimationState state { get { return _skel.state; } }

    public Action cbIdle = null;
    public Action cbRun = null;
    public Action<float> cbAttack = null;
    public Action<float> cbSkill0 = null;
    public Action<float> cbSkill1 = null;
    public Action<float> cbSkill2 = null;
    public Action cbHit = null;
    public Action cbCondition = null;
    public Action cbVictory = null;
    public Action cbDie = null;

    public SkillBase skill;

    private int _battleMode = 0;

    /// <summary>
    /// FSM에서 애니메이션 재생할 스파인데이터 입력
    /// </summary>
    /// <param name="skeleton">스파인 데이터</param>
    public void Init(SkeletonAnimation skeleton, int battleMode)
    {
        this._skel = skeleton;
        this._battleMode = battleMode;

        Initialize<Constant.UnitState>();
        ChangeState(Constant.UnitState.Idle);
    }

    /// <summary>
    /// 현재 재생되는 애니메이션의 NormalizedTime을 확인
    /// </summary>
    /// <returns></returns>
    private float GetCurrentAnimationProgress()
    {
        Spine.TrackEntry track = _skel.AnimationState.GetCurrent(0);
        return (track.AnimationTime / track.Animation.Duration) * 100f;
    }

    #region State Function
    // ==================================================
    // Constant.UnitState.Idle
    // ==================================================
    public void Idle(bool force = false)
    {
        ChangeState(Constant.UnitState.Idle, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        ChangeState(Constant.UnitState.Idle, StateTransition.Overwrite);
    }

    private void Idle_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "idle", true);
    }

    private void Idle_Update()
    {
        if (cbIdle != null)
            cbIdle();
    }

    private void Idle_Exit()
    {

    }

    // ==================================================
    // Constant.UnitState.Run
    // ==================================================
    public void Run(bool force = false)
    {
        ChangeState(Constant.UnitState.Run, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private void Run_Enter()
    {
        if (_skel.skeleton.Data.Animations.Find(e => e.Name == "run") != null)
            _skel.AnimationState.SetAnimation(0, "run", true);
    }

    private void Run_Update()
    {
        if (cbRun != null)
            cbRun();
    }

    private void Run_Exit()
    {

    }
    // ==================================================
    // Constant.UnitState.Attack
    // ==================================================
    public void Attack(bool force = false)
    {
        ChangeState(Constant.UnitState.Attack, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private IEnumerator Attack_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "attack", false);

        yield return StartCoroutine(skill.coSkillNormal());

        while (_skel.AnimationState.GetCurrent(0).IsComplete == false)
            yield return null;
    }

    private void Attack_Update()
    {
        if (_skel.AnimationState.GetCurrent(0).IsComplete)
            Idle();
    }

    private void Attack_Exit()
    {
        StopCoroutine(skill.coSkillNormal());
    }
    // ==================================================
    // Constant.UnitState.Skill0
    // ==================================================
    public void Skill0(bool force = false)
    {
        ChangeState(Constant.UnitState.Skill0, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private IEnumerator Skill0_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "skill3", false);

        yield return StartCoroutine(skill.coSkill_3());

        while (_skel.AnimationState.GetCurrent(0).IsComplete == false)
            yield return null;
    }

    private void Skill0_Update()
    {
        // if (cbSkill0 != null)
        //     cbSkill0(GetCurrentAnimationProgress());

        if (_skel.AnimationState.GetCurrent(0).IsComplete)
            Idle();
    }

    private void Skill0_Exit()
    {
        if (_battleMode == 0)   // 일반
            Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg());
        // else if (_battleMode == 1)  // 디펜스
        //     Message.Send<Battle.Raid.SendUseSkillMsg>(new Battle.Raid.SendUseSkillMsg());
        else    // 레이드
            Message.Send<Battle.Raid.SendUseSkillMsg>(new Battle.Raid.SendUseSkillMsg());

        StopCoroutine(skill.coSkill_3());
    }
    // ==================================================
    // Constant.UnitState.Skill1
    // ==================================================
    public void Skill1(bool force = false)
    {
        ChangeState(Constant.UnitState.Skill1, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private IEnumerator Skill1_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "skill1", false);

        yield return StartCoroutine(skill.coSkill_1());

        while (_skel.AnimationState.GetCurrent(0).IsComplete == false)
            yield return null;
    }

    private void Skill1_Update()
    {
        // if (cbSkill1 != null)
        //     cbSkill1(GetCurrentAnimationProgress());

        if (_skel.AnimationState.GetCurrent(0).IsComplete)
            Idle();
    }

    private void Skill1_Exit()
    {
        StopCoroutine(skill.coSkill_1());
    }
    // ==================================================
    // Constant.UnitState.Skill2
    // ==================================================
    public void Skill2(bool force = false)
    {
        ChangeState(Constant.UnitState.Skill2, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private IEnumerator Skill2_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "skill2", false);

        yield return StartCoroutine(skill.coSkill_2());

        while (_skel.AnimationState.GetCurrent(0).IsComplete == false)
            yield return null;
    }

    private void Skill2_Update()
    {
        // if (cbSkill2 != null)
        //     cbSkill2(GetCurrentAnimationProgress());

        if (_skel.AnimationState.GetCurrent(0).IsComplete)
            Idle();
    }

    private void Skill2_Exit()
    {
        StopCoroutine(skill.coSkill_2());
    }
    // ==================================================
    // Constant.UnitState.Hit
    // ==================================================
    public void Hit(bool force = false)
    {
        ChangeState(Constant.UnitState.Hit, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private void Hit_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "hit", false);
    }

    private void Hit_Update()
    {
        if (cbHit != null)
            cbHit();

        if (_skel.AnimationState.GetCurrent(0).IsComplete)
            Idle();
    }

    private void Hit_Exit()
    {

    }
    // ==================================================
    // Constant.UnitState.Condition
    // ==================================================
    public void Condition(bool force = false)
    {
        ChangeState(Constant.UnitState.Condition, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private void Condition_Enter()
    {
        if (_skel.skeleton.Data.Animations.Find(e => e.Name == "condition") != null)
            _skel.AnimationState.SetAnimation(0, "condition", true);
        else
            _skel.AnimationState.SetAnimation(0, "hit", true);
    }

    private void Condition_Update()
    {
        if (cbCondition != null)
            cbCondition();
    }

    private void Condition_Exit()
    {

    }

    // ==================================================
    // Constant.UnitState.Victory
    // ==================================================
    public void Victory(bool force = false)
    {
        ChangeState(Constant.UnitState.Victory, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private void Victory_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "victory", false);
    }

    private void Victory_Update()
    {

    }

    private void Victory_Exit()
    {

    }
    // ==================================================
    // Constant.UnitState.Die
    // ==================================================
    public void Die(bool force = false)
    {
        ChangeState(Constant.UnitState.Die, force == true ? StateTransition.Overwrite : StateTransition.Safe);
    }

    private IEnumerator Die_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "die", false);

        while (_skel.AnimationState.GetCurrent(0).IsComplete == false)
            yield return null;

        // 2단 연출이 있는 경우
        if (_skel.skeleton.Data.Animations.Find(e => e.Name == "die2") != null)
            _skel.AnimationState.AddAnimation(0, "die2", true, 0);

        cbDie();
    }

    private void Die_Update()
    {
        // if (cbDie != null)
        //     cbDie();
    }

    private void Die_Exit()
    {

    }
    #endregion
}