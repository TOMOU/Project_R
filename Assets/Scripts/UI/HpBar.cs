// ==================================================
// HpBar.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HpBar : MonoBehaviour
{
    /// <summary>
    /// 현재 활성화가 되어있는지
    /// </summary>
    private bool _isEnabled = false;
    /// <summary>
    /// 현재 활성화가 되어있는지
    /// </summary>
    /// <value></value>
    public bool Enabled { get { return _isEnabled; } }
    /// <summary>
    /// 체력바 이미지
    /// </summary>
    [SerializeField] private Image _value = null;
    /// <summary>
    /// 체력바의 대상이 되는 캐릭터
    /// </summary>
    private UnitInfo_Defence _targetA = null;
    private Coroutine _coroutine = null;
    private Camera _worldCam;
    private Camera _uiCam;

    /// <summary>
    /// 유닛의 현재, 최대 체력에 비례하여 체력바를 출력해준다
    /// </summary>
    /// <param name="cur">현재 체력 (깎이고 난 후 수치)</param>
    /// <param name="max">최대 체력</param>
    /// <param name="dam">입은 데미지</param>
    public void ShowImage(UnitInfo_Defence info, int dam)
    {
        if (_worldCam == null || _uiCam == null)
        {
            if (_worldCam == null)
            {
                _worldCam = CameraManager.Singleton.MainCamera;
            }
            if (_uiCam == null)
            {
                _uiCam = CameraManager.Singleton.UICamera;
            }

            if (_worldCam == null || _uiCam == null)
            {
                Logger.LogErrorFormat("Camera가 null입니다.\nMain Camera is null? = {0}\nUI Camera is null? = {1}", _worldCam == null, _uiCam == null);
                return;
            }
        }

        _isEnabled = true;
        gameObject.SetActive(true);
        _targetA = info;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _coroutine = StartCoroutine(coShowImage(info, dam));
    }

    private IEnumerator coShowImage(UnitInfo_Defence info, int dam)
    {
        // 데미지 변수 복사
        int cur = info.Status.hp;
        int max = info.Status.hpFull;
        float d = (float)dam;

        // Renderer r = info.GetComponent<Renderer>();
        // MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        // materialPropertyBlock.SetColor("_FillColor", Color.black);
        // materialPropertyBlock.SetFloat("_FillPhase", 1f);
        // r.SetPropertyBlock(materialPropertyBlock);

        // 데미지 UI가 서서히 줄어든다.
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            d = Mathf.Lerp(d, 0, t);
            _value.fillAmount = (cur + d) / (float)max;
            yield return null;
        }

        // materialPropertyBlock.SetColor("_FillColor", Color.black);
        // materialPropertyBlock.SetFloat("_FillPhase", 0f);
        // r.SetPropertyBlock(materialPropertyBlock);

        // 로직 종료. 비활성화해준다.
        _targetA = null;
        _isEnabled = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        SetPosition_Defence();
    }

    [Range(0f, 4f)] public float offset;
    private void SetPosition_Defence()
    {
        if (_targetA == null)
            return;

        Vector3 p1 = _worldCam.WorldToScreenPoint(_targetA.transform.position + Vector3.up * offset);
        p1.z = 100;
        Vector3 p2 = _uiCam.ScreenToWorldPoint(p1);
        transform.position = p2;
    }
}
