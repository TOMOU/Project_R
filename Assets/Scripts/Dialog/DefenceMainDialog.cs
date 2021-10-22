// ==================================================
// DefenceMainDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace Dialog
{
    public class DefenceMainDialog : IDialog
    {
        [Header("유닛 슬롯 정보")]
        [SerializeField] private Transform _unitSlotBoard;
        [SerializeField] private List<UnitCardSlot_Defence> _slotList;

        [Header("맵 상 유닛의 체력 정보")]
        [SerializeField] private Transform _unitHPBarPool;
        [SerializeField] private List<HpBar> _unitHPBarList;

        [Header("유닛 소환 가능정보")]
        private bool _refreshStart;
        private int _summonPoint;
        [SerializeField] private Text _summonPointText;

        [Header("게임 현황")]
        private int _curWaveCountValue;
        private int _totalWaveCountValue;
        [SerializeField] private Text _waveCountText;
        private int _lifeCountValue;
        [SerializeField] private Text _lifeCountText;

        [Header("기타")]
        [SerializeField] private Button _fastOffButton;
        [SerializeField] private Button _fastOnButton;
        [SerializeField] private Button _pauseButton;

        protected override void OnLoad()
        {
            _refreshStart = false;

            // _unitInfoList를 초기화하고 슬롯 리스틓를 가져온다.
            _slotList = new List<UnitCardSlot_Defence>();
            for (int i = 0; i < _unitSlotBoard.childCount; i++)
            {
                UnitCardSlot_Defence unit = _unitSlotBoard.GetChild(i).GetComponent<UnitCardSlot_Defence>();
                if (unit != null)
                {
                    _slotList.Add(unit);
                }
            }

            Info.Inventory userInfo = Info.My.Singleton.Inventory;

            // SlotList에 장착한 유닛 정보를 업데이트한다.
            for (int i = 0; i < _slotList.Count; i++)
            {
                if (i < userInfo.characterList.Count)
                    _slotList[i].Init(userInfo.characterList[i].GetStatus());
                else
                    _slotList[i].Init(null);
            }

            // UnitHPBarList를 초기화하고 기본 여유분으로 10개를 만들어둔다.
            _unitHPBarList = new List<HpBar>();
            for (int i = 0; i < 10; i++)
            {
                GameObject prefab = Resources.Load<GameObject>("UI/Battle/HpBar");
                GameObject obj = GameObject.Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(_unitHPBarPool);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one * 0.7f;
                HpBar bar = obj.GetComponent<HpBar>();
                _unitHPBarList.Add(bar);

                prefab = null;
                obj = null;
                bar = null;
            }

            // Button Listener 등록
            bool fastMode = TimeManager.Singleton.FastMode;
            _fastOffButton.onClick.AddListener(OnClickFastMode);
            _fastOnButton.onClick.AddListener(OnClickFastMode);
            _pauseButton.onClick.AddListener(OnClickPause);

            _fastOffButton.gameObject.SetActive(fastMode == false);
            _fastOnButton.gameObject.SetActive(fastMode == true);
        }

        protected override void OnUnload()
        {
            _unitSlotBoard = null;
            if (_slotList != null)
            {
                _slotList.Clear();
                _slotList = null;
            }

            _unitHPBarPool = null;
            if (_unitHPBarList != null)
            {
                _unitHPBarList.Clear();
                _unitHPBarList = null;
            }

            _summonPointText = null;

            _waveCountText = null;
            _lifeCountText = null;

            _fastOffButton.onClick.RemoveAllListeners();
            _fastOffButton = null;
            _fastOnButton.onClick.RemoveAllListeners();
            _fastOnButton = null;
            _pauseButton.onClick.RemoveAllListeners();
            _pauseButton = null;
        }

        protected override void OnEnter()
        {
            Message.AddListener<Battle.Defence.ShowHPBarMsg>(OnShowHPBar);
            Message.AddListener<Battle.Defence.InitSummonPointMsg>(OnChangeSummonPoint);
            Message.AddListener<Battle.Defence.AddSummonPointMsg>(OnAddSummonPoint);
            Message.AddListener<Battle.Defence.RemoveSummonPointMsg>(OnRemoveSummonPoint);
            Message.AddListener<Battle.Defence.InitReportBoardMsg>(OnInitReportBoard);
            Message.AddListener<Battle.Defence.KillEnemyMsg>(OnKillEnemy);
            Message.AddListener<Battle.Defence.PassEnemyMsg>(OnPassEnemy);
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Battle.Defence.ShowHPBarMsg>(OnShowHPBar);
            Message.RemoveListener<Battle.Defence.InitSummonPointMsg>(OnChangeSummonPoint);
            Message.RemoveListener<Battle.Defence.AddSummonPointMsg>(OnAddSummonPoint);
            Message.RemoveListener<Battle.Defence.RemoveSummonPointMsg>(OnRemoveSummonPoint);
            Message.RemoveListener<Battle.Defence.InitReportBoardMsg>(OnInitReportBoard);
            Message.RemoveListener<Battle.Defence.KillEnemyMsg>(OnKillEnemy);
            Message.RemoveListener<Battle.Defence.PassEnemyMsg>(OnPassEnemy);
        }

        private void Update()
        {
            if (_refreshStart == false)
                return;

            if (_slotList == null)
                return;

            for (int i = 0; i < _slotList.Count; i++)
            {
                _slotList[i].Refresh();
            }
        }

        private void OnInitReportBoard(Battle.Defence.InitReportBoardMsg msg)
        {
            _curWaveCountValue = 0;
            _totalWaveCountValue = msg.WaveCount;
            _lifeCountValue = msg.LifeCount;

            _waveCountText.text = string.Format("{0}/{1}", _curWaveCountValue, _totalWaveCountValue);
            _lifeCountText.text = string.Format("{0}", _lifeCountValue);
        }

        private void OnKillEnemy(Battle.Defence.KillEnemyMsg msg)
        {
            // 적을 Kill 하였으니 적 waveCount만 증가시켜 준다.
            // 플레이어의 라이프 수는 그대로
            _curWaveCountValue++;
            _waveCountText.text = string.Format("{0}/{1}", _curWaveCountValue, _totalWaveCountValue);

            // if (_lifeCountValue == 0 || _curWaveCountValue == _totalWaveCountValue)
            //     return;

            // _curWaveCountValue++;
            // _waveCountText.text = string.Format("{0}/{1}", _curWaveCountValue, _totalWaveCountValue);
        }

        private void OnPassEnemy(Battle.Defence.PassEnemyMsg msg)
        {
            // 적이 통과하였으니 waveCount와 플레이어 라이프를 동시에 삭감한다.
            _curWaveCountValue++;
            _lifeCountValue--;

            _waveCountText.text = string.Format("{0}/{1}", _curWaveCountValue, _totalWaveCountValue);
            _lifeCountText.text = string.Format("{0}", _lifeCountValue);

            // if (_lifeCountValue == 0 || _curWaveCountValue == _totalWaveCountValue)
            //     return;

            // _lifeCountValue--;
            // _lifeCountText.text = string.Format("{0}", _lifeCountValue);
        }

        private void OnShowHPBar(Battle.Defence.ShowHPBarMsg msg)
        {
            // 예외처리
            if (msg.Info == null)
                return;

            // _unitHPBarList 중에 활성화되지 않은 녀석을 찾는다.
            HpBar bar = _unitHPBarList.Find(e => e.Enabled == false);

            // 비활성화된 녀석이 없으므로 새로 생성&추가해준다.
            if (bar == null)
            {
                GameObject prefab = Resources.Load<GameObject>("UI/Battle/HpBar");
                GameObject obj = GameObject.Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(_unitHPBarPool);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one * 0.7f;
                bar = obj.GetComponent<HpBar>();
                _unitHPBarList.Add(bar);

                prefab = null;
                obj = null;
            }

            bar.ShowImage(msg.Info, msg.Damage);
            bar = null;
        }

        private void OnChangeSummonPoint(Battle.Defence.InitSummonPointMsg msg)
        {
            _summonPoint = msg.SummonPoint;
            _summonPointText.text = string.Format("{0}", _summonPoint);
            _refreshStart = true;
        }

        private void OnAddSummonPoint(Battle.Defence.AddSummonPointMsg msg)
        {
            _summonPoint++;
            _summonPointText.text = string.Format("{0}", _summonPoint);
        }

        private void OnRemoveSummonPoint(Battle.Defence.RemoveSummonPointMsg msg)
        {
            _summonPoint -= msg.Price;
            _summonPointText.text = string.Format("{0}", _summonPoint);
        }

        private void OnClickFastMode()
        {
            // 현재의 빠르기 상태
            bool isFastMode = TimeManager.Singleton.FastMode;

            // 매니저에 값 전송
            if (isFastMode == true)
            {
                Message.Send<Global.NormalSpeedMsg>(new Global.NormalSpeedMsg());
                isFastMode = false;
            }
            else
            {
                Message.Send<Global.FastSpeedMsg>(new Global.FastSpeedMsg());
                isFastMode = true;
            }

            // 버튼 상태 바꾸기
            _fastOffButton.gameObject.SetActive(isFastMode == false);
            _fastOnButton.gameObject.SetActive(isFastMode == true);
        }

        private void OnClickPause()
        {
            RequestDialogEnter<DefencePauseDialog>();
        }
    }
}