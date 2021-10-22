// ==================================================
// BattleDefenceLogic.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections.Generic;

namespace Logic
{
    public class BattleDefenceLogic : ILogic
    {
        [Header("Enemy Unit Parameter")]
        [SerializeField] private List<UnitInfo_Defence> _enemyList;
        /// <summary>
        /// 적 소환 타이머
        /// </summary>
        [SerializeField] private float _summonDelta = 0f;
        /// <summary>
        /// 적 소환 간격
        /// </summary>
        [SerializeField] private float _summonInterval = 1f;

        [Header("Ally Unit Parameter")]
        [SerializeField] private int _summonIdx = 0;
        /// <summary>
        /// 아군 소환대기 유닛 (드래그 중인...)
        /// </summary>
        [SerializeField] private GameObject _summonUnit = null;
        /// <summary>
        /// 소환 가능한 타겟 블록
        /// </summary>
        [SerializeField] private BlockProperty _targetBlock = null;
        /// <summary>
        /// UI 드래그 중인지 (유닛 소환)
        /// </summary>
        [SerializeField] private bool _isDrag = false;

        [Header("공격범위 표시")]
        /// <summary>
        /// 아군 소환 시 공격범위 표시
        /// </summary>
        [SerializeField] private LineRenderer _rangeRenderer = null;
        private float _thetaScale = 0.01f;
        private float _theta = 0f;
        private float _radius = 5f / 2f;
        private int _size = 0;

        [Header("소환 가능 포인트")]
        [SerializeField] private float _summonPointDelta = 0f;
        [SerializeField] private int _summonPoint = 0;

        private int _curWaveCount = 0;
        private int _totalWaveCount = 40;
        private int _lifeCount = 10;

        private bool _isLock = true;

        protected override void Initialize()
        {
            _enemyList = new List<UnitInfo_Defence>();

            var um = Model.First<UnitModel>();
            if (um != null)
            {
                UnitModel.Unit unit = null;
                for (int i = 0; i < test_enemy_idx.Count; i++)
                {
                    unit = um.unitTable.Find(e => e.code == test_enemy_idx[i]);
                    if (unit != null)
                    {
                        GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", unit.unitName));
                        GameObject obj = GameObject.Instantiate(prefab);
                        obj.transform.SetParent(transform);
                        obj.transform.localScale = Vector3.one * 0.5f;
                        obj.transform.localPosition = Vector3.one * 1000f;

                        UnitInfo_Defence u = obj.AddComponent<UnitInfo_Defence>();
                        UnitStatus status = new UnitStatus(unit, 0, 1, 1);
                        UnitFSM fsm = obj.AddComponent<UnitFSM>();
                        u.Init(Constant.Team.Red, status, fsm);

                        obj.SetActive(false);

                        _enemyList.Add(u);

                        status = null;
                        fsm = null;
                        prefab = null;
                        obj = null;
                    }
                }

                unit = null;
            }

            um = null;

            // LineRenderer 초기화
            _size = (int)((1f / _thetaScale) + 1f);

            _rangeRenderer = gameObject.AddComponent<LineRenderer>();
            _rangeRenderer.material = new Material(Shader.Find("Standard"));
            _rangeRenderer.positionCount = _size;
            _rangeRenderer.startWidth = 0.1f;
            _rangeRenderer.endWidth = 0.1f;
            _rangeRenderer.startColor = Color.yellow;
            _rangeRenderer.endColor = Color.yellow;
            _rangeRenderer.enabled = false;

            // 기본 소환포인트 지정
            _summonPoint = 100;
            // 소환포인트값 전달
            Message.Send<Battle.Defence.InitSummonPointMsg>(new Battle.Defence.InitSummonPointMsg(_summonPoint));

            // 게임 시작정보 전달 (wave 캐릭터수, 유저 생명수)
            Message.Send<Battle.Defence.InitReportBoardMsg>(new Battle.Defence.InitReportBoardMsg(_totalWaveCount, _lifeCount));

            Message.AddListener<Battle.Defence.SummonDragStartMsg>(OnSummonDragStart);
            Message.AddListener<Battle.Defence.SummonDragEndMsg>(OnSummonDragEnd);
            Message.AddListener<Battle.Defence.DragEnterInBlockMsg>(OnDragEnterInBlock);
            Message.AddListener<Battle.Defence.DragExitInBlockMsg>(OnDragExitInBlock);
            Message.AddListener<Battle.Defence.KillEnemyMsg>(OnKillEnemy);
            Message.AddListener<Battle.Defence.PassEnemyMsg>(OnPassEnemy);

            _isLock = false;
        }

        protected override void Release()
        {
            if (_enemyList != null)
            {
                _enemyList.Clear();
                _enemyList = null;
            }

            if (_rangeRenderer != null)
            {
                _rangeRenderer = null;
            }

            Message.RemoveListener<Battle.Defence.SummonDragStartMsg>(OnSummonDragStart);
            Message.RemoveListener<Battle.Defence.SummonDragEndMsg>(OnSummonDragEnd);
            Message.RemoveListener<Battle.Defence.DragEnterInBlockMsg>(OnDragEnterInBlock);
            Message.RemoveListener<Battle.Defence.DragExitInBlockMsg>(OnDragExitInBlock);
            Message.RemoveListener<Battle.Defence.KillEnemyMsg>(OnKillEnemy);
            Message.RemoveListener<Battle.Defence.PassEnemyMsg>(OnPassEnemy);
        }

