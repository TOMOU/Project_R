// ==================================================
// Skill_Minion_Sword.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections;
using Spine.Unity;

public class Skill_Minion_Sword : SkillBase
{
    public override IEnumerator coSkillNormal()
    {
        yield return new WaitForSpineEvent(_skel, "attack");

        //todo 사운드 연출
        SoundManager.Singleton.PlayAttackSound(true);

        // 데미지 총합 계산 후 전달
        SendDamage();
    }
}
