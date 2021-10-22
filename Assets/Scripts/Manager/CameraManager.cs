// ==================================================
// CameraManager.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections;

public class CameraManager : MonoSingleton<CameraManager>
{
    public Camera MainCamera { get; private set; }
    public Camera UICamera { get; private set; }
    public Camera GlobalCamera { get; private set; }
    private CameraFade _cameraEffect;
    private Coroutine _coroutine;

    public void Test(Camera main, Camera ui)
    {
        MainCamera = main;
        UICamera = ui;
        GlobalCamera = Camera.main;
    }

    protected override void Init()
    {
        Message.AddListener<Global.InitMainCameraMsg>(OnInitMainCamera);
        Message.AddListener<Global.InitUICameraMsg>(OnInitUICamera);
        Message.AddListener<Global.InitGlobalCameraMsg>(OnInitGlobalCamera);
    }

    protected override void Release()
    {
        MainCamera = null;
        UICamera = null;
        GlobalCamera = null;

        _cameraEffect.Release();
        _cameraEffect = null;

        Message.RemoveListener<Global.InitMainCameraMsg>(OnInitMainCamera);
        Message.RemoveListener<Global.InitUICameraMsg>(OnInitUICamera);
        Message.RemoveListener<Global.InitGlobalCameraMsg>(OnInitGlobalCamera);
    }

    private void OnInitMainCamera(Global.InitMainCameraMsg msg)
    {
        MainCamera = msg.Camera;
    }

    private void OnInitUICamera(Global.InitUICameraMsg msg)
    {
        UICamera = msg.Camera;
    }

    private void OnInitGlobalCamera(Global.InitGlobalCameraMsg msg)
    {
        GlobalCamera = msg.Camera;

        if (_cameraEffect == null)
        {
            _cameraEffect = GlobalCamera.GetComponent<CameraFade>();
            if (_cameraEffect == null)
            {
                Logger.LogError("Global 카메라에 CameraFade 컴포넌트가 없습니다.");
                return;
            }
        }
    }

    public IEnumerator coFadeLoading(bool isLoadingEnter)
    {
        if (_cameraEffect == null)
        {
            _cameraEffect = GlobalCamera.GetComponent<CameraFade>();
            if (_cameraEffect == null)
            {
                Logger.LogError("Global 카메라에 CameraFade 컴포넌트가 없습니다.");
                yield break;
            }
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            _cameraEffect.maskValue = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        if (isLoadingEnter == true)
            Dialog.IDialog.RequestDialogEnter<Dialog.GlobalLoadingDialog>();
        else
            Dialog.IDialog.RequestDialogExit<Dialog.GlobalLoadingDialog>();

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            _cameraEffect.maskValue = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
    }

    public IEnumerator coFadeLoading_Battle(bool isLoadingEnter)
    {
        if (_cameraEffect == null)
        {
            _cameraEffect = GlobalCamera.GetComponent<CameraFade>();
            if (_cameraEffect == null)
            {
                Logger.LogError("Global 카메라에 CameraFade 컴포넌트가 없습니다.");
                yield break;
            }
        }

        float t = 0f;
        if (isLoadingEnter == true)
        {
            while (t < 1f)
            {
                t += Time.deltaTime;
                _cameraEffect.maskValue = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime;
                _cameraEffect.maskValue = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
        }
    }

    public void FadeLoadingImmediately(bool isLoadingEnter)
    {
        if (_cameraEffect == null)
        {
            _cameraEffect = GlobalCamera.GetComponent<CameraFade>();
            if (_cameraEffect == null)
            {
                Logger.LogError("Global 카메라에 CameraFade 컴포넌트가 없습니다.");
                return;
            }
        }
        _cameraEffect.maskValue = 1f;

        if (isLoadingEnter == true)
            Dialog.IDialog.RequestDialogEnter<Dialog.GlobalLoadingDialog>();
        else
            Dialog.IDialog.RequestDialogExit<Dialog.GlobalLoadingDialog>();
    }
}
