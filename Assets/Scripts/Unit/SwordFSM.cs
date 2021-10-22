// ==================================================
// SwordFSM.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using FSM;
using System.Collections;
using Spine.Unity;

public class SwordFSM : StateBehaviour
{
    private SkeletonAnimation _skel;
    private IUnitInfo _owner;
    private IUnitInfo _target;
    public enum State
    {
        None,
        Attack,
        Run,
    }

    public void Init(SkeletonAnimation skeleton)
    {
        this._skel = skeleton;

        Initialize<State>();

        if (_renderer == null)
            _renderer = GetComponent<Renderer>();

        if (_resizeTex == null)
            SpineManager.Singleton.spineTextureDic.TryGetValue("jinshi_minion_sword", out _resizeTex);

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
    private void UpdateComplete(Spine.Unity.ISkeletonAnimation anim)
    {
        if (_resizeTex != null && _renderer.material.mainTexture != _resizeTex)
        {
            _renderer.material.mainTexture = _resizeTex;
        }
    }

    private void None_Enter()
    {
        gameObject.SetActive(false);
    }

    public void Attack(IUnitInfo owner, IUnitInfo target)
    {
        if (target == null)
            return;

        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
            this._owner = owner;
            this._target = target;

            // 적과 나 사이의 방향을 잡는다.
            var vec = target.transform.position - owner.transform.position;

            if (vec.x > 0)   // 적이 나의 오른쪽에 있으니 왼쪽에 검마용 배치
            {
                transform.position = target.transform.position - new Vector3(6f, 0f, 0f);
                _skel.skeleton.ScaleX = 1f;
            }
            else
            {
                transform.position = target.transform.position + new Vector3(6f, 0f, 0f);
                _skel.skeleton.ScaleX = -1f;
            }
        }

        ChangeState(State.Attack);
    }

    private IEnumerator Attack_Enter()
    {
        _skel.AnimationState.SetAnimation(0, "attack", false);

        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Attack_Sword_1));

        UnitInfo_Normal un = _owner as UnitInfo_Normal;

        if (un.Target != null)
        {
            // 데미지 연산
            int damage = BattleCalc.Calculate_Damage(un.Status.damage, un.Target.Status.pDef);

            // 크리티컬 계산
            bool isCritical = BattleCalc.Calculate_ActiveCritical(un.Status.criticalPercentage, un.Status.level, un.Target.Status.level);

            if (_owner.Status.isBlind == true)
                damage = 0;

            // 타겟에 데미지 전달
            if (un.isRaidLogic == true)
                Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(un.Target, damage, isCritical, un.slotIndex));
            else
                Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(un.Target, damage, isCritical));
        }
    }

    private void Attack_Update()
    {
        if (_skel.AnimationState.GetCurrent(0).IsComplete && gameObject.activeSelf == true)
        {
            ChangeState(State.None);
        }
    }

    private void Attack_Exit()
    {

    }

    public void Run()
    {
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
}