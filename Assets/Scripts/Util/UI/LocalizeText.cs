// ==================================================
// LocalizeText.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class LocalizeText : MonoBehaviour
{
    [SerializeField] private int _idx;
    [SerializeField] private Text _text;
    [SerializeField] private TextMeshProUGUI _textPro;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponent<Text>();
        }
        if (_textPro == null)
        {
            _textPro = GetComponent<TextMeshProUGUI>();
        }

        if (Application.isPlaying == false)
            return;

        ChangeText();

        Message.AddListener<Global.ChangeLocaleMsg>(OnChangeLocale);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<Global.ChangeLocaleMsg>(OnChangeLocale);
    }

    private void OnChangeLocale(Global.ChangeLocaleMsg msg)
    {
        ChangeText();
    }

    private void ChangeText()
    {
        if (_text != null)
        {
            _text.text = LocalizeManager.Singleton.GetString(_idx);
        }
        else if (_textPro != null)
        {
            _textPro.text = LocalizeManager.Singleton.GetString(_idx);
        }
    }
}
