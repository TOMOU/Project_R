// ==================================================
// GlobalRewardDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dialog
{
    public class GlobalRewardDialog : IDialog
    {
        [SerializeField] private List<RewardSlot> _slotList;

        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _equipButton;

        private System.Action _callback;

        protected override void OnLoad()
        {
            base.OnLoad();

            _confirmButton.onClick.AddListener(OnClickConfirm);
            _equipButton.onClick.AddListener(OnClickEquipAuto);

            Message.AddListener<Global.ShowRewardMsg>(OnShowReward);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _confirmButton.onClick.RemoveAllListeners();
            _equipButton.onClick.RemoveAllListeners();

            Message.RemoveListener<Global.ShowRewardMsg>(OnShowReward);
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                Dialog.IDialog.RequestDialogExit<GlobalRewardDialog>();
            }));
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void OnClickConfirm()
        {
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClickEquipAuto()
        {
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnShowReward(Global.ShowRewardMsg msg)
        {
            bool isDNA = false;

            int count = msg.Items.Count;
            _callback = msg.Callback;

            for (int i = 0; i < _slotList.Count; i++)
            {
                if (i < count)
                {
                    _slotList[i].gameObject.SetActive(true);
                    _slotList[i].Init(msg.Items[i], msg.Values[i]);

                    if (msg.Items[i] == 46)
                        isDNA = true;
                }
                else
                {
                    _slotList[i].gameObject.SetActive(false);
                }
            }

            _equipButton.gameObject.SetActive(isDNA);

            CheckScenario();
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 79)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_confirmButton.transform, _grid, () =>
                {
                    OnClickConfirm();

                    if (_callback != null)
                        _callback();
                }));
            }
            if (sidx == 81)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_equipButton.transform, _grid, () =>
                {
                    OnClickEquipAuto();

                    if (_callback != null)
                        _callback();
                }));
            }
        }
    }
}