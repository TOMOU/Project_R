// ==================================================
// EffectManager.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoSingleton<EffectManager>
{
    [System.Serializable]
    private class EffectContainer
    {
        public string Key;
        public GameObject Object;
        public ParticleSystem Particle;
        public Transform Target;
        public int SortingLayerID;
        public int OrderLayerID;
        public List<Renderer> RendList;
        public EffectContainer(string key, GameObject obj)
        {
            this.Key = key;
            this.Object = obj;
            this.Particle = obj.GetComponent<ParticleSystem>();

            if (RendList == null)
                this.RendList = new List<Renderer>();

            RendList.Clear();
            AddRenderer(Object.transform);

            this.Target = null;
        }

        public void SetSortingLayerID(int sortingLayerID, int orderLayerID)
        {
            if (sortingLayerID != this.SortingLayerID)
            {
                this.SortingLayerID = sortingLayerID;
                this.OrderLayerID = orderLayerID;
                ChangeSortingLayerID();
            }
        }

        public void Refresh()
        {
            if (Target == null)
                return;

            if (Key == "buff")
                Object.transform.localPosition = Target.transform.position + new Vector3(0f, 2f, -0.001f);
            else
                Object.transform.localPosition = Target.transform.position + new Vector3(0f, 0f, -0.001f);
        }

        public void AddRenderer(Transform root)
        {
            Renderer r = root.GetComponent<Renderer>();
            if (r != null)
            {
                RendList.Add(r);
            }

            foreach (Transform child in root)
            {
                AddRenderer(child);
            }
        }

        private void ChangeSortingLayerID()
        {
            for (int i = 0; i < RendList.Count; i++)
            {
                RendList[i].sortingLayerID = SortingLayerID;
                RendList[i].sortingOrder = OrderLayerID;
            }
        }
    }

    [SerializeField] private List<EffectContainer> _effectList;
    public List<GameObject> missileList;

    protected override void Init()
    {
        base.Init();

        if (_effectList == null)
            _effectList = new List<EffectContainer>();

        if (missileList == null)
            missileList = new List<GameObject>();
    }

    protected override void Release()
    {
        base.Release();

        if (_effectList != null)
        {
            Clear();
            _effectList = null;
        }
    }

    public void Clear()
    {
        if (_effectList != null)
        {
            foreach (var p in _effectList)
            {
                GameObject.Destroy(p.Object);
            }

            _effectList.Clear();
        }

        if (missileList != null)
        {
            foreach (var p in missileList)
            {
                GameObject.Destroy(p);
            }

            missileList.Clear();
        }
    }

    public void AllEffectOff()
    {
        for (int i = 0; i < _effectList.Count; i++)
        {
            _effectList[i].Object.SetActive(false);
        }
    }

    public void BuffOff()
    {
        for (int i = 0; i < _effectList.Count; i++)
        {
            if (_effectList[i].Key == "buff")
                _effectList[i].Object.SetActive(false);
        }
    }

    private void Update()
    {
        if (_effectList == null)
            return;

        for (int i = 0; i < _effectList.Count; i++)
        {
            _effectList[i].Refresh();
        }
    }

    public void OnCreateParticle(string key)
    {
        GameObject prefab = Resources.Load<GameObject>(string.Format("Effect/Skill/{0}", key));
        if (prefab == null)
        {
            Logger.LogErrorFormat("Can't load effect name {0}", key);
            return;
        }

        GameObject obj = GameObject.Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);
        obj.SetActive(false);

        _effectList.Add(new EffectContainer(key, obj));
    }

    public void OnPlayParticle(Vector3 pos, string key, float scaleX, int sortingLayerID, int orderLayer)
    {
        EffectContainer eff = _effectList.Find(e => e.Key == key && e.Object.activeSelf == false);
        if (eff == null)
        {
            GameObject prefab = Resources.Load<GameObject>(string.Format("Effect/Skill/{0}", key));
            if (prefab == null)
            {
                Logger.LogErrorFormat("Can't load effect name {0}", key);
                return;
            }

            GameObject obj = GameObject.Instantiate(prefab);
            obj.transform.SetParent(transform);
            eff = new EffectContainer(key, obj);

            _effectList.Add(eff);
        }

        eff.Object.transform.localPosition = pos;
        eff.Object.transform.localRotation = Quaternion.Euler(0f, scaleX > 0f ? 0f : 180f, 0f);//Quaternion.identity;           
        eff.Object.transform.localScale = Vector3.one;

        eff.SetSortingLayerID(sortingLayerID, orderLayer);

        eff.Object.SetActive(true);
    }

    public void OnParticleFollow(string key, Transform target, bool isActive, float scaleX, UnitInfo_Normal info)
    {
        EffectContainer eff = _effectList.Find(e => e.Key == key && e.Target == target);
        if (eff == null)
        {
            GameObject prefab = Resources.Load<GameObject>(string.Format("Effect/Skill/{0}", key));
            if (prefab == null)
            {
                Logger.LogErrorFormat("Can't load effect name {0}", key);
                return;
            }

            GameObject obj = GameObject.Instantiate(prefab);
            obj.transform.SetParent(transform);
            eff = new EffectContainer(key, obj);

            _effectList.Add(eff);
        }
        else
        {
            // 이미 타겟을 따라다니고 있다.
            if (eff.Object.activeSelf == true && isActive == true)
                return;
            // 이미 비활성화되어있다.
            else if (eff.Object.activeSelf == false && isActive == false)
                return;
        }

        eff.Target = target;
        eff.Object.transform.localPosition = target.position + new Vector3(0f, 2f);
        eff.Object.transform.localRotation = Quaternion.Euler(0f, scaleX > 0f ? 0f : 180f, 0f);//Quaternion.identity;        
        eff.Object.transform.localScale = Vector3.one;

        if (info != null)
            eff.SetSortingLayerID(info.Rend.sortingLayerID, info.Rend.sortingOrder);

        eff.Object.SetActive(isActive);
    }

    public void OnParticleAttach(string key, Transform target, bool isActive, float scale, float scaleX, int sortingLayerID, int orderLayer)
    {
        EffectContainer eff = _effectList.Find(e => e.Key == key);
        if (eff == null)
        {
            GameObject prefab = Resources.Load<GameObject>(string.Format("Effect/Skill/{0}", key));
            if (prefab == null)
            {
                Logger.LogErrorFormat("Can't load effect name {0}", key);
                return;
            }

            GameObject obj = GameObject.Instantiate(prefab);
            obj.transform.SetParent(transform);
            eff = new EffectContainer(key, obj);

            _effectList.Add(eff);
        }
        else
        {
            // 이미 타겟을 따라다니고 있다.
            if (eff.Object.activeSelf == true && isActive == true)
                return;
            // 이미 비활성화되어있다.
            else if (eff.Object.activeSelf == false && isActive == false)
                return;
        }

        eff.Object.transform.SetParent(target);
        eff.Object.transform.localPosition = Vector3.zero;
        eff.Object.transform.localRotation = Quaternion.Euler(0f, scaleX > 0f ? 0f : 180f, 0f);//Quaternion.identity;        
        eff.Object.transform.localScale = Vector3.one * scale;

        eff.SetSortingLayerID(sortingLayerID, orderLayer);

        eff.Object.SetActive(isActive);
    }
}
