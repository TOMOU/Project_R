// ==================================================
// BabelFlaskSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class BabelFlaskSlot : MonoBehaviour
{
    [SerializeField] private Image _flaskImage;
    [SerializeField] private Text _flaskName;

    [SerializeField] private Button _matRemoveBtn;
    [SerializeField] private Text _materialValue;
    [SerializeField] private Button _matAddBtn;

    [SerializeField] private Button _minus100;
    [SerializeField] private Button _plus100;
    [SerializeField] private Button _minus1000;
    [SerializeField] private Button _plus1000;

    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _maxButton;

    private int _idx;
    public int MaterialCount { get; private set; }

    private void Awake()
    {
        _matRemoveBtn.onClick.AddListener(OnClickRemove100);
        _matAddBtn.onClick.AddListener(OnClickAdd100);

        _minus100.onClick.AddListener(OnClickRemove100);
        _minus1000.onClick.AddListener(OnClickRemove1000);

        _plus100.onClick.AddListener(OnClickAdd100);
        _plus1000.onClick.AddListener(OnClickAdd1000);

        _resetButton.onClick.AddListener(OnClickReset);
        _maxButton.onClick.AddListener(OnClickMax);
    }

    private void OnDestroy()
    {
        _matRemoveBtn.onClick.RemoveAllListeners();
        _matAddBtn.onClick.RemoveAllListeners();
        _minus100.onClick.RemoveAllListeners();
        _plus100.onClick.RemoveAllListeners();
        _minus1000.onClick.RemoveAllListeners();
        _plus1000.onClick.RemoveAllListeners();
        _resetButton.onClick.RemoveAllListeners();
        _maxButton.onClick.RemoveAllListeners();
    }

    public void Init(int idx)
    {
        _idx = idx;
        MaterialCount = 100;
        _materialValue.text = string.Format("{0}", MaterialCount);
    }

    private void OnClickRemove100()
    {
        MaterialCount -= 100;

        if (MaterialCount < 100)
            MaterialCount = 100;

        _materialValue.text = string.Format("{0}", MaterialCount);
    }

    private void OnClickAdd100()
    {
        MaterialCount += 100;

        if (MaterialCount > 10000)
            MaterialCount = 10000;

        _materialValue.text = string.Format("{0}", MaterialCount);
    }

    private void OnClickRemove1000()
    {
        MaterialCount -= 1000;

        if (MaterialCount < 100)
            MaterialCount = 100;

        _materialValue.text = string.Format("{0}", MaterialCount);
    }

    private void OnClickAdd1000()
    {
        MaterialCount += 1000;

        if (MaterialCount > 10000)
            MaterialCount = 10000;

        _materialValue.text = string.Format("{0}", MaterialCount);
    }

    private void OnClickReset()
    {
        MaterialCount = 100;

        _materialValue.text = string.Format("{0}", MaterialCount);
    }

    private void OnClickMax()
    {
        MaterialCount = 10000;

        _materialValue.text = string.Format("{0}", MaterialCount);
    }
}
