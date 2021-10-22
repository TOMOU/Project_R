namespace Lobby
{
    #region 캐릭터관리
    public class UseExpPotionMsg : Message
    {
        public uint Target { get; private set; }
        public int Exp { get; private set; }
        public int Count { get; private set; }
        public UseExpPotionMsg(uint target, int exp, int count)
        {
            this.Target = target;
            this.Exp = exp;
            this.Count = count;
        }
    }

    public class AddCharacterExpMsg : Message
    {
        public uint Target { get; private set; }
        public int AddExp { get; private set; }
        public AddCharacterExpMsg(uint target, int addExp)
        {
            this.Target = target;
            this.AddExp = addExp;
        }
    }

    public class UseEvolveMaterialMsg : Message
    {
        public uint Target { get; private set; }
        public UseEvolveMaterialMsg(uint target)
        {
            this.Target = target;
        }
    }

    public class EvolveCharacterMsg : Message
    {
        public uint Target { get; private set; }
        public EvolveCharacterMsg(uint target)
        {
            this.Target = target;
        }
    }

    public class RefreshCharacterDialogMsg : Message
    {
        public bool IsShowUpgradeProgress { get; private set; }
        public RefreshCharacterDialogMsg(bool isShowUpgradeProgress = false)
        {
            this.IsShowUpgradeProgress = isShowUpgradeProgress;
        }
    }
    #endregion

    public class InstantBossAppearMsg : Message { }
    public class EnterContentDialogMsg : Message { }
}