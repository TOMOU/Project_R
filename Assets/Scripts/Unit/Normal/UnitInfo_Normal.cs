// ==================================================
// UnitInfo_Normal.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo_Normal : IUnitInfo
{
    public bool isLock;
    public UnitInfo_Normal owner;
    public bool isSummonUnit = false;
    public bool IsLock
    {
        get
        {
            return _isLock;
        }
        set
        {
            _isLock = value;
        }
    }
    public bool useSkill;
    private bool _useSkillGeneric = false;
    public List<bool> SyncVertList;
    private bool _syncVert = false;
    [SerializeField] private bool _isImmortal = false;
    private Renderer _renderer;
    public Renderer Rend { get { return _renderer; } }
    public int slotIndex;
    private int _baseSortingOrder;
    /// <summary>
    /// 전투시작 시 위치해야할 Vector
    /// </summary>
    private Vector3 _basePosition;
    private UnitInfo_Normal _target;
    public bool followAggro = false;
    public UnitInfo_Normal Target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
        }
    }
    private List<UnitInfo_Normal> _targets;
    /// <summary>
    /// 공격 페이즈 인덱스
    /// </summary>
    private int _attackPhaseIndex = 0;
    /// <summary>
    /// 공격 딜레이 타임
    /// </summary>
    private float _delayTime;

    private float _searchTime;

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
    // private bool _useSkillGeneric = false;

    private Texture2D _resizeTex = null;
    public bool isRaidLogic = false;
    public List<int> breakGauge;
    public List<float> breakTimer;

    private BossPartInfo _partInfo;
    public BossPartInfo PartInfo { get { return _partInfo; } }

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

        // FSM.Skeleton.OnMeshAndMaterialsUpdated += UpdateOnCallback;
        // // FSM.Skeleton.BeforeApply += UpdateOnCallback;
        // FSM.Skeleton.UpdateLocal += UpdateLocal;
        // FSM.Skeleton.UpdateWorld += UpdateOnCallback;

        if (Status.unitName != "j")
            FSM.Skeleton.UpdateComplete += UpdateComplete;

        // Texture2D tex = Resources.Load<Texture2D>(string.Format("Character/SpineData/{0}/{0}", Status.unitName));
        // _renderer.ResizeTexture(tex, Constant.ImageFilterMode.Average, transform.localScale.x);

        // Add Animation Key Event.
        fsm.state.Event += OnAnimationKeyEvent;

        // if (Team == Constant.Team.Red)
        // {
        //     MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        //     materialPropertyBlock.SetColor("_FillColor", Color.black);
        //     materialPropertyBlock.SetFloat("_FillPhase", 0.4f);
        //     _renderer.SetPropertyBlock(materialPropertyBlock);
        // }
    }

    public override void Release()
    {
        base.Release();

        if (Status.unitName != "j")
            FSM.Skeleton.UpdateComplete -= UpdateComplete;

        _renderer = null;
        _target = null;
        _resizeTex = null;

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

    void UpdateComplete(Spine.Unity.ISkeletonAnimation anim)
    {
        if (_resizeTex == null)
            SpineManager.Singleton.spineTextureDic.TryGetValue(Status.unitName, out _resizeTex);

        if (_resizeTex != null && _renderer.material.mainTexture != _resizeTex)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetTexture("_MainTex", _resizeTex);
            _renderer.SetPropertyBlock(propertyBlock);

            // _renderer.sharedMaterial.mainTexture = _resizeTex;
            // _renderer.material.mainTexture = _resizeTex;
        }
    }

    protected override void AddMessageListener()
    {
        Message.AddListener<Battle.Normal.PlayUseSkillMsg>(OnPlayUseSkill);
        Message.AddListener<Battle.Normal.SendUseSkillMsg>(OnSendUseSkill);
        Message.AddListener<Battle.Normal.SendImDyingMsg>(OnImDying);
        Message.AddListener<Battle.Normal.MoveToBaseMsg>(OnMoveToBase);
        Message.AddListener<Battle.Normal.StartBattleMsg>(OnStartBattle);
        Message.AddListener<Battle.Normal.MoveToNextMsg>(OnMoveToNext);
        Message.AddListener<Battle.Normal.MoveToEndMsg>(OnMoveToEnd);
        Message.AddListener<Battle.Normal.GenericUseSkillMsg>(OnGenericUseSkill);
        Message.AddListener<Battle.Normal.ImmortalEnemyMsg>(OnImmortalEnemy);
        Message.AddListener<Battle.Normal.SendDamageMsg>(OnSendDamage);
        Message.AddListener<Battle.Normal.SendHealMsg>(OnSendHeal);
        Message.AddListener<Battle.Normal.ForceIdleMsg>(OnForceIdle);
    }

    protected override void RemoveMessageListener()
    {
        Message.RemoveListener<Battle.Normal.PlayUseSkillMsg>(OnPlayUseSkill);
        Message.RemoveListener<Battle.Normal.SendUseSkillMsg>(OnSendUseSkill);
        Message.RemoveListener<Battle.Normal.SendImDyingMsg>(OnImDying);
        Message.RemoveListener<Battle.Normal.MoveToBaseMsg>(OnMoveToBase);
        Message.RemoveListener<Battle.Normal.StartBattleMsg>(OnStartBattle);
        Message.RemoveListener<Battle.Normal.MoveToNextMsg>(OnMoveToNext);
        Message.RemoveListener<Battle.Normal.MoveToEndMsg>(OnMoveToEnd);
        Message.RemoveListener<Battle.Normal.GenericUseSkillMsg>(OnGenericUseSkill);
        Message.RemoveListener<Battle.Normal.ImmortalEnemyMsg>(OnImmortalEnemy);
        Message.RemoveListener<Battle.Normal.SendDamageMsg>(OnSendDamage);
        Message.RemoveListener<Battle.Normal.SendHealMsg>(OnSendHeal);
        Message.RemoveListener<Battle.Normal.ForceIdleMsg>(OnForceIdle);
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
        this._baseSortingOrder = 0;// order;
        _renderer.sortingOrder = _baseSortingOrder;
    }

    /// <summary>
    /// 테스트용 함수. 캐릭터의 체력을 99999로 만든다.
    /// </summary>
    public void CheatHP(int multi = 5)
    {
        Status.hp *= multi;
        Status.hpFull *= multi;
    }

    public void CheatDamage(int multi = 2)
    {
        Status.damage *= multi;
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

    private void Update()
    {
        isLock = _isLock;

        if (_isLock == true)
            return;

        // if (Status.buffCount_Plus > 0)
        // {
        //     EffectManager.Singleton.OnParticleFollow("buff", transform, true);
        // }
        // else
        // {
        //     EffectManager.Singleton.OnParticleFollow("buff", transform, false);
        // }

        _searchTime += Time.deltaTime;
        if (_searchTime >= 1f)
        {
            _searchTime = 0f;
            Search();
        }

        Status.Refresh(_isLock);

        if (isRaidLogic && Status.code == 110001)
            RefreshBreakGauge();
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

    private void OnImmortalEnemy(Battle.Normal.ImmortalEnemyMsg msg)
    {
        // 아군 제외
        if (Team == Constant.Team.Blue)
            return;

        _isImmortal = true;
    }

    // 수동으로 스킬 사용
    private void OnGenericUseSkill(Battle.Normal.GenericUseSkillMsg msg)
    {
        // 나한테 온 메세지인지 확인
        if (msg.IDX != Status.idx)
            return;

        // 여러가지 조건을 체크한다.
        else if (CheckException() == false)
            return;

        // 내가 스킬사용이 가능한 캐릭터인지 판별
        else if (Status.skill_0 == null)
            return;

        // 메세지 수신 중 먼저 스킬을 사용했을수도 있다...사용가능 여부 체크하자
        else if (Status.mp < Status.mpFull)
            return;

        // 여기라면 스킬사용은 가능한데 거리가 안되는거다...예약 걸어주자!
        else if (IsAttackRangeIn() == false)
        {
            _useSkillGeneric = true;
            return;
        }

        // 여기까지 왔다면 바로 사용이 가능한 상태!!
        // 바로 스킬을 사용해준다.

        _useSkillGeneric = false;
        _isAttackComplete = false;
        _targets = BattleManager.Singleton.GetSkillTargetAll<UnitInfo_Normal>(this, Status.skill_0, _targets);

        // Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg(Status.idx, Status.code, true, _targets, () =>
        // {
        //     Status.mp = 0;
        //     FSM.Skill0();
        // }));

        EffectManager.Singleton.AllEffectOff();

        Message.Send<Battle.Normal.PlayUseSkillMsg>(new Battle.Normal.PlayUseSkillMsg(this, () =>
        {
            Status.mp = 0;
            _useSkillGeneric = false;
            FSM.Skill0(true);
        }));

        // Message.Send<Battle.Normal.SendUseSkillMsg>(new Battle.Normal.SendUseSkillMsg(this, true, _targets, () =>
        // {
        //     Status.mp = 0;
        //     FSM.Skill0(true);
        // }));
    }

    private void OnPlayUseSkill(Battle.Normal.PlayUseSkillMsg msg)
    {
        if (Status.hp <= 0)
            return;

        if (isDie)
            return;

        _isLock = true;

        if (gameObject.activeSelf == true && msg.Sender != this)
            FSM.Idle(true);

        EffectManager.Singleton.AllEffectOff();
    }

    /// <summary>
    /// 유닛이 궁스킬을 쓰면 영상재생 연출
    /// </summary>
    /// <param name="msg"></param>
    private void OnSendUseSkill(Battle.Normal.SendUseSkillMsg msg)
    {
        // 아무 데이터가 안들어가있다. 궁스킬이 종료된 것!!
        if (msg.isEnter == false && msg.targetList == null)
        {
            _isLock = false;
            _renderer.sortingLayerID = SortingLayer.NameToID("Character");
            _renderer.sortingOrder = _baseSortingOrder;

            Status.MinionShield_Hide(0);

            if (_targets != null && _targets.Count > 0)
                _targets.Clear();

            return;
        }

        // 죽었으면 제외한다.
        if (isDie || Status.hp <= 0)
        {
            _renderer.sortingLayerID = SortingLayer.NameToID("Character_Invisible");
            return;
        }

        // 여기까지 왔으면 일단 모든 State를 잠가준다.
        _isLock = msg.isEnter;

        EffectManager.Singleton.BuffOff();

        // 스킬 사용하는 캐릭터를 음영 위로 출력한다.
        if (msg.Sender == this)
        {
            _renderer.sortingLayerID = SortingLayer.NameToID("Skill_Field");
            _renderer.sortingOrder = 1;

            Status.MinionShield_Hide(1);
            return;
        }
        // 타겟 캐릭터들을 음영 위로 출력한다.
        else if (msg.targetList != null && msg.targetList.Contains(this))
        {
            _renderer.sortingLayerID = SortingLayer.NameToID("Skill_Field");
            _renderer.sortingOrder = 1;
            Status.MinionShield_Hide(1);
        }
        // 그 외의 캐릭터는 원래의 sortingOrder대로 출력한다.
        else
        {
            _renderer.sortingLayerID = SortingLayer.NameToID("Character_Invisible");
            Status.MinionShield_Hide(2);
        }

        // 모션 강제고정 상태를 제외하고 대기상태로 전환한다.
        if (Status.isStun == false && Status.isKnockback == false)
            FSM.Idle(true);

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
    private void OnImDying(Battle.Normal.SendImDyingMsg msg)
    {
        if (_isImmortal == true)
        {
            Status.hp = 10;
            return;
        }

        if (Team == msg.info.Team && this == msg.info)
        {
            Status.hp = 0;
            isDie = true;
            FSM.Die(true);

            Status.AllEffectOff();
        }
    }

    #region BattleLogic과의 상호작용 함수들
    /// <summary>
    /// MessageListener: 전투 시작지점으로 이동
    /// </summary>
    /// <param name="msg"></param>
    private void OnMoveToBase(Battle.Normal.MoveToBaseMsg msg)
    {
        _isLock = true;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (isDie == true)
            return;

        if (isSummonUnit == false)
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
        while (t < 2f)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(vec, _basePosition, t / 2);
            yield return null;
        }

        FSM.Idle();

        yield return new WaitForSeconds(1f);

        // 이동이 완료되었다고 BattleLogic에 통보.
        Message.Send<Battle.Normal.MoveToBaseCompleteMsg>(new Battle.Normal.MoveToBaseCompleteMsg());
    }

    /// <summary>
    /// MessageListener: 이동이 완료되었으니 전투를 시작
    /// </summary>
    /// <param name="msg"></param>
    private void OnStartBattle(Battle.Normal.StartBattleMsg msg)
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
    private void OnMoveToNext(Battle.Normal.MoveToNextMsg msg)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (isDie == true)
            return;

        if (isSummonUnit == true)
        {
            Status.hp = 0;
            isDie = true;
            FSM.Die(true);

            Status.AllEffectOff();
            return;
        }

        Status.AllEffectOff();

        // 방향 원래대로
        FSM.Skeleton.skeleton.ScaleX = 1f;

        if (isSummonUnit == false)
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

        FSM.Run(true);

        // 화면 밖으로 이동
        while (t < 2f)
        {
            t += Time.deltaTime;
            if (Team == Constant.Team.Blue) transform.localPosition = Vector3.Lerp(vec, dest, t / 2f);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // 다음 맵으로의 이동이 완료되었다고 BattleLogic에 통보.
        Message.Send<Battle.Normal.MoveToNextCompleteMsg>(new Battle.Normal.MoveToNextCompleteMsg());
    }

    /// <summary>
    /// MessageListener: 모든 전투가 끝났으니 결과 페이즈로 이동
    /// </summary>
    /// <param name="msg"></param>
    private void OnMoveToEnd(Battle.Normal.MoveToEndMsg msg)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (isDie == true)
            return;

        if (isSummonUnit == true)
        {
            Status.hp = 0;
            isDie = true;
            FSM.Die(true);

            Status.AllEffectOff();
            return;
        }

        Status.AllEffectOff();

        if (isSummonUnit == false)
            _coroutine = StartCoroutine(coMoveToEnd());
    }

    /// <summary>
    /// Victory 재생을 위한 결과화면 위치로의 이동
    /// </summary>
    /// <returns></returns>
    private IEnumerator coMoveToEnd()
    {
        var list = Team == Constant.Team.Blue ? BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>();
        int idx = list.IndexOf(this);
        float t = 0f;
        Vector3 vec = transform.localPosition;
        Vector3 dest = new Vector3(-2.5f * (list.Count - 1) + 5f * idx, -2.1f, -2.1f);
        bool isLeft = (dest.x - vec.x) < 0;

        // 시간을 원래대로 되돌려준다.
        Time.timeScale = 1f;

        yield return new WaitForSeconds(1f);

        // layer를 최상단으로
        _renderer.sortingOrder += 10;

        // 달리는 방향으로 플립해준다.
        if (isLeft == true)
            FSM.Skeleton.skeleton.ScaleX = -1f;
        else
            FSM.Skeleton.skeleton.ScaleX = 1f;

        FSM.Run();

        // 승리 포지션으로 이동
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(vec, dest, t);
            yield return null;
        }

        // 우측을 바라보도록 플립해준다.
        FSM.Skeleton.skeleton.ScaleX = 1f;
        FSM.Idle();

        yield return new WaitForSeconds(1f);

        // FSM.Victory();

        // 결과 포지션에 도착하였다고 BattleLogic에 통보.
        Message.Send<Battle.Normal.MoveToEndCompleteMsg>(new Battle.Normal.MoveToEndCompleteMsg());
    }
    #endregion

    #region FSM과 상호작용하는 함수들
    public void CheckException_Force()
    {
        CheckException();
    }
    protected override bool CheckException()
    {
        // 잠금 상태인가?
        if (_isLock == true)
            return false;
        // 내 체력이 0 이하인가? (내가 죽음)
        else if (Status.hp <= 0)
        {
            if (isDie == false)
                Message.Send<Battle.Normal.SendImDyingMsg>(new Battle.Normal.SendImDyingMsg(this));
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
        if (_isLock == true)
            return false;

        if (Status.isKnockback)
        {
            FSM.Condition(true);

            // 화면 밖으로는 넉백당하지 않도록 조정
            if ((-13f < transform.localPosition.x) && (transform.localPosition.x < 13f))
                transform.Translate(Status.dirKnockback * Vector3.right * 8f * Time.deltaTime);

            return true;
        }

        if (Status.isStun)
        {
            FSM.Condition(true);
            return true;
        }

        if (Status.isAggroState)
        {
            var list = Team == Constant.Team.Blue ? BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>() : BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Target != this)
                {
                    list[i].followAggro = true;
                    list[i].Target = this;
                }
            }
        }

        return false;
    }
    public string StateName;
    private void CheckDirection()
    {
        if (_target == null)
            return;

        // 방향 뒤집기
        Vector3 dir = _target.transform.position - transform.position;
        if (dir.x < 0 && FSM.Skeleton.skeleton.ScaleX == 1f)
        {
            FSM.Skeleton.skeleton.ScaleX = -1f;
        }
        else if (dir.x > 0 && FSM.Skeleton.skeleton.ScaleX == -1f)
        {
            FSM.Skeleton.skeleton.ScaleX = 1f;
        }
    }

    /// <summary>
    /// 전투할 상대를 찾는다
    /// </summary>
    /// <returns>true -> "상대를 찾았다"</returns>
    private bool Search()
    {
        if (isDie == true)
            return false;

        if (followAggro)
        {
            if (isSummonUnit == true)
            {
                if (_target == null || _target.isDie == true || _target.Status.isStun == false)
                {
                    Status.hp = 0;
                    isDie = true;
                    FSM.Die(true);

                    Status.AllEffectOff();

                    return true;
                }

                return true;
            }
            else if (_target != null && _target.isDie == false)
            {
                return true;
            }
        }

        // if (_target != null && _target.isDie == false)
        //     return true;

        // 나랑 가장 가까운 녀석을 찾는다.
        _target = BattleManager.Singleton.FindUnitByDistance_XY<UnitInfo_Normal>(this, Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue);

        // 이미 적이 다 죽었다. 대기상태로 종료하자.
        if (_target == null)
        {
            _isLock = true;

            _useSkillGeneric = false;

            FSM.Idle(true);

            // 현재의 전투가 종료되었다고 BattleLogic에 통보.
            if (isSummonUnit == false)
            {
                Message.Send<Battle.Normal.StartBattleCompleteMsg>(new Battle.Normal.StartBattleCompleteMsg());
            }

            return false;
        }

        return true;
    }

    public void Search_Force()
    {
        followAggro = false;
        _target = null;
        Search();
    }

    /// <summary>
    /// 타겟과의 거리를 가져온다
    /// </summary>
    /// <returns>타겟과의 거리</returns>
    private float GetDistance()
    {
        return Vector3.Distance(_target.transform.position, transform.position);
        // return Mathf.Abs(_target.transform.localPosition.x - transform.localPosition.x);
    }

    private bool IsAttackRangeIn()
    {
        if (isRaidLogic == false)
        {
            // 직선 거리가 공격범위 이하인 경우
            if (Vector3.Distance(_target.transform.position, transform.position) > Status.attackRange)
                return false;

            if (SyncVertList != null)
            {
                if (_attackPhaseIndex == 1 && SyncVertList[0] == true && Mathf.Abs(_target.transform.position.y - transform.position.y) > 0.1f)
                {
                    _syncVert = true;
                    return false;
                }

                if (_attackPhaseIndex == 3 && SyncVertList[1] == true && Mathf.Abs(_target.transform.position.y - transform.position.y) > 0.1f)
                {
                    _syncVert = true;
                    return false;
                }

                if (Team == Constant.Team.Blue && (BattleManager.Singleton.battleType == 6 || BattleManager.Singleton.AutoSkill == true || _useSkillGeneric == true) && Status.mp >= Status.mpFull && SyncVertList[2] == true && Mathf.Abs(_target.transform.position.y - transform.position.y) > 0.1f)
                {
                    _syncVert = true;
                    return false;
                }

                else if (Team == Constant.Team.Red && Status.mp >= Status.mpFull && SyncVertList[2] == true && Mathf.Abs(_target.transform.position.y - transform.position.y) > 0.1f)
                {
                    _syncVert = true;
                    return false;
                }
            }
        }
        else
        {
            if (Mathf.Abs(_target.transform.position.x - transform.position.x) > Status.attackRange)
                return false;
        }

        return true;
    }

    /// <summary>
    /// attackPhaseIndex에 따라 공격타입을 선별
    /// </summary>
    private void SelectAttackType()
    {
        if (CheckException() == false)
            return;

        // 방향을 틀어준다.
        CheckDirection();

        _syncVert = false;

        if (useSkill == false)
        {
            FSM.Attack();
            return;
        }
        //me one area all
        if ((Team == Constant.Team.Blue && (BattleManager.Singleton.battleType == 6 || BattleManager.Singleton.AutoSkill || _useSkillGeneric == true) && Status.skill_0 != null && Status.mp >= 1000) ||
        (Team == Constant.Team.Red && Status.skill_0 != null && Status.mp >= 1000))
        {
            Message.Send<Battle.Normal.PlayUseSkillMsg>(new Battle.Normal.PlayUseSkillMsg(this, () =>
            {
                Status.mp = 0;
                _useSkillGeneric = false;
                FSM.Skill0(true);
            }));
        }
        else
        {
            switch (_attackPhaseIndex)
            {
                case 0:
                case 2:
                    FSM.Attack();
                    break;

                case 1:
                    FSM.Skill1();
                    break;

                case 3:
                    FSM.Skill2();
                    break;
            }
        }

        _attackPhaseIndex++;
        if (_attackPhaseIndex > 3)
            _attackPhaseIndex = 0;
    }

    /// <summary>
    /// Idle 상태에서 실행할 Callback
    /// </summary>
    protected override void OnIdle()
    {
        if (CheckException() == false)
            return;

        // 방향을 틀어준다.
        CheckDirection();

        // 공격 사정거리 이내면 공격 쿨타임 찰때까지 대기.
        // if (GetDistance() <= Status.attackRange)
        if (IsAttackRangeIn())
        {
            Status.MinionShield_Idle();

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

        // 방향을 틀어준다.
        // CheckDirection();

        _delayTime += Time.deltaTime;

        // 공격 사정거리 이내면 쿨타임 찰때까지 대기.
        // if (GetDistance() <= Status.attackRange)
        if (IsAttackRangeIn())
        {
            FSM.Idle();
        }
        // 거리가 안된다면 계속 이동한다.
        else
        {
            Status.MinionShield_Run();

            if (isRaidLogic == false)
            {
                if (_syncVert == true)
                    transform.MoveToY(_target.transform, Status.moveSpeed, Status.attackRange, FSM.Skeleton);
                else
                    transform.MoveTo(_target.transform, Status.moveSpeed, Status.attackRange, FSM.Skeleton);
            }
            else
            {
                transform.MoveToX(_target.transform, Status.moveSpeed, Status.attackRange, FSM.Skeleton);
            }
        }
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

    protected override void OnAttack(float progress)
    {
        base.OnAttack(progress);

        // if (_target.isDie == true || _target == null)
        //     FSM.Idle(true);
    }

    protected override void OnSkill0(float progress)
    {
        base.OnSkill0(progress);

        // if (_target.isDie == true || _target == null)
        //     FSM.Idle(true);
    }

    protected override void OnSkill1(float progress)
    {
        base.OnSkill1(progress);

        // if (_target.isDie == true || _target == null)
        //     FSM.Idle(true);
    }

    protected override void OnSkill2(float progress)
    {
        base.OnSkill2(progress);

        // if (_target.isDie == true || _target == null)
        //     FSM.Idle(true);
    }

    protected override void OnCondition()
    {
        if (CheckException() == false)
            return;
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

    public void SetRaidLogic()
    {
        isRaidLogic = true;

        if (Status.unitName == "jinshi")
        {
            Status.minionSpear.SetRaidLogic();
            Status.minionSpear_AP.ForEach(e => e.SetRaidLogic());
        }

        Status.attackRange *= 10;

        if (Status.code != 110001)
            return;

        breakGauge = new List<int>();
        breakTimer = new List<float>();
        _partInfo = GetComponent<BossPartInfo>();
        for (int i = 0; i < 5; i++)
        {
            breakGauge.Add(1000);
            breakTimer.Add(0f);
        }
    }

    private void OnSendDamage(Battle.Normal.SendDamageMsg msg)
    {
        // 이미 죽은 상태면 건너뛴다.
        if (isDie == true)
            return;

        // 나한테 온 메세지가 아니면 무시한다.
        if (msg.target != this)
            return;

        if (Status.minionShieldCount > 0)
        {
            Status.minionShieldCount--;
            var minion = Status.minionShieldList.Find(e => e.shieldEnabled == true);
            if (minion != null)
            {
                minion.Die();
            }
            return;
        }

        int damage = 0;

        // 보스전에서 J의 경우
        if (isRaidLogic == true && Status.code == 110001)
        {
            if (breakGauge[msg.partIndex] > 0)
            {
                breakGauge[msg.partIndex] -= msg.damage;

                // 데미지 이펙트 출력
                if (_partInfo != null)
                {
                    EffectManager.Singleton.OnPlayParticle(_partInfo.socketList[msg.partIndex].position, "hit", 1f, _renderer.sortingLayerID, _renderer.sortingOrder);
                }

                if (breakGauge[msg.partIndex] <= 0)
                {
                    breakTimer[msg.partIndex] = 10f;

                    // DEBUG용 로그 찍기
                    int cnt = breakTimer.FindAll(e => e > 0f).Count;
                    Logger.LogFormat("J Breakpoint {0}개 활성화", cnt);
                }
            }
            else
            {
                // 데미지 배율
                float multi = 1f;

                // 파괴된 부위에 따라 데미지 배율 적용
                int bCnt = breakGauge.FindAll(e => e <= 0).Count;
                if (bCnt > 1)
                    multi = 1f + 0.25f * Mathf.Pow(2f, bCnt - 2);

                damage = (int)(msg.damage * multi);
            }
        }
        else
        {
            damage = msg.damage;
        }

        // 데미지 적용
        Status.hp -= damage;

        if (_isImmortal == true & Status.hp <= 0)
            Status.hp = 10;

        if (damage == 0)
            return;

        if (isRaidLogic == true && Status.code == 110001)
        {
            // 데미지 이펙트 출력
            if (_partInfo != null)
            {
                EffectManager.Singleton.OnPlayParticle(_partInfo.socketList[msg.partIndex].position, "hit", 1f, _renderer.sortingLayerID, _renderer.sortingOrder);
            }

            Message.Send<Battle.Normal.ShowDamageMsg>(new Battle.Normal.ShowDamageMsg(msg.target, damage, msg.isCritical, msg.partIndex));
        }
        else
        {
            EffectManager.Singleton.OnPlayParticle(msg.target.transform.localPosition + new Vector3(0f, 2f), "hit", 1f, _renderer.sortingLayerID, _renderer.sortingOrder);
            Message.Send<Battle.Normal.ShowDamageMsg>(new Battle.Normal.ShowDamageMsg(msg.target, damage, msg.isCritical));
        }

        // 체력이 0이 되었다면 죽음
        if (Status.hp <= 0 && isDie == false)
        {
            isDie = true;
            Message.Send<Battle.Normal.SendImDyingMsg>(new Battle.Normal.SendImDyingMsg(this));
        }
        // Idle 상태라면 타격상태 전환
        else if (damage > 0 && (Constant.UnitState)FSM.GetState() == Constant.UnitState.Idle)
        {
            FSM.Hit(true);
        }
    }

    private void RefreshBreakGauge()
    {
        if (_isLock == true)
            return;

        for (int i = 0; i < breakTimer.Count; i++)
        {
            if (breakTimer[i] <= 0f && breakGauge[i] <= 0)
            {
                breakTimer[i] = 0f;
                breakGauge[i] = 1000;

                continue;
            }

            breakTimer[i] -= Time.deltaTime;
        }
    }

    private void OnSendHeal(Battle.Normal.SendHealMsg msg)
    {
        // 이미 죽은 상태면 건너뛴다.
        if (isDie == true)
            return;

        // 나한테 온 메세지가 아니면 무시한다.
        if (msg.Target != this)
            return;

        Status.hp += msg.Value;

        Message.Send<Battle.Normal.ShowHealMsg>(new Battle.Normal.ShowHealMsg(msg.Target, msg.Value));
    }

    private void OnForceIdle(Battle.Normal.ForceIdleMsg msg)
    {
        _isLock = true;

        if (gameObject.activeSelf == true)
            FSM.Idle(true);
    }
}