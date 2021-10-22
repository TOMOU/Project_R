using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class GlobalLoadingDialog : IDialog
    {
        /// <summary>
        /// Title 로딩 때만 사용하는 게임오브젝트
        /// </summary>
        [SerializeField] private GameObject _loadingTitleObj;
        /// <summary>
        /// Title 로딩 이외의 경우에 사용되는 게임오브젝트
        /// </summary>
        [SerializeField] private GameObject _loadingNormalObj;
        /// <summary>
        /// _loadingNormalObj의 이미지 (로딩이미지 랜덤 변경)
        /// </summary>
        [SerializeField] private Image _loadingImage;
        [SerializeField] private Image _loadingProgressValue;
        [SerializeField] private RectTransform _characterIcon;
        [SerializeField] private List<GameObject> _objCharacterList;

        [SerializeField] private Text _loadingMessageText;

        private int _curLoadingCount;
        private int _maxLoadingCount;
        private bool _isEnterFirst = false;

        private Coroutine _coroutine;
        private int _loadingIdx = 2001;

        protected override void OnLoad()
        {
            _curLoadingCount = 0;
            _loadingProgressValue.fillAmount = 0f;
            _characterIcon.anchoredPosition = new Vector2(0, 80f);
            _objCharacterList[0].SetActive(true);
            _objCharacterList[1].SetActive(false);
            _objCharacterList[2].SetActive(false);
            // _objCharacterList[3].SetActive(false);
            // _objCharacterList[4].SetActive(false);
            Message.AddListener<Global.LoadingCountAddMsg>(OnLoadingCountAdd);
            Message.AddListener<Global.MaxLoadingCountMsg>(OnMaxLoadingCount);
        }

        protected override void OnUnload()
        {
            _loadingTitleObj = null;
            _loadingNormalObj = null;
            _loadingImage = null;

            Message.RemoveListener<Global.LoadingCountAddMsg>(OnLoadingCountAdd);
            Message.RemoveListener<Global.MaxLoadingCountMsg>(OnMaxLoadingCount);
        }

        protected override void OnEnter()
        {
            _curLoadingCount = 0;
            _loadingProgressValue.fillAmount = 0f;
            _characterIcon.anchoredPosition = new Vector2(0, 80f);
            _objCharacterList[0].SetActive(true);
            _objCharacterList[1].SetActive(false);
            _objCharacterList[2].SetActive(false);

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coChangeLoadingMessage());

            if (CheckTutorial() == true)
                return;

            _characterIcon.anchoredPosition = new Vector2(0f, 80f);
            _objCharacterList[1].SetActive(false);
            _objCharacterList[2].SetActive(false);
            // _objCharacterList[3].SetActive(false);
            // _objCharacterList[4].SetActive(false);

            _loadingTitleObj.SetActive(_isEnterFirst == false);
            _loadingNormalObj.SetActive(_isEnterFirst == true);
            if (_isEnterFirst == false)
            {
                _isEnterFirst = true;
            }
            else
            {
                ChangeLoadingImage();
            }
        }

        protected override void OnExit()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _curLoadingCount = 0;
            _loadingProgressValue.fillAmount = 0f;
            _characterIcon.anchoredPosition = new Vector2(0, 80f);
            _objCharacterList[0].SetActive(true);
            _objCharacterList[1].SetActive(false);
            _objCharacterList[2].SetActive(false);
        }

        private void OnLoadingCountAdd(Global.LoadingCountAddMsg msg)
        {
            _curLoadingCount++;

            float progress = _curLoadingCount / (float)_maxLoadingCount;
            _loadingProgressValue.fillAmount = progress;

            _characterIcon.anchoredPosition = new Vector2(2300f * progress, 80f);

            if (_objCharacterList[1].activeSelf == false && progress > 0.3f)
                _objCharacterList[1].SetActive(true);
            if (_objCharacterList[2].activeSelf == false && progress > 0.6f)
                _objCharacterList[2].SetActive(true);
            // if (_objCharacterList[3].activeSelf == false && progress > 0.6f)
            //     _objCharacterList[3].SetActive(true);
            // if (_objCharacterList[4].activeSelf == false && progress > 0.8f)
            //     _objCharacterList[4].SetActive(true);


            Logger.LogFormat("[{0}] {1} / {2}", msg.sender, _curLoadingCount, _maxLoadingCount);
        }

        private void OnMaxLoadingCount(Global.MaxLoadingCountMsg msg)
        {
            _curLoadingCount = 0;
            _objCharacterList[0].SetActive(true);
            _objCharacterList[1].SetActive(false);
            _objCharacterList[2].SetActive(false);
            _maxLoadingCount = msg.Max;

            float progress = _curLoadingCount / (float)_maxLoadingCount;
            _loadingProgressValue.fillAmount = progress;
            _characterIcon.anchoredPosition = new Vector2(2300f * progress, 80f);

            Logger.LogFormat("최대 로딩 카운트 = {0}", _maxLoadingCount);
        }

        /// <summary>
        /// 튜토리얼때는 일반 로딩이미지를 랜덤 출력한다
        /// </summary>
        /// <returns></returns>
        private bool CheckTutorial()
        {
            if (TutorialManager.Singleton.curTutorialIndex < 5)
            {
                _loadingTitleObj.SetActive(false);
                _loadingNormalObj.SetActive(true);
                ChangeLoadingImage();
                return true;
            }

            return false;
        }

        private void ChangeLoadingImage()
        {
            int rand = Random.Range(1, 6);
            _loadingImage.sprite = Resources.Load<Sprite>(string.Format("Texture/Loading/Loading_{0:D2}", rand));
        }

        private IEnumerator coChangeLoadingMessage()
        {
            _loadingIdx++;
            if (_loadingIdx > 2005)
                _loadingIdx = 2001;

            while (true)
            {
                _loadingMessageText.text = LocalizeManager.Singleton.GetString(_loadingIdx);

                yield return new WaitForSeconds(3f);

                _loadingIdx++;
                if (_loadingIdx > 2005)
                    _loadingIdx = 2001;

                yield return null;
            }
        }
    }
}