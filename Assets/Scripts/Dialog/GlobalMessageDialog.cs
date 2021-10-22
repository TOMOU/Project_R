using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class GlobalMessageDialog : IDialog
    {
        private Text _titleText = null;
        private Text _msgText = null;
        private Button _cancelButton = null;
        private Button _confirmButton = null;
        private Text _confirmText = null;

        System.Action _confirmCallback = null;
        System.Action _cancelCallback = null;

        protected override void OnLoad()
        {
            _titleText = _dialogView.transform.Find("Popup/Top").GetComponent<Text>();
            _msgText = _dialogView.transform.Find("Popup/Middle").GetComponent<Text>();
            _cancelButton = _dialogView.transform.Find("Popup/Button/No").GetComponent<Button>();
            _confirmButton = _dialogView.transform.Find("Popup/Button/Yes").GetComponent<Button>();
            _confirmText = _confirmButton.transform.Find("Text").GetComponent<Text>();

            Message.AddListener<Global.MessageBoxMsg>(OnMessageBox);
        }

        protected override void OnUnload()
        {
            _titleText = null;
            _msgText = null;
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton = null;
            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton = null;
            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton = null;
            _confirmText = null;

            Message.RemoveListener<Global.MessageBoxMsg>(OnMessageBox);
        }

        protected override void OnEnter()
        {
            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                if (_dialogView != null)
                    _dialogView.SetActive(false);

                // Only 확인버튼에서 Escape할 때
                if (_cancelButton.gameObject.activeSelf == false && _confirmCallback != null)
                    _confirmCallback();
                else if (_cancelButton.gameObject.activeSelf == true && _cancelCallback != null)
                    _cancelCallback();
            }));
        }

        protected override void OnExit()
        {

        }

        private void OnMessageBox(Global.MessageBoxMsg msg)
        {
            if (_dialogView != null)
                _dialogView.SetActive(true);

            OnEnter();

            _titleText.text = msg.Title;
            _msgText.text = msg.Message;

            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _confirmCallback = msg.Confirm;
            _cancelCallback = msg.Cancel;

            _confirmButton.onClick.AddListener(() =>
            {
                CloseMessageBox();

                if (msg.OnlyConfirm == false)
                {
                    if (_confirmCallback != null)
                    {
                        // Message.Send<Global.PlaySoundMsg> (new Global.PlaySoundMsg (Constant.SoundName.button_confirm));
                        _confirmCallback();
                    }
                    else
                    {
                        // Message.Send<Global.PlaySoundMsg> (new Global.PlaySoundMsg (Constant.SoundName.button_click));
                    }
                }
            });

            _cancelButton.onClick.AddListener(() =>
            {
                CloseMessageBox();

                if (_cancelCallback != null)
                    _cancelCallback();
            });

            _cancelButton.gameObject.SetActive(msg.OnlyConfirm == false);
            _confirmText.text = msg.OnlyConfirm ? LocalizeManager.Singleton.GetString(6) : LocalizeManager.Singleton.GetString(3);
        }

        private void CloseMessageBox()
        {
            if (_dialogView != null)
                _dialogView.SetActive(false);

            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }
    }
}