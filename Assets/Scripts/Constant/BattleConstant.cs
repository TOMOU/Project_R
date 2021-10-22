namespace Constant
{
    public enum Position
    {
        None = 0, // 없음
        Front = 1, // 전열
        Middle = 2, // 중열
        Back = 3, // 후열
        Max = 999,
    }

    public enum BasicAttackType
    {
        None = 0, // 없음
        Physics = 1, // 물리공격
        Magic = 2, // 마법공격
        Max = 999,
    }

    public enum SkillType
    {
        None = 0, // 없음
        Damage = 1, // 데미지
        Heal = 2, // 힐
        Buff = 3, // 버프
        Debuff = 4, // 디버프
        Max = 999,
    }

    public enum SkillTeam
    {
        None = 0, // 없음
        Ally = 1, // 아군
        Enemy = 2, // 적군
        Max = 999,
    }

    public enum SkillTarget
    {
        None = 0, // 없음
        All = 1, // 전체
        Area = 2, // 범위
        One = 3, // 한명
        Me = 4, // 자기자신
        HP = 5, // 체력이 가장 낮은
        One_Second = 6, // 두번째
        One_Third = 7, // 세번째
        One_Fourth = 8, // 네번째
        One_Fifth = 9, // 다섯번째
        Two = 10, // 두명
        Three = 11, // 세명
        Four = 12, // 네명
        Random = 13, // 랜덤
        Max = 999,
    }

    public enum SkillProperty
    {
        None = 0, // 없음
        PhysicalDamage = 1, // 물리 데미지
        MagicalDamage = 2, // 마법 데미지
        PhysicalDefence = 3, // 물리 방어력
        MagicalDefence = 4, // 마법 방어력
        Critical = 5, // 치명율
        TP = 6, // TP
        Speed = 7, // 공격속도
        PhysicalShield = 8, // 물리 실드
        MagicalShield = 9, // 마법 실드
        Heal_Oneshot = 10, // 단일힐
        Heal_Tick = 11, // 도트힐
        Blind = 12, // 실명
        Burn = 13, // 화상
        KnockBack = 14, // 넉백
        Stun = 15, // 스턴
        Silence = 16, // 침묵
        AbsorbHP = 17, // 흡혈
        Aggro = 18,     // 어그로
        Bleed,          // 출혈
        Immune,         // 모상저
        MinionSpear,
        MinionShield,
        Max = 999,
    }

    public enum UnitState
    {
        None = 0,
        Idle = 1, // 대기
        Run = 2, // 이동
        Attack = 3, // 평타
        Skill0 = 4, // 궁스킬
        Skill1 = 5, // 스킬1
        Skill2 = 6, // 스킬2
        Hit = 7, // 타격받음
        Victory = 8, // 승리
        Die = 9, // 죽음
        Condition = 10,
        Max = 999,
    }

    public enum BattlePhase
    {
        None = 0,
        MoveToBase = 1, // 시작지점으로 이동
        StartBattle = 2, // 전투 시작
        BattleNext = 3, // 다음맵으로 이동
        BattleEnd = 4, // 전투 종료 (결과화면)
        Summon_J = 5,  // 시간종료
        TimeUp,
        Max = 999,
    }

    public enum Team
    {
        None = 0,
        Blue = 1, // 아군
        Red = 2, // 적군
        Max = 999,
    }

    public enum MissileMoveType
    {
        None,
        Follow,
        Move,
        Fixed,
        Line,
        Line_Raid,
    }
}