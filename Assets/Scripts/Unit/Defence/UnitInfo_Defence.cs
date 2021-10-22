// ==================================================
// UnitInfo_Defence.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using Spine.Unity;

public class UnitInfo_Defence : IUnitInfo
{
    /// <summary>
    /// 다음 노드에 대한 정보
    /// </summary>
    private WayPointNode _node;
    /// <summary>
    /// SpineAnimation에 대한 정보
    /// </summary>
    private SkeletonAnimation _skel;
    /// <summary>
    /// 바라보고 있는 방향 벡터
    /// </summary>
    private Vector3 _moveDir;
    /// <summary>
    /// 내가 바라보고있는 상대(적)
    /// </summary>
    [SerializeField] private UnitInfo_Defence _target;

    /// <summary>
    /// 유닛 생성 후 초기화 함수
    /// </summary>
    /// <param name="Team">해당 유닛의 팀</param>
    /// <param name="status">해당 유닛의 status</param>
    /// <param name="fsm">해당 유닛의 상태머신</param>
    public override void Init(Constant.Team Team, UnitStatus status, UnitFSM fsm)
    {
        base.Init(Team, status, fsm);

        // _skel 캐싱
        if (_skel == null)
        {
            _skel = GetComponent<SkeletonAnimation>();

            // Component의 추가가 안되었다면 새로 추가해준다.
            if (_skel == null)
            {
                _skel = gameObject.AddComponent<SkeletonAnimation>();

                // 그래도 추가가 안되었다면 에러 메세지를 출력한다.
                if (_skel == null)
                {
                    Logger.LogErrorFormat("{0}에 SkeletonAnimation 컴포넌트의 추가가 되지 않았습니다.", gameObject.name);
                    return;
                }
            }
        }

        Status.Reset();

        // FSM에 SkeletonAnimation 전달
        fsm.Init(_skel, 1);
        fsm.Reset();

        // RedTeam이면 첫 node 정보를 가져온다.
        if (Team == Constant.Team.Red)
        {
            // 첫 노드 설정을 시작한다.
            Message.Send<Battle.Defence.FindFirstNodeMsg>(new Battle.Defence.FindFirstNodeMsg(Random.Range(0, 2), GetFirstNode));

            // tag 지정
            gameObject.tag = "Unit_Red";
        }
        else
        {
            fsm.Idle();

            // Collider 체크용 RigidBody 추가
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            // tag 지정
            gameObject.tag = "Unit_Blue";

            _isLock = false;
        }

        // 충돌 체크용 Collider 추가
        SphereCollider col = GetComponent<SphereCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
        }
        col.center = Vector3.zero;

        if (Team == Constant.Team.Blue)
            col.radius = 5f;
        else
            col.radius = 0.01f;
        col.isTrigger = true;

        // Raycast에 충돌하지 않는다.
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public override void Release()
    {
        base.Release();

        _node = null;
        _skel = null;
        _target = null;
    }

    protected override void AddMessageListener()
    {
        if (_skel == null)
            Message.AddListener<Battle.Defence.GameEndMsg>(OnGameEnd);
    }

