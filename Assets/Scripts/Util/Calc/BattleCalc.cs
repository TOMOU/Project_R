using System;
using UnityEngine;

public class BattleCalc
{
    public static int Calculate_Damage(float dam, float def, float multiple = 1f)
    {
        // 적용 데미지 = 데미지 / (1 + 방어력 / 100)
        return (int)Math.Truncate(dam / (1 + def / 100) * multiple);
    }

    public static bool Calculate_ActiveCritical(float crit, int myLevel, int enemyLevel)
    {
        float rand = UnityEngine.Random.Range(0f, 100f);

        // random 발생수치가 적용 크리티컬 수치보다 낮거나 동일하면 크리티컬 발생한것으로 처리.
        return rand <= crit;
    }

    // 전투력 수치 반환.
    public static int Calculate_BattlePower(UnitStatus status)
    {
        int bp = status.hp + (int)(status.damage * status.attackRange) + status.pDef + status.mDef; //? 임시 전투력수치 출력

        return bp;
    }

    public static float GetDistance(Vector3 s, Vector3 e)
    {
        return Vector3.Distance(s, e);
    }

    #region Status
    public static int Calculate_Status(int o, int lv, float lvPer, int grade, float gradePer)
    {
        return o + (int)((lv - 1) * (lvPer + ((grade - 1) * gradePer)));
    }
    #endregion

    #region 위치 변환
    //todo 2포인트+1상단 간 베지어곡선
    public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (Mathf.Pow(1 - t, 2) * p0) + (2 * t * (1 - t) * p1) + (Mathf.Pow(t, 2) * p2);
    }

    public static Vector3 KnockbackMove(int my)
    {
        return (my == 0 ? Vector3.left : Vector3.right) * 3f * Time.deltaTime;
    }
    #endregion
}