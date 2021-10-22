using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private RectTransform _rect;
    private Text _damageText = null;
    [SerializeField] private Font _fontNormal;
    [SerializeField] private Font _fontCritical;
    [SerializeField] private Font _fontHeal;
    private int _value = 0;
    private float _delta = 0f;
    private bool _active = false;
    public bool IsActive { get { return _active; } }


    private Camera _worldCam;
    private Camera _uiCam;

    private BossPartInfo _bossPartInfo;

    private void Awake()
    {
        if (_rect == null)
        {
            _rect = GetComponent<RectTransform>();
        }

        if (_damageText == null)
        {
            _damageText = GetComponent<Text>();
            if (_damageText == null)
            {
                Logger.LogError("데미지 오브젝트에 텍스트 컴포넌트가 없습니다.");
                return;
            }
        }

        _damageText.font = _fontNormal;

        _damageText.text = string.Format("{0}", _value);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_active == false)
            return;

        if (_delta > 1f)
        {
            _delta = 0f;
            Inactive();
        }

        _delta += Time.deltaTime;
        _rect.anchoredPosition += new Vector2(0f, 100f * Time.deltaTime);
    }

    public void ActiveDamage(Transform target, int damage, bool isCritical, int slotIndex)
    {
        _active = true;
        gameObject.SetActive(_active);

        if (isCritical)
        {
            _damageText.font = _fontCritical;
            _damageText.fontSize = 50;
        }
        else
        {
            _damageText.font = _fontNormal;
            _damageText.fontSize = 32;
        }

        if (target == null)
            return;

        _worldCam = CameraManager.Singleton.MainCamera;
        _uiCam = CameraManager.Singleton.UICamera;

        Transform trans = target;
        if (slotIndex >= 0)
        {
            if (_bossPartInfo == null)
                _bossPartInfo = target.GetComponent<BossPartInfo>();

            trans = _bossPartInfo.socketList[slotIndex];
            if (trans == null)
            {
                trans = target;
                Logger.LogWarningFormat("{0}의 {1}앵커를 찾지 못함", target.name, slotIndex);
            }
        }

        Vector3 p1 = _worldCam.WorldToScreenPoint(trans.position);
        p1.z = 100;
        Vector3 p2 = _uiCam.ScreenToWorldPoint(p1);
        transform.position = p2;

        // Vector3 pos = _worldCam.WorldToNormalizedViewportPoint(target.transform.position);
        // Vector3 pos2 = _uiCam.NormalizedViewportToWorldPoint(pos);

        // if (_uiCam.orthographic == false)
        // {
        //     pos2.x *= 320f; //  1280    640     320
        //     pos2.y *= 180f;
        // }

        // transform.position = pos2;
        // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, _rect.anchoredPosition.y + 200f);

        _damageText.text = string.Format("{0}", damage);
    }

    public void ActiveHeal(Transform target, int value)
    {
        _active = true;
        gameObject.SetActive(_active);

        _damageText.font = _fontHeal;
        _damageText.fontSize = 38;

        if (target == null)
            return;

        _worldCam = CameraManager.Singleton.MainCamera;
        _uiCam = CameraManager.Singleton.UICamera;

        Vector3 p1 = _worldCam.WorldToScreenPoint(target.transform.position);
        p1.z = 100;
        Vector3 p2 = _uiCam.ScreenToWorldPoint(p1);
        transform.position = p2;

        // Vector3 pos = _worldCam.WorldToNormalizedViewportPoint(target.transform.position);
        // Vector3 pos2 = _uiCam.NormalizedViewportToWorldPoint(pos);

        // if (_uiCam.orthographic == false)
        // {
        //     pos2.x *= 320f; //  1280    640     320
        //     pos2.y *= 180f;
        // }

        // transform.position = pos2;
        // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, _rect.anchoredPosition.y + 200f);

        _damageText.text = string.Format("{0}", value);
    }

    private void Inactive()
    {
        _active = false;
        gameObject.SetActive(false);
    }
}