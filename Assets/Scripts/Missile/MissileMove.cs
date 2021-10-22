// ==================================================
// MissileMove.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System;

public class MissileMove
{
    public bool Active { get; private set; }
    public Constant.MissileMoveType MoveType { get; private set; }
    private Vector3 _origin;
    private IUnitInfo _targetUnit;
    private Vector3 _targetVec;
    private MissileInfo _self;
    private float _moveSpeed;
    private float _activeTime;
    private Action _arrivedCallback;

    public bool isLock;

    public MissileMove(MissileInfo info)
    {
        this._self = info;

        Message.AddListener<Battle.Normal.PlayUseSkillMsg>(OnPlayUseSkill);
        Message.AddListener<Battle.Normal.SendUseSkillMsg>(OnSendUseSkill);
    }

    private void OnDestroy()
    {
        _targetUnit = null;
        _self = null;

        Message.RemoveListener<Battle.Normal.PlayUseSkillMsg>(OnPlayUseSkill);
        Message.RemoveListener<Battle.Normal.SendUseSkillMsg>(OnSendUseSkill);
    }

    public void InitLine(IUnitInfo spawner, IUnitInfo target, float moveSpeed)
    {
        float direction = spawner.FSM.Skeleton.skeleton.ScaleX;
        _origin = spawner.transform.position + new Vector3(direction, 2f);

        this._targetUnit = target;
        this._moveSpeed = moveSpeed;

        MoveType = Constant.MissileMoveType.Line;

        Active = true;
    }

    public void InitLine_Raid(IUnitInfo spawner, Vector3 target, float moveSpeed)
    {
        float direction = spawner.FSM.Skeleton.skeleton.ScaleX;
        _origin = spawner.transform.position + new Vector3(direction, 2f);

        this._targetVec = target;
        this._moveSpeed = moveSpeed;

        MoveType = Constant.MissileMoveType.Line_Raid;

        Active = true;
    }

    public void Init(IUnitInfo spawner, IUnitInfo target, float moveSpeed)
    {
        _origin = spawner.transform.position;

        this._targetUnit = target;
        this._moveSpeed = moveSpeed;

        MoveType = Constant.MissileMoveType.Follow;

        Active = true;
    }

    public void Init(Vector3 target, float moveSpeed)
    {
        this._targetVec = target;
        this._moveSpeed = moveSpeed;

        MoveType = Constant.MissileMoveType.Move;

        Active = true;
    }

    public void Init(float time, float max)
    {
        this._moveSpeed = time;
        MoveType = Constant.MissileMoveType.Fixed;
        Active = true;
    }

    public void Refresh(float deltaTime)
    {
        if (isLock == true)
            return;

        if (Active == false)
        {
            _arrivedCallback = null;
            return;
        }

        switch (MoveType)
        {
            // 지정 타겟 추적 (타겟형)
            case Constant.MissileMoveType.Follow:
                UpdateFollow(deltaTime);
                break;

            // 지정 위치 추적 (범위형)
            case Constant.MissileMoveType.Move:
                UpdateMove(deltaTime);
                break;

            // 위치 고정 (장판형)
            case Constant.MissileMoveType.Fixed:
                UpdateFixed(deltaTime);
                break;

            case Constant.MissileMoveType.Line:
                UpdateLine(deltaTime);
                break;

            case Constant.MissileMoveType.Line_Raid:
                UpdateLine_Raid(deltaTime);
                break;
        }
    }

    private void OnPlayUseSkill(Battle.Normal.PlayUseSkillMsg msg)
    {
        isLock = true;
    }

    private void OnSendUseSkill(Battle.Normal.SendUseSkillMsg msg)
    {
        if (msg.isEnter == false && msg.targetList == null)
        {
            isLock = false;
        }
    }

