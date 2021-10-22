// ==================================================
// UnitInfo_Raid.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo_Raid : IUnitInfo
{
    private Renderer _renderer;
    private int _baseSortingOrder;

    /// <summary>
    /// 팀에서의 내 인덱스번호
    /// </summary>
    private int _teamIndex;
    /// <summary>
    /// 전투시작 시 위치해야할 Vector
    /// </summary>
    private Vector3 _basePosition;
    private UnitInfo_Raid _target;
    private List<UnitInfo_Raid> _targets;
    /// <summary>
    /// 공격 페이즈 인덱스
    /// </summary>
    private int _attackPhaseIndex = 0;
    /// <summary>
    /// 공격 딜레이 타임
    /// </summary>
    private float _delayTime;

    /// <summary>
    /// UnitInfo 내에서 코루틴을 담당하는 변수.
    /// </summary>
    private Coroutine _coroutine = null;

    /// <summary>
    /// 어떠한 공격동작을 했을 때 얻는 MP양
    /// </summary>
    private const int mpAdd_Action = 90;
    /// <summary>
    /// 타격을 받을 때 얻는 MP양
    /// </summary>
    private const int mpAdd_Hit = 50;
    /// <summary>
    /// 킬을 했을 때 얻는 MP양
    /// </summary>
    private const int mpAdd_Kill = 200;

    /// <summary>
    /// UnitInfo 초기화
    /// </summary>
    /// <param name="team">My Team</param>
    /// <param name="status">My Status</param>
    /// <param name="fsm">My FSM</param>
    public override void Init(Constant.Team team, UnitStatus status, UnitFSM fsm)
    {
        base.Init(team, status, fsm);

        base.Status.InitUnitInfo(this);

        this._renderer = GetComponent<Renderer>();

        // Add Animation Key Event.
        fsm.state.Event += OnAnimationKeyEvent;
    }

    public override void Release()
    {
        base.Release();

        _renderer = null;
        _target = null;
        if (_targets != null)
        {
            _targets.Clear();
            _targets = null;
        }

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    protected override void AddMessageListener()
    {
        Message.AddListener<Battle.Raid.SendUseSkillMsg>(OnSendUseSkill);
        Message.AddListener<Battle.Raid.SendImDyingMsg>(OnImDying);
        Message.AddListener<Battle.Raid.MoveToBaseMsg>(OnMoveToBase);
        Message.AddListener<Battle.Raid.StartBattleMsg>(OnStartBattle);
        Message.AddListener<Battle.Raid.MoveToNextMsg>(OnMoveToNext);
        Message.AddListener<Battle.Raid.MoveToEndMsg>(OnMoveToEnd);
    }

    protected override void RemoveMessageListener()
    {
        Message.RemoveListener<Battle.Raid.SendUseSkillMsg>(OnSendUseSkill);
        Message.RemoveListener<Battle.Raid.SendImDyingMsg>(OnImDying);
        Message.RemoveListener<Battle.Raid.MoveToBaseMsg>(OnMoveToBase);
        Message.RemoveListener<Battle.Raid.StartBattleMsg>(OnStartBattle);
        Message.RemoveListener<Battle.Raid.MoveToNextMsg>(OnMoveToNext);
        Message.RemoveListener<Battle.Raid.MoveToEndMsg>(OnMoveToEnd);
    }

    public void InitTeamIndex(int idx)
    {
        _teamIndex = idx;
    }

    /// <summary>
    /// 현재의 위치로 기본 위치를 지정한다
    /// </summary>
    public void InitBasePosition()
    {
        this._basePosition = transform.localPosition;
    }

    public void InitBaseSortingOrder(int order)
    {
        _renderer.sortingLayerID = SortingLayer.NameToID("Character");
        this._baseSortingOrder = order;
        _renderer.sortingOrder = _baseSortingOrder;
    }

    /// <summary>
    /// 테스트용 함수. 캐릭터의 체력을 99999로 만든다.
    /// </summary>
    public void CheatStatus()
    {
        Status.hp *= 3;
        Status.hpFull *= 3;
    }

    /// <summary>
    /// 캐릭터를 부활시킨다
    /// </summary>
    /// <param name="isLock">잠금상태로 부활시킬지 (게임종료 후 결과연출 등에서)</param>
    public void Revive(bool isLock = true)
    {
        _isLock = isLock;
        isDie = false;
        Status.Reset();
        FSM.Reset();
    }

    private void FixedUpdate()
    {
        Status.Refresh(_isLock);
    }

    /// <summary>
    /// 애니메이션에 키값이 있으면 아래의 로직을 실행
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="e"></param>
    private void OnAnimationKeyEvent(Spine.TrackEntry entry, Spine.Event e)
    {
        if (e.Data.Name == "move") // 특수 스킬에 의한 위치 순간이동
        {
            if (_isMoveState == false)
            {
                float delta = (Team == Constant.Team.Blue ? -2.8f : 2.8f);
                _moveOrigin = transform.localPosition;

                if (_targets != null && _targets.Count > 0)
                {
                    transform.localPosition = new Vector3(_targets[0].transform.localPosition.x + delta, -4f, 0f);
                }
                else if (_target != null)
                {
                    transform.localPosition = new Vector3(_target.transform.localPosition.x + delta, _target.transform.localPosition.y, 0f);
                }
            }
            else
            {
                transform.localPosition = _moveOrigin;
            }

            _isMoveState = !_isMoveState;
        }
        else if (e.Data.Name == "stateStop") // 특수 상태이상에 의한 행동정지 (Stun, Knockback, ...)
        {
            if (Status.isStun)
                FSM.state.GetCurrent(0).TimeScale = 0f;
            else if (Status.isKnockback)
                FSM.state.GetCurrent(0).TimeScale = 0.5f;
            else
                FSM.state.GetCurrent(0).TimeScale = 1f;
        }
    }

    /// <summary>
    /// 유닛이 궁스킬을 쓰면 영상재생 연출
    /// </summary>
    /// <param name="msg"></param>
    private void OnSendUseSkill(Battle.Raid.SendUseSkillMsg msg)
    {
        // 아무 데이터가 안들어가있다. 궁스킬이 종료된 것!!
        if (msg.idx == 0 && msg.isEnter == false && msg.targetList == null)
        {
            _isLock = false;
            _renderer.sortingLayerID = SortingLayer.NameToID("Character");
            _renderer.sortingOrder = _baseSortingOrder;

            if (_targets != null && _targets.Count > 0)
                _targets.Clear();

            return;
        }

        // 죽었으면 제외한다.
        if (isDie || Status.hp <= 0)
            return;

        // 여기까지 왔으면 일단 모든 State를 잠가준다.
        _isLock = msg.isEnter;

        // 스킬 사용하는 캐릭터를 음영 위로 출력한다.
        if (msg.idx == Status.idx)
        {
            _renderer.sortingLayerID = SortingLayer.NameToID("Character");
            // _renderer.sortingOrder = _baseSortingOrder + 10;
            return;
        }
        // 타겟 캐릭터들을 음영 위로 출력한다.
        else if (msg.targetList != null && msg.targetList.Contains(this))
        {
            _renderer.sortingLayerID = SortingLayer.NameToID("Character");
            // _renderer.sortingOrder = _baseSortingOrder + 10;
        }
        // 그 외의 캐릭터는 원래의 sortingOrder대로 출력한다.
        else
        {
            _renderer.sortingLayerID = SortingLayer.NameToID("Character_SkillBlock");
            // _renderer.sortingOrder = _baseSortingOrder;
        }

        // 모션 강제고정 상태를 제외하고 대기상태로 전환한다.
        if (Status.isStun == false && Status.isKnockback == false)
            FSM.Idle();

        // 이동스킬 사용중인 캐릭터들은 원래의 위치로 다시 이동시킨다.
        if (_isMoveState)
        {
            _isMoveState = false;
            transform.localPosition = _moveOrigin;
        }
    }

    /// <summary>
    /// 캐릭터를 죽은 상태로 전환
    /// </summary>
    /// <param name="msg"></param>
    private void OnImDying(Battle.Raid.SendImDyingMsg msg)
    {
        if (Team == msg.info.Team && this == msg.info)
        {
            Status.hp = 0;
            isDie = true;
            FSM.Die();

            Status.AllEffectOff();
        }
    }

    #region BattleLogic과의 상호작용 함수들
    /// <summary>
    /// MessageListener: 전투 시작지점으로 이동
    /// </summary>
    /// <param name="msg"></param>
    private void OnMoveToBase(Battle.Raid.MoveToBaseMsg msg)
    {
        _isLock = true;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (isDie == true)
            return;

        _coroutine = StartCoroutine(coMoveToBase());
    }

    /// <summary>
    /// 전투시작을 위한 화면 밖 -> 지정 위치로의 이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator coMoveToBase()
    {
        float t = 0f;
        Vector3 vec = transform.localPosition;

        FSM.Run();

        // 1초동안 화면 밖에서 지정 위치로 이동.
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(vec, _basePosition, t);
            yield return null;
        }

        FSM.Idle();

        yield return new WaitForSeconds(1f);

        // 이동이 완료되었다고 BattleLogic에 통보.
        Message.Send<Battle.Raid.MoveToBaseCompleteMsg>(new Battle.Raid.MoveToBaseCompleteMsg());
    }

    /// <summary>
    /// MessageListener: 이동이 완료되었으니 전투를 시작
    /// </summary>
    /// <param name="msg"></param>
    private void OnStartBattle(Battle.Raid.StartBattleMsg msg)
    {
        _isLock = false;

        if (isDie)
            return;

        FSM.Idle();
    }

    /// <summary>
    /// MessageListener: 현재 전투가 끝났으니 다음 맵으로 이동
    /// </summary>
    /// <param name="msg"></param>
    private void OnMoveToNext(Battle.Raid.MoveToNextMsg msg)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (isDie == true)
            return;

        Status.AllEffectOff();

        _coroutine = StartCoroutine(coMoveToNext());
    }

    /// <summary>
    /// 다음 맵으로의 이동을 위한 화면 밖으로의 이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator coMoveToNext()
    {
        float t = 0f;
        Vector3 vec = transform.localPosition;
        Vector3 dest = vec;
        dest.x += 9f;

        FSM.Run();

        // 화면 밖으로 이동
        while (t < 1f)
        {
            t += Time.deltaTime;
            if (Team == Constant.Team.Blue) transform.localPosition = Vector3.Lerp(vec, dest, t);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // 다음 맵으로의 이동이 완료되었다고 BattleLogic에 통보.
        Message.Send<Battle.Raid.MoveToNextCompleteMsg>(new Battle.Raid.MoveToNextCompleteMsg());
    }

    /// <summary>
    /// MessageListener: 모든 전투가 끝났으니 결과 페이즈로 이동
    /// </summary>
    /// <param name="msg"></param>
    private void OnMoveToEnd(Battle.Raid.MoveToEndMsg msg)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (isDie == true)
            return;

        Status.AllEffectOff();

        _coroutine = StartCoroutine(coMoveToEnd());
    }

    /// <summary>
    /// Victory 재생을 위한 결과화면 위치로의 이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator coMoveToEnd()
    {
        float t = 0f;
        Vector3 vec = transform.localPosition;
        Vector3 dest = new Vector3(10f - 5f * _teamIndex, -4.3f, 0f);
        bool isLeft = (dest.x - vec.x) < 0;

        // 시간을 원래대로 되돌려준다.
        Time.timeScale = 1f;

        yield return new WaitForSeconds(1f);

        // 달리는 방향으로 플립해준다.
        if (isLeft == true)
            FSM.Skeleton.initialFlipX = true;
        else
            FSM.Skeleton.initialFlipX = false;

        FSM.Skeleton.Initialize(true);

        FSM.Run();

        // 승리 포지션으로 이동
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(vec, dest, t);
            yield return null;
        }

        FSM.Idle();

        yield return new WaitForSeconds(1f);

        // 우측을 바라보도록 플립해준다.
        FSM.Skeleton.initialFlipX = false;
        FSM.Skeleton.Initialize(true);

        FSM.Victory();

        yield return new WaitForSeconds(3f);

        // 결과 포지션에 도착하였다고 BattleLogic에 통보.
        Message.Send<Battle.Raid.MoveToEndCompleteMsg>(new Battle.Raid.MoveToEndCompleteMsg());
    }
    #endregion

    #region FSM과 상호작용하는 함수들
    protected override bool CheckException()
    {
        // 잠금 상태인가?
        if (_isLock == true)
            return false;
        // 내 체력이 0 이하인가? (내가 죽음)
        else if (Status.hp <= 0)
        {
            if (isDie == false)
                Message.Send<Battle.Raid.SendImDyingMsg>(new Battle.Raid.SendImDyingMsg(this));
            return false;
        }
        // 적을 찾는데 실패했는가? (적이 다 죽음)
        else if (Search() == false)
            return false;
        // 유닛의 상태를 강제로 고정하는 형태라면
        else if (CheckDebuffState())
            return false;

        return true;
    }

    private bool CheckDebuffState()
    {
        if (Status.isKnockback)
        {
            FSM.Hit();

            // 화면 밖으로는 넉백당하지 않도록 조정
            if ((Team == Constant.Team.Blue && transform.localPosition.x > -8f) || (Team == Constant.Team.Red && transform.localPosition.x < 8f))
                transform.Translate((Team == Constant.Team.Blue ? Vector3.left : Vector3.right) * 8f * Time.deltaTime);

            return true;
        }

        if (Status.isStun)
        {
            FSM.Hit();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 전투할 상대를 찾는다
    /// </summary>
    /// <returns>true -> "상대를 찾았다"</returns>
    private bool Search()
    {
        // 나랑 가장 가까운 녀석을 찾는다.
        _target = BattleManager.Singleton.FindUnitByDistance_X<UnitInfo_Raid>(this, Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue);

        // 이미 적이 다 죽었다. 대기상태로 종료하자.
        if (_target == null)
        {
            _isLock = true;

            FSM.Idle();

            // 현재의 전투가 종료되었다고 BattleLogic에 통보.
            Message.Send<Battle.Raid.StartBattleCompleteMsg>(new Battle.Raid.StartBattleCompleteMsg());

            return false;
        }

        return true;
    }

    /// <summary>
    /// 타겟과의 거리를 가져온다
    /// </summary>
    /// <returns>타겟과의 거리</returns>
    private float GetDistance()
    {
        return Mathf.Abs(_target.transform.localPosition.x - transform.localPosition.x);
    }

    /// <summary>
    /// attackPhaseIndex에 따라 공격타입을 선별
    /// </summary>
    private void SelectAttackType()
    {
        if (CheckException() == false)
            return;

        // MP가 가득 찼다. 궁스킬을 쓸 때다.
        if (Status.skill_0 != null && Status.mp >= 1000)
        {
            _targets = BattleManager.Singleton.GetSkillTargetAll<UnitInfo_Raid>(this, Status.skill_0, _targets);

            Message.Send<Battle.Raid.SendUseSkillMsg>(new Battle.Raid.SendUseSkillMsg(Status.idx, Status.code, true, _targets, () =>
            {
                Status.mp = 0;
                FSM.Skill0();
            }));
        }
        // 1스킬을 사용한다.
        else if (Status.skill_1 != null && _attackPhaseIndex == 1)
        {
            _targets = BattleManager.Singleton.GetSkillTargetAll<UnitInfo_Raid>(this, Status.skill_1, _targets);
            FSM.Skill1();
        }
        // 2스킬을 사용한다.
        else if (Status.skill_2 != null && _attackPhaseIndex == 3)
        {
            _targets = BattleManager.Singleton.GetSkillTargetAll<UnitInfo_Raid>(this, Status.skill_2, _targets);
            FSM.Skill2();
        }
        // 평타를 쓴다.
        else
        {
            FSM.Attack();
        }
    }

    /// <summary>
    /// Idle 상태에서 실행할 Callback
    /// </summary>
    protected override void OnIdle()
    {
        if (CheckException() == false)
            return;

        // 공격 사정거리 이내면 공격 쿨타임 찰때까지 대기.
        if (GetDistance() <= Status.attackRange)
        {
            _delayTime += Time.deltaTime;
            if (_delayTime >= Status.attackSpeed)
            {
                _delayTime = 0f;
                _isAttackComplete = false;

                SelectAttackType();
            }
        }
        // 거리가 안된다면 이동한다.
        else
        {
            FSM.Run();
        }
    }

    /// <summary>
    /// Run 상태에서 실행할 Callback
    /// </summary>
    protected override void OnRun()
    {
        if (CheckException() == false)
            return;

        // 공격 사정거리 이내면 쿨타임 찰때까지 대기.
        if (GetDistance() <= Status.attackRange)
        {
            _delayTime = 0f;
            _isAttackComplete = false;

            SelectAttackType();
        }
        // 거리가 안된다면 계속 이동한다.
        else
        {
            transform.Translate((Team == Constant.Team.Blue ? Vector3.right : Vector3.left) * Status.moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Attack 상태에서 실행할 Callback
    /// </summary>
    /// <param name="progress">Animation.normalizedTime</param>
    protected override void OnAttack(float progress)
    {
        if (_isAttackComplete == true)
            return;

        // 공격 적용 키프레임까지 대기.
        if (progress < Status.attackKeyFrame * 100f)
            return;

        _isAttackComplete = true;
        int damage = 0;
        bool isCritical = false;

        // 실명상태가 아닌 경우에만 효과 계산.
        if (Status.isBlind == false)
        {
            // 물리, 마법공격에 따른 베이스 데미지 계산
            if (Status.basicProperty == Constant.BasicAttackType.Physics)
                damage = BattleCalc.Calculate_Damage(Status.damage, _target.Status.pDef);
            else
                damage = BattleCalc.Calculate_Damage(Status.damage, _target.Status.mDef);

            // 치명타 발동 계산
            isCritical = BattleCalc.Calculate_ActiveCritical(Status.criticalPercentage, Status.level, _target.Status.level);

            // 치명타 발동에 따른 데미지 재계산.
            if (isCritical == true)
                damage *= (int)Status.criticalMultiple;
        }

        // 내 MP를 회복
        Status.mp += mpAdd_Action;

        // 타겟에게 데미지 전달.
        _target.ApplyDamage(damage, isCritical, this, Status.basicProperty);

        // 흡혈 상태일 경우 내 체력 회복
        if (Status.isAbsorbHP == true && damage > 0)
            ApplyHealing(damage);

        // 공격 페이즈 인덱스를 올려준다.
        _attackPhaseIndex++;
        if (_attackPhaseIndex > 3)
            _attackPhaseIndex = 0;
    }

    /// <summary>
    /// Skill0 상태에서 실행할 Callback
    /// </summary>
    /// <param name="progress">Animation.normalizedTime</param>
    protected override void OnSkill0(float progress)
    {
        if (_isAttackComplete == true)
            return;

        // 공격 적용 키프레임까지 대기.
        if (progress < Status.skill_0.keyFrame * 100f)
            return;

        _isAttackComplete = true;

        // 스킬 효과 적용
        ActiveSkill(Status.skill_0);

        // 공격 페이즈 인덱스를 올려준다.
        _attackPhaseIndex++;
        if (_attackPhaseIndex > 3)
            _attackPhaseIndex = 0;
    }

    /// <summary>
    /// Skill1 상태에서 실행할 Callback
    /// </summary>
    /// <param name="progress">Animation.normalizedTime</param>
    protected override void OnSkill1(float progress)
    {
        if (_isAttackComplete == true)
            return;

        // 공격 적용 키프레임까지 대기.
        if (progress < Status.skill_1.keyFrame * 100f)
            return;

        _isAttackComplete = true;

        // 스킬 효과 적용
        ActiveSkill(Status.skill_1);

        // 내 MP를 회복
        Status.mp += mpAdd_Action;

        // 공격 페이즈 인덱스를 올려준다.
        _attackPhaseIndex++;
        if (_attackPhaseIndex > 3)
            _attackPhaseIndex = 0;
    }

    /// <summary>
    /// Skill2 상태에서 실행할 Callback
    /// </summary>
    /// <param name="progress">Animation.normalizedTime</param>
    protected override void OnSkill2(float progress)
    {
        if (_isAttackComplete == true)
            return;

        // 공격 적용 키프레임까지 대기.
        if (progress < Status.skill_2.keyFrame * 100f)
            return;

        _isAttackComplete = true;

        // 스킬 효과 적용
        ActiveSkill(Status.skill_2);

        // 내 MP를 회복
        Status.mp += mpAdd_Action;

        // 공격 페이즈 인덱스를 올려준다.
        _attackPhaseIndex++;
        if (_attackPhaseIndex > 3)
            _attackPhaseIndex = 0;
    }

    /// <summary>
    /// Hit 상태에서 실행할 Callback
    /// </summary>
    protected override void OnHit()
    {
        if (CheckException() == false)
            return;

        // 공격 쿨타임이 된다면 그냥 Idle로 진행
        // (CheckException에서 스턴, 넉백등은 return처리하기 때문에)
        _delayTime += Time.deltaTime;
        if (_delayTime >= Status.attackSpeed)
        {
            FSM.Idle();
        }
    }

    /// <summary>
    /// Victory 상태에서 실행할 Callback
    /// </summary>
    protected override void OnVictory()
    {
        if (CheckException() == false)
            return;
    }

    /// <summary>
    /// Die 상태에서 실행할 Callback
    /// </summary>
    protected override void OnDie()
    {
        if (CheckException() == false)
            return;
    }
    #endregion

    #region Status와 상호작용하는 함수
    private void ActiveSkill(SkillModel.Skill skill)
    {
        for (int i = 0; i < skill.types.Count; i++)
        {
            if (skill.types[i] == Constant.SkillType.Damage) // 데미지를 주는 효과
            {
                int damage = 0;
                bool isCritical = false;
                List<UnitInfo_Raid> list = null;
                list = BattleManager.Singleton.GetSkillTarget<UnitInfo_Raid>(this, skill, list, i);
                if (list != null && list.Count > 0)
                {
                    foreach (UnitInfo_Raid target in list)
                    {
                        if (target == null)
                            continue;

                        // 체력이 없는지 다시 한번 체크
                        if (target.Status.hp <= 0)
                            return;

                        if (Status.isBlind == false)
                        {
                            if (skill.properties[i] == Constant.SkillProperty.PhysicalDamage)
                                damage = BattleCalc.Calculate_Damage(Status.damage, target.Status.pDef, skill.values[i]);
                            else
                                damage = BattleCalc.Calculate_Damage(Status.damage, target.Status.mDef, skill.values[i]);

                            isCritical = BattleCalc.Calculate_ActiveCritical(Status.criticalPercentage, Status.level, target.Status.level);
                            if (isCritical)
                                damage = damage * (int)Status.criticalMultiple;
                        }

                        target.ApplyDamage(damage, isCritical, this, skill.properties[i]);

                        if (Status.isAbsorbHP == true && damage > 0)
                            ApplyHealing(damage);
                    }
                }
            }
            else if (skill.types[i] == Constant.SkillType.Heal) // 체력 회복하는 효과
            {
                int heal = 0;
                List<UnitInfo_Raid> list = null;
                list = BattleManager.Singleton.GetSkillTarget<UnitInfo_Raid>(this, skill, list, i);

                if (list != null && list.Count > 0)
                {
                    foreach (UnitInfo_Raid target in list)
                    {
                        if (target == null)
                            continue;

                        // 체력이 없는지 다시 한번 체크
                        if (target.Status.hp <= 0)
                            return;

                        // 단일 힐
                        if (skill.properties[i] == Constant.SkillProperty.Heal_Oneshot)
                        {
                            if (Status.isBlind == false || skill.values[i] < 0)
                            {
                                heal = BattleCalc.Calculate_Damage(Status.damage, 0, skill.values[i]);
                            }
                            target.ApplyHealing(heal);
                        }
                        // 도트 힐 (버프로 적용)
                        else
                        {
                            if (Status.isBlind == false)
                            {
                                target.ApplyBuff(skill.properties[i], skill.values[i]);
                            }
                        }
                    }
                }
            }
            else if (skill.types[i] == Constant.SkillType.Buff) // 버프를 주는 효과 (실드, 스텟업 등)
            {
                List<UnitInfo_Raid> list = null;
                list = BattleManager.Singleton.GetSkillTarget<UnitInfo_Raid>(this, skill, list, i);

                if (list != null && list.Count > 0)
                {
                    foreach (UnitInfo_Raid target in list)
                    {
                        if (target == null)
                            continue;

                        // 체력이 없는지 다시 한번 체크
                        if (target.Status.hp <= 0)
                            return;

                        if (Status.isBlind == false)
                            target.ApplyBuff(skill.properties[i], skill.values[i]);
                    }
                }
            }
            else if (skill.types[i] == Constant.SkillType.Debuff) // 디버프를 주는 효과 (방깎, 스텟다운 등)
            {
                List<UnitInfo_Raid> list = null;
                list = BattleManager.Singleton.GetSkillTarget<UnitInfo_Raid>(this, skill, list, i);

                if (list != null && list.Count > 0)
                {
                    foreach (UnitInfo_Raid target in list)
                    {
                        if (target == null)
                            continue;

                        // 체력이 없는지 다시 한번 체크
                        if (target.Status.hp <= 0)
                            return;

                        if (Status.isBlind == false)
                            target.ApplyDebuff(skill.properties[i], skill.values[i]);
                    }
                }
            }
        }
    }

    private void ApplyDamage(int damage, bool isCritical, UnitInfo_Raid attacker, Constant.BasicAttackType attackType)
    {
        int dam = 0;
        // 물리실드가 있는데 물리공격이 들어왔다.
        if (attackType == Constant.BasicAttackType.Physics && Status.shieldPhysical > 0)
        {
            Status.shieldPhysical -= damage;

            // 실드로 다 막지 못했다.
            if (Status.shieldPhysical < 0)
            {
                dam = -Status.shieldPhysical;
                Status.shieldPhysical = 0;
            }
        }
        // 마법실드가 있는데 마법공격이 들어왔다.
        else if (attackType == Constant.BasicAttackType.Magic && Status.shieldMagical > 0)
        {
            Status.shieldMagical -= damage;

            // 실드로 다 막지 못했다.
            if (Status.shieldMagical < 0)
            {
                dam = -Status.shieldMagical;
                Status.shieldMagical = 0;
            }
        }
        // 그냥 공격을 받았다.
        else
        {
            dam = damage;
        }

        // 내 체력을 깎고 데미지 표기 UI를 띄워준다.
        Status.hp -= dam;
        Message.Send<Battle.Raid.SendDamageMsg>(new Battle.Raid.SendDamageMsg(transform, dam, isCritical, false));

        // 내가 맞은만큼 mp를 회복해준다. (실드로 막기 전 데미지)
        Status.mp += (int)(1000 * (damage / (float)Status.hpFull) * 0.5f);

        // Idle 상태에서만 타격 상태로 들어간다.
        if ((Constant.UnitState)FSM.GetState() == Constant.UnitState.Idle)
            FSM.Hit();

        // 내 체력이 0이 되면 죽으면서 공격자에게 mp를 채워준다.
        if (Status.hp <= 0 && isDie == false)
        {
            attacker.Status.mp += mpAdd_Kill;
            Message.Send<Battle.Raid.SendImDyingMsg>(new Battle.Raid.SendImDyingMsg(this));
        }
    }

    private void ApplyDamage(int damage, bool isCritical, UnitInfo_Raid attacker, Constant.SkillProperty property)
    {
        int dam = 0;
        // 물리실드가 있는데 물리공격이 들어왔다.
        if (property == Constant.SkillProperty.PhysicalDamage && Status.shieldPhysical > 0)
        {
            Status.shieldPhysical -= damage;

            // 실드로 다 막지 못했다.
            if (Status.shieldPhysical < 0)
            {
                dam = -Status.shieldPhysical;
                Status.shieldPhysical = 0;
            }
        }
        // 마법실드가 있는데 마법공격이 들어왔다.
        else if (property == Constant.SkillProperty.MagicalDamage && Status.shieldMagical > 0)
        {
            Status.shieldMagical -= damage;

            // 실드로 다 막지 못했다.
            if (Status.shieldMagical < 0)
            {
                dam = -Status.shieldMagical;
                Status.shieldMagical = 0;
            }
        }
        // 그냥 공격을 받았다.
        else
        {
            dam = damage;
        }

        // 내 체력을 깎고 데미지 표기 UI를 띄워준다.
        Status.hp -= dam;
        Message.Send<Battle.Raid.SendDamageMsg>(new Battle.Raid.SendDamageMsg(transform, dam, isCritical, false));

        // 내가 맞은만큼 mp를 회복해준다. (실드로 막기 전 데미지)
        Status.mp += (int)(1000 * (damage / (float)Status.hpFull) * 0.5f);

        // Idle 상태에서만 타격 상태로 들어간다.
        if ((Constant.UnitState)FSM.GetState() == Constant.UnitState.Idle)
            FSM.Hit();

        // 내 체력이 0이 되면 죽으면서 공격자에게 mp를 채워준다.
        if (Status.hp <= 0 && isDie == false)
        {
            attacker.Status.mp += mpAdd_Kill;
            Message.Send<Battle.Raid.SendImDyingMsg>(new Battle.Raid.SendImDyingMsg(this));
        }
    }

    private void ApplyHealing(int heal)
    {
        if (isDie)
            return;

        Status.hp += heal;
        if (Status.hp > Status.hpFull)
            Status.hp = Status.hpFull;

        if (heal > 0)
        {
            Message.Send<Battle.Raid.SendDamageMsg>(new Battle.Raid.SendDamageMsg(transform, heal, false, true));
        }
        else if (heal < 0)
        {
            Message.Send<Battle.Raid.SendDamageMsg>(new Battle.Raid.SendDamageMsg(transform, heal, false, false));
        }

        // 역회복을 하는 캐릭터도 있으니
        if (Status.hp <= 0 && isDie == false)
        {
            Message.Send<Battle.Raid.SendImDyingMsg>(new Battle.Raid.SendImDyingMsg(this));
        }
    }

    private void ApplyBuff(Constant.SkillProperty property, float value)
    {
        if (property == Constant.SkillProperty.Stun)
            Status.buffList.Add(new UnitBuff(Status, Constant.SkillType.Buff, property, value, 2f));
        else if (property == Constant.SkillProperty.KnockBack)
            Status.buffList.Add(new UnitBuff(Status, Constant.SkillType.Buff, property, value, 0.5f));
        else
            Status.buffList.Add(new UnitBuff(Status, Constant.SkillType.Buff, property, value, 10f));
    }

    private void ApplyDebuff(Constant.SkillProperty property, float value)
    {
        if (property == Constant.SkillProperty.Stun)
            Status.buffList.Add(new UnitBuff(Status, Constant.SkillType.Debuff, property, value, 2f));
        else if (property == Constant.SkillProperty.KnockBack)
            Status.buffList.Add(new UnitBuff(Status, Constant.SkillType.Debuff, property, value, 0.5f));
        else
            Status.buffList.Add(new UnitBuff(Status, Constant.SkillType.Debuff, property, value, 10f));
    }
    #endregion
}