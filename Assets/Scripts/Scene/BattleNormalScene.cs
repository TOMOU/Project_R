// ==================================================
// BattleNormalScene.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

namespace Scene
{
    public class BattleNormalScene : IScene
    {
        public override void Preload()
        {
            base.Preload();

            // 컨텐츠 등록
            _contentList.Add(typeof(Content.BattleNormalContent));
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // 글로벌 컨텐츠는 최초 1번만 로드
            GameModel gm = Model.First<GameModel>();
            if (gm != null && gm.loadCompleteGlobalContent == false)
            {
                _contentList.Add(typeof(Content.GlobalContent));

                // 메모리 반환
                gm = null;
            }

            _loadSceneName = GetType().Name;
        }

        protected override void OnUnload()
        {

        }

        protected override void OnLoadComplete()
        {
            if (TutorialManager.Singleton.curTutorialIndex < 4)
            {
                Message.Send<Global.AddBaseEscapeActionMsg>(new Global.AddBaseEscapeActionMsg(Tutorial_OnEscape));
            }
            else
            {
                // Escape 동작 추가
                base.OnEscape();
            }

            // 로직 추가
            UnityEngine.GameObject obj = new UnityEngine.GameObject();
            _loadedLogic = obj.AddComponent<Logic.BattleNormalLogic>();
            _loadedLogic.Setup();

            // 사운드 전부 끄고
            Message.Send<Global.StopAllSoundMsg>(new Global.StopAllSoundMsg());

            // BGM 재생
            Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Battle));
        }

        protected override void OnEscape()
        {
            Dialog.IDialog.RequestDialogEnter<Dialog.NormalPauseDialog>();
        }

        private void Tutorial_OnEscape()
        {
            // 시간을 멈추고
            Message.Send<Global.EnablePauseMsg>(new Global.EnablePauseMsg());

            // 메세지박스를 출력한다.
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(1), LocalizeManager.Singleton.GetString(2), Tutorial_EscapeConfirm, Tutorial_EscapeCancel));
        }

        private void Tutorial_EscapeConfirm()
        {
            UnityEngine.Application.Quit();
        }

        private void Tutorial_EscapeCancel()
        {
            // 시간을 다시 되돌린다.
            Message.Send<Global.DisablePauseMsg>(new Global.DisablePauseMsg());
        }
    }
}