// ==================================================
// LobbyBackgroundAnimation.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

public class LobbyBackgroundAnimation : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _bg;

    private void Awake()
    {
        Message.AddListener<Lobby.EnterContentDialogMsg>(OnEnterContentDialog);
        // Message.AddListener<Dialog.EnterDialogMsg>("LobbyContentDialog", EnterContent);
        Message.AddListener<Dialog.EnterDialogMsg>("LobbyMainDialog", EnterLobby);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<Lobby.EnterContentDialogMsg>(OnEnterContentDialog);
        // Message.RemoveListener<Dialog.EnterDialogMsg>("LobbyContentDialog", EnterContent);
        Message.RemoveListener<Dialog.EnterDialogMsg>("LobbyMainDialog", EnterLobby);
    }

    private void EnterLobby(Dialog.EnterDialogMsg msg)
    {
        _bg.localScale = Vector3.one * 0.45f;
        _anim.SetTrigger("EnterLobby");
    }

    private void OnEnterContentDialog(Lobby.EnterContentDialogMsg msg)
    {
        _bg.localScale = Vector3.one * 0.325f;
        _anim.SetTrigger("EnterContent");
    }
}
