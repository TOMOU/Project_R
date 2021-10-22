// ==================================================
// BabelFloorSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using System;

public class BabelFloorSlot : MonoBehaviour
{
    [Header("- Floor")]
    [SerializeField] private Sprite[] _floorSpr;
    [SerializeField] private Image _floorImage;
    [SerializeField] private Text _floorCountText;

    [Header("- Background")]
    [SerializeField] private Image _floorBackground;
    [SerializeField] private GameObject _objLock;

    [Header("- Character Info")]
    [SerializeField] private Image _characterImage;
    [SerializeField] private GameObject[] _batchObj;

    [Header("- Function Button")]
    [SerializeField] private Button _button;

    [Header("- Spine")]
    [SerializeField] private GameObject _objSpine;
    [SerializeField] private SkeletonGraphic _character;
    [SerializeField] private SkeletonGraphic _monster;
    [SerializeField] private Text _remainTimeText;


    private int _floor;
    private int _code;
    private System.Action<int> _enterCallback;
    private System.Action<int> _dispatchCallback;
    private Info.User.BabelDispatchState _data;
    private int[] mCode = { 104001, 104002, 105001, 105002, 105003, 106001, 106002, 106003, 106004, 107001, 107002, 107003, 107004, 108001, 108002, 108003, 108004, 109001 };
    private TimeSpan _remainTime;

    public void Init(int floor, System.Action<int> enter, System.Action<int> dispatch)
    {
        _floor = floor;
        _enterCallback = enter;
        _dispatchCallback = dispatch;

        gameObject.SetActive(true);

        bool isLock = _floor > Info.My.Singleton.User.maxClearedBabel + 1;

        // 층 정보 초기화
        _floorImage.sprite = _floorSpr[isLock ? 1 : 0];
        _floorCountText.text = string.Format(LocalizeManager.Singleton.GetString(11002), _floor);

        // 배경 정보 초기화
        Sprite spr = Resources.Load<Sprite>(string.Format("Texture/Babel/field_{0}", _floor % 3));
        if (spr != null)
            _floorBackground.sprite = spr;
        _objLock.SetActive(isLock);

        _data = Info.My.Singleton.User.babelDispatchSlotList.Find(e => e.idx == _floor - 1);

        Refresh();
    }

    public void Init()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        // 버튼동작 등록
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();

        _enterCallback = null;
    }

    public void Refresh()
    {
        if (_data == null)
            return;

        // 캐릭터 정보 초기화
        if (_data.character > 0)
        {
            _code = Info.My.Singleton.Inventory.GetCharacterByIndex(_data.character).code;
            _characterImage.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/3_portrait_facesize/{0}", _code));

            SetCharacter();
            SetMonster();
        }

        _objSpine.SetActive(_data.character > 0);

        _batchObj[0].SetActive(_data.character > 0);      // 배치한 캐릭터가 있다면
        _batchObj[1].SetActive(_data.character == 0);       // 배치한 캐릭터가 없다면        
    }

    private void Update()
    {
        if (_data == null)
            return;


        if (_data.character == 0)
            return;

        if (_data.state != 1)
            return;

        _remainTime = _data.endTime - DateTime.Now;
        if (_remainTime.TotalMilliseconds <= 0)
        {
            _data.state = 2;
            _remainTimeText.text = LocalizeManager.Singleton.GetString(11023);

            _character.AnimationState.SetAnimation(0, "victory", false);
            _monster.AnimationState.SetAnimation(0, "die", false);
            return;
        }

        _remainTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", _remainTime.Hours, _remainTime.Minutes, _remainTime.Seconds);
    }

    private void OnClick()
    {
        if (_floor < Info.My.Singleton.User.maxClearedBabel + 1)
        {
            if (_dispatchCallback != null)
                _dispatchCallback(_floor);
        }
        else if (_floor == Info.My.Singleton.User.maxClearedBabel + 1)
        {
            if (_enterCallback != null)
                _enterCallback(_floor);
        }
    }

    private void SetCharacter()
    {
        var um = Model.First<UnitModel>();
        string uName = um.unitTable.Find(e => e.code == _code).unitName;
        _character.skeletonDataAsset = SpineManager.Singleton.GetSkeletonDataAsset(uName);
        // SpineManager.Singleton.spineAssetDic.TryGetValue(uName, out _character.skeletonDataAsset);
        // _character.skeletonDataAsset = Resources.Load<Spine.Unity.SkeletonDataAsset>(string.Format("Character/SpineData/{0}/{0}_SkeletonData", uName));
        _character.Initialize(true);

        if (_data.state == 1)
        {
            _character.AnimationState.SetAnimation(0, "idle", true);
            float delay = _character.Skeleton.Data.Animations.Find(e => e.Name == "idle").Duration;
            _character.AnimationState.AddAnimation(0, "attack", true, delay);
        }
        else if (_data.state == 2)
        {
            _character.AnimationState.SetAnimation(0, "victory", false);
        }
    }

    private void SetMonster()
    {
        var um = Model.First<UnitModel>();
        int rCode = mCode[UnityEngine.Random.Range(0, mCode.Length - 1)];
        string mName = um.unitTable.Find(e => e.code == rCode).unitName;
        int skinIndex = 0;

        if (mName.Contains("_") == true)
        {
            var split = mName.Split('_');
            mName = split[0];
            skinIndex = int.Parse(split[1]);
        }

        _monster.skeletonDataAsset = SpineManager.Singleton.GetSkeletonDataAsset(mName);
        // SpineManager.Singleton.spineAssetDic.TryGetValue(mName, out _monster.skeletonDataAsset);
        // _monster.skeletonDataAsset = Resources.Load<Spine.Unity.SkeletonDataAsset>(string.Format("Character/SpineData/{0}/{0}_SkeletonData", mName));
        _monster.Initialize(true);

        // Set Skin
        if (skinIndex > 0)
        {
            _monster.Skeleton.SetSkin(_monster.Skeleton.Data.Skins.Items[skinIndex]);
        }

        _monster.Skeleton.ScaleX = -1f;

        if (_data.state == 1)
        {
            _monster.AnimationState.SetAnimation(0, "idle", true);
            float delay = _monster.Skeleton.Data.Animations.Find(e => e.Name == "idle").Duration;
            _monster.AnimationState.AddAnimation(0, "attack", true, delay);
        }
        else if (_data.state == 2)
        {
            _monster.AnimationState.SetAnimation(0, "die", false);
        }

    }
}