    private float _time = 0f;
    private void UpdateFollow(float delta)
    {
        // 타겟이 없는 경우 건너뛴다.
        if (_targetUnit == null)
            return;

        // // 방향벡터를 생성해 준다.
        // Vector3 dir = _targetUnit.transform.position - _self.transform.position;

        // 지정 타겟과의 거리가 가깝다. 효과를 발동시킨다.
        if ((_targetUnit.transform.position - _self.transform.position).sqrMagnitude <= 1f)
        {
            if (_arrivedCallback != null)
            {
                _arrivedCallback();
                _arrivedCallback = null;
            }

            Active = false;
            return;
        }

        _time += delta;
        _self.transform.position = MoveBezier();

        // // 방향벡터 노말라이즈
        // dir.Normalize();

        // // 방향벡터가 0면 무시
        // if (dir == Vector3.zero)
        //     return;

        // // 타겟의 방향을 바라보고
        // // _self.transform.rotation = Quaternion.LookRotation(dir);

        // // 타겟을 향해 이동시켜준다.
        // _self.transform.position += dir * _moveSpeed * delta;
    }

    private void UpdateMove(float delta)
    {
        // 방향벡터를 생성해 준다.
        Vector3 dir = _targetVec - _self.transform.position;

        // 지정 타겟과의 거리가 가깝다. 효과를 발동시킨다.
        if (dir.sqrMagnitude <= 1f)
        {
            if (_arrivedCallback != null)
            {
                _arrivedCallback();
                _arrivedCallback = null;
            }

            Active = false;
            return;
        }

        // 방향벡터 노말라이즈
        dir.Normalize();

        // 방향벡터가 0면 무시
        if (dir == Vector3.zero)
            return;

        // 타겟의 방향을 바라보고
        _self.transform.rotation = Quaternion.LookRotation(dir);

        // 타겟을 향해 이동시켜준다.
        _self.transform.position += dir * _moveSpeed * delta;
    }

    private void UpdateFixed(float delta)
    {
        if (Active == false)
            return;

        // Not to do
        _time += Time.deltaTime;

        if (_time >= _activeTime)
        {
            if (_arrivedCallback != null)
            {
                _arrivedCallback();
                _arrivedCallback = null;
            }
        }

        if (_time >= _moveSpeed)
        {
            Active = false;
            _self.SetOffMissile();
            return;
        }
    }

    private void UpdateLine(float delta)
    {
        /// 타겟이 없는 경우 건너뛴다.
        if (_targetUnit == null)
            return;

        Vector3 dest = _targetUnit.transform.position + new Vector3(_targetUnit.FSM.Skeleton.skeleton.ScaleX, 2f);

        // 방향벡터를 생성해 준다.
        Vector3 dir = dest - _self.transform.position;

        // 방향벡터 노말라이즈
        dir.Normalize();

        // 지정 타겟과의 거리가 가깝다. 효과를 발동시킨다.
        if ((dest - _self.transform.position).sqrMagnitude <= 1f)
        {
            if (_arrivedCallback != null)
            {
                _arrivedCallback();
                _arrivedCallback = null;
            }

            Active = false;
            _self.SetOffMissile();
            return;
        }

        _time += delta;
        // 타겟을 향해 이동시켜준다.
        _self.transform.position += dir * _moveSpeed * delta;
    }

    private void UpdateLine_Raid(float delta)
    {
        Vector3 dest = _targetVec;

        // 방향벡터를 생성해 준다.
        Vector3 dir = dest - _self.transform.position;

        // 방향벡터 노말라이즈
        dir.Normalize();

        // 지정 타겟과의 거리가 가깝다. 효과를 발동시킨다.
        if ((dest - _self.transform.position).sqrMagnitude <= 1f)
        {
            if (_arrivedCallback != null)
            {
                _arrivedCallback();
                _arrivedCallback = null;
            }

            Active = false;
            _self.SetOffMissile();
            return;
        }

        _time += delta;
        // 타겟을 향해 이동시켜준다.
        _self.transform.position += dir * _moveSpeed * delta;
    }

    public void AddCallback(Action callback)
    {
        _arrivedCallback = callback;
    }

    private Vector3 MoveBezier()
    {
        return BattleCalc.CalculateCubicBezierPoint(_time, _origin, (_targetUnit.transform.position + _self.transform.position) * 0.5f + Vector3.up * 5f, _targetUnit.transform.position);
    }
}
