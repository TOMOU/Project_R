// ==================================================
// UIBlur.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class UIBlurSelf : MonoBehaviour
{
    private Image _image;
    private Material _material;
    [SerializeField] private Color _color = Color.white;
    [SerializeField, Range(0f, 1f)] private float _intensity = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _multiplier = 0.5f;

    private void Awake()
    {
        if (_image == null)
            _image = GetComponent<Image>();

        if (_image.material.name != "Krivodeling/UI/UIBlur")
        {
            _material = new Material(Shader.Find("Krivodeling/UI/UIBlur"));
            _material.SetColor("_Color", _color);
            _material.SetFloat("_Intensity", _intensity);
            _material.SetFloat("_Multiplier", _multiplier);

            _image.material = _material;
        }
    }

    private void OnDestroy()
    {
        _material = null;
    }
}
