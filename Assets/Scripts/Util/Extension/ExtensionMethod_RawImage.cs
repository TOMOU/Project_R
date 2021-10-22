// ==================================================
// ExtensionMethod_RawImage.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static partial class ExtensionMethod
{
    public static void SetAlpha(this RawImage image, Color s, Color e, float t)
    {
        image.color = Color.Lerp(s, e, t);
    }

    public static void SetAlphaWithChild(this RawImage image, Color s, Color e, float t)
    {
        List<RawImage> list = image.transform.GetChildAll<RawImage>();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].color = Color.Lerp(s, e, t);
        }

        list.Clear();
        list = null;
    }

    /// <summary>
    /// 시간에 따라 색상값을 변경
    /// <para>(시간관련 변수값은 최대 1f까지 가능))</para>
    /// </summary>
    /// <param name="image"></param>
    /// <param name="s">시작 색상</param>
    /// <param name="e">종료 색상</param>
    /// <param name="t">시간값 (최대 1f)</param>
    public static void SetColorWithChild(this RawImage image, Color s, Color e, float t)
    {
        List<RawImage> list = image.transform.GetChildAll<RawImage>();
        for (int i = 0; i < list.Count; i++)
        {
            list[i].color = Color.Lerp(s, e, t);
        }

        list.Clear();
        list = null;
    }

    /// <summary>
    /// 이미지를 회색으로 전환할지?
    /// </summary>
    /// <param name="image">적용할 이미지</param>
    /// <param name="isEnable">true -> 회색으로 변경</param>
    public static void SetGrayColor(this RawImage image, bool isEnable)
    {
        image.color = (isEnable == true ? Color.gray : Color.white);
    }

    /// <summary>
    /// 이미지를 회색으로 전환할지?
    /// </summary>
    /// <param name="image">적용할 이미지</param>
    /// <param name="isEnable">true -> 회색으로 변경</param>
    public static void SetGrayColorWithChild(this RawImage image, bool isEnable)
    {
        // 모든 하위의 Image Component 정보를 얻는다.
        List<RawImage> list = image.transform.GetChildAll<RawImage>();

        // 개별로 GrayColor 적용
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetGrayColor(isEnable);
        }

        list.Clear();
        list = null;
    }

    /// <summary>
    /// 그레이스케일을 적용할지?
    /// </summary>
    /// <param name="image">적용할 이미지</param>
    /// <param name="isEnable">true -> Grayscale On</param>
    public static void SetGrayScale(this RawImage image, bool isEnable)
    {
        if (image.material.name != "Grayscale")
            image.material = new Material(Shader.Find("Grayscale"));

        image.material.SetFloat("_GrayscaleAmount", isEnable == true ? 1f : 0f);
    }

    /// <summary>
    /// 그레이 스케일을 적용할지?
    /// <para>(하위의 이미지들도 전부 포함)</para>
    /// </summary>
    /// <param name="image"></param>
    /// <param name="isEnable">true -> Grayscale On</param>
    public static void SetGrayScaleWithChild(this RawImage image, bool isEnable)
    {
        // 하위의 모든 Image Component 정보를 모은다.
        List<RawImage> list = image.transform.GetChildAll<RawImage>();

        // 개별로 Grayscale 적용
        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetGrayScale(isEnable);
        }

        list.Clear();
        list = null;
    }
}