        /// <summary>
        /// 유닛 소환 UI를 통해 드래그를 시작할 때 메세지 수신
        /// </summary>
        /// <param name="msg"></param>
        private void OnSummonDragStart(Battle.Defence.SummonDragStartMsg msg)
        {
            _summonIdx = msg.Index;
            var um = Model.First<UnitModel>();
            if (um != null)
            {
                var unit = um.unitTable.Find(e => e.code == _summonIdx);
                if (unit != null)
                {
                    GameObject obj = Resources.Load<GameObject>(string.Format("Character/{0}", unit.unitName));
                    _summonUnit = GameObject.Instantiate(obj);
                    _summonUnit.transform.localScale = Vector3.one * 0.5f;

                    Spine.Unity.SkeletonAnimation skel = _summonUnit.GetComponent<Spine.Unity.SkeletonAnimation>();
                    if (skel != null)
                    {
                        skel.AnimationState.SetAnimation(0, "idle", true);
                    }
                    // skel.                    
                }
            }


            // Drag 상태이다
            _isDrag = true;

            //! 수정 필요
            //! 시간을 느리게 해준다.
            Time.timeScale = 0.1f;
        }

        /// <summary>
        /// 유닛 소환 UI를 통해 드래그가 종료될 때 메세지 수신
        /// </summary>
        /// <param name="msg"></param>
        private void OnSummonDragEnd(Battle.Defence.SummonDragEndMsg msg)
        {
            if (_summonUnit != null)
            {
                // 포인터 충돌된 블록이 있다면 해당 블록의 위치 offset에 유닛 스파인을 위치시킨다.
                if (_targetBlock != null)
                {
                    var um = Model.First<UnitModel>();
                    if (um != null)
                    {
                        var unit = um.unitTable.Find(e => e.code == _summonIdx);
                        if (unit != null)
                        {
                            // UnitInfo, UnitFSM을 추가해준다.
                            UnitInfo_Defence u = _summonUnit.AddComponent<UnitInfo_Defence>();
                            UnitStatus status = new UnitStatus(unit, 0, 1, 1);
                            UnitFSM fsm = _summonUnit.AddComponent<UnitFSM>();
                            u.Init(Constant.Team.Blue, status, fsm);

                            // 해당 블록에 유닛정보를 담는다 (유닛이 이미 올라와있는지 여부에 대한 체크)
                            _targetBlock.InsertUnitSlot(u);

                            // 로직에서 비용 삭감하고, 필요한 포인트를 삭감하는 메세지 전달
                            _summonPoint -= msg.Price;
                            Message.Send<Battle.Defence.RemoveSummonPointMsg>(new Battle.Defence.RemoveSummonPointMsg(msg.Price));
                        }
                    }

                    // 파라미터를 비워준다 (재참조 문제 방지)
                    _targetBlock = null;
                    _summonUnit = null;
                }
                else
                {
                    // 포인터 충돌된 블록이 없기 때문에 유닛 스파인을 제거한다.
                    _summonUnit.SetActive(false);
                    Destroy(_summonUnit);
                }
            }

            // Drag 상태에서 나온다.
            _isDrag = false;

            // 시간을 원래대로 돌려준다.
            Message.Send<Global.ReloadSpeedMsg>(new Global.ReloadSpeedMsg());
        }

        /// <summary>
        /// 블록과의 포인터 충돌이 일어났을 때 메세지 수신
        /// </summary>
        /// <param name="msg"></param>
        private void OnDragEnterInBlock(Battle.Defence.DragEnterInBlockMsg msg)
        {
            _targetBlock = msg.Block;

            // 해당 블록에 유닛이 설치되었을 때 공격범위를 보여준다.            
            for (int i = 0; i < _size; i++)
            {
                _theta += (2f * Mathf.PI * _thetaScale);
                float x = _radius * Mathf.Cos(_theta);
                float y = _radius * Mathf.Sin(_theta);
                Vector3 vec = new Vector3(x, 0f, y);
                vec += _targetBlock.transform.position;
                vec.y += 1.2f;
                vec.z -= 0.5f;
                _rangeRenderer.SetPosition(i, vec);
            }

            _rangeRenderer.enabled = true;
        }

        /// <summary>
        /// 블록과의 포인터 충돌 이후 나왔을 때 메세지 수신
        /// </summary>
        /// <param name="msg"></param>
        private void OnDragExitInBlock(Battle.Defence.DragExitInBlockMsg msg)
        {
            if (_targetBlock != null && _targetBlock == msg.Block)
            {
                _targetBlock = null;
                _rangeRenderer.enabled = false;
            }
        }

