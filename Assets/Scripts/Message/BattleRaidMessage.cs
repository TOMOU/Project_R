namespace Battle.Raid
{
    public class UnitSlotInitMsg : Message
    {
        public int index { get; private set; }
        public UnitStatus status { get; private set; }
        public UnitSlotInitMsg(int index, UnitStatus status)
        {
            this.index = index;
            this.status = status;
        }
    }

    public class SendDamageMsg : Message
    {
        public UnityEngine.Transform target { get; private set; }
        public int damage { get; private set; }
        public bool isCritical { get; private set; }
        public bool isHealing { get; private set; }
        public SendDamageMsg(UnityEngine.Transform target, int damage, bool isCritical, bool isHealing)
        {
            this.target = target;
            this.damage = damage;
            this.isCritical = isCritical;
            this.isHealing = isHealing;
        }
    }

    public class ShowHPBarMsg : Message
    {
        public UnitInfo_Raid Info { get; private set; }
        public ShowHPBarMsg(UnitInfo_Raid info)
        {
            this.Info = info;
        }
    }

    public class SendImDyingMsg : Message
    {
        public IUnitInfo info { get; private set; }
        public SendImDyingMsg(IUnitInfo info)
        {
            this.info = info;
        }
    }

    public class SendUseSkillMsg : Message
    {
        public uint idx { get; private set; }
        public int code { get; private set; }
        public bool isEnter { get; private set; }
        public System.Collections.Generic.List<UnitInfo_Raid> targetList { get; private set; }
        public System.Action action { get; private set; }
        public SendUseSkillMsg()
        {
            this.idx = 0;
            this.code = 0;
            this.isEnter = false;
            this.targetList = null;
            this.action = null;
        }
        public SendUseSkillMsg(uint idx, int code, bool isEnter, System.Collections.Generic.List<UnitInfo_Raid> targetList, System.Action action)
        {
            this.idx = idx;
            this.code = code;
            this.isEnter = isEnter;
            this.targetList = targetList;
            this.action = action;
        }
    }

    public class BattleNextMsg : Message { }
    public class PlayBattleResult : Message { }
    public class BattleDrawMsg : Message { }
    public class BattleVictoryMsg : Message { }
    public class BattleDefeatMsg : Message { }

    // ==================================================
    // New Refactoring Message Function
    // ==================================================
    public class MoveToBaseMsg : Message { }
    public class MoveToBaseCompleteMsg : Message { }
    public class StartBattleMsg : Message { }
    public class StartBattleCompleteMsg : Message { }
    public class MoveToNextMsg : Message { }
    public class MoveToNextCompleteMsg : Message { }
    public class MoveToEndMsg : Message { }
    public class MoveToEndCompleteMsg : Message { }
}