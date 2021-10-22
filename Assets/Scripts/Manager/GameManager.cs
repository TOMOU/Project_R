// ==================================================
// GameManager.cs
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
using DG.Tweening;
using UnityEngine.Profiling;

public class GameManager : MonoSingleton<GameManager>
{
    private Transform _managerGroup = null;
    private Transform _logicGroup = null;
    private Transform _sceneGroup = null;
    private Transform _contentGroup = null;
    private Transform _uiDialogGroup = null;
    private Transform _globalDialogGroup = null;
    private Scene.IScene _loadedScene = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Init();
    }

    protected override void Init()
    {
        // ! 튜토리얼 강제 진입.
        // ! 세이브데이터 전부 날리기
        PlayerPrefs.DeleteAll();

        // 앱 최초 설정
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Logger 설정
#if UNITY_EDITOR
        Logger.logLevel = Constant.LogLevel.All;
#else
        Logger.logLevel = Constant.LogLevel.Nothing;        
#endif

        // Transform 그룹 캐싱
        _managerGroup = transform.Find("Managers");
        _logicGroup = transform.Find("Logics");
        _sceneGroup = transform.Find("Scenes");
        _contentGroup = transform.Find("Contents");
        _uiDialogGroup = transform.Find("Dialogs/UI/UICanvas");
        _globalDialogGroup = transform.Find("Dialogs/Global/GlobalCanvas");

        // 그룹 하위를 전부 비운다.
        foreach (Transform child in _managerGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _logicGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _sceneGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _contentGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _uiDialogGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _globalDialogGroup)
            Destroy(child.gameObject);

        // Message 등록
        Message.AddListener<Global.ChangeSceneMsg>(OnChangeScene);
        Message.AddListener<Global.TransformAttachMsg>(OnTransformAttach);

        Init_All();

        // 첫 실행 시에 Title씬으로 전환해준다.
        GameModel gm = Model.First<GameModel>();
        if (gm != null && gm.loadCompleteGlobalContent == false)
        {
            Message.Send<Global.ChangeSceneMsg>(new Global.ChangeSceneMsg(typeof(Scene.TitleScene)));
            gm = null;
        }
    }

    protected override void Release()
    {
        // 그룹 하위를 전부 비운다.
        foreach (Transform child in _managerGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _logicGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _sceneGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _contentGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _uiDialogGroup)
            Destroy(child.gameObject);
        foreach (Transform child in _globalDialogGroup)
            Destroy(child.gameObject);

        _managerGroup = null;
        _logicGroup = null;
        _sceneGroup = null;
        _contentGroup = null;
        _uiDialogGroup = null;
        _globalDialogGroup = null;

        StartCoroutine(_loadedScene.Unload());

        // Message 등록해제
        Message.RemoveListener<Global.ChangeSceneMsg>(OnChangeScene);
        Message.RemoveListener<Global.TransformAttachMsg>(OnTransformAttach);
    }

    #region Init
    /// <summary>
    /// 초기화해야하는 함수들 통합 호출
    /// </summary>
    private void Init_All()
    {
        // 모델 초기화
        Init_Model();

        // 매니저 생성
        Init_Manager();

        // 카메라 등록
        Init_Camera();

        // 캔버스 해상도 조절 (현재는 LetterBox 처리됨)
        Init_Canvas(_uiDialogGroup);
        Init_Canvas(_globalDialogGroup);
        Init_LetterBox();

        DOTween.Init();
    }

    /// <summary>
    /// 데이터를 초기화, 로드한다
    /// </summary>
    private void Init_Model()
    {
        GameModel gm = new GameModel();
        gm.Setup();
    }

    /// <summary>
    /// 매니저를 초기화, 생성한다
    /// </summary>
    private void Init_Manager()
    {
        Info.My.Singleton.Setup();

        TimeManager.Singleton.Setup();
        InputManager.Singleton.Setup();
        CameraManager.Singleton.Setup();
        SoundManager.Singleton.Setup();
        BattleManager.Singleton.Setup();
        LocalizeManager.Singleton.Setup();
        TutorialManager.Singleton.Setup();
        EffectManager.Singleton.Setup();
        SpineManager.Singleton.Setup();
    }

    private void Init_Camera()
    {
        // 카메라 등록
        GameObject obj = GameObject.Find("UICamera");
        if (obj != null)
        {
            Camera cam = obj.GetComponent<Camera>();
            if (cam != null)
            {
                Message.Send<Global.InitUICameraMsg>(new Global.InitUICameraMsg(cam));
            }
            else
            {
                Logger.LogError("UICamera 오브젝트에 Camera 컴포넌트가 없습니다.");
            }

            cam = null;
        }
        else
        {
            Logger.LogError("UICamera 게임오브젝트를 찾지 못했습니다.");
        }

        obj = GameObject.Find("GlobalCamera");
        if (obj != null)
        {
            Camera cam = obj.GetComponent<Camera>();
            if (cam != null)
            {
                Message.Send<Global.InitGlobalCameraMsg>(new Global.InitGlobalCameraMsg(cam));
            }
            else
            {
                Logger.LogError("GlobalCamera 오브젝트에 Camera 컴포넌트가 없습니다.");
            }

            cam = null;
        }
        else
        {
            Logger.LogError("GlobalCamera 게임오브젝트를 찾지 못했습니다.");
        }

        obj = null;
    }

    /// <summary>
    /// 해당 캔버스의 Canvas Scaler를 수정한다
    /// </summary>
    /// <param name="canvas"></param>
    private void Init_Canvas(Transform canvas)
    {
        // Canvas Scaler 설정
        UnityEngine.UI.CanvasScaler canvasScaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
        if (canvasScaler == null)
        {
            Logger.LogError("Canvas 파싱이 잘못되었습니다.");
            return;
        }

        if (Screen.width / (float)Screen.height > 1.77f)
        {
            canvasScaler.matchWidthOrHeight = 1;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0;
        }

        // 메모리 정리
        canvasScaler = null;
    }

    /// <summary>
    /// LetterBox의 CanvasScaler를 수정한다
    /// </summary>
    private void Init_LetterBox()
    {
        // Canvas Scaler 설정
        UnityEngine.UI.CanvasScaler canvasScaler = transform.Find("LetterBox/LetterBoxCanvas").GetComponent<UnityEngine.UI.CanvasScaler>();
        if (canvasScaler == null)
        {
            Logger.LogError("Canvas 파싱이 잘못되었습니다.");
            return;
        }

        if (Screen.width / (float)Screen.height > 1.77f)
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 1;
        }

        // 메모리 정리
        canvasScaler = null;
    }
    #endregion

    private void OnChangeScene(Global.ChangeSceneMsg msg)
    {
        StartCoroutine(LoadRoot(msg.Scene));

        // 씬 전환 간 Clear 해줘야 하는 매니저
        BattleManager.Singleton.RemoveAllUnit();
        EffectManager.Singleton.Clear();
    }

    private IEnumerator LoadRoot(Type scene)
    {
        // 로드 시작 전 입력을 잠금
        Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());

        Time.timeScale = 1f;

        if (scene.Name != "TitleScene")
        {
            // 아직 로컬라이징 안되었으면 출력한다.
            if (Info.My.Singleton.User.isCompleteLocalize == false)
            {
                Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());

                Dialog.IDialog.RequestDialogEnter<Dialog.GlobalLocalizeDialog>();

                yield return new WaitUntil(() => Info.My.Singleton.User.isCompleteLocalize == true);
            }
            else
            {
                yield return StartCoroutine(CameraManager.Singleton.coFadeLoading(true));
            }
        }

        if (_loadedScene != null)
        {
            yield return StartCoroutine(_loadedScene.Unload());
        }

        GameObject obj = new GameObject();
        Scene.IScene script = obj.AddComponent(scene) as Scene.IScene;
        if (script != null)
        {
            if (scene.Name != "TitleScene")
            {
                int maxCount = 0;

                if (SpineManager.Singleton.spineTextureDic.Count == 0 && BuildOptionDefine.isResizeSpineTexture == true)
                {
                    var um = Model.First<UnitModel>();
                    if (um == null)
                    {
                        Logger.LogError("유닛모드 로드 실패");
                        yield break;
                    }

                    maxCount = um.unitTable.Count + script.MaxLoadingCount;

                    // 여기서 GlobalLoadingDialog에 MaxLoadingCount를 전달
                    Message.Send<Global.MaxLoadingCountMsg>(new Global.MaxLoadingCountMsg(maxCount));

                    // Spine 로드 시작
                    yield return StartCoroutine(SpineManager.Singleton.coLoad(um.unitTable));
                }
                else
                {
                    maxCount = script.MaxLoadingCount;

                    // 여기서 GlobalLoadingDialog에 MaxLoadingCount를 전달
                    Message.Send<Global.MaxLoadingCountMsg>(new Global.MaxLoadingCountMsg(maxCount));
                }
            }

            yield return StartCoroutine(script.Load(() =>
            {
                _loadedScene = script;
                SortUISibling(_uiDialogGroup);
                SortUISibling(_globalDialogGroup);

                // MainCamera를 갱신
                RefreshMainCamera();

                // GameSpeed를 갱신
                RefreshGameSpeed(scene);
            }));
        }
        else
        {
            Logger.LogErrorFormat("Failed create content object...Please check class name is ={0}", scene.Name);
        }

        // 완료되었으면 메모리 정리
        obj = null;
        script = null;

        // 미사용하는 로드에셋 메모리에서 해제
        Resources.UnloadUnusedAssets();
    }

    private void OnTransformAttach(Global.TransformAttachMsg msg)
    {
        if (msg.Type == Constant.BehaviourType.Manager)
            msg.Transform.SetParent(_managerGroup, false);
        else if (msg.Type == Constant.BehaviourType.Logic)
            msg.Transform.SetParent(_logicGroup, false);
        else if (msg.Type == Constant.BehaviourType.Scene)
            msg.Transform.SetParent(_sceneGroup, false);
        else if (msg.Type == Constant.BehaviourType.Content)
            msg.Transform.SetParent(_contentGroup, false);
        else if (msg.Type == Constant.BehaviourType.UIDialog)
            msg.Transform.SetParent(_uiDialogGroup, false);
        else if (msg.Type == Constant.BehaviourType.GlobalDialog)
            msg.Transform.SetParent(_globalDialogGroup, false);
    }

    private void SortUISibling(Transform dialogParent)
    {
        Dictionary<int, Transform> uiDic = new Dictionary<int, Transform>();
        foreach (Transform child in dialogParent)
        {
            int idx = (int)Enum.Parse(typeof(Constant.UISibling), child.name);
            uiDic.Add(idx, child);
        }

        int maxCount = (int)Enum.Parse(typeof(Constant.UISibling), "Max");
        Transform source = null;
        for (int i = 0; i < maxCount; i++)
        {
            if (uiDic.TryGetValue(i, out source) == true)
            {
                source.SetAsLastSibling();
            }
        }

        // 메모리 정리
        uiDic.Clear();
        uiDic = null;
    }

    private void RefreshMainCamera()
    {
        GameObject obj = GameObject.Find("Main Camera");
        if (obj != null)
        {
            Camera cam = obj.GetComponent<Camera>();
            if (cam != null)
            {
                Message.Send<Global.InitMainCameraMsg>(new Global.InitMainCameraMsg(cam));
            }
            else
            {
                Logger.LogError("Main Camera 오브젝트에 Camera 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Logger.LogError("Main Camera 게임오브젝트를 찾지 못했습니다.");
        }
    }

    private void RefreshGameSpeed(Type type)
    {
        if (type.Name.Contains("Battle"))
        {
            TimeManager.Singleton.IsBattleMode = true;
            Message.Send<Global.ReloadSpeedMsg>(new Global.ReloadSpeedMsg());
        }
        else
        {
            TimeManager.Singleton.IsBattleMode = false;
            Time.timeScale = 1f;
        }
    }
}
