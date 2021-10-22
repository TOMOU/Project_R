// ==================================================
// GlobalGuideDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

namespace Dialog
{
    public class GlobalGuideDialog : IDialog
    {
        [Header("Root")]
        [SerializeField] private Transform _focusRoot;
        [SerializeField] private Transform _buttonGuideRoot;
        [SerializeField] private Transform _imageGuideRoot;

        [Header("Focus Guide")]
        [SerializeField] private Button _focusTouchButton;

        [Header("Button Guide")]
        [SerializeField] private Transform _guidePointer;

        [Header("Image Guide")]
        [SerializeField] private Image _imageGuideSprite;
        [SerializeField] private Button _imageGuidePrevButton;
        [SerializeField] private Button _imageGuideNextButton;

        private GameObject _originButtonObj;
        private GameObject _copyButtonObj;
        private Coroutine _coroutine;

        protected override void OnLoad()
        {
            // 튜토리얼 매니저에 이 Dialog를 등록
            TutorialManager.Singleton.AddGuideDialog(_dialogView);

            Message.AddListener<Global.ForceButtonGuideMsg>(OnForceButtonGuide);
            Message.AddListener<Global.ForceFocusGuideMsg>(OnForceFocusGuide);
        }

        protected override void OnUnload()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            Message.RemoveListener<Global.ForceButtonGuideMsg>(OnForceButtonGuide);
            Message.RemoveListener<Global.ForceFocusGuideMsg>(OnForceFocusGuide);
        }

        protected override void OnEnter()
        {
            // 시나리오 BGM 재생
            SoundManager.Singleton.PlayScenarioBGM(true);
        }

        protected override void OnExit()
        {
            // 시나리오 BGM 재생
            SoundManager.Singleton.PlayScenarioBGM(false);
        }

