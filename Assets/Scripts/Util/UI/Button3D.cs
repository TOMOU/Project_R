// ==================================================
// Button3D.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class Button3D : MonoBehaviour
{
    private EventTrigger _trigger;
    private System.Action _callback;

    private void Awake()
    {
        // MeshCollider 추가.
        AddCollider(transform);

        // EventTrigger 캐싱
        _trigger = GetComponent<EventTrigger>();

        // Event 생성
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnPointerClickDelegate((PointerEventData)data); });

        // EventTrigger에 Event 연결.
        _trigger.triggers.Add(entry);
    }

    private void OnPointerClickDelegate(PointerEventData data)
    {
        if (InputManager.Singleton.IsLock() == true)
            return;

        if (_callback != null)
            _callback();
    }

    public void AddCallback(System.Action callback)
    {
        this._callback = callback;
    }

    /// <summary>
    /// 현재 및 하위에 재귀적으로 MeshCollider 추가
    /// </summary>
    /// <param name="trans">적용할 루트 트랜스폼</param>
    private void AddCollider(Transform trans)
    {
        // 렌더러가 있으면, 컬라이더 추가.
        Renderer r = trans.GetComponent<Renderer>();
        if (r != null)
        {
            MeshCollider col = trans.gameObject.GetComponent<MeshCollider>();
            if (col == null)
            {
                trans.gameObject.AddComponent<MeshCollider>();
            }
        }

        // 재귀로 적용.
        foreach (Transform t in trans)
        {
            AddCollider(t);
        }
    }
}