// ==================================================
// DefenceMessage.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

namespace Battle.Defence
{
    public class FindFirstNodeMsg : Message
    {
        public int Group { get; private set; }
        public System.Action<WayPointNode> Action { get; private set; }
        public FindFirstNodeMsg(int group, System.Action<WayPointNode> action)
        {
            this.Group = group;
            this.Action = action;
        }
    }

    public class SummonDragStartMsg : Message
    {
        public int Index { get; private set; }
        public SummonDragStartMsg(int index)
        {
            this.Index = index;
        }
    }

    public class SummonDragEndMsg : Message
    {
        public int Price { get; private set; }
        public SummonDragEndMsg(int price)
        {
            this.Price = price;
        }
    }

    public class DragEnterInBlockMsg : Message
    {
        public BlockProperty Block { get; private set; }
        public DragEnterInBlockMsg(BlockProperty block)
        {
            this.Block = block;
        }
    }

    public class DragExitInBlockMsg : Message
    {
        public BlockProperty Block { get; private set; }
        public DragExitInBlockMsg(BlockProperty block)
        {
            this.Block = block;
        }
    }

    public class ShowHPBarMsg : Message
    {
        public UnitInfo_Defence Info { get; private set; }
        public int Damage { get; private set; }
        public ShowHPBarMsg(UnitInfo_Defence info, int damage)
        {
            this.Info = info;
            this.Damage = damage;
        }
    }

    /// <summary>
    /// 소환 포인트 초기화
    /// </summary>
    public class InitSummonPointMsg : Message
    {
        /// <summary>
        /// 시작하는 소환 포인트
        /// </summary>
        /// <value></value>
        public int SummonPoint { get; private set; }
        /// <summary>
        /// 소환 포인트 초기화
        /// </summary>
        /// <param name="summonPoint">시작하려고 하는 소환 포인트</param>
        public InitSummonPointMsg(int summonPoint)
        {
            this.SummonPoint = summonPoint;
        }
    }

    /// <summary>
    /// 소환 포인트 1 추가
    /// </summary>
    public class AddSummonPointMsg : Message { }
    /// <summary>
    /// 소환 포인트 1 감소
    /// </summary>
    public class RemoveSummonPointMsg : Message
    {
        public int Price { get; private set; }
        public RemoveSummonPointMsg(int price)
        {
            this.Price = price;
        }
    }

    /// <summary>
    /// 상단 정보 초기화
    /// </summary>
    public class InitReportBoardMsg : Message
    {
        /// <summary>
        /// 전체 웨이브 수 (적 유닛의 수)
        /// </summary>
        /// <value></value>
        public int WaveCount { get; private set; }
        /// <summary>
        /// 유저 라이프 수
        /// </summary>
        /// <value></value>
        public int LifeCount { get; private set; }
        /// <summary>
        /// 상단 정보 초기화
        /// </summary>
        /// <param name="waveCount">전체 웨이브 수</param>
        /// <param name="lifeCount">유저 라이프 수</param>
        public InitReportBoardMsg(int waveCount, int lifeCount)
        {
            this.WaveCount = waveCount;
            this.LifeCount = lifeCount;
        }
    }

    /// <summary>
    /// 디펜스 적 처치
    /// </summary>
    public class KillEnemyMsg : Message { }

    /// <summary>
    /// 디펜스 적 통과
    /// </summary>
    public class PassEnemyMsg : Message { }

    /// <summary>
    /// 조건을 충족하여 게임이 끝났다 (승리, 패배)
    /// </summary>
    public class GameEndMsg : Message
    {
        /// <summary>
        /// 승리한 게임인지?
        /// </summary>
        /// <value></value>
        public bool Victory { get; private set; }
        public GameEndMsg(bool victory)
        {
            this.Victory = victory;
        }
    }
}
