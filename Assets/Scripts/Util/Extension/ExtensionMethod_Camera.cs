// ==================================================
// ExtensionMethod_Camera.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

public static partial class ExtensionMethod
{
    /// <summary>
    /// 터치포인터 위치에 해당하는 터치된 지점 Vector를 반환
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Vector3 GetTouchVector(this Camera camera)
    {
        Vector3 vec = Vector3.zero;
        RaycastHit raycastHit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out raycastHit))
        {
            vec = raycastHit.point;
        }

        return vec;
    }

    public static Vector3 WorldToNormalizedViewportPoint(this Camera camera, Vector3 point)
    {
        point = camera.WorldToViewportPoint(point);

        if (camera.orthographic)
        {
            point.z = (2 * (point.z - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) - 1f;
        }
        else
        {
            point.z = ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) +
                (1 / point.z) * (-2 * camera.farClipPlane * camera.nearClipPlane / (camera.farClipPlane - camera.nearClipPlane));
        }

        return point;
    }

    public static Vector3 NormalizedViewportToWorldPoint(this Camera camera, Vector3 point)
    {
        if (camera.orthographic)
        {
            point.z = (point.z + 1f) * (camera.farClipPlane - camera.nearClipPlane) * 0.5f + camera.nearClipPlane;
        }
        else
        {
            point.z = ((-2 * camera.farClipPlane * camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) /
                (point.z - ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)));
        }

        return camera.ViewportToWorldPoint(point);
    }
}
