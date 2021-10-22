// ==================================================
// ButtonBlur.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class ButtonBlur : MonoBehaviour
{
    private RectTransform _rect;
    private Image _image;

    private void Awake ()
    {
        // _rect = GetComponent<RectTransform> ();
        // if (_rect == null)
        // {
        //     Logger.LogWarningFormat ("{0} has no RectTransform...Please check it is UI element.", gameObject.name);
        //     return;
        // }

        // _image = GetComponent<Image> ();
        // if (_image == null)
        // {
        //     Logger.LogWarningFormat ("{0} has no Image Component...Please check it is UI element.", gameObject.name);
        //     return;
        // }

        // GameObject obj = new GameObject ();
        // obj.name = "Masked_Blur";
        // obj.SetActive (false);
        // obj.transform.SetParent (transform);
        // obj.transform.SetAsFirstSibling ();
        // obj.transform.localPosition = Vector3.zero;
        // obj.transform.localRotation = Quaternion.identity;
        // obj.transform.localScale = Vector3.one;
        // obj.SetActive (true);

        // RectTransform rect = obj.AddComponent<RectTransform> ();
        // Vector3 vec = _rect.position;
        // vec.z += 5f;
        // rect.position = vec;
        // rect.sizeDelta = _rect.sizeDelta;

        // Image img = CopyComponent<Image> (_image, obj);
        // img.SetMaskedBlur (1000f);
    }

    private T CopyComponent<T> (T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType ();
        var dst = destination.GetComponent (type) as T;
        if (!dst) dst = destination.AddComponent (type) as T;
        var fields = type.GetFields ();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue (dst, field.GetValue (original));
        }
        var props = type.GetProperties ();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue (dst, prop.GetValue (original, null), null);
        }
        return dst as T;
    }
}