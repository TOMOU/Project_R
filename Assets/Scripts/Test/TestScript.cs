// ==================================================
// TestScript.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TestScript : MonoBehaviour
{
    public Transform Root;

    public void OnEnable()
    {
        if (Root == null)
            return;

        Recursive(Root);
    }

    private void Recursive(Transform root)
    {
        ParticleSystemRenderer ps = root.GetComponent<ParticleSystemRenderer>();
        if (ps != null)
        {
            if (ps.renderMode != ParticleSystemRenderMode.None && ps.renderMode != ParticleSystemRenderMode.Stretch)
            {
                ps.renderMode = ParticleSystemRenderMode.Billboard;
                ps.alignment = ParticleSystemRenderSpace.Local;
            }
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Recursive(root.GetChild(i));
        }
    }
}
