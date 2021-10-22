// ==================================================
// DefenceResultDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class DefenceResultDialog : IDialog
    {
        private GameObject _bg;
        private GameObject _winObj;
        private GameObject _loseObj;
        private Button _moveLobbyButton;
        private Button _retryBattleButton;
        private int _msgCallCount;

        protected override void OnLoad()
        {
            _bg = _dialogView.transform.Find("bg").gameObject;
            _winObj = _dialogView.transform.Find("Win").gameObject;
            _loseObj = _dialogView.transform.Find("Lose").gameObject;
            _moveLobbyButton = _dialogView.transform.Find("Lobby").GetComponent<Button>();
            _retryBattleButton = _dialogView.transform.Find("Retry").GetComponent<Button>();

            _moveLobbyButton.onClick.AddListener(OnClickLobby);
            _retryBattleButton.onClick.AddListener(OnClickRetry);

            _bg.SetActive(false);
            _winObj.SetActive(false);
            _loseObj.SetActive(false);
            _moveLobbyButton.gameObject.SetActive(false);
            _retryBattleButton.gameObject.SetActive(false);

        }

        protected override void OnUnload()
        {
            _bg = null;
            _winObj = null;
            _loseObj = null;
            _moveLobbyButton.onClick.RemoveAllListeners();
            _moveLobbyButton = null;
            _retryBattleButton.onClick.RemoveAllListeners();
            _retryBattleButton = null;
        }

        protected override void OnEnter()
        {
            // Escape 동작 전부 지운다.
            Message.Send<Global.RemoveEscapeActionAllMsg>(new Global.RemoveEscapeActionAllMsg());

            Message.AddListener<Battle.Defence.GameEndMsg>(OnGameEnd);
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Battle.Defence.GameEndMsg>(OnGameEnd);
        }

        private void OnClickLobby()
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg("알림", "로비씬으로 이동하시겠습니까?",
                () => Scene.IScene.LoadScene<Scene.LobbyScene>(), null));
        }

        private void OnClickRetry()
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg("알림", "전투를 다시 시작하시겠습니까?",
                () => Scene.IScene.LoadScene<Scene.BattleDefenceScene>(), null));
        }

        private void OnGameEnd(Battle.Defence.GameEndMsg msg)
        {
            _bg.SetActive(true);
            _winObj.SetActive(msg.Victory == true);
            _loseObj.SetActive(msg.Victory == false);
            _moveLobbyButton.gameObject.SetActive(true);
            _retryBattleButton.gameObject.SetActive(true);
        }
    }
}