// ==================================================
// LocalizeImage.cs
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
public class LocalizeImage : MonoBehaviour
{
    [SerializeField] private string _sprName;
    [SerializeField] private Image _image;

    private void Awake()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
            if (_image != null)
            {
                _sprName = _image.sprite.name;
            }
        }

        if (Application.isPlaying == false)
            return;

        ChangeImage();

        Message.AddListener<Global.ChangeLocaleMsg>(OnChangeLocale);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<Global.ChangeLocaleMsg>(OnChangeLocale);
    }

    private void OnChangeLocale(Global.ChangeLocaleMsg msg)
    {
        ChangeImage();
    }

    private void ChangeImage()
    {
        if (_image != null)
        {
            _image.sprite = Resources.Load<Sprite>(string.Format("Texture/Localize/{0}/{1}", LocalizeManager.Singleton.locale, _sprName));
        }
    }
}