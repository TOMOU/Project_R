// ==================================================
// BattleNormalMessage.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

namespace Battle.Normal
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

    public class BossInitMsg : Message
    {
        public UnitInfo_Normal Info { get; private set; }
        public BossInitMsg(UnitInfo_Normal info)
        {
            this.Info = info;
        }
    }

    public class SendDamageMsg : Message
    {
        public IUnitInfo target { get; private set; }
        public int damage { get; private set; }
        public bool isCritical { get; private set; }
        public int partIndex { get; private set; }
        public SendDamageMsg(IUnitInfo target, int damage, bool isCritical)
        {
            this.target = target;
            this.damage = damage;
            this.isCritical = isCritical;
            this.partIndex = -1;
        }

        public SendDamageMsg(IUnitInfo target, int damage, bool isCritical, int partIndex)
        {
            this.target = target;
            this.damage = damage;
            this.isCritical = isCritical;
            this.partIndex = partIndex;
        }
    }

    public class ShowDamageMsg : Message
    {
        public IUnitInfo target { get; private set; }
        public int damage { get; private set; }
        public bool isCritical { get; private set; }
        public int slotIndex { get; private set; }
        public ShowDamageMsg(IUnitInfo target, int damage, bool isCritical)
        {
            this.target = target;
            this.damage = damage;
            this.isCritical = isCritical;
            this.slotIndex = -1;
        }

        public ShowDamageMsg(IUnitInfo target, int damage, bool isCritical, int slotIndex)
        {
            this.target = target;
            this.damage = damage;
            this.isCritical = isCritical;
            this.slotIndex = slotIndex;
        }
    }

    public class SendHealMsg : Message
    {
        public IUnitInfo Target { get; private set; }
        public int Value { get; private set; }
        public SendHealMsg(IUnitInfo target, int value)
        {
            this.Target = target;
            this.Value = value;
        }
    }

    public class ShowHealMsg : Message
    {
        public IUnitInfo Target { get; private set; }
        public int Value { get; private set; }
        public ShowHealMsg(IUnitInfo target, int value)
        {
            this.Target = target;
            this.Value = value;
        }
    }


    public class ShowHPBarMsg : Message
    {
        public UnitInfo_Normal Info { get; private set; }
        public ShowHPBarMsg(UnitInfo_Normal info)
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

    public class GenericUseSkillMsg : Message
    {
        public uint IDX { get; private set; }
        public GenericUseSkillMsg(uint idx)
        {
            this.IDX = idx;
        }
    }

    public class PlayUseSkillMsg : Message
    {
        public UnitInfo_Normal Sender { get; private set; }
        public System.Action action { get; private set; }
        public PlayUseSkillMsg(UnitInfo_Normal sender, System.Action action)
        {
            this.Sender = sender;
            this.action = action;
        }
    }

    public class SendUseSkillMsg : Message
    {
        public UnitInfo_Normal Sender { get; private set; }
        public bool isEnter { get; private set; }
        public System.Collections.Generic.List<UnitInfo_Normal> targetList { get; private set; }
        public System.Action action { get; private set; }
        public SendUseSkillMsg()
        {
            this.Sender = null;
            this.isEnter = false;
            this.targetList = null;
            this.action = null;
        }
        public SendUseSkillMsg(UnitInfo_Normal sender, bool isEnter, System.Collections.Generic.List<UnitInfo_Normal> targetList, System.Action action)
        {
            this.Sender = sender;
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

    // Tutorial Message
    public class Scenario_PandoraMsg : Message { }
    public class LockInstantKillMsg : Message { }
    public class UnlockInstantKillMsg : Message { }

    // J 등장때부터 사용하는 메세지
    public class ImmortalEnemyMsg : Message { }
    public class ForceDieAllyMsg : Message { }
    public class ForceSummonJMsg : Message { }
    public class ForceIdleMsg : Message { }
    public delegate void OnTimeProcess(UnityEngine.UI.Text text);
    public delegate void OnInstantKillProcess(UnityEngine.GameObject gameObject);
    public class AttachStageTimeMsg : Message
    {
        public OnTimeProcess Delegate { get; private set; }
        public AttachStageTimeMsg(OnTimeProcess del)
        {
            this.Delegate = del;
        }
    }
    public class AttachInstantKillMsg : Message
    {
        public OnInstantKillProcess Delegate { get; private set; }
        public AttachInstantKillMsg(OnInstantKillProcess del)
        {
            this.Delegate = del;
        }
    }
    public class AttachStageWaveMsg : Message
    {
        public int Current { get; private set; }
        public int Max { get; private set; }
        public AttachStageWaveMsg(int current, int max)
        {
            this.Current = current;
            this.Max = max;
        }
    }
}