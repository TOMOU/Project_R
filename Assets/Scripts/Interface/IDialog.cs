// ==================================================
// IDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class IDialog : MonoBehaviour
    {
        /// <summary>
        /// Dialog의 Root가 되는 오브젝트 (On/Off 시에 이 오브젝트를 활성/비활성화)
        /// </summary>
        protected GameObject _dialogView;
        /// <summary>
        /// Load가 완료된 뒤에 할 동작
        /// </summary>
        protected Constant.OnComplete _onComplete = null;

        public IEnumerator Load(Constant.OnComplete onComplete)
        {
            // _onComplete 동작을 추가한다.
            _onComplete = onComplete;

            // 오브젝트의 이름을 변경한다.
            gameObject.name = GetType().Name;

            // _dialogView를 캐싱한다.
            if (_dialogView == null)
            {
                _dialogView = transform.Find("View").gameObject;
            }
            _dialogView.SetActive(false);

            // 베이스 메세지동작을 등록한다.
            Message.AddListener<EnterDialogMsg>(GetType().Name, OnEnterDialog);
            Message.AddListener<ExitDialogMsg>(GetType().Name, OnExitDialog);

            // Dialog에서 초기 세팅.
            OnLoad();

            // Dialog에 맞는 부모를 찾아 연결해주는 메세지를 전달한다.
            int siblingIndex = (int)(System.Enum.Parse(typeof(Constant.UISibling), GetType().Name));
            if (siblingIndex > 1000)
            {
                gameObject.layer = LayerMask.NameToLayer("Global");
                Message.Send<Global.TransformAttachMsg>(new Global.TransformAttachMsg(Constant.BehaviourType.GlobalDialog, this.transform));
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("UI");
                Message.Send<Global.TransformAttachMsg>(new Global.TransformAttachMsg(Constant.BehaviourType.UIDialog, this.transform));
            }

            // Dialog 로드 동작이 완료되었으니 _onComplete 동작을 수행한다.
            if (_onComplete != null)
            {
                _onComplete();
            }

            yield return null;
        }

        public IEnumerator Unload()
        {
            // 베이스 메세지동작을 등록 해제한다.
            Message.RemoveListener<EnterDialogMsg>(GetType().Name, OnEnterDialog);
            Message.RemoveListener<ExitDialogMsg>(GetType().Name, OnExitDialog);

            // Dialog 비활성화
            OnExit();
            OnUnload();

            yield return null;

            // 모든 동작이 완료되었으니 오브젝트를 파괴한다.
            Destroy(this.gameObject);
        }

        protected virtual void OnLoad() { }
        protected virtual void OnUnload() { }
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }

        /// <summary>
        /// 외부에서 Dialog를 활성화할 때 호출
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RequestDialogEnter<T>() where T : IDialog
        {
            Message.Send<EnterDialogMsg>(typeof(T).Name, new EnterDialogMsg());
        }

        /// <summary>
        /// 외부에서 Dialog를 비활성화할 때 호출
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RequestDialogExit<T>() where T : IDialog
        {
            Message.Send<ExitDialogMsg>(typeof(T).Name, new ExitDialogMsg());
        }

        private void OnEnterDialog(EnterDialogMsg msg)
        {
            if (_dialogView != null)
            {
                _dialogView.SetActive(true);
            }

            OnEnter();
        }

        private void OnExitDialog(ExitDialogMsg msg)
        {
            if (_dialogView != null)
            {
                _dialogView.SetActive(false);
            }

            OnExit();
        }
    }
}