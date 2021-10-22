namespace Content
{
    public class TitleContent : IContent
    {
        public override void Preload()
        {
            base.Preload();

            // UI 등록
            _dialogList.Add(typeof(Dialog.TitleMainDialog));
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