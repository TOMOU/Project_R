// ==================================================
// WayPointNode.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

public class WayPointNode : MonoBehaviour
{
    /// <summary>
    /// 현재 노드의 Group
    /// </summary>
    /// <value></value>
    [SerializeField] private int _group;
    /// <summary>
    /// 현재 노드의 인덱스
    /// </summary>
    /// <value></value>
    [SerializeField] private int _idx;
    /// <summary>
    /// 다음 노드에 대한 정보
    /// </summary>
    /// <value></value>
    [SerializeField] private WayPointNode _next;
    /// <summary>
    /// 다음 노드에 대한 정보
    /// </summary>
    /// <value></value>
    public WayPointNode Next
    {
        get
        {
            return _next;
        }
    }

    public void Init(int group, int index, WayPointNode next)
    {
        _group = group;
        _idx = index;
        _next = next;

        gameObject.name = string.Format("point_{0}_{1}", group, index);
    }
}