        private void OnKillEnemy(Battle.Defence.KillEnemyMsg msg)
        {
            // 적을 Kill 하였으니 waveCount를 증가시켜 준다.
            _curWaveCount++;

            // 현재 Kill수와 전체 적 수와 동일하다면 게임이 끝났다.
            // 승리 처리한다.
            if (_curWaveCount == _totalWaveCount)
            {
                GameEnd(true);
            }
        }

        private void OnPassEnemy(Battle.Defence.PassEnemyMsg msg)
        {
            // 적이 통과하였으니 waveCount를 증가시켜 주고
            // 플레이어 라이프수를 줄인다.
            _curWaveCount++;
            _lifeCount--;

            // 유저 라이프수가 먼저 0이 되면 패배 처리한다.
            if (_lifeCount == 0)
            {
                GameEnd(false);
            }
            // 다음으로 현재 킬수와 적 전체수가 동일하다면 승리 처리한다.
            else if (_curWaveCount == _totalWaveCount)
            {
                GameEnd(true);
            }
        }

        /// <summary>
        /// 게임결과 출력
        /// </summary>
        /// <param name="isVictory"></param>
        private void GameEnd(bool isVictory)
        {
            _isLock = true;

            Dialog.IDialog.RequestDialogExit<Dialog.DefenceMainDialog>();
            Dialog.IDialog.RequestDialogEnter<Dialog.DefenceResultDialog>();

            Message.Send<Battle.Defence.GameEndMsg>(new Battle.Defence.GameEndMsg(isVictory));
        }

        /// <summary>
        /// 지속적으로 적유닛 소환과 아군유닛 소환 로직을 돌린다 (이후 시간제한을 둘 지는 보류)
        /// </summary>
        private void Update()
        {
            if (_isLock == true)
                return;

            AddPointByTime();
            SummonEnemyUnit();
            SummonAllyUnit();
        }

        private void AddPointByTime()
        {
            _summonPointDelta += Time.deltaTime;
            if (_summonPointDelta >= 1f)
            {
                _summonPointDelta = 0f;

                if (_summonPoint < 100)
                {
                    _summonPoint++;
                    Message.Send<Battle.Defence.AddSummonPointMsg>(new Battle.Defence.AddSummonPointMsg());
                }
            }
        }

        /// <summary>
        /// 아군 유닛 소환 로직
        /// </summary>
        private void SummonAllyUnit()
        {
            // 드래그 상태일 때가 아니면 이 로직을 건너뛴다.
            if (_isDrag == false)
            {
                _rangeRenderer.enabled = false;
                return;
            }

            // 유닛 스파인의 빌보드 설정한다.
            _summonUnit.transform.SetupBillboard(Camera.main);

            // 블록과 포인터 충돌이 일어났다면 해당 블록의 offset 위치에 고정시킨다.
            if (_targetBlock != null)
            {
                Vector3 vec = _targetBlock.transform.position;
                vec.y += 1.2f;
                vec.z -= 0.5f;
                _summonUnit.transform.position = vec;
            }
            else
            {
                Vector3 vec = Camera.main.GetTouchVector();
                vec.y += 1.2f;
                vec.z -= 0.5f;
                _summonUnit.transform.position = vec;
            }
        }

        /// <summary>
        /// 적 유닛 소환 로직
        /// </summary>
        private void SummonEnemyUnit()
        {
            // 소환간격 시간이 충족된다면 적 유닛을 소환한다.
            _summonDelta += Time.deltaTime;
            if (_summonDelta >= _summonInterval)
            {
                _summonDelta = 0f;
                int idx = test_enemy_idx[Random.Range(0, 10)];
                var um = Model.First<UnitModel>();
                if (um != null)
                {
                    var unit = um.unitTable.Find(e => e.code == idx);
                    if (unit != null)
                    {
                        UnitInfo_Defence u = _enemyList.Find(e => e.gameObject.activeSelf == false && e.Status.code == unit.code);
                        if (u != null)
                        {
                            u.gameObject.SetActive(true);
                            u.transform.SetAsFirstSibling();
                            UnitStatus status = new UnitStatus(unit, 0, 1, 1);
                            UnitFSM fsm = u.GetComponent<UnitFSM>();
                            u.Init(Constant.Team.Red, status, fsm);

                            status = null;
                            fsm = null;
                        }
                        else
                        {
                            GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", unit.unitName));
                            GameObject obj = GameObject.Instantiate(prefab);
                            obj.transform.SetParent(transform);
                            obj.transform.SetAsFirstSibling();
                            obj.transform.localScale = Vector3.one * 0.5f;

                            u = obj.AddComponent<UnitInfo_Defence>();
                            UnitStatus status = new UnitStatus(unit, 0, 1, 1);
                            UnitFSM fsm = obj.AddComponent<UnitFSM>();
                            u.Init(Constant.Team.Red, status, fsm);

                            _enemyList.Add(u);

                            prefab = null;
                            obj = null;
                        }

                        u = null;
                    }
                }
            }
        }
        private List<int> test_enemy_idx = new List<int>() { 100211, 101031, 101131, 102831, 103031, 104431, 104711, 104931, 105911, 106011 };
    }
}