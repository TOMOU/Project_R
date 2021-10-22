// ==================================================
// TutorialManager.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    public bool IsTutorialProcess
    {
        get
        {
            if (_tutorialDialogView == null)
                return false;
            return _tutorialDialogView.Find(e => e.activeSelf == true) != null;
        }
    }
    public int curTutorialIndex = 0;
    private List<GameObject> _tutorialDialogView;
    public Constant.TutorialCallbackType openFirst = Constant.TutorialCallbackType.None;

    protected override void Init()
    {
        curTutorialIndex = PlayerPrefs.GetInt("curTutorialIndex");

        Message.AddListener<Global.ShowScenarioTextMsg>(OnShowScenarioText);
    }

    protected override void Release()
    {
        if (_tutorialDialogView != null)
        {
            _tutorialDialogView.Clear();
            _tutorialDialogView = null;
        }

        Message.RemoveListener<Global.ShowScenarioTextMsg>(OnShowScenarioText);
    }

    private void OnShowScenarioText(Global.ShowScenarioTextMsg msg)
    {
        if (msg.scenarioName != "Main")
            return;

        curTutorialIndex = msg.sceneIndex + 1;
        PlayerPrefs.SetInt("curTutorialIndex", curTutorialIndex);
    }

    public void AddGuideDialog(GameObject obj)
    {
        if (_tutorialDialogView == null)
            _tutorialDialogView = new List<GameObject>();
        _tutorialDialogView.Add(obj);
    }
}