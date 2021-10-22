// ==================================================
// UnitCardSlot_Defence.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitCardSlot_Defence : ICardSlot
{
    [Header("Additional")]
    [SerializeField] private EventTrigger _trigger;
    private int _needPoint;
    [SerializeField] private Text _needPointText;
    private int _totalPoint;

    public override void Init(UnitStatus status)
    {
        base.Init(status);

        if (_status == null)
        {
            _needPointText.gameObject.SetActive(false);
            return;
        }

        _needPoint = Random.Range(5, 15);
        _needPointText.text = _needPoint.ToString();

        // EventTrigger 동작 정의
        AddEventCallback();

        // Message 등록
        Message.AddListener<Battle.Defence.InitSummonPointMsg>(OnInitSummonPoint);
        Message.AddListener<Battle.Defence.AddSummonPointMsg>(OnAddSummonPoint);
        Message.AddListener<Battle.Defence.RemoveSummonPointMsg>(OnRemoveSummonPoint);
    }

    public override void Release()
    {
        base.Release();

        _trigger.triggers.Clear();
        _trigger = null;
        _needPointText = null;

        Message.RemoveListener<Battle.Defence.InitSummonPointMsg>(OnInitSummonPoint);
        Message.RemoveListener<Battle.Defence.AddSummonPointMsg>(OnAddSummonPoint);
        Message.RemoveListener<Battle.Defence.RemoveSummonPointMsg>(OnRemoveSummonPoint);
    }

    public override void Refresh()
    {
        if (_totalPoint < _needPoint)
        {
            // _portrait.SetGrayColor(true);
            _portrait.SetGrayScaleWithChild(true);
            _needPointText.color = Color.red;
            return;
        }
        else
        {
            // _portrait.SetGrayColor(false);
            _portrait.SetGrayScaleWithChild(false);
            _needPointText.color = Color.white;
        }
    }

    private void AddEventCallback()
    {
        // Begin Drag 정의
        EventTrigger.Entry bDrag = new EventTrigger.Entry();
        bDrag.eventID = EventTriggerType.BeginDrag;
        bDrag.callback.AddListener((eventData) => { OnBeginDrag(); });
        _trigger.triggers.Add(bDrag);

        // End Drag 정의
        EventTrigger.Entry eDrag = new EventTrigger.Entry();
        eDrag.eventID = EventTriggerType.EndDrag;
        eDrag.callback.AddListener((eventData) => { OnEndDrag(); });
        _trigger.triggers.Add(eDrag);
    }

    private void OnBeginDrag()
    {
        if (_totalPoint < _needPoint)
            return;

        Message.Send<Battle.Defence.SummonDragStartMsg>(new Battle.Defence.SummonDragStartMsg(_status.code));
    }

    private void OnEndDrag()
    {
        if (_totalPoint < _needPoint)
            return;

        Message.Send<Battle.Defence.SummonDragEndMsg>(new Battle.Defence.SummonDragEndMsg(_needPoint));
    }

    private void OnInitSummonPoint(Battle.Defence.InitSummonPointMsg msg)
    {
        _totalPoint = msg.SummonPoint;
    }

    private void OnAddSummonPoint(Battle.Defence.AddSummonPointMsg msg)
    {
        _totalPoint++;
    }

    private void OnRemoveSummonPoint(Battle.Defence.RemoveSummonPointMsg msg)
    {
        _totalPoint -= msg.Price;
    }
}
