// ==================================================
// IContent.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    public class IContent : MonoBehaviour
    {
        /// <summary>
        /// Dialog를 로드할 경로
        /// </summary>
        /// <value></value>
        // protected const string _path = "UI/Dialog/{0}";////"UI/{0}/{1}";
        protected const string _path = "UI/{0}/{1}";
        /// <summary>
        /// Content가 로드해야 하는 Dialog List
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <returns></returns>
        protected List<Type> _dialogList = new List<Type>();
        /// <summary>
        /// 로드가 완료된 Dialog List
        /// </summary>
        /// <typeparam name="Dialog.IDialog"></typeparam>
        /// <returns></returns>
        protected List<Dialog.IDialog> _loadedDialogList = new List<Dialog.IDialog>();
        /// <summary>
        /// Load가 완료된 뒤에 할 동작
        /// </summary>
        private Constant.OnComplete _onComplete = null;
        /// <summary>
        /// 전역적으로 사용되는 컨텐츠인지
        /// </summary>
        protected bool _isGlobalContent = false;
        public int MaxCount { get { return _dialogList.Count; } }

        public IEnumerator Load(Constant.OnComplete onComplete)
        {
            // _onComplete 동작을 추가한다.
            _onComplete = onComplete;

            // 오브젝트의 이름을 변경한다.
            gameObject.name = GetType().Name;

            // 기본 메세지동작을 등록한다.
            Message.AddListener<EnterContentMsg>(GetType().Name, OnEnterContent);
            Message.AddListener<ExitContentMsg>(GetType().Name, OnExitContent);

            // Content가 소유한 UIList를 세팅한다.
            OnLoad();

            // Content의 부모를 찾아 연결한다.
            Message.Send<Global.TransformAttachMsg>(new Global.TransformAttachMsg(Constant.BehaviourType.Content, this.transform));

            // 세팅이 완료되었으니 UI로딩을 시작한다.
            yield return StartCoroutine(Load_Dialog());

            // 모든 로드 동작이 완료되었으니 _onComplete 동작을 수행한다.
            if (_onComplete != null)
            {
                _onComplete();
            }
        }

        public IEnumerator Unload()
        {
            // GlobalContent의 경우 건너뛴다.
            if (_isGlobalContent == true)
                yield break;

            // 기본 메세지 등록 해제
            Message.RemoveListener<EnterContentMsg>(GetType().Name, OnEnterContent);
            Message.RemoveListener<ExitContentMsg>(GetType().Name, OnExitContent);

            // Content 비활성화
            OnExit();
            OnUnload();

            // 로드된 Dialog를 비활성화
            for (int i = 0; i < _loadedDialogList.Count; i++)
            {
                yield return StartCoroutine(_loadedDialogList[i].Unload());
            }

            // _loadedDialogList를 비운다.
            _loadedDialogList.Clear();
            _loadedDialogList = null;

            // 모든 동작이 완료되었으니 오브젝트를 파괴한다.
            Destroy(this.gameObject);
        }

        private IEnumerator Load_Dialog()
        {
            // 현재 Content의 구분 string값을 얻는다 (LobbyContent -> "Lobby")
            string context = GetType().Name;
            context = context.Remove(context.Length - 7);

            // _uiList의 갯수만큼 반복한다.
            for (int i = 0; i < _dialogList.Count; i++)
            {
                GameObject ui = null;
                string separatedName = _dialogList[i].ToString().Split('.')[1];
                ResourceRequest async = Resources.LoadAsync(string.Format(_path, context, separatedName), typeof(GameObject));
                // ResourceRequest async = Resources.LoadAsync(string.Format(_path, separatedName), typeof(GameObject));
                yield return async;

                if (async.asset != null)
                {
                    ui = Instantiate(async.asset) as GameObject;
                    ui.SetActive(true);
                    ui.name = async.asset.name;

                    Dialog.IDialog script = ui.GetComponent(_dialogList[i]) as Dialog.IDialog;
                    if (script == null)
                    {
                        script = ui.AddComponent(_dialogList[i]) as Dialog.IDialog;
                    }

                    if (script != null)
                    {
                        yield return StartCoroutine(script.Load(() =>
                        {
                            _loadedDialogList.Add(script);
                        }));
                    }
                    else
                    {
                        Logger.LogErrorFormat("Failed instantiate dialog object...Please check prefab's name and class name is [ {0} ]", separatedName);
                    }

                    script = null;
                }
                else
                {
                    Logger.LogErrorFormat("Failed load dialog prefab...Please check prefab's name is [ {0} ]", separatedName);
                }

                async = null;
                ui = null;

                Message.Send<Global.LoadingCountAddMsg>(new Global.LoadingCountAddMsg(_dialogList[i].Name));

                yield return null;
            }
        }

        public virtual void Preload()
        {
            _dialogList.Clear();
        }
        protected virtual void OnLoad()
        {
            Preload();
        }
        protected virtual void OnUnload() { }
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }

        public static void RequestContentEnter<T>() where T : IContent
        {
            Message.Send<EnterContentMsg>(typeof(T).Name, new EnterContentMsg());
        }

        public static void RequestContentExit<T>() where T : IContent
        {
            Message.Send<ExitContentMsg>(typeof(T).Name, new ExitContentMsg());
        }

        private void OnEnterContent(EnterContentMsg msg)
        {
            OnEnter();
        }

        private void OnExitContent(ExitContentMsg msg)
        {
            OnExit();
        }
    }
}