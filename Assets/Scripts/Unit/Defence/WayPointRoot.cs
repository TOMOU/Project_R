// ==================================================
// WayPointRoot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class WayPointRoot : MonoBehaviour
{
    /// <summary>
    /// 노드의 Root Group번호
    /// </summary>
    /// <value></value>
    [SerializeField] private int _group;
    /// <summary>
    /// 하위 노드 리스트
    /// </summary>
    /// <value></value>
    [SerializeField] private List<WayPointNode> _nodeList;
    /// <summary>
    /// 첫번째 노드의 정보
    /// </summary>
    /// <value></value>
    public WayPointNode First { get { return _nodeList[0]; } }

    private void Awake()
    {
        Message.AddListener<Battle.Defence.FindFirstNodeMsg>(OnFindFirstNode);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<Battle.Defence.FindFirstNodeMsg>(OnFindFirstNode);
    }

    private void OnEnable()
    {
        if (_nodeList != null)
        {
            _nodeList.Clear();
            _nodeList = null;
        }

        _nodeList = new List<WayPointNode>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf == false)
                continue;

            WayPointNode point = child.GetComponent<WayPointNode>();
            if (point != null)
            {
                _nodeList.Add(point);
            }
        }

        for (int i = 0; i < _nodeList.Count; i++)
        {
            _nodeList[i].Init(_group, i, (i < _nodeList.Count - 1) ? _nodeList[i + 1] : null);
        }
    }

    private void OnFindFirstNode(Battle.Defence.FindFirstNodeMsg msg)
    {
        // 메세지로 받은 그룹이 자신의 그룹과 동일하면 First를 반환한다.
        if (msg.Group == _group)
        {
            msg.Action(First);
        }
    }
}
