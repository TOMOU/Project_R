// ==================================================
// ISingleton.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance = null;
    public static T Singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;

                // 아직 생성된 오브젝트가 없으면 새로 생성
                if (_instance == null)
                {
                    var obj = new GameObject();
                    _instance = obj.AddComponent<T>();

                    if (_instance == null)
                    {
                        Logger.LogErrorFormat("Singleton 생성 오류. 클래스명에 오류가 있는지 확인해 주세요.\n클래스명 = {0}", typeof(T).ToString());
                    }
                }
                else
                {
                    _instance.Init();
                }
            }

            return _instance;
        }
    }

    public void Setup()
    {
        gameObject.name = GetType().Name;
        DontDestroyOnLoad(_instance.gameObject);

        Init();
        Message.Send<Global.TransformAttachMsg>(new Global.TransformAttachMsg(Constant.BehaviourType.Manager, this.transform));
    }

    protected virtual void Init() { }

    protected virtual void Release() { }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance.Release();
            _instance = null;
        }
    }
}