using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    public class GlobalContent : IContent
    {
        public override void Preload()
        {
            base.Preload();

            _isGlobalContent = true;

            // UI 등록
            _dialogList.Add(typeof(Dialog.GlobalLoadingDialog));
            _dialogList.Add(typeof(Dialog.GlobalMessageDialog));
            _dialogList.Add(typeof(Dialog.GlobalScenarioDialog));
            _dialogList.Add(typeof(Dialog.GlobalGuideDialog));
            // _dialogList.Add(typeof(Dialog.GlobalDebugDialog));
            _dialogList.Add(typeof(Dialog.GlobalRewardDialog));
            _dialogList.Add(typeof(Dialog.GlobalLocalizeDialog));
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Model.First<GameModel>().loadCompleteGlobalContent = true;
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