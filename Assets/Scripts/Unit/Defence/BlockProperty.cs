// ==================================================
// BlockProperty.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.EventSystems;

public class BlockProperty : MonoBehaviour
{
    [SerializeField] private Constant.Block _property;
    public Constant.Block Property { get { return _property; } }
    private bool _enableSummon;
    private Renderer _renderer;
    private UnitInfo_Defence _unitSlot = null;

    private void Start()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }

        Message.AddListener<Battle.Defence.SummonDragStartMsg>(OnSummonDragStart);
        Message.AddListener<Battle.Defence.SummonDragEndMsg>(OnSummonDragEnd);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<Battle.Defence.SummonDragStartMsg>(OnSummonDragStart);
        Message.RemoveListener<Battle.Defence.SummonDragEndMsg>(OnSummonDragEnd);
    }

    private void OnSummonDragStart(Battle.Defence.SummonDragStartMsg msg)
    {
        if (_unitSlot != null)
            return;

        if (_property != Constant.Block.Installable)
            return;

        _enableSummon = true;
        _renderer.material.color = Color.green;
    }

    private void OnSummonDragEnd(Battle.Defence.SummonDragEndMsg msg)
    {
        _enableSummon = false;
        _renderer.material.color = Color.white;
    }

    /// <summary>
    /// 터치 포인터가 오브젝트에 닿았을때의 동작
    /// UI를 통해 소환상태의 로직일 경우 선택된 위치의 Material을 파란색(현재 캐릭터의 소환 예정 표시)으로 변경한다
    /// </summary>
    private void OnMouseEnter()
    {
        if (_enableSummon == true)
        {
            _renderer.material.color = Color.blue;

            Message.Send<Battle.Defence.DragEnterInBlockMsg>(new Battle.Defence.DragEnterInBlockMsg(this));
        }
    }

    /// <summary>
    /// 터치 포인터가 오브젝트에서 나갔을때의 동작
    /// UI를 통해 소환상태의 로직일 경우 선택된 위치의 Material을 초록색(기본 소환범위 표시)으로 변경한다
    /// </summary>
    private void OnMouseExit()
    {
        if (_enableSummon == true)
        {
            _renderer.material.color = Color.green;

            Message.Send<Battle.Defence.DragExitInBlockMsg>(new Battle.Defence.DragExitInBlockMsg(this));
        }
    }

    public void InsertUnitSlot(UnitInfo_Defence unit)
    {
        this._unitSlot = unit;
    }

    private void OnDrawGizmos()
    {
        if (_property == Constant.Block.Installable)
            Gizmos.color = Color.green;
        else if (_property == Constant.Block.UnableToInstall)
            Gizmos.color = Color.gray;
        else if (_property == Constant.Block.Route)
            Gizmos.color = Color.white;
        else if (_property == Constant.Block.Blue_Goal)
            Gizmos.color = Color.blue;
        else if (_property == Constant.Block.Red_Spawn)
            Gizmos.color = Color.red;
        else if (_property == Constant.Block.None)
            Gizmos.color = Color.magenta;

        Gizmos.DrawCube(transform.position, Vector3.one * 2f);
    }
}
