// ==================================================
// TutorialLogic.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

namespace Logic
{
    public class TutorialLogic : ILogic
    {
        protected override void Initialize()
        {
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(1, () =>
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(2, CreateNickname));
            }));
        }

        private void CreateNickname()
        {
            Dialog.IDialog.RequestDialogEnter<Dialog.TutorialNicknameDialog>();
        }
    }
}