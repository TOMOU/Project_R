// ==================================================
// ExtensionMethod_Spine.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using Spine.Unity;

public static partial class ExtensionMethod
{
    public static void SetOpacity(this SkeletonAnimation skeleton, float s, float e, float t)
    {
        foreach (Spine.Slot slot in skeleton.skeleton.Slots)
        {
            slot.R = Mathf.Lerp(s, e, t);
            slot.G = Mathf.Lerp(s, e, t);
            slot.B = Mathf.Lerp(s, e, t);
            slot.A = Mathf.Lerp(s, e, t);
        }
    }
}