    protected override void RemoveMessageListener()
    {
        Message.RemoveListener<Battle.Defence.GameEndMsg>(OnGameEnd);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (Team == Constant.Team.Red)
            return;

        // 충돌체가 나와 정반대의 유닛이 아니면 건너뛴다.
        if (Team == Constant.Team.Blue)
        {
            if (col.tag != "Unit_Red")
                return;
        }
        else
        {
            if (col.tag != "Unit_Blue")
                return;
        }

        // 충돌체에 UnitInfo가 포함되어 있고 내 지정상대가 비어있으면 해당 유닛으로 지정
        UnitInfo_Defence unit = col.GetComponent<UnitInfo_Defence>();
        if (unit != null && unit.Status.hp > 0 && _target == null)
        {
            _target = unit;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (Team == Constant.Team.Red)
            return;

        // 충돌체가 나와 정반대의 유닛이 아니면 건너뛴다.
        if (Team == Constant.Team.Blue)
        {
            if (col.tag != "Unit_Red")
                return;
        }
        else
        {
            if (col.tag != "Unit_Blue")
                return;
        }

        if (_target == null)
        {
            // 충돌체에 UnitInfo가 포함되어 있고 내 지정상대가 비어있으면 해당 유닛으로 지정
            UnitInfo_Defence unit = col.GetComponent<UnitInfo_Defence>();
            if (unit != null && unit.Status.hp > 0 && _target == null)
            {
                _target = unit;
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (Team == Constant.Team.Red)
            return;

        // 충돌체가 나와 정반대의 유닛이 아니면 건너뛴다.
        if (Team == Constant.Team.Blue)
        {
            if (col.tag != "Unit_Red")
                return;
        }
        else
        {
            if (col.tag != "Unit_Blue")
                return;
        }

        // 충돌체에 UnitInfo가 포함되어 있고, 내 지정상대와 같다면 비워준다.
        UnitInfo_Defence unit = col.GetComponent<UnitInfo_Defence>();
        if (unit != null && _target != null && unit == _target)
        {
            _target = null;
        }
    }

    /// <summary>
    /// 첫 노드를 찾는다.
    /// </summary>
    /// <param name="first">시작하는 지점</param>
    private void GetFirstNode(WayPointNode first)
    {
        _node = first;

        transform.position = _node.transform.position;

        FSM.Idle();

        _isLock = false;
    }

    /// <summary>
    /// 지속적으로 빌보드 로직을 실행한다.
    /// </summary>
    private void FixedUpdate()
    {
        // 카메라에 Billboard 방식으로 출력한다.
        transform.SetupBillboard(Camera.main);
    }

    #region FSM과 상호작용하는 함수들
    protected override void OnIdle()
    {
        if (CheckException() == true)
            return;

        _isAttackComplete = false;

        // 아군일 때
        if (Team == Constant.Team.Blue)
        {
            /*
            대기    적이 없을 때 & 공격 대기
            공격
            (죽음)
            */
            if (_target == null)
            {

            }
            else
            {
                LookAt();
                FSM.Attack();
            }
        }
        // 적군일 때
        else
        {
            /*
            대기    공격 대기
            공격
            이동
            죽음
            */

            // _target이 없으니 이동한다.
            if (_target == null)
            {
                FSM.Run();
            }
            // _target이 있으니 해당 캐릭터를 공격한다.
            //? 현재는 enemy의 타겟 서칭이 없으므로 이 로직은 건너뛴다.
            else
            {

            }
        }
    }

    protected override void OnRun()
    {
        // 잠금 상태로 전환되면 강제로 Idle상태로 변환
        if (_isLock == true)
        {
            FSM.Idle();
            return;
        }

        if (CheckException() == true)
            return;

        // 목표지점과의 거리가 0.1f 미만이면 다음 노드로 변경한다.
        if (BattleCalc.GetDistance(transform.position, _node.transform.position) < 0.1f)
        {
            // 그 다음 노드에 대한 정보가 없으면 현재가 최종지점이니 로직 종료.
            if (_node.Next == null)
            {
                // 적 유닛이 골 지점에 들어왔다.
                Message.Send<Battle.Defence.PassEnemyMsg>(new Battle.Defence.PassEnemyMsg());

                gameObject.SetActive(false);
                // Destroy(gameObject);
            }
            // 다음에 이동할 노드로 정보를 변경한다.
            else
            {
                _node = _node.Next;
            }

            return;
        }

        // 아직 목표지점까지 도착하지 않았다. 계속 이동한다.
        // 이동 방향 벡터를 지정한다.
        _moveDir = (_node.transform.position - transform.position).normalized;

        // 이동 방향에 따라 X축을 Flip해준다.
        if (_moveDir.x > 0 && _skel.initialFlipX == true)
        {
            _skel.initialFlipX = false;
            _skel.Initialize(true);
            _skel.AnimationState.SetAnimation(0, "run", true);
        }
        else if (_moveDir.x < 0 && _skel.initialFlipX == false)
        {
            _skel.initialFlipX = true;
            _skel.Initialize(true);
            _skel.AnimationState.SetAnimation(0, "run", true);
        }

        // 캐릭터를 이동시킨다.
        transform.position += _moveDir * Time.deltaTime * 3f;
    }

    protected override void OnAttack(float progress)
    {
        if (CheckException() == true)
            return;

        if (_isAttackComplete == true)
            return;

        // 공격 적용 키프레임까지 대기.
        if (progress < Status.attackKeyFrame * 100f)
            return;

        if (_target == null)
            return;

        _isAttackComplete = true;
        _target.Status.hp -= Status.damage;
        // Message.Send<Battle.SendDamageMsg>(new Battle.SendDamageMsg(_target.transform, status.damage, false, false));
        Message.Send<Battle.Defence.ShowHPBarMsg>(new Battle.Defence.ShowHPBarMsg(_target, Status.damage));
    }

    protected override void OnVictory()
    {

    }

    protected override void OnDie()
    {
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }

    protected override bool CheckException()
    {
        if (_isLock)
            return true;

        if (Status.hp <= 0)
        {
            Message.Send<Battle.Defence.KillEnemyMsg>(new Battle.Defence.KillEnemyMsg());
            FSM.Die();
            return true;
        }
        else if (_target != null && _target.Status.hp <= 0)
        {
            _target = null;
            return true;
        }

        return false;
    }

    private void LookAt()
    {
        if (_target == null)
            return;

        if (_target.transform.position.x > transform.position.x)
        {
            _skel.initialFlipX = false;
        }
        else
        {
            _skel.initialFlipX = true;
        }

        _skel.Initialize(true);
    }
    #endregion

    private void OnGameEnd(Battle.Defence.GameEndMsg msg)
    {
        _isLock = true;
    }

    private void OnDrawGizmos()
    {
        if (_target == null)
            return;

        if (Team == Constant.Team.Red)
            return;

        if (Team == Constant.Team.Red)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, _target.transform.position);
        Gizmos.DrawCube(_target.transform.position, Vector3.one);
    }
}
