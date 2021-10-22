// ==================================================
// GachaCardSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GachaCardSlot : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Image _imgCharacter;
    [SerializeField] private GameObject _backObj;
    [SerializeField] private Button _button;
    [SerializeField] private Image _effect;
    private System.Action _callback;
    private Coroutine _coroutine;
    private bool _isFlipped = false;
    public bool IsComplete { get { return _backObj.activeSelf == false; } }

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Init(int code, System.Action callback)
    {
        _imgCharacter.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/2_portrait_cardsize/{0}", code));
        _callback = callback;

        Color color;
        string hex = string.Empty;

        switch (code)
        {
            case 100211:
                hex = "#D400FFFF";
                break;

            case 101031:
                hex = "#FF8900FF";
                break;

            case 101131:
                hex = "#F6FF00FF";
                break;

            case 102831:
                hex = "#0099FFFF";
                break;

            case 103031:
                hex = "#FF99EBFF";
                break;

            case 111001:
                hex = "#D39D6AFF";
                break;

            case 112001:
                hex = "#FF0000FF";
                break;
        }

        if (ColorUtility.TryParseHtmlString(hex, out color) == true)
            _effect.color = color;

        _isFlipped = false;
    }

    public void OnClick()
    {
        if (_isFlipped == true)
            return;

        if (IsComplete == true)
            return;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _isFlipped = true;

        _coroutine = StartCoroutine(coFlip());
    }

    private IEnumerator coFlip()
    {
        _anim.SetTrigger("Flip");

        yield return new WaitForSeconds(0.5f);

        while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        if (_callback != null)
            _callback();
    }
}
