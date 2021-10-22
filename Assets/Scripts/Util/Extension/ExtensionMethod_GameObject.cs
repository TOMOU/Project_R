// ==================================================
// ExtensionMethod_GameObject.cs
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
    /// 점진적으로 레이어 변경
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layerName"></param>
    public static void SetLayerRecursively(this GameObject gameObject, string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);

        Transform transform = gameObject.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            SetLayerRecursively(transform.GetChild(i).gameObject, layerName);
        }
    }

    /// <summary>
    /// 카메라를 항상 빌보드 방식으로 바라보게 수정
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="camera"></param>
    public static void SetBillboard(this GameObject gameObject, Camera camera)
    {
        gameObject.transform.LookAt(gameObject.transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }

    public static void SetAlphaWithChild(this GameObject gameObject, float alpha)
    {
        UnityEngine.UI.Image img = gameObject.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }

        UnityEngine.UI.Text text = gameObject.GetComponent<UnityEngine.UI.Text>();
        if (text != null)
        {
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            SetAlphaWithChild(gameObject.transform.GetChild(i).gameObject, alpha);
        }
    }
}
