// ==================================================
// ShieldFSM.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using FSM;
using Spine.Unity;

public class ShieldFSM : StateBehaviour
{
    private SkeletonAnimation _skel;
    public bool shieldEnabled = false;
    public enum State
    {
        Idle,
        Run,
        Die,
    }

    public void Init(SkeletonAnimation skeleton)
    {
        this._skel = skeleton;

        Initialize<State>();

        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        if (_resizeTex == null)
            SpineManager.Singleton.spineTextureDic.TryGetValue("jinshi_minion_shield", out _resizeTex);

        _skel.UpdateComplete += UpdateComplete;
    }

    private void OnDestroy()
    {
        _skel.UpdateComplete -= UpdateComplete;
        _resizeTex = null;
        _renderer = null;
    }

    private Texture2D _resizeTex = null;
    private Renderer _renderer = null;
    public Renderer Rend { get { return _renderer; } }
    private void UpdateComplete(Spine.Unity.ISkeletonAnimation anim)
    {
        if (_resizeTex != null && _renderer.material.mainTexture != _resizeTex)
        {
            _renderer.material.mainTexture = _resizeTex;
        }
    }

    public void Idle()
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        shieldEnabled = true;
        ChangeState(State.Idle);
    }

    private void Idle_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "idle", true);
    }

    private void Idle_Update()
    {

    }

    private void Idle_Exit()
    {

    }

    public void Run()
    {
        shieldEnabled = true;
        ChangeState(State.Run);
    }

    private void Run_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "run", true);
    }

    private void Run_Update()
    {

    }

    private void Run_Exit()
    {

    }

    public void Die()
    {
        shieldEnabled = false;
        ChangeState(State.Die);
    }

    private void Die_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "die", false);
    }

    private void Die_Update()
    {
        if (_skel.AnimationState.GetCurrent(0).IsComplete && gameObject.activeSelf == true)
            gameObject.SetActive(false);
    }

    private void Die_Exit()
    {

    }
}
