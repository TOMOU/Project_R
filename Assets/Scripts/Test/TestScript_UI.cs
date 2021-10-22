// ==================================================
// TestScript_UI.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections.Generic;
using UnityEngine;

public class TestScript_UI : MonoBehaviour
{
    [SerializeField] private List<Animator> animList;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (animList == null)
        {
            animList = new List<Animator>();
        }

        AddAnimator(transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SendTrigger("OnEnter");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SendTrigger("OnExit");
        }
    }

    private void AddAnimator(Transform trans)
    {
        foreach (Transform child in trans)
        {
            Animator anim = child.GetComponent<Animator>();
            if (anim != null)
            {
                animList.Add(anim);
            }

            AddAnimator(child);
        }
    }

    private void SendTrigger(string key)
    {
        foreach (var p in animList)
        {
            p.SetTrigger(key);
        }
    }
}
