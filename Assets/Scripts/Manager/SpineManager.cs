// ==================================================
// SpineManager.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpineManager : MonoSingleton<SpineManager>
{
    public Dictionary<string, Texture2D> spineTextureDic;
    public Dictionary<string, SkeletonDataAsset> spineAssetDic;
    public float sSize;
    public float scale = 0.5f;

    protected override void Init()
    {
        base.Init();

        // spineTextureList 초기화
        if (spineTextureDic == null)
            spineTextureDic = new Dictionary<string, Texture2D>();

        if (spineAssetDic == null)
            spineAssetDic = new Dictionary<string, SkeletonDataAsset>();
    }

    protected override void Release()
    {
        base.Release();

        if (spineTextureDic != null)
        {
            spineTextureDic.Clear();
            spineTextureDic = null;
        }

        if (spineAssetDic != null)
        {
            spineAssetDic.Clear();
            spineAssetDic = null;
        }
    }

    public IEnumerator coLoad(List<UnitModel.Unit> unitTable)
    {
        // scale Size
        sSize = (float)System.Math.Round(Screen.height / 2160f * scale, 2);

        for (int i = 0; i < unitTable.Count; i++)
        {
            yield return StartCoroutine(coLoadSkeletonData(unitTable[i].unitName));

            if (unitTable[i].unitName != "j")
            {
                yield return StartCoroutine(coResizeTexture(unitTable[i].unitName));
            }

            Message.Send<Global.LoadingCountAddMsg>(new Global.LoadingCountAddMsg("Loading_Spine Texture"));

            if (i > 0 && i % 5 == 0)
            {
                yield return Resources.UnloadUnusedAssets();
                System.GC.Collect();
            }
        }

        yield return null;
    }

    private IEnumerator coLoadSkeletonData(string unitName)
    {
        // 임시변수 최상댄에 선언
        string fileName = unitName;
        ResourceRequest res = null;
        SkeletonDataAsset asset = null;

        if (unitName.Contains("_") == true && unitName.Contains("jinshi") == false)
        {
            fileName = unitName.Split('_')[0];
        }
        res = Resources.LoadAsync<SkeletonDataAsset>(string.Format("Character/SpineData/{0}/{0}_SkeletonData", fileName));

        yield return new WaitUntil(() => res.isDone);

        asset = res.asset as SkeletonDataAsset;

        if (asset != null)
        {
            spineAssetDic.Add(unitName, asset);
        }

        // 임시 변수들 전부 null처리 후 메모리 비우기       
        fileName = string.Empty;
        res = null;
        asset.Clear();

        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator coResizeTexture(string unitName)
    {
        // 임시변수 최상댄에 선언
        string fileName = unitName;
        ResourceRequest res = null;
        Texture2D tex = null;

        if (unitName.Contains("_") == true && unitName.Contains("jinshi") == false)
        {
            fileName = unitName.Split('_')[0];
        }
        res = Resources.LoadAsync<Texture2D>(string.Format("Character/SpineData/{0}/{0}", fileName));

        yield return new WaitUntil(() => res.isDone);

        tex = res.asset as Texture2D;

        // Texture2D tex = Resources.Load<Texture2D>(string.Format("Character/SpineData/{0}/{0}", unitName));
        if (tex != null)
        {
            spineTextureDic.Add(unitName, tex.ResizeTexture(Constant.ImageFilterMode.Average, sSize));
        }

        // 임시 변수들 전부 null처리 후 메모리 비우기       
        fileName = string.Empty;
        res = null;
        tex = null;

        yield return new WaitForSeconds(0.1f);
    }

    public SkeletonDataAsset GetSkeletonDataAsset(string key)
    {
        SkeletonDataAsset result = null;

        if (spineAssetDic.TryGetValue(key, out result) == true)
        {
            return result;
        }

        // 임시변수 최상댄에 선언
        string fileName = key;

        if (key.Contains("_") == true && key.Contains("jinshi") == false)
        {
            fileName = key.Split('_')[0];
        }

        result = Resources.Load<SkeletonDataAsset>(string.Format("Character/SpineData/{0}/{0}_SkeletonData", fileName));
        return result;
    }
}
