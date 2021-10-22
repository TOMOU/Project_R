// ==================================================
// UnitCardSlot_Raid.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitCardSlot_Raid : ICardSlot
{
    #region BuffIcon
    private class BuffIcon
    {
        public Constant.SkillType type;
        public Constant.SkillProperty property;
        public GameObject obj;
        public Image icon;
        public BuffIcon(Constant.SkillType type, Constant.SkillProperty property, GameObject obj, Image icon)
        {
            this.type = type;
            this.property = property;
            this.obj = obj;
            this.icon = icon;
        }
    }

    private List<BuffIcon> _buffIconList = new List<BuffIcon>();
    private int _buffCount = 0;
    #endregion

    [Header("Additional")]
    [SerializeField] private Image _hpSlider;
    [SerializeField] private Image _mpSlider;
    [SerializeField] private Button _useSkillButton;
    [SerializeField] private Transform _buffIconRoot;

    private bool _isDie;
    public UnitStatus Status { get { return _status; } }

    public override void Init(UnitStatus status)
    {
        base.Init(status);

        if (_status == null)
        {
            _portrait.SetGrayScaleWithChild(true);
            _hpSlider.fillAmount = 0f;
            _mpSlider.fillAmount = 0f;
            gameObject.SetActive(false);
        }
        else
        {
            _portrait.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/3_portrait_facesize/{0}", status.code));
            _portrait.SetGrayScaleWithChild(false);
            gameObject.SetActive(true);
            _isDie = false;
        }

        _useSkillButton.onClick.AddListener(OnClickUseSkill);
    }

    public override void Release()
    {
        base.Release();

        if (_buffIconList != null)
        {
            _buffIconList.Clear();
            _buffIconList = null;
        }

        _hpSlider = null;
        _mpSlider = null;

        _useSkillButton.onClick.RemoveAllListeners();
        _useSkillButton = null;
    }

    public override void Refresh()
    {
        if (_status == null)
            return;
        else if (_isDie == true)
        {
            _buffIconList.ForEach(e => e.obj.SetActive(false));
            return;
        }
        else if (_status.hp <= 0 && _isDie == false)
        {
            _isDie = true;
            _hpSlider.fillAmount = 0f;
            _portrait.SetGrayScaleWithChild(true);
            return;
        }

        // 버프 아이콘 갱신
        CheckBuffEnabled();

        // 체력과 마나 갱신
        _hpSlider.fillAmount = _status.hp / (float)_status.hpFull;
        _mpSlider.fillAmount = _status.mp / (float)_status.mpFull;
    }

    private void CheckBuffEnabled()
    {
        if (_buffCount == _status.buffCount)
            return;

        // 신규버프가 추가되었다.
        if (_buffCount < _status.buffCount)
        {
            // 새로 추가된 버프가 뭔지 검색
            for (int i = 0; i < _status.buffList.Count; i++)
            {
                BuffIcon bi = _buffIconList.Find(e => e.type == _status.buffList[i].type && e.property == _status.buffList[i].property);

                // 아이콘 자체가 없다.
                if (bi == null)
                {
                    GameObject obj = new GameObject(_status.buffList[i].property.ToString());

                    obj.transform.SetParent(_buffIconRoot);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localScale = Vector3.one;

                    Image icon = obj.AddComponent<Image>();

                    string path = string.Format("Texture/Battle/{0}/buffIcon_{1}", _status.buffList[i].type, _status.buffList[i].property);
                    Sprite spr = Resources.Load<Sprite>(path);
                    if (spr != null)
                    {
                        icon.sprite = spr;
                    }

                    bi = new BuffIcon(_status.buffList[i].type, _status.buffList[i].property, obj, icon);
                    _buffIconList.Add(bi);

                    if (bi.property == Constant.SkillProperty.Aggro || bi.property == Constant.SkillProperty.MinionShield || bi.property == Constant.SkillProperty.MinionSpear)
                        bi.obj.SetActive(false);
                }
                // 아이콘은 있는데 활성화가 안되었다.
                else if (bi.obj.activeSelf == false)
                {
                    if (bi.property == Constant.SkillProperty.Aggro || bi.property == Constant.SkillProperty.MinionShield || bi.property == Constant.SkillProperty.MinionSpear)
                        bi.obj.SetActive(false);
                    else
                    {
                        bi.obj.SetActive(true);
                        bi.obj.transform.SetAsLastSibling();
                    }
                }
            }
        }
        // 버프가 사라졌다.
        else
        {
            for (int i = 0; i < _buffIconList.Count; i++)
            {
                if (_buffIconList[i].obj.activeSelf == false)
                    continue;

                if (_status.buffList.Find(e => e.type == _buffIconList[i].type && e.property == _buffIconList[i].property) == null)
                {
                    _buffIconList[i].obj.SetActive(false);
                }
            }
        }

        _buffCount = _status.buffCount;
    }

    private void OnClickUseSkill()
    {
        if (_status == null)
            return;

        // 먼저 내가 죽었는지 판별
        if (_isDie == true || _status.hp <= 0)
            return;

        // 스킬 사용 가능한지 판별
        if (_status.mp < _status.mpFull)
            return;

        // 스킬 사용
        Message.Send<Battle.Normal.GenericUseSkillMsg>(new Battle.Normal.GenericUseSkillMsg(_status.idx));
    }
}
