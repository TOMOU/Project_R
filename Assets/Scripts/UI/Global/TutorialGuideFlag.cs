// ==================================================
// TutorialGuideFlag.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class TutorialGuideFlag : MonoBehaviour
{
    // [Serializable]
    // public class Flag
    // {
    //     [Tooltip("표시되어야 하는 시점의 Scene 인덱스.\n만약 3번째 씬 그룹에 나와야 한다면 => 3")] public int sceneIndex;
    //     public Constant.TutorialActiveType activeType;
    //     public Constant.TutorialCallbackType callback;
    //     public Flag(int scene, Constant.TutorialActiveType tutorialActiveType, Constant.TutorialCallbackType callbackType)
    //     {
    //         this.sceneIndex = scene;
    //         this.activeType = tutorialActiveType;
    //         this.callback = callbackType;
    //     }
    // }

    // [SerializeField] private List<Flag> _flagList;
    // [Space(20)]
    // public Vector2 anchorPosOffset;

    // private void Awake()
    // {
    //     RegisterFlag();
    // }

    // public void Add(int scene, Constant.TutorialActiveType tutorialActiveType, Constant.TutorialCallbackType callbackType)
    // {
    //     if (_flagList == null)
    //         _flagList = new List<Flag>();

    //     _flagList.Add(new Flag(scene, tutorialActiveType, callbackType));
    // }

    // public void RegisterFlag()
    // {
    //     if (_flagList == null)
    //         return;

    //     for (int i = 0; i < _flagList.Count; i++)
    //     {
    //         TutorialManager.Singleton.AddGuideFlag(_flagList[i].sceneIndex, _flagList[i].activeType, _flagList[i].callback, this);
    //     }
    // }
}