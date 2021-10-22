// ==================================================
// MissileInfo.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

public class MissileInfo : MonoBehaviour
{
    // 테이블에서 전달받은 미사일 정보
    private MissileModel.Data _missile;

    // 미사일 이동
    private MissileMove _missileMover;

    public bool IsComplete { get { return _missileMover.Active == false; } }

    public void Init(int id)
    {
        MissileModel m = Model.First<MissileModel>();
        if (m != null)
        {
            MissileModel.Data missile = m.Table.Find(e => e.ID == id);
            if (missile != null)
            {
                this._missile = missile;
            }
        }
    }

    private void OnDestroy()
    {
        _missile = null;
    }

    public void Line(IUnitInfo spawner, IUnitInfo target, System.Action callback)
    {
        _missileMover = new MissileMove(this);
        _missileMover.InitLine(spawner, target, _missile.MoveSpeeed);
        _missileMover.AddCallback(callback);

        transform.position = spawner.transform.position + new Vector3(spawner.FSM.Skeleton.skeleton.ScaleX, 2f);

        EffectManager.Singleton.OnParticleFollow(_missile.EffectName, transform, true, spawner.FSM.Skeleton.skeleton.ScaleX, spawner as UnitInfo_Normal);
    }

    public void Line(IUnitInfo spawner, Vector3 target, System.Action callback)
    {
        _missileMover = new MissileMove(this);
        _missileMover.InitLine_Raid(spawner, target, _missile.MoveSpeeed);
        _missileMover.AddCallback(callback);

        transform.position = spawner.transform.position + new Vector3(spawner.FSM.Skeleton.skeleton.ScaleX, 2f);

        EffectManager.Singleton.OnParticleFollow(_missile.EffectName, transform, true, spawner.FSM.Skeleton.skeleton.ScaleX, spawner as UnitInfo_Normal);
    }

    public void Fixed(IUnitInfo spawner, Vector3 target, System.Action callback)
    {
        _missileMover = new MissileMove(this);
        _missileMover.Init(_missile.MoveSpeeed, _missile.ActiveTime);
        _missileMover.AddCallback(callback);

        transform.position = target;

        EffectManager.Singleton.OnParticleFollow(_missile.EffectName, transform, true, spawner.FSM.Skeleton.skeleton.ScaleX, spawner as UnitInfo_Normal);
    }

    public void SetOffMissile()
    {
        EffectManager.Singleton.OnParticleFollow(_missile.EffectName, transform, false, 1f, null);
    }

    private void Update()
    {
        if (_missileMover == null)
            return;

        // MissileMove가 활성화되지 않았다면 Update를 하지 않는다.
        if (_missileMover.Active == false)
            return;

        // 각 Type에 맞게 업데이트한다.
        _missileMover.Refresh(Time.deltaTime);
    }
}