        private void OnForceButtonGuide(Global.ForceButtonGuideMsg msg)
        {
            if (_dialogView != null)
                _dialogView.gameObject.SetActive(true);

            OnEnter();

            _focusRoot.gameObject.SetActive(false);
            _buttonGuideRoot.gameObject.SetActive(false);
            _imageGuideRoot.gameObject.SetActive(false);

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coForceButtonGuide(() =>
            {
                if (msg.GridLayout != null)
                {
                    msg.GridLayout.enabled = false;
                }

                _focusRoot.gameObject.SetActive(false);
                _buttonGuideRoot.gameObject.SetActive(true);
                _imageGuideRoot.gameObject.SetActive(false);

                // 오브젝트를 복사하여 Transform을 동일하게 설정한다.
                _originButtonObj = msg.Target.gameObject;
                if (_copyButtonObj != null)
                    Destroy(_copyButtonObj);

                _copyButtonObj = GameObject.Instantiate(_originButtonObj);
                _copyButtonObj.SetActive(false);
                _copyButtonObj.transform.SetParent(_originButtonObj.transform.parent);
                _copyButtonObj.transform.localPosition = _originButtonObj.transform.localPosition;
                _copyButtonObj.transform.localRotation = _originButtonObj.transform.localRotation;
                _copyButtonObj.transform.localScale = _originButtonObj.transform.localScale;

                if (msg.GridLayout != null)
                {
                    Image img = _copyButtonObj.GetComponent<Image>();
                    if (img != null && img.sprite != null)
                        _copyButtonObj.SetAlphaWithChild(255f);
                }

                // 버튼을 올릴 부모를 찾는다.
                Transform parent = _buttonGuideRoot;

                // 복제된 오브젝트를 이 Dialog에 올린다.
                _originButtonObj.SetActive(false);
                _copyButtonObj.SetActive(false);
                _copyButtonObj.transform.SetParent(parent);
                _copyButtonObj.transform.SetAsFirstSibling();
                _copyButtonObj.SetLayerRecursively("Global");

                // 사이즈가 fitter일수도 있으니 다시 사이즈 지정
                RectTransform rt = _originButtonObj.GetComponent<RectTransform>();
                RectTransform rt2 = _copyButtonObj.GetComponent<RectTransform>();
                if (rt != null && rt2 != null)
                {
                    rt2.sizeDelta = rt.sizeDelta;
                }

                // sortingGroup 추가
                UnityEngine.Rendering.SortingGroup sg = _copyButtonObj.AddComponent<UnityEngine.Rendering.SortingGroup>();
                sg.sortingOrder = 1;

                _copyButtonObj.SetActive(true);

                // Vector3 p1 = CameraManager.Singleton.UICamera.WorldToScreenPoint(_copyButtonObj.transform.position);
                // p1.z = 0;
                // Vector3 p2 = CameraManager.Singleton.GlobalCamera.ScreenToWorldPoint(p1);
                // _guidePointer.position = p2;

                // z값이 동일하면 평면상이므로 RectTransform의 동일화
                if (_copyButtonObj.transform.localPosition.z == 0)
                {
                    RectTransform gpRect = _guidePointer.GetComponent<RectTransform>();
                    gpRect.anchorMin = rt2.anchorMin;
                    gpRect.anchorMax = rt2.anchorMax;
                    gpRect.pivot = rt2.pivot;
                    gpRect.sizeDelta = rt2.sizeDelta;
                    gpRect.anchoredPosition = rt2.anchoredPosition;
                }
                else
                {
                    // 가이드 손가락 애니를 위치에 맞춘다.

                    Vector3 pos = CameraManager.Singleton.UICamera.WorldToScreenPoint(_copyButtonObj.transform.position);
                    pos.z = 100f;
                    Vector3 pos2 = CameraManager.Singleton.GlobalCamera.ScreenToWorldPoint(pos);


                    RectTransform gpRect = _guidePointer.GetComponent<RectTransform>();
                    gpRect.anchorMin = Vector2.one * 0.5f;
                    gpRect.anchorMax = Vector2.one * 0.5f;
                    gpRect.pivot = Vector2.one * 0.5f;
                    // if (cam.orthographic == false)
                    // {
                    //     pos2.x *= 320f; //  1280    640     320
                    //     pos2.y *= 180f;
                    // }

                    _guidePointer.position = pos2;
                    // _guidePointer.localPosition = new Vector3(_guidePointer.localPosition.x, _guidePointer.localPosition.y, 0f);
                }


                Button newButton = _copyButtonObj.GetComponent<Button>();
                if (newButton != null)
                {
                    newButton.onClick.AddListener(() =>
                    {
                        OnClick_Button();

                        if (msg.GridLayout != null)
                        {
                            msg.GridLayout.enabled = true;
                        }

                        if (msg.Callback != null)
                            msg.Callback();
                    });

                    if (_originButtonObj.name == "world")
                    {
                        GameObject obj = new GameObject("buttonlayer");
                        obj.transform.SetParent(transform);
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localRotation = Quaternion.identity;
                        obj.transform.localScale = Vector3.one;

                        RectTransform rect = obj.AddComponent<RectTransform>();
                        Image img = obj.AddComponent<Image>();
                        Button btn = obj.AddComponent<Button>();

                        rect.anchoredPosition = new Vector2(864f, -218f);
                        rect.sizeDelta = new Vector2(250f, 250f);
                        Color c = Color.white;
                        c.a = 0f;
                        img.color = c;

                        btn.onClick.AddListener(() =>
                        {
                            OnClick_Button();

                            if (msg.GridLayout != null)
                            {
                                msg.GridLayout.enabled = true;
                            }

                            if (msg.Callback != null)
                                msg.Callback();
                        });

                        obj.transform.SetParent(newButton.transform);
                    }
                }
                else
                {
                    Toggle newToggle = _copyButtonObj.GetComponent<Toggle>();
                    if (newToggle != null)
                    {
                        newToggle.onValueChanged.AddListener((isOn) =>
                        {
                            OnClick_Button();

                            if (msg.GridLayout != null)
                            {
                                msg.GridLayout.enabled = true;
                            }

                            if (msg.Callback != null)
                                msg.Callback();
                        });
                    }
                }
            }));
        }

        private void OnForceFocusGuide(Global.ForceFocusGuideMsg msg)
        {
            if (_dialogView != null)
                _dialogView.gameObject.SetActive(true);

            OnEnter();

            _focusRoot.gameObject.SetActive(true);
            _buttonGuideRoot.gameObject.SetActive(false);
            _imageGuideRoot.gameObject.SetActive(false);

            // 오브젝트를 복사하여 Transform을 동일하게 설정한다.
            _originButtonObj = msg.Target.gameObject;
            if (_copyButtonObj != null)
                Destroy(_copyButtonObj);

            _copyButtonObj = GameObject.Instantiate(_originButtonObj);
            _copyButtonObj.SetActive(false);
            _copyButtonObj.transform.SetParent(_originButtonObj.transform.parent);
            _copyButtonObj.transform.localPosition = _originButtonObj.transform.localPosition;
            _copyButtonObj.transform.localRotation = _originButtonObj.transform.localRotation;
            _copyButtonObj.transform.localScale = _originButtonObj.transform.localScale;

            // 버튼을 올릴 부모를 찾는다.
            Transform parent = _focusRoot;

            // 복제된 오브젝트를 이 Dialog에 올린다.
            _originButtonObj.SetActive(false);
            _copyButtonObj.SetActive(false);
            _copyButtonObj.transform.SetParent(parent);
            _copyButtonObj.transform.SetAsFirstSibling();
            _copyButtonObj.SetLayerRecursively("Global");

            // 사이즈가 fitter일수도 있으니 다시 사이즈 지정
            RectTransform rt = _originButtonObj.GetComponent<RectTransform>();
            RectTransform rt2 = _copyButtonObj.GetComponent<RectTransform>();
            if (rt != null && rt2 != null)
            {
                rt2.sizeDelta = rt.sizeDelta;
            }

            // sortingGroup 추가
            UnityEngine.Rendering.SortingGroup sg = _copyButtonObj.AddComponent<UnityEngine.Rendering.SortingGroup>();
            sg.sortingOrder = 1;

            SetFocusedObjInteractable(_copyButtonObj);

            _copyButtonObj.SetActive(true);

            if (msg.ShowAnimation == true)
            {
                FocusSequence(_copyButtonObj.transform, msg.Callback);
            }
            else
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                _coroutine = StartCoroutine(coForceFocus(msg.Callback));
            }
        }

        #region 가이드 인덱스에 따른 Button Action
        /// <summary>
        /// 버튼 가이드의 해당 버튼을 눌렀을 때 동작하는 Action
        /// </summary>
        private void OnClick_Button()
        {
            _originButtonObj.gameObject.SetActive(true);

            BlendModes.BlendModeEffect[] eff = _originButtonObj.GetComponentsInChildren<BlendModes.BlendModeEffect>();
            if (eff != null)
            {
                for (int i = 0; i < eff.Length; i++)
                {
                    eff[i].SetMaterialDirty();
                }
            }

            _copyButtonObj.gameObject.SetActive(false);
            Destroy(_copyButtonObj);

            if (_dialogView != null)
                _dialogView.gameObject.SetActive(false);

            OnExit();
        }
        #endregion

        private void SetFocusedObjInteractable(GameObject obj)
        {
            // Image RaycastTarget false
            Image img = obj.GetComponent<Image>();
            if (img != null)
            {
                img.raycastTarget = false;
            }

            // Text RaycastTarget false
            Text txt = obj.GetComponent<Text>();
            if (txt != null)
            {
                txt.raycastTarget = false;
            }

            // Recursive
            foreach (Transform child in obj.transform)
            {
                SetFocusedObjInteractable(child.gameObject);
            }
        }

        private IEnumerator coForceFocus(System.Action callback)
        {
            yield return new WaitForSecondsRealtime(3f);

            if (callback != null)
                callback();

            OnClick_Button();
        }

        private IEnumerator coForceButtonGuide(System.Action callback)
        {
            yield return new WaitForEndOfFrame();
            // yield return new WaitForSecondsRealtime(0.1f);

            if (callback != null)
                callback();
        }

        private void FocusSequence(Transform obj, System.Action callback)
        {
            // 기존 상태 임시저장
            RectTransform rt = obj.GetComponent<RectTransform>();
            Vector3 position = rt.position;

            // // anchor, pivot 초기화
            // rt.anchorMin = Vector2.one * 0.5f;
            // rt.anchorMax = Vector2.one * 0.5f;
            // rt.pivot = Vector2.one * 0.5f;
            // rt.position = position;

            // Vector2 anchoredPosition = rt.anchoredPosition;

            // Tweening Sequence 시작
            Sequence seq = DOTween.Sequence();
            seq.SetUpdate(true);
            seq.AppendInterval(0.3f).Append(obj.DOScale(obj.localScale * 1.2f, 0.5f).SetLoops(6, LoopType.Yoyo)).AppendCallback(() =>
            {
                OnClick_Button();

                if (callback != null)
                    callback();
            });
        }
    }
}