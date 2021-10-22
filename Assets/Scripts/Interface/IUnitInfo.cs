// ==================================================
// IUnitInfo.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IUnitInfo : MonoBehaviour
{
    /// <summary>
    /// 해당 유닛의 팀
    /// </summary>
    /// <value></value>
    public Constant.Team Team { get; private set; }
    /// <summary>
    /// 해당 유닛의 스테이터스
    /// </summary>
    /// <value></value>
    public UnitStatus Status { get; private set; }
    /// <summary>
    /// 해당 유닛의 FSM
    /// </summary>
    /// <value></value>
    public UnitFSM FSM { get; private set; }

    /// <summary>
    /// 이동을 하는 스킬을 사용중인지
    /// </summary>
    protected bool _isMoveState = false;
    protected Vector3 _moveOrigin;
    public Vector3 CurrentPosition
    {
        get
        {
            if (_isMoveState)
                return _moveOrigin;
            else
                return transform.localPosition;
        }
    }

    /// <summary>
    /// 공격이 종료되었는지 체크하는 bool값
    /// </summary>
    protected bool _isAttackComplete;
    /// <summary>
    /// 캐릭터가 죽었는지
    /// </summary>
    public bool isDie;
    /// <summary>
    /// 전체 행동이 잠금 처리되었는지 체크하는 bool값
    /// </summary>
    protected bool _isLock;

    /// <summary>
    /// UnitInfo 클래스 초기화
    /// </summary>
    /// <param name="team">해당 유닛의 팀</param>
    /// <param name="status">해당 유닛의 스테이터스</param>
    /// <param name="fsm">유닛에 붙여 줄 FSM</param>
    public virtual void Init(Constant.Team team, UnitStatus status, UnitFSM fsm)
    {
        // 잠금상태
        _isLock = true;

        // 기본정보 등록
        this.Team = team;
        this.Status = status;
        this.FSM = fsm;

        // FSM 콜백 등록
        this.FSM.cbIdle = OnIdle;
        this.FSM.cbRun = OnRun;
        this.FSM.cbAttack = OnAttack;
        this.FSM.cbSkill0 = OnSkill0;
        this.FSM.cbSkill1 = OnSkill1;
        this.FSM.cbSkill2 = OnSkill2;
        this.FSM.cbHit = OnHit;
        this.FSM.cbCondition = OnCondition;
        this.FSM.cbVictory = OnVictory;
        this.FSM.cbDie = OnDie;

        // Message 등록
        AddMessageListener();
    }

    /// <summary>
    /// 씬을 전환할 때 메모리 해제
    /// </summary>
    public virtual void Release()
    {
        _isLock = true;

        // FSM 콜백 등록해제
        this.FSM.cbIdle = null;
        this.FSM.cbRun = null;
        this.FSM.cbAttack = null;
        this.FSM.cbSkill0 = null;
        this.FSM.cbSkill1 = null;
        this.FSM.cbSkill2 = null;
        this.FSM.cbHit = null;
        this.FSM.cbCondition = null;
        this.FSM.cbVictory = null;
        this.FSM.cbDie = null;

        // Message 등록 해제
        RemoveMessageListener();
    }

    /// <summary>
    /// 메세지 등록 함수
    /// </summary>
    protected virtual void AddMessageListener() { }
    /// <summary>
    /// 메세지 해제 함수
    /// </summary>
    protected virtual void RemoveMessageListener() { }

    protected virtual void OnIdle() { }
    protected virtual void OnRun() { }
    protected virtual void OnAttack(float progress) { }
    protected virtual void OnSkill0(float progress) { }
    protected virtual void OnSkill1(float progress) { }
    protected virtual void OnSkill2(float progress) { }
    protected virtual void OnHit() { }
    protected virtual void OnCondition() { }
    protected virtual void OnVictory() { }
    protected virtual void OnDie() { }
    /// <summary>
    /// FSM State를 실행하는 도중 예외처리 상황 체크 용도
    /// </summary>
    /// <returns>true -> 진행해도 된다!!</returns>
    protected virtual bool CheckException() { return true; }
}
