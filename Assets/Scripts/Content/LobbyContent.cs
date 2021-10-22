namespace Content
{
    public class LobbyContent : IContent
    {
        public override void Preload()
        {
            base.Preload();

            // UI 등록
            _dialogList.Add(typeof(Dialog.LobbyMainDialog));
            _dialogList.Add(typeof(Dialog.LobbyCharacterDialog));
            _dialogList.Add(typeof(Dialog.LobbyFormationDialog));
            _dialogList.Add(typeof(Dialog.LobbyGachaDialog));
            _dialogList.Add(typeof(Dialog.LobbyContentDialog));
            _dialogList.Add(typeof(Dialog.LobbyMissionDialog));
            _dialogList.Add(typeof(Dialog.LobbyMainChapDialog));
            _dialogList.Add(typeof(Dialog.LobbyDimensionDialog));
            _dialogList.Add(typeof(Dialog.LobbyEquipDialog));
            _dialogList.Add(typeof(Dialog.LobbyBabelDialog));
            _dialogList.Add(typeof(Dialog.LobbyStoryDialog));
            _dialogList.Add(typeof(Dialog.LobbyStageInfoDialog));
            _dialogList.Add(typeof(Dialog.LobbyBossDialog));
            _dialogList.Add(typeof(Dialog.LobbyArenaDialog));
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