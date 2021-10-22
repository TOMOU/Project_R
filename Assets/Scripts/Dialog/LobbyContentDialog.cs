// ==================================================
// LobbyContentDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Dialog
{
    public class LobbyContentDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _topRect;

        [Header("- Content Button")]
        [SerializeField] private RectTransform _contentMap;
        [SerializeField] private Button _mainButton;
        [SerializeField] private Button _dimentionButton;
        [SerializeField] private Button _colosseumButton;
        [SerializeField] private Button _storyButton;
        [SerializeField] private Button _babelButton;

        [SerializeField] private RectTransform _bossEncounterRect;
        [SerializeField] private Image _bossEncounterValue;
        [SerializeField] private Button _bossEncounterButton;

        [SerializeField] private RectTransform _bossJackpotRect;
        [SerializeField] private Image _bossJackpotValue;
        [SerializeField] private Text _bossJackpotText;

        [SerializeField] private Animator _animator;
        private Coroutine _coroutine = null;

        protected override void OnLoad()
        {
            base.OnLoad();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _mainButton.onClick.AddListener(OnClickMainStory);
            _dimentionButton.onClick.AddListener(OnClickDimensionDungeon);
            _babelButton.onClick.AddListener(OnClickBabel);
            _storyButton.onClick.AddListener(OnClickStory);
            _colosseumButton.onClick.AddListener(OnClickArena);

            _bossEncounterButton.onClick.AddListener(() => OnInstantBossAppear(new Lobby.InstantBossAppearMsg()));

            Message.AddListener<Lobby.InstantBossAppearMsg>(OnInstantBossAppear);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _backButton.onClick.RemoveAllListeners();
            _backButton = null;
            _homeButton.onClick.RemoveAllListeners();
            _homeButton = null;
            _closeButton.onClick.RemoveAllListeners();
            _closeButton = null;

            _mainButton.onClick.RemoveAllListeners();
            _mainButton = null;
            _dimentionButton.onClick.RemoveAllListeners();
            _dimentionButton = null;
            _babelButton.onClick.RemoveAllListeners();
            _babelButton = null;
            _storyButton.onClick.RemoveAllListeners();
            _storyButton = null;
            _colosseumButton.onClick.RemoveAllListeners();
            _colosseumButton = null;

            _bossEncounterButton.onClick.RemoveAllListeners();

            Message.RemoveListener<Lobby.InstantBossAppearMsg>(OnInstantBossAppear);
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyContentDialog>();
                RequestDialogEnter<LobbyMainDialog>();
            }));

            _contentMap.anchoredPosition = Vector2.zero;
            _contentMap.localScale = Vector3.one * 0.643f;
            _animator.enabled = true;
            _animator.SetTrigger("Default");

            _bossEncounterValue.fillAmount = 0f;

            _bossJackpotValue.fillAmount = 0.7f;
            _bossJackpotText.text = string.Format("{0}%", 70);

            OpenFirst();
        }

        protected override void OnExit()
        {
            base.OnExit();

            _contentMap.anchoredPosition = Vector2.zero;
            _contentMap.localScale = Vector3.one;
        }

        private void OpenFirst()
        {
            switch (TutorialManager.Singleton.openFirst)
            {
                case Constant.TutorialCallbackType.None:
                case Constant.TutorialCallbackType.ContentMap:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    CheckScenario();
                    break;

                case Constant.TutorialCallbackType.MainChapter:
                    OpenFirst_ContentMap(-2313f, 152f, 1.6075f);
                    // RequestDialogExit<LobbyContentDialog>();
                    RequestDialogEnter<LobbyMainChapDialog>();
                    break;

                case Constant.TutorialCallbackType.Dimension:
                    OpenFirst_ContentMap(-2063f, -1013f, 1.2f);
                    // RequestDialogExit<LobbyContentDialog>();
                    RequestDialogEnter<LobbyDimensionDialog>();
                    break;

                case Constant.TutorialCallbackType.Babel:
                    OpenFirst_ContentMap(-573f, -428f, 1f);
                    // RequestDialogExit<LobbyContentDialog>();
                    RequestDialogEnter<LobbyBabelDialog>();
                    break;

                case Constant.TutorialCallbackType.Story:
                    OpenFirst_ContentMap(548f, 218f, 1.286f);
                    // RequestDialogExit<LobbyContentDialog>();
                    RequestDialogEnter<LobbyStoryDialog>();
                    break;

                case Constant.TutorialCallbackType.Arena:
                    OpenFirst_ContentMap(0f, 1296f, 1.6075f);
                    // RequestDialogExit<LobbyContentDialog>();
                    RequestDialogEnter<LobbyArenaDialog>();
                    break;
            }
        }

        private void OpenFirst_ContentMap(float x, float y, float s)
        {
            _animator.enabled = false;
            _contentMap.anchoredPosition = new Vector2(x, y);
            _contentMap.localScale = Vector3.one * s;

            _topRect.anchoredPosition = new Vector2(_topRect.anchoredPosition.x, 500f);
            _bossEncounterRect.anchoredPosition = new Vector2(_bossEncounterRect.anchoredPosition.x, -500f);
            _bossJackpotRect.anchoredPosition = new Vector2(_bossJackpotRect.anchoredPosition.x, -500f);
        }

        private void OnClickBack()
        {
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClickHome()
        {
            Message.Send<Global.ReturnToHomeMsg>(new Global.ReturnToHomeMsg());
        }

        private void OnClickClose()
        {
            //? 임시로 back키 할당.
            //? 나중에 기능이 확정되면 커스텀.
            Message.Send<Global.PopEscapeActionMsg>(new Global.PopEscapeActionMsg());
        }

        private void OnClickMainStory()
        {
            _animator.SetTrigger("LobbyMainChapDialog");

            EnterContent(() =>
            {
                // RequestDialogExit<LobbyContentDialog>();
                RequestDialogEnter<LobbyMainChapDialog>();
            });
        }

        private void OnClickDimensionDungeon()
        {
            _animator.SetTrigger("LobbyDimensionDialog");

            EnterContent(() =>
            {
                // RequestDialogExit<LobbyContentDialog>();
                RequestDialogEnter<LobbyDimensionDialog>();
            });
        }

        private void OnClickBabel()
        {
            _animator.SetTrigger("LobbyBabelDialog");

            EnterContent(() =>
            {
                // RequestDialogExit<LobbyContentDialog>();
                RequestDialogEnter<LobbyBabelDialog>();
            });
        }

        private void OnClickStory()
        {
            _animator.SetTrigger("LobbyStoryDialog");
            EnterContent(() =>
            {
                // RequestDialogExit<LobbyContentDialog>();
                RequestDialogEnter<LobbyStoryDialog>();
            });
        }

        private void OnClickArena()
        {
            _animator.SetTrigger("LobbyArenaDialog");
            EnterContent(() =>
            {
                RequestDialogEnter<LobbyArenaDialog>();
            });
        }

        private void EnterContent(System.Action callback)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coEnterContent(callback));
        }

        private IEnumerator coEnterContent(System.Action callback)
        {
            Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());

            yield return new WaitForSeconds(0.5f);

            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;

            if (callback != null)
            {
                callback();
            }

            Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());
        }

        private void OnInstantBossAppear(Lobby.InstantBossAppearMsg msg)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coAppearBoss());
        }

        private IEnumerator coAppearBoss()
        {
            Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime;
                _bossEncounterValue.fillAmount = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            int cnt = 0;
            t = 0f;
            Color s = Color.white;
            Color e = Color.red;

            while (cnt < 6)
            {
                t += Time.deltaTime * 3;

                if (t >= 1f)
                {
                    if (cnt % 2 == 0)
                    {
                        s = Color.red;
                        e = Color.white;
                    }
                    else
                    {
                        s = Color.white;
                        e = Color.red;
                    }

                    t = 0f;
                    cnt++;
                }

                _bossEncounterValue.SetColorWithChild(s, e, t);

                yield return null;
            }

            // BossDialog를 열어준다
            Dialog.IDialog.RequestDialogEnter<LobbyBossDialog>();

            yield return null;
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 9)
                PlayScenario_09();
            // else if (sidx == 22)
            //     PlayScenario_22();
            else if (sidx == 31)
                PlayScenario_31();
            // else if (sidx == 34)
            //     PlayScenario_34();
            else if (sidx == 38)
                PlayScenario_38();
            else if (sidx == 40)
                PlayScenario_40();
            else if (sidx == 45)
                PlayScenario_45();
            else if (sidx == 50)
                PlayScenario_50();
            // else if (sidx == 53)
            // {
            //     Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
            // }
            else if (sidx == 55)
            {
                if (Info.My.Singleton.User.maxClearedStory < 4)
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_storyButton.transform, () =>
                    {
                        OnClickStory();
                    }));
                }
                else
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(55, () =>
                        {
                            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(56, () =>
                            {
                                Message.Send<Global.ShowScenarioFadeMsg>(new Global.ShowScenarioFadeMsg(true, () =>
                                {
                                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(57, () =>
                                    {
                                        Message.Send<Global.ShowScenarioFadeMsg>(new Global.ShowScenarioFadeMsg(false, () =>
                                        {
                                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
                                        }));
                                    }));
                                }));
                            }));
                        }));
                }
            }
            else if (sidx == 65)         // 잭팟 보스전UI 출력
            {
                Message.Send<Lobby.InstantBossAppearMsg>(new Lobby.InstantBossAppearMsg());
            }
            else if (sidx == 68)
            {
                Message.Send<Global.ForceFocusGuideMsg>(new Global.ForceFocusGuideMsg(_babelButton.transform, () =>
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(68, () =>
                    {
                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_babelButton.transform, OnClickBabel));
                    }));
                }, false));
            }
            else if (sidx == 72)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
            }
            else if (sidx == 78)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_babelButton.transform, OnClickBabel));
            }
            else if (sidx == 82)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
            }
            else if (sidx == 88)
            {
                if (Info.My.Singleton.User.isCompleteArena == false)
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_colosseumButton.transform, OnClickArena));
                else
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
            }
        }

        private void PlayScenario_09()
        {
            // scene 9. scenario -> touching worldmap button, open mainstory
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(9, () =>
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, () =>
                {
                    OnClickMainStory();
                }));
            }));
        }

        private void PlayScenario_22()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
        }

        private void PlayScenario_31()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
        }

        private void PlayScenario_34()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
        }

        private void PlayScenario_38()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
        }

        private void PlayScenario_40()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
        }

        private void PlayScenario_45()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_mainButton.transform, OnClickMainStory));
        }

        private void PlayScenario_50()
        {
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(50, () =>
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_dimentionButton.transform, OnClickDimensionDungeon));
            }));
        }
    }
}