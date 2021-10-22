// ==================================================
// StoryCharacterSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class StoryCharacterSlot : MonoBehaviour
{
    private enum SlotType
    {
        None,
        Character,
        Story,
    }
    [SerializeField] private SlotType _slotType;
    [SerializeField] private Image _imgPortrait;
    [SerializeField] private Text _isClearText;
    [SerializeField] private Text _txtLabel;
    [SerializeField] private Button _slotButton;
    [SerializeField] private GameObject _lockObj;

    private int _idx;
    private int _chapter;
    private System.Action<int> _callback;

    private void Awake()
    {
        _slotButton.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _slotButton.onClick.RemoveAllListeners();
    }

    public void InitCharacter(int idx, System.Action<int> callback)
    {
        this._idx = idx;
        this._slotType = SlotType.Character;

        _imgPortrait.sprite = Resources.Load<Sprite>(string.Format("Texture/Story/{0}", idx));

        // var um = Model.First<UnitModel>();
        // UnitModel.Unit unit = um.unitTable.Find(e => e.code == idx);

        switch (idx)
        {
            case 100211:
                _txtLabel.text = LocalizeManager.Singleton.GetString(10003);
                break;

            case 101031:
                _txtLabel.text = LocalizeManager.Singleton.GetString(10002);
                break;

            case 101131:
                _txtLabel.text = LocalizeManager.Singleton.GetString(10004);
                break;

            case 102831:
                _txtLabel.text = LocalizeManager.Singleton.GetString(10005);
                break;

            case 103031:
                _txtLabel.text = LocalizeManager.Singleton.GetString(10006);
                break;
        }

        bool isLock = idx != 101031;

        _imgPortrait.SetGrayScaleWithChild(isLock);
        _lockObj.SetActive(isLock);

        if (isLock == false)
            _isClearText.text = string.Format("OPEN");
        else
            _isClearText.text = string.Format("LOCK");

        _callback = callback;
    }

    public void InitChapter(int idx, int chapter, System.Action<int> callback)
    {
        this._idx = idx;
        this._chapter = chapter;
        this._slotType = SlotType.Story;

        _imgPortrait.sprite = Resources.Load<Sprite>(string.Format("Texture/Story/{0}", idx));
        _txtLabel.text = string.Format("Chapter. {0}", _chapter);

        bool isLock = chapter != 1;

        _imgPortrait.SetGrayScaleWithChild(isLock);
        _lockObj.SetActive(isLock);

        if (isLock == false)
            _isClearText.text = string.Format("OPEN");
        else
            _isClearText.text = string.Format("LOCK");


        _callback = callback;
    }

    private void OnClick()
    {
        if (_callback != null)
        {
            if (_slotType == SlotType.Character)
                _callback(_idx);
            else
                _callback(_chapter);
        }
    }
}
