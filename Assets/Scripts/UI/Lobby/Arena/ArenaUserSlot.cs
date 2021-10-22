// ==================================================
// ArenaUserSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class ArenaUserSlot : MonoBehaviour
{
    [SerializeField] private Image _tierImage;
    [SerializeField] private Text _rankText;
    [SerializeField] private Text _guildNameText;
    [SerializeField] private Text _nickNameText;
    [SerializeField] private Text _arenaPointText;
    [SerializeField] private Text _battlePointText;
    [SerializeField] private Button _button;
    public Button ChallengeButton { get { return _button; } }
    public ArenaUserModel.Data Data { get { return _data; } }

    private ArenaUserModel.Data _data;
    private System.Action<ArenaUserModel.Data> _callback;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Init(ArenaUserModel.Data data, System.Action<ArenaUserModel.Data> callback)
    {
        _tierImage.sprite = Resources.Load<Sprite>(string.Format("Texture/Arena/TierIcon/{0}", data.grade));
        _rankText.text = data.idx.ToString();
        _guildNameText.text = data.guildName.ToString();
        _nickNameText.text = data.userName.ToString();
        _arenaPointText.text = string.Format("{0:##,##0} bp", data.arenaPoint);

        int bp = CalculateBP(data);
        _battlePointText.text = string.Format("{0:##,##0}", bp);

        _data = data;
        _callback = callback;
    }

    private int CalculateBP(ArenaUserModel.Data data)
    {
        int result = 0;

        var um = Model.First<UnitModel>();
        var table = um.unitTable;
        UnitStatus status;



        if (data.unit_id_1 > 0)
        {
            status = new UnitStatus(table.Find(e => e.code == data.unit_id_1), 0, data.unit_lv_1, 1);
            result += BattleCalc.Calculate_BattlePower(status);
        }

        if (data.unit_id_2 > 0)
        {
            status = new UnitStatus(table.Find(e => e.code == data.unit_id_2), 0, data.unit_lv_2, 1);
            result += BattleCalc.Calculate_BattlePower(status);
        }

        if (data.unit_id_3 > 0)
        {
            status = new UnitStatus(table.Find(e => e.code == data.unit_id_3), 0, data.unit_lv_3, 1);
            result += BattleCalc.Calculate_BattlePower(status);
        }

        if (data.unit_id_4 > 0)
        {
            status = new UnitStatus(table.Find(e => e.code == data.unit_id_4), 0, data.unit_lv_4, 1);
            result += BattleCalc.Calculate_BattlePower(status);
        }

        if (data.unit_id_5 > 0)
        {
            status = new UnitStatus(table.Find(e => e.code == data.unit_id_5), 0, data.unit_lv_5, 1);
            result += BattleCalc.Calculate_BattlePower(status);
        }

        return result;
    }

    private void OnClick()
    {
        if (_data == null)
            return;

        if (_callback != null)
            _callback(_data);
    }
}
