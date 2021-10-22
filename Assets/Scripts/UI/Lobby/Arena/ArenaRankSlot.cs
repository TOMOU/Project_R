// ==================================================
// ArenaRankSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class ArenaRankSlot : MonoBehaviour
{
    [SerializeField] private Image _tierImage;
    [SerializeField] private Image _topTierImage;
    [SerializeField] private Text _rankText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _nickNameText;
    [SerializeField] private Text _gradeText;
    [SerializeField] private Text _arenaPointText;

    public void Init(ArenaUserModel.Data data)
    {
        _tierImage.sprite = Resources.Load<Sprite>(string.Format("Texture/Arena/TierIcon/{0}", data.grade));
        _topTierImage.enabled = data.idx <= 3;


        _topTierImage.rectTransform.anchoredPosition = new Vector2(-450f, data.idx <= 3 ? 0f : -15f);

        _rankText.text = data.idx.ToString();
        _levelText.text = string.Format("Lv. {0}", data.level);
        _nickNameText.text = data.userName.ToString();
        _gradeText.text = LocalizeManager.Singleton.GetString(12011 + data.grade - 1);
        _arenaPointText.text = string.Format(LocalizeManager.Singleton.GetString(12031), data.arenaPoint);
    }
}
