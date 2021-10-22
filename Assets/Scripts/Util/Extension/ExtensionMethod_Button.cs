// ==================================================
// ExtensionMethod_Button.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public static partial class ExtensionMethod
{
    /// <summary>
    /// 이미지를 회색으로 전환할지?
    /// </summary>
    /// <param name="button">적용할 이미지</param>
    /// <param name="isEnable">true -> 회색으로 변경</param>
    public static void SetGrayColor(this Button button, bool isEnable)
    {
        button.interactable = !isEnable;
        button.image.SetGrayColor(isEnable);
    }

    /// <summary>
    /// 그레이스케일을 적용할지?
    /// </summary>
    /// <param name="button">적용할 이미지</param>
    /// <param name="isEnable">true -> 그레이스케일 on</param>
    public static void SetGrayScale(this Button button, bool isEnable)
    {
        button.interactable = !isEnable;
        button.image.SetGrayScale(isEnable);
    }

    /// <summary>
    /// 이미지를 회색으로 전환할지?
    /// </summary>
    /// <param name="button">적용할 이미지</param>
    /// <param name="isEnable">true -> 회색으로 변경</param>
    public static void SetGrayColorWithChild(this Button button, bool isEnable)
    {
        button.interactable = !isEnable;
        button.image.SetGrayColorWithChild(isEnable);
    }

    /// <summary>
    /// 그레이스케일을 적용할지?
    /// </summary>
    /// <param name="button">적용할 이미지</param>
    /// <param name="isEnable">true -> 그레이스케일 on</param>
    public static void SetGrayScaleWithChild(this Button button, bool isEnable)
    {
        button.interactable = !isEnable;
        button.image.SetGrayScaleWithChild(isEnable);
    }

    public static void AddCallback(this Button button, System.Action callback)
    {
        button.onClick.AddListener(() => callback());

        // Button3D b = button.GetComponentInChildren<Button3D>();
        // if (b != null)
        // {
        //     b.AddCallback(callback);
        // }
    }
}
