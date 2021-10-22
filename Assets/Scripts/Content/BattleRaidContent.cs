namespace Content
{
    public class BattleRaidContent : IContent
    {
        public override void Preload()
        {
            base.Preload();

            // UI 등록
            _dialogList.Add(typeof(Dialog.RaidMainDialog));
            _dialogList.Add(typeof(Dialog.RaidResultDialog));
            _dialogList.Add(typeof(Dialog.RaidPauseDialog));
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUnload()
        {

        }

        protected override void OnEnter()
        {

        }

        protected override void OnExit()
        {

        }
    }
}