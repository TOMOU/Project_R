// ==================================================
// InputManager.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputManager : MonoSingleton<InputManager>
{
    private Stack<Action> _escapeStack;
    private bool _isLock;
    private GraphicRaycaster _rayCaster;
    private GameObject _eventSystem;
    public int count = 0;
    private float _touchTimer = 0f;

    protected override void Init()
    {
        _escapeStack = new Stack<Action>();
        _isLock = false;

        _rayCaster = GameObject.Find("UICanvas").GetComponent<GraphicRaycaster>();
        _eventSystem = GameObject.Find("EventSystem");

        Message.AddListener<Global.AddBaseEscapeActionMsg>(OnAddBaseEscapeAction);
        Message.AddListener<Global.AddEscapeActionMsg>(OnAddEscapeAction);
        Message.AddListener<Global.RemoveEscapeActionMsg>(OnRemoveEscapeAction);
        Message.AddListener<Global.RemoveEscapeActionAllMsg>(OnRemoveEscapeActionAll);
        Message.AddListener<Global.PopEscapeActionMsg>(OnPopEscapeAction);
        Message.AddListener<Global.ReturnToHomeMsg>(OnReturnToHome);
        Message.AddListener<Global.InputLockMsg>(OnInputLock);
        Message.AddListener<Global.InputUnlockMsg>(OnInputUnlock);
        Message.AddListener<Global.EscapeLockMsg>(OnEscapeLock);
        Message.AddListener<Global.EscapeUnlockMsg>(OnEscapeUnlock);
    }

    protected override void Release()
    {
        if (_escapeStack != null)
        {
            _escapeStack.Clear();
            _escapeStack = null;
        }

        Message.RemoveListener<Global.AddBaseEscapeActionMsg>(OnAddBaseEscapeAction);
        Message.RemoveListener<Global.AddEscapeActionMsg>(OnAddEscapeAction);
        Message.RemoveListener<Global.RemoveEscapeActionMsg>(OnRemoveEscapeAction);
        Message.RemoveListener<Global.RemoveEscapeActionAllMsg>(OnRemoveEscapeActionAll);
        Message.RemoveListener<Global.PopEscapeActionMsg>(OnPopEscapeAction);
        Message.RemoveListener<Global.ReturnToHomeMsg>(OnReturnToHome);
        Message.RemoveListener<Global.InputLockMsg>(OnInputLock);
        Message.RemoveListener<Global.InputUnlockMsg>(OnInputUnlock);
        Message.RemoveListener<Global.EscapeLockMsg>(OnEscapeLock);
        Message.RemoveListener<Global.EscapeUnlockMsg>(OnEscapeUnlock);
    }

    private void Update()
    {
        count = _escapeStack.Count;

        if (TutorialManager.Singleton.IsTutorialProcess == true)
            return;

        if (_isLock == true)
            return;

        // if (_eventSystem.activeSelf == false)
        // {
        //     _touchTimer += Time.deltaTime;

        //     if (_touchTimer >= 0.5f)
        //     {
        //         _touchTimer = 0f;
        //         _eventSystem.SetActive(true);
        //     }
        // }

        // if (Input.GetKeyUp(KeyCode.Mouse0) && _eventSystem.activeSelf == true)
        // {
        //     _eventSystem.SetActive(false);
        // }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPopEscapeAction(new Global.PopEscapeActionMsg());
        }
    }

    private void OnAddBaseEscapeAction(Global.AddBaseEscapeActionMsg msg)
    {
        _escapeStack.Clear();
        _escapeStack.Push(msg.Action);
    }

    private void OnAddEscapeAction(Global.AddEscapeActionMsg msg)
    {
        // 중복으로 들어가는걸 방지하기 위한 임시방책
        if (_escapeStack.Contains(msg.Action) == false)
            _escapeStack.Push(msg.Action);
    }

    private void OnPopEscapeAction(Global.PopEscapeActionMsg msg)
    {
        if (_escapeStack.Count == 0)
            return;

        Action escape = _escapeStack.Peek();
        if (escape != null)
        {
            if (_escapeStack.Count > 1)
            {
                _escapeStack.Pop();
            }

            escape();
        }

        escape = null;
    }

    private void OnRemoveEscapeAction(Global.RemoveEscapeActionMsg msg)
    {
        if (_escapeStack.Count > 1)
        {
            _escapeStack.Pop();
        }
    }

    private void OnRemoveEscapeActionAll(Global.RemoveEscapeActionAllMsg msg)
    {
        while (_escapeStack.Count > 1)
        {
            _escapeStack.Pop();
        }
    }

    private void OnReturnToHome(Global.ReturnToHomeMsg msg)
    {
        while (_escapeStack.Count > 1)
        {
            Action escape = _escapeStack.Peek();
            if (escape != null)
            {
                if (_escapeStack.Count > 1)
                {
                    _escapeStack.Pop();
                }

                escape();
            }

            escape = null;
        }
    }

    private void OnInputLock(Global.InputLockMsg msg)
    {
        _isLock = true;
        _eventSystem.SetActive(_isLock == false);

        if (_rayCaster != null)
            _rayCaster.enabled = (_isLock == false);
    }

    private void OnInputUnlock(Global.InputUnlockMsg msg)
    {
        _isLock = false;
        _eventSystem.SetActive(_isLock == false);

        if (_rayCaster != null)
            _rayCaster.enabled = (_isLock == false);
    }

    private void OnEscapeLock(Global.EscapeLockMsg msg)
    {
        _isLock = true;
    }

    private void OnEscapeUnlock(Global.EscapeUnlockMsg msg)
    {
        _isLock = false;
    }

    public bool IsLock()
    {
        return _isLock;
    }
}
