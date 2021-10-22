// ==================================================
// TimeManager.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System;
using UnityEngine;

public class TimeManager : MonoSingleton<TimeManager>
{
    private const float _normalSpeed = 1.4f;
    private const float _fastSpeed = 2.2f;
    public bool FastMode { get; private set; }
    public bool Paused { get; private set; }
    public bool IsBattleMode { get; set; }

    private float _gcTime;
    private float _gcTimeMax;

    protected override void Init()
    {
        FastMode = PlayerPrefs.GetInt("isFastMode", 0) == 0 ? false : true;

        _gcTime = 0f;
        _gcTimeMax = 30f;

        Message.AddListener<Global.FastSpeedMsg>(OnFastSpeed);
        Message.AddListener<Global.NormalSpeedMsg>(OnNormalSpeed);
        Message.AddListener<Global.ReloadSpeedMsg>(OnReloadSpeed);
        Message.AddListener<Global.EnablePauseMsg>(OnEnablePause);
        Message.AddListener<Global.DisablePauseMsg>(OnDisablePause);
    }

    protected override void Release()
    {
        PlayerPrefs.SetInt("isFastMode", FastMode ? 1 : 0);

        Message.RemoveListener<Global.FastSpeedMsg>(OnFastSpeed);
        Message.RemoveListener<Global.NormalSpeedMsg>(OnNormalSpeed);
        Message.RemoveListener<Global.ReloadSpeedMsg>(OnReloadSpeed);
        Message.RemoveListener<Global.EnablePauseMsg>(OnEnablePause);
        Message.RemoveListener<Global.DisablePauseMsg>(OnDisablePause);
    }

    private void Update()
    {
        _gcTime += Time.deltaTime;
        if (_gcTime > _gcTimeMax)
        {
            _gcTime = 0f;
            GC.Collect();
        }
    }

    private void OnFastSpeed(Global.FastSpeedMsg msg)
    {
        FastMode = true;
        Refresh();
    }

    private void OnNormalSpeed(Global.NormalSpeedMsg msg)
    {
        FastMode = false;
        Refresh();
    }

    private void OnReloadSpeed(Global.ReloadSpeedMsg msg)
    {
        Refresh();
    }

    private void OnEnablePause(Global.EnablePauseMsg msg)
    {
        Paused = true;
        Refresh();
    }

    private void OnDisablePause(Global.DisablePauseMsg msg)
    {
        Paused = false;
        Refresh();
    }

    private void Refresh()
    {
        if (Paused == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            if (IsBattleMode == true)
            {
                if (FastMode == true)
                {
                    Time.timeScale = _fastSpeed;
                }
                else
                {
                    Time.timeScale = _normalSpeed;
                }
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}
