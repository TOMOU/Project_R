// ==================================================
// ResultCharSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class ResultCharSlot : MonoBehaviour
{
    [SerializeField] private SkeletonGraphic _skeleton;

    [SerializeField] private Text _charLevelText;
    [SerializeField] private Image _charExpValue;
    [SerializeField] private Text _remainExpText;

    [SerializeField] private Image _charFavorValue;
    [SerializeField] private Text _charFavorText;

    private Texture2D _resizeTex;
    private string _unitName;

    public void Init(SkeletonDataAsset data, UnitStatus status)
    {
        _skeleton.skeletonDataAsset = data;

        _unitName = status.unitName;

        Info.Character character = Info.My.Singleton.Inventory.GetCharacterByIndex(status.idx);

        _charLevelText.text = string.Format("Lv. {0}", character.level);
        _charExpValue.fillAmount = character.exp / (float)character.nexp;
        _remainExpText.text = string.Format(LocalizeManager.Singleton.GetString(17001), character.nexp - character.exp);

        _charFavorValue.fillAmount = 1f;
        _charFavorText.text = "MAX";

        _skeleton.UpdateComplete += UpdateComplete;
    }

    public void Active()
    {
        _skeleton.Initialize(true);
        _skeleton.AnimationState.SetAnimation(0, "idle", true);
    }

    private void OnDestroy()
    {
        _skeleton.UpdateComplete -= UpdateComplete;
    }

    private void UpdateComplete(Spine.Unity.ISkeletonAnimation anim)
    {
        if (_resizeTex == null)
            SpineManager.Singleton.spineTextureDic.TryGetValue(_unitName, out _resizeTex);

        if (_resizeTex != null && _skeleton.OverrideTexture != _resizeTex)
        {
            _skeleton.OverrideTexture = _resizeTex;
            // _renderer.material.mainTexture = _resizeTex;
        }
    }

    public void Victory()
    {
        _skeleton.AnimationState.SetAnimation(0, "victory", false);
    }
}
