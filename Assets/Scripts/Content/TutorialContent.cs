// ==================================================
// TutorialContent.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

namespace Content
{
    public class TutorialContent : IContent
    {
        public override void Preload()
        {
            base.Preload();

            // UI 등록
            _dialogList.Add(typeof(Dialog.TutorialNicknameDialog));
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