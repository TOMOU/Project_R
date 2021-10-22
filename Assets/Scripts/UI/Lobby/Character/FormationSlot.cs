// ==================================================
// FormationSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FormationSlot : MonoBehaviour
{
    [SerializeField] private GameObject _emptyIconObj;
    [SerializeField] private GameObject _insertIconObj;
    [SerializeField] private Button _button;
    [SerializeField] private SkeletonGraphic _spine;
    [HideInInspector] public int slotNumber;
    [HideInInspector] public uint idx;
    private System.Action<uint, FormationSlot> _callback;
    public Button FormationButton { get { return _button; } }
    private Texture2D _tex;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);

        _spine.UpdateComplete += UpdateComplete;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();

        _spine.UpdateComplete -= UpdateComplete;

        _tex = null;
    }

    void UpdateComplete(Spine.Unity.ISkeletonAnimation anim)
    {
        if (_tex == null)
            return;

        if (_spine.OverrideTexture != _tex)
        {
            _spine.OverrideTexture = _tex;
        }
    }

    public void Init(int index, Info.Character character)
    {
        this.slotNumber = index;

        _emptyIconObj.SetActive(character == null);
        _insertIconObj.SetActive(character != null);

        if (character == null)
        {
            idx = 0;
            return;
        }

        idx = character.idx;

        var um = Model.First<UnitModel>();
        var unit = um.unitTable.Find(e => e.code == character.code);

        _spine.skeletonDataAsset = SpineManager.Singleton.GetSkeletonDataAsset(unit.unitName);
        // SpineManager.Singleton.spineAssetDic.TryGetValue(unit.unitName, out _spine.skeletonDataAsset);
        // _spine.skeletonDataAsset = Resources.Load<Spine.Unity.SkeletonDataAsset>(string.Format("Character/SpineData/{0}/{0}_SkeletonData", unit.unitName));
        _spine.Initialize(true);

        _tex = null;
        if (SpineManager.Singleton.spineTextureDic.TryGetValue(unit.unitName, out _tex) == true)
        {

        }
    }

    public void AddCallback(System.Action<uint, FormationSlot> callback = null)
    {
        _callback = callback;
    }

    public void Refresh(Info.Character character)
    {
        _button.gameObject.SetActive(character == null);
        _spine.gameObject.SetActive(character != null);

        if (character == null)
        {
            idx = 0;
            return;
        }

        idx = character.idx;

        var um = Model.First<UnitModel>();
        var unit = um.unitTable.Find(e => e.code == character.code);
        _spine.skeletonDataAsset = SpineManager.Singleton.GetSkeletonDataAsset(unit.unitName);
        // SpineManager.Singleton.spineAssetDic.TryGetValue(unit.unitName, out _spine.skeletonDataAsset);
        // _spine.skeletonDataAsset = Resources.Load<Spine.Unity.SkeletonDataAsset>(string.Format("Character/SpineData/{0}/{0}_SkeletonData", character.code));
        _spine.Initialize(true);
    }

    private void OnClick()
    {
        if (_callback != null)
        {
            _callback(idx, this);
        }
    }
}
