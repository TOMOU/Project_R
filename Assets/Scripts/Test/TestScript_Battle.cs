// ==================================================
// TestScript_Battle.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using Spine.Unity;
using System.Collections.Generic;

public class TestScript_Battle : MonoBehaviour
{
    public Transform unitRoot = null;
    public Dialog.NormalMainDialog dialog;
    public Camera mainCam;
    public Camera uiCam;

    private void Start()
    {
        GameModel gm = new GameModel();
        gm.Setup();

        BattleManager.Singleton.Setup();
        CameraManager.Singleton.Setup();
        CameraManager.Singleton.Test(mainCam, uiCam);

        dialog.Test();

        var um = Model.First<UnitModel>();
        if (um != null)
        {
            var ut = um.unitTable;

            UnitStatus blue = new UnitStatus(ut.Find(e => e.unitName_Kor == "치우"), 1, 1, 1);
            UnitStatus red = new UnitStatus(ut.Find(e => e.unitName_Kor == "진시황"), 2, 1, 1);

            Summon(Constant.Team.Blue, blue, 1);
            Summon(Constant.Team.Red, red, 1);
        }

        Message.Send<Battle.Normal.StartBattleMsg>(new Battle.Normal.StartBattleMsg());
    }

    private void Summon(Constant.Team team, UnitStatus status, int idx)
    {
        // Load UnitModel
        var um = Model.First<UnitModel>();
        if (um == null)
        {
            Logger.LogError("Can't load UnitModel");
            return;
        }

        // Load Prefab based in UnitStatus.
        GameObject prefab = Resources.Load<GameObject>(string.Format("Character/{0}", status.unitName));
        if (prefab == null)
        {
            Logger.LogErrorFormat("Can't load Prefab in Character/{0}.", status.unitName);
            return;
        }

        // Instantiate prefab in scene.
        GameObject obj = GameObject.Instantiate(prefab);

        // Active false before positioning.
        obj.SetActive(false);

        // Set position.
        obj.transform.SetParent(unitRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        // Active true after positioning.
        obj.SetActive(true);

        // Caching SkeletonAnimation.
        SkeletonAnimation skel = obj.GetComponent<SkeletonAnimation>();
        if (skel == null)
        {
            Logger.LogErrorFormat("Can't find <SkeletonAnimation> Component in {0} object.", obj.name);
            return;
        }

        // If unit is RedTeam, SkeletonAnimation FlipX.
        if (team == Constant.Team.Red)
        {
            // SkeletonAnimation FlipX
            skel.initialFlipX = true;
            skel.Initialize(true);
        }

        // Add UnitFSM
        UnitFSM fsm = obj.AddComponent<UnitFSM>();
        fsm.Init(skel, 0);

        // Add UnitInfo
        UnitInfo_Normal info = obj.AddComponent<UnitInfo_Normal>();
        info.Init(team, status, fsm);
        // info.InitTeamIndex(idx);

        // Add unit to BattleManager.
        BattleManager.Singleton.AddUnit(team, info);

        obj.transform.localPosition = team == Constant.Team.Blue ? new Vector3(-15f, -5f, -5f) : new Vector3(15f, -5f, -5f);
    }
}
