using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardSlot : ICardSlot
{
    public uint idx;
    public int code;
    [SerializeField] private GameObject[] _gradeObj;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _charNameText;
    [SerializeField] private Button _button;
    [SerializeField] private Image _frame;
    private System.Action<uint> _callback;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    public override void Init(UnitStatus status)
    {
        base.Init(status);

        if (status == null)
        {
            idx = 0;
            _levelText.text = string.Empty;
            _charNameText.text = string.Empty;

            return;
        }

        idx = status.idx;
        code = status.code;

        // 성급
        for (int i = 0; i < _gradeObj.Length; i++)
        {
            _gradeObj[i].SetActive(i < _status.grade);
        }

        // 레벨
        _levelText.text = string.Format("Lv.{0}", _status.level);

        // 이름
        _charNameText.text = _status.unitName_Kor;
    }

    public override void Release()
    {
        base.Release();

        _button.onClick.RemoveAllListeners();
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public void AddCallback(System.Action<uint> callback = null)
    {
        _callback = callback;
    }

    public void OnClick()
    {
        if (_callback != null)
        {
            _callback(idx);
        }
    }

    public void OnSelect()
    {
        _frame.color = Color.red;
    }

    public void OnDeselect()
    {
        _frame.color = Color.white;
    }
}