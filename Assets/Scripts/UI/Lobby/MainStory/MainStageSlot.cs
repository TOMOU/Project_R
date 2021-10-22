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

public class MainStageSlot : MonoBehaviour
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _slotIcon;
    [SerializeField] private Text _txtStageNum;
    [SerializeField] private GameObject[] _starArr;
    [SerializeField] private RectTransform[] _routeRectArr;
    [SerializeField] private Image[] _routeImgArr;
    [SerializeField] private Button _slotButton;

    private int _chapter;
    private int _stage;
    private bool _isLock;
    private bool _isOpen;
    private bool _isClear;
    private System.Action<int> _callback;

    private void Awake()
    {
        _slotButton.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _slotButton.onClick.RemoveAllListeners();
    }

    public void Init(int chapter, int stage, System.Action<int> callback)
    {
        gameObject.SetActive(true);

        this._chapter = chapter;
        this._stage = stage;
        this._callback = callback;

        _txtStageNum.text = string.Format("{0}-{1}", _chapter, _stage);

        _isLock = (_chapter - 1) * 4 + _stage > Info.My.Singleton.User.maxClearedMainstory + 1;
        _isOpen = (_chapter - 1) * 4 + _stage == Info.My.Singleton.User.maxClearedMainstory + 1;
        _isClear = (_chapter - 1) * 4 + _stage < Info.My.Singleton.User.maxClearedMainstory + 1;

        _rect.localScale = (_chapter == 4) ? Vector3.one * 1.5f : Vector3.one;

        if (_isLock == true)
        {
            _slotIcon.sprite = Resources.Load<Sprite>("Texture/MainChapter/UI/stage_slot_lock");
            _slotIcon.SetGrayScaleWithChild(true);
        }
        else if (_isOpen)
        {
            _slotIcon.sprite = Resources.Load<Sprite>("Texture/MainChapter/UI/stage_slot_open");
            _slotIcon.SetGrayScaleWithChild(false);
        }
        else if (_isClear)
        {
            _slotIcon.sprite = Resources.Load<Sprite>("Texture/MainChapter/UI/stage_slot_clear");
            _slotIcon.SetGrayScaleWithChild(false);
        }

        for (int i = 0; i < _starArr.Length; i++)
        {
            _starArr[i].SetActive(_isClear == true);
        }
    }

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector2 vec, Vector2 next)
    {
        _rect.anchoredPosition = vec;

        for (int i = 0; i < _routeRectArr.Length; i++)
        {
            _routeRectArr[i].gameObject.SetActive(true);
            _routeRectArr[i].anchoredPosition = (next - vec) / 4 * (i + 1);
            _routeRectArr[i].localRotation = Quaternion.Euler(0f, 0f, GetAngle(vec, next));

            if (_isClear == true)
            {
                _routeRectArr[i].GetComponent<Image>().SetGrayScale(false);
                _routeRectArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/MainChapter/UI/stage_route_active");
            }
            else
            {
                _routeRectArr[i].GetComponent<Image>().SetGrayScale(true);
                _routeRectArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Texture/MainChapter/UI/stage_route_inactive");
            }
        }
    }

    public void SetPosition(Vector2 vec)
    {
        _rect.anchoredPosition = vec;

        for (int i = 0; i < _routeRectArr.Length; i++)
        {
            _routeRectArr[i].gameObject.SetActive(false);
        }
    }

    private void OnClick()
    {
        if (_isLock == true)
        {
            Message.Send<Global.MessageBoxMsg>(new Global.MessageBoxMsg(LocalizeManager.Singleton.GetString(13), LocalizeManager.Singleton.GetString(17), null, true));
            return;
        }

        if (_callback != null)
            _callback(_stage);
    }

    private float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 v2 = end - start;
        return Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
    }
}
