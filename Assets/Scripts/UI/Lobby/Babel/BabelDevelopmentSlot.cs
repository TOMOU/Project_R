// ==================================================
// BabelDevelopmentSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class BabelDevelopmentSlot : MonoBehaviour
{
    [SerializeField] private Image _imgBoard;
    [SerializeField] private Text _txtState;
    [SerializeField] private Button _button;

    private int _idx;
    private Info.User.BabelDevState _data;
    private TimeSpan _remainTime;
    private System.Action<int> _openProduction;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void AddOpenProduction(System.Action<int> callback)
    {
        _openProduction = callback;
    }

    public void Init(Info.User.BabelDevState state)
    {
        _data = state;
        _idx = _data.idx;

        ChangeBoard();
    }

    public void Refresh()
    {
        if (_data.state == 2)
        {
            _remainTime = _data.endTime - DateTime.Now;
            if (_remainTime.Ticks <= 0)
            {
                _data.state = 3;
            }

            ChangeBoard();
        }
    }

    private void ChangeBoard()
    {
        string sprName = string.Empty;
        string msgName = string.Empty;
        switch (_data.state)
        {
            case 0:
                sprName = "labo_btn_lock";
                msgName = string.Format("{0}\n{1}", LocalizeManager.Singleton.GetString(11010), LocalizeManager.Singleton.GetString(11011));
                break;

            case 1:
                sprName = "labo_btn_wait";
                msgName = LocalizeManager.Singleton.GetString(11012);
                break;

            case 2:
                sprName = "labo_btn_progress";
                msgName = string.Format("<size=80>{0:D2}:{1:D2}:{2:D2}</size>\n<size=50>{3}</size>", _remainTime.Hours, _remainTime.Minutes, _remainTime.Seconds, LocalizeManager.Singleton.GetString(11013));
                break;

            case 3:
                sprName = "labo_btn_complete";
                msgName = string.Format("<size=80>00:00:00</size>\n<size=50>{0}</size>", LocalizeManager.Singleton.GetString(11014));
                break;
        }

        _imgBoard.sprite = Resources.Load<Sprite>(string.Format("Texture/Button/{0}", sprName));
        _txtState.text = msgName;
    }

    public void OnClick()
    {
        if (_data == null)
            return;

        var info = Info.My.Singleton.User.babelDevSlotList[_idx];

        switch (_data.state)
        {
            case 0:
                info.state = 1;
                break;

            case 1:
                if (_openProduction != null)
                    _openProduction(_idx);
                break;

            case 2:
                info.state = 3;
                info.endTime = DateTime.Now;
                break;

            case 3:
                info.state = 1;

                //! 보상 DNA아이템 생성
                List<int> reward_id = new List<int>();
                List<int> reward_value = new List<int>();

                reward_id.Add(46);
                reward_value.Add(1);

                // 보상UI 출력 후 바로 장착
                Dialog.IDialog.RequestDialogEnter<Dialog.GlobalRewardDialog>();
                Message.Send<Global.ShowRewardMsg>(new Global.ShowRewardMsg(reward_id, reward_value, () =>
                {

                }));
                break;
        }

        ChangeBoard();
    }

    public void ConfirmCraft()
    {
        var info = Info.My.Singleton.User.babelDevSlotList[_idx];

        info.state = 2;
        info.endTime = DateTime.Now.AddHours(UnityEngine.Random.Range(1, 5)).AddMinutes(UnityEngine.Random.Range(0, 6) * 10);
        _remainTime = _data.endTime - DateTime.Now;
        ChangeBoard();
    }
}
