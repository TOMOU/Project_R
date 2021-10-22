// ==================================================
// CameraFade.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFade : MonoBehaviour
{
    private Shader _shader;
    [Range(0, 1)] public float maskValue = 1;
    private Material _cachedMaterial;
    private Material _material
    {
        get
        {
            if (_cachedMaterial == null)
            {
                _cachedMaterial = new Material(_shader);
                _cachedMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return _cachedMaterial;
        }
    }

    private void Start()
    {
        _shader = Shader.Find("Hidden/CameraFade");

        if (_shader == null || _shader.isSupported == false)
            enabled = false;
    }

    public void Release()
    {
        _shader = null;

        if (_cachedMaterial != null)
        {
            DestroyImmediate(_cachedMaterial);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (enabled == false)
        {
            Graphics.Blit(source, dest);
            return;
        }

        _material.SetFloat("_MaskValue", maskValue);
        _material.SetTexture("_MainTex", source);

        Graphics.Blit(source, dest, _material);
    }
}
