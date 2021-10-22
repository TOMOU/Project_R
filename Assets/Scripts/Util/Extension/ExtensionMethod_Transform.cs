// ==================================================
// ExtensionMethod_Transform.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections.Generic;

public static partial class ExtensionMethod
{
    /// <summary>
    /// 하위의 모든 Transform에 대한 정보를 얻는다.
    /// </summary>
    /// <param name="root">루트가 되는 Transform</param>
    /// <returns></returns>
    public static List<Transform> GetChildAll(this Transform root)
    {
        // new List를 생성하고 루트 Transform을 넣는다.
        List<Transform> result = new List<Transform>();
        result.Add(root);

        // 하위의 Transform을 모두 찾는다.
        for (int i = 0; i < root.childCount; i++)
        {
            result.AddRange(root.GetChild(i).GetChildAll());
        }

        return result;
    }

    /// <summary>
    /// 하위의 모든 Transform에 대해 T를 포함하고 있는지 찾는다.
    /// </summary>
    /// <param name="root"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> GetChildAll<T>(this Transform root)
    {
        // new List를 생성하고 T를 포함하고 있다면 루트 Transform을 넣는다.
        List<T> result = new List<T>();
        T component = root.GetComponent<T>();
        if (component != null)
        {
            result.Add(component);
        }

        // 하위의 Transform이 T를 포함하고 있는지 재귀 탐색으로 찾는다.
        for (int i = 0; i < root.childCount; i++)
        {
            result.AddRange(root.GetChild(i).GetChildAll<T>());
        }

        return result;
    }

    /// <summary>
    /// 해당 Transform을 Camera에 대해 빌보드 형식을 취하도록 한다
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="camera"></param>
    public static void SetupBillboard(this Transform transform, Camera camera)
    {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }

    public static void MoveTo(this Transform my, Transform target, float speed, float attackRange, Spine.Unity.SkeletonAnimation skeleton)
    {
        // 이동할 최종 목적지를 지정
        Vector3 dest = target.position;

        // 방향벡터 선언 (z축은 0으로 지정)
        Vector3 dir = dest - my.position;
        dir.Normalize();

        if (dir.x < 0 && skeleton.skeleton.ScaleX == 1f)
            skeleton.skeleton.ScaleX = -1f;
        else if (dir.x > 0 && skeleton.skeleton.ScaleX == -1f)
            skeleton.skeleton.ScaleX = 1f;

        Vector3 pos = my.transform.position;
        pos += dir * speed * Time.deltaTime;
        pos.z = pos.y;

        // 내 위치 이동
        my.transform.position = pos;
    }

    public static void MoveToX(this Transform my, Transform target, float speed, float attackRange, Spine.Unity.SkeletonAnimation skeleton)
    {
        // 이동할 최종 목적지를 지정
        Vector3 dest = target.position;

        // 방향벡터 선언 (z축은 0으로 지정)
        Vector3 dir = dest - my.position;
        dir.y = 0f;
        dir.z = 0f;
        dir.Normalize();

        if (dir.x < 0 && skeleton.skeleton.ScaleX == 1f)
            skeleton.skeleton.ScaleX = -1f;
        else if (dir.x > 0 && skeleton.skeleton.ScaleX == -1f)
            skeleton.skeleton.ScaleX = 1f;

        Vector3 pos = my.transform.position;
        pos += dir * speed * Time.deltaTime;
        pos.z = pos.y;

        // 내 위치 이동
        my.transform.position = pos;
    }

    public static void MoveToY(this Transform my, Transform target, float speed, float attackRange, Spine.Unity.SkeletonAnimation skeleton)
    {
        // 이동할 최종 목적지를 지정
        Vector3 dest = target.position;

        // 방향벡터 선언 (z축은 0으로 지정)
        Vector3 dir = dest - my.position;

        if (Mathf.Abs(dir.x) < attackRange)
            dir.x = 0f;

        dir.Normalize();

        if (dir.x < 0 && skeleton.skeleton.ScaleX == 1f)
            skeleton.skeleton.ScaleX = -1f;
        else if (dir.x > 0 && skeleton.skeleton.ScaleX == -1f)
            skeleton.skeleton.ScaleX = 1f;

        Vector3 pos = my.transform.position;
        pos += dir * speed * Time.deltaTime;
        pos.z = pos.y;

        // 내 위치 이동
        my.transform.position = pos;
    }
}
