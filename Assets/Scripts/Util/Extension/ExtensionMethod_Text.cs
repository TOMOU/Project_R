// ==================================================
// ExtensionMethod_Text.cs
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
    public static void SetGrayColor(this Text text, bool isEnable)
    {
        text.color = (isEnable == true ? Color.gray : Color.white);
    }

    public static void SetGrayScale(this Text text, bool isEnable)
    {
        if (text.material.name != "Grayscale")
            text.material = new Material(Shader.Find("Grayscale"));

        text.material.SetFloat("_GrayscaleAmount", isEnable == true ? 0f : 1f);
    }
}
