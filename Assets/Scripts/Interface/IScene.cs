// ==================================================
// IScene.cs
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
using UnityEngine.SceneManagement;

namespace Scene
{
    public class IScene : MonoBehaviour
    {
        protected List<Type> _contentList = new List<Type>();
        protected List<Content.IContent> _loadedContentList = new List<Content.IContent>();
        protected Logic.ILogic _loadedLogic;
        protected string _loadSceneName = string.Empty;
        protected Constant.OnComplete _onComplete = null;
        public bool isComplete = false;
        public int MaxLoadingCount
        {
            get
            {
                // 먼저 컨텐츠리스트 로드
                Preload();

                // 컨텐츠들 + 맵(Scene)
                int cnt = _contentList.Count + 1;

                GameObject obj = new GameObject("Sample");

                for (int i = 0; i < _contentList.Count; i++)
                {
                    Content.IContent script = obj.AddComponent(_contentList[i]) as Content.IContent;
                    script.Preload();
                    cnt += script.MaxCount;
                }

                Destroy(obj);
                return cnt;
            }
        }

        public IEnumerator Load(Constant.OnComplete onComplete)
        {
            // _onComplete 동작을 추가한다.
            _onComplete = onComplete;

            // 오브젝트의 이름을 변경한다.
            gameObject.name = GetType().Name;

            // Scene이 소유한 Content List를 세팅한다.
            OnLoad();

            // Scene의 부모를 찾아 연결한다.
            Message.Send<Global.TransformAttachMsg>(new Global.TransformAttachMsg(Constant.BehaviourType.Scene, this.transform));

            // 세팅이 완료되었으니 Content로딩을 시작한다.
            yield return StartCoroutine(Load_Content());

            // 세팅이 완료되었으니 UnityScene 로딩을 시작한다.
            yield return StartCoroutine(Load_Scene());

            // 모든 로드 동작이 완료되었으니 _onComplete 동작을 수행한다.
            OnLoadComplete();
            if (_onComplete != null)
            {
                _onComplete();
            }
        }

        public IEnumerator Unload()
        {
            OnUnload();

            if (_loadedLogic != null)
            {
                _loadedLogic.ReleaseLogic();
                Destroy(_loadedLogic.gameObject);
                _loadedLogic = null;
            }

            foreach (Content.IContent c in _loadedContentList)
            {
                yield return StartCoroutine(c.Unload());
            }

            _loadedContentList.Clear();

            while (_loadedContentList.Count > 0)
                yield return null;

            _loadedContentList = null;

            Destroy(this.gameObject);
        }

        public virtual void Preload()
        {
            _contentList.Clear();
        }
        protected virtual void OnLoad()
        {
            Preload();
        }
        protected virtual void OnUnload() { }
        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnLoadComplete() { }
        protected virtual void OnEscape()
        {
            Message.Send<Global.AddBaseEscapeActionMsg>(new Global.AddBaseEscapeActionMsg(OnEscape));
        }

        protected IEnumerator Load_Content()
        {
            // _contentList의 갯수만큼 반복한다.
            for (int i = 0; i < _contentList.Count; i++)
            {
                GameObject content = new GameObject();
                content.SetActive(true);

                Content.IContent script = content.AddComponent(_contentList[i]) as Content.IContent;
                if (script != null)
                {
                    yield return StartCoroutine(script.Load(() =>
                    {
                        _loadedContentList.Add(script);
                    }));
                }
                else
                {
                    Logger.LogErrorFormat("Failed create content object...Please check class name is ={0}", _contentList[i].GetType().Name);
                }

                content = null;
                script = null;

                Message.Send<Global.LoadingCountAddMsg>(new Global.LoadingCountAddMsg(_contentList[i].Name));
            }
        }

        protected IEnumerator Load_Scene()
        {
            if (string.IsNullOrEmpty(_loadSceneName) == true)
            {
                Logger.LogError("Please insert value in _loadSceneName.");
                yield break;
            }

            AsyncOperation async = SceneManager.LoadSceneAsync(_loadSceneName);
            while (async.isDone == false)
                yield return null;

            Message.Send<Global.LoadingCountAddMsg>(new Global.LoadingCountAddMsg(string.Format("LoadScene : {0}", GetType().Name)));

            yield return new WaitForSeconds(2f);

            if (_loadSceneName != "TitleScene")
            {
                CameraManager.Singleton.FadeLoadingImmediately(false);
                // yield return StartCoroutine(CameraManager.Singleton.coFadeLoading(false));
            }

            async = null;
        }

        public static void LoadScene<T>() where T : IScene
        {
            Time.timeScale = 1f;
            Message.Send<Global.ChangeSceneMsg>(new Global.ChangeSceneMsg(typeof(T)));
        }
    }
}