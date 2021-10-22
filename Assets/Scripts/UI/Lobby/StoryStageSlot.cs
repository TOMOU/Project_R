// ==================================================
// StoryStageSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class StoryStageSlot : MonoBehaviour
{
    [SerializeField] private Text _txtStageNum;
    [SerializeField] private GameObject _lockObj;
    [SerializeField] private Button _slotButton;
    [SerializeField] private RectTransform _rect;
    private int _stageNum;
    private System.Action<int> _callback;

    private void Awake()
    {
        _slotButton.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _slotButton.onClick.RemoveAllListeners();
    }

    public void Init(int stageNum, System.Action<int> callback)
    {
        gameObject.SetActive(true);

        this._stageNum = stageNum;
        this._callback = callback;

        _txtStageNum.text = string.Format("Stage.{0}", _stageNum);

        _lockObj.SetActive(_stageNum > Info.My.Singleton.User.maxClearedStory + 1);
    }

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector2 vec)
    {
        _rect.anchoredPosition = vec;
    }


    private void OnClick()
    {
        if (_stageNum > Info.My.Singleton.User.maxClearedStory + 1)
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(17), null, true));
            return;
        }

        if (_callback != null)
            _callback(_stageNum);
    }
}
