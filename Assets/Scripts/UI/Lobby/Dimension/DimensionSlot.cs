// ==================================================
// DimensionSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class DimensionSlot : MonoBehaviour
{
    [SerializeField] private Image _portrait;
    [SerializeField] private Text _chapterName;
    [SerializeField] private Text _isClearText;
    [SerializeField] private Button _button;

    private int _level;
    private bool _isLock;
    private System.Action<int> _callback = null;


    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Init(int level, System.Action<int> callback)
    {
        this._level = level;

        _portrait.sprite = Resources.Load<Sprite>(string.Format("Texture/MainChapter/Icon/Img_StorySlot_{0}", level));
        _chapterName.text = string.Format("{0}\n{1}", LocalizeManager.Singleton.GetString(9001), LocalizeManager.Singleton.GetString(9002 + level - 1));

        _isLock = _level > Info.My.Singleton.User.maxClearedDimension + 1;

        _portrait.SetGrayScaleWithChild(_isLock);

        if (_isLock == true)
        {
            _isClearText.text = "LOCK";
        }
        else
        {
            if (_level == Info.My.Singleton.User.maxClearedDimension + 1)
                _isClearText.text = "OPEN";
            else
                _isClearText.text = "CLEAR";
        }

        this._callback = callback;
    }

    private void OnClick()
    {
        if (_callback != null)
        {
            if (_isLock)
            {
                Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(12), null, true));
                return;
            }

            _callback(_level);
        }
    }
}