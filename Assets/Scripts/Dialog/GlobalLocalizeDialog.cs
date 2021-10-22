// ==================================================
// GlobalLocalizeDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Dialog
{
    public class GlobalLocalizeDialog : IDialog
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private ToggleGroup _tgGroup;
        [SerializeField] private List<Toggle> _toggleList;
        [SerializeField] private List<Text> _tgTextList;
        [SerializeField] private Button _confirmButton;

        private Coroutine _coroutine = null;

        protected override void OnLoad()
        {
            base.OnLoad();

            for (int i = 0; i < _toggleList.Count; i++)
            {
                int idx = i;
                _toggleList[i].onValueChanged.AddListener(isOn =>
                {
                    if (isOn == true)
                        _tgTextList[idx].color = Color.black;
                    else
                        _tgTextList[idx].color = Color.gray;
                });
            }

            _confirmButton.onClick.AddListener(OnClickConfirm);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            for (int i = 0; i < _toggleList.Count; i++)
            {
                _toggleList[i].onValueChanged.RemoveAllListeners();
            }

            _confirmButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            if (Info.My.Singleton.User.isCompleteLocalize == false)
            {
                _anim.SetTrigger("fade");
                Message.Send<Global.EscapeLockMsg>(new Global.EscapeLockMsg());
            }
            else
            {
                Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
                {
                    Dialog.IDialog.RequestDialogExit<GlobalLocalizeDialog>();
                }));
            }

            RefreshToggleList();
        }

        protected override void OnExit()
        {
            base.OnExit();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private void RefreshToggleList()
        {
            _tgGroup.SetAllTogglesOff();

            var locale = LocalizeManager.Singleton.locale;

            for (int i = 0; i < _toggleList.Count; i++)
            {
                _toggleList[i].isOn = (locale == (Constant.Locale)i);
            }
        }

        private void OnClickConfirm()
        {
            for (int i = 0; i < _toggleList.Count; i++)
            {
                if (_toggleList[i].isOn == true)
                {
                    LocalizeManager.Singleton.locale = (Constant.Locale)i;
                    break;
                }
            }

            Message.Send<Global.ChangeLocaleMsg>(new Global.ChangeLocaleMsg());

            if (Info.My.Singleton.User.isCompleteLocalize == false)
            {
                Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());

                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                _coroutine = StartCoroutine(coDelay());
            }
            else
            {
                Dialog.IDialog.RequestDialogExit<GlobalLocalizeDialog>();
            }
        }

        private IEnumerator coDelay()
        {
            _anim.SetTrigger("out");

            yield return StartCoroutine(CameraManager.Singleton.coFadeLoading(true));

            Info.My.Singleton.User.isCompleteLocalize = true;

            Dialog.IDialog.RequestDialogExit<GlobalLocalizeDialog>();
        }
    }
}