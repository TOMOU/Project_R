// ==================================================
// LobbyDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Dialog
{
    public class LobbyMainDialog : IDialog
    {
        [Header("로비 애니메이션")]
        [SerializeField] private List<Animator> _animators;

        [Header("유저 재화 정보")]
        [SerializeField] private Button _addDiamondButton;
        [SerializeField] private Text _diamondValueText;
        [SerializeField] private Button _addGoldButton;
        [SerializeField] private Text _goldValueText;
        [SerializeField] private Button _addMoneyButton;
        [SerializeField] private Text _moneyValueText;

        [Header("유저 정보")]
        [SerializeField] private Text _userLevelText;
        [SerializeField] private Text _curStaminaText;
        [SerializeField] private Text _maxStaminaText;
        [SerializeField] private Image _userStaminaSlider;
        [SerializeField] private Text _curExpText;
        [SerializeField] private Text _maxExpText;
        [SerializeField] private Image _userExpSlider;

        [Header("메인 캐릭터 포트레이트")]
        private int _mainCharacterIndex;
        [SerializeField] private Image _mainCharacterImage;

        [Header("메뉴이동 & 팝업 버튼 (중앙)")]
        [SerializeField] private Button _worldMapButton;
        [SerializeField] private Button _worldMapButton_3D;
        [SerializeField] private Button _storyButton;
        [SerializeField] private Button _storyButton_3D;
        [SerializeField] private Button _guildButton;
        [SerializeField] private Button _guildButton_3D;
        [SerializeField] private Button _presentButton;
        [SerializeField] private Button _presentButton_3D;
        [SerializeField] private Button _dictionaryButton;
        [SerializeField] private Button _dictionaryButton_3D;
        [SerializeField] private Button _inventoryButton;
        [SerializeField] private Button _inventoryButton_3D;

        [Header("메뉴이동 & 팝업 버튼 (아래)")]
        [SerializeField] private Button _characterButton;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _stellaButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _summonButton;
        [SerializeField] private Button _missionButton;

        [Header("메뉴이동 & 팝업 버튼 (우상단)")]
        [SerializeField] private Button _noticeButton;
        [SerializeField] private Button _postButton;
        [SerializeField] private Button _optionButton;

        private Info.User _user;
        private Coroutine _coroutine = null;

        protected override void OnLoad()
        {
            _user = Info.My.Singleton.User;

            _mainCharacterIndex = 0;

            // 버튼 터치영역 수정
            SetTouchArea();

            // 개발 완료된 UI나 컨텐츠 진입
            _worldMapButton.AddCallback(OnClickWorldmap);
            _worldMapButton_3D.AddCallback(OnClickWorldmap);
            _characterButton.AddCallback(OnClickOpenCharacterDialog);
            _summonButton.AddCallback(OnClickOpenGachaDialog);
            _missionButton.AddCallback(OnClickOpenMissionDialog);
            _equipButton.AddCallback(OnClickOpenEquipDialog);

            // 미개발된 컨텐츠 (클릭 시 로비의 메인 캐릭터 포트레이트가 변경됨)
            _addDiamondButton.AddCallback(OnClickUndeveloped);
            _storyButton.AddCallback(OnClickUndeveloped);
            _storyButton_3D.AddCallback(OnClickUndeveloped);
            _guildButton.AddCallback(OnClickUndeveloped);
            _guildButton_3D.AddCallback(OnClickUndeveloped);
            _presentButton.AddCallback(OnClickUndeveloped);
            _presentButton_3D.AddCallback(OnClickUndeveloped);
            _dictionaryButton.AddCallback(OnClickUndeveloped);
            _dictionaryButton_3D.AddCallback(OnClickUndeveloped);
            _inventoryButton.AddCallback(OnClickUndeveloped);
            _inventoryButton_3D.AddCallback(OnClickUndeveloped);
            _stellaButton.AddCallback(OnClickUndeveloped);
            _shopButton.AddCallback(OnClickUndeveloped);
            _noticeButton.AddCallback(OnClickUndeveloped);
            _postButton.AddCallback(OnClickUndeveloped);

            // Option버튼 클릭 시 배경 블러 시전.
            _optionButton.AddCallback(OnClickOption);
        }

        protected override void OnUnload()
        {
            _addDiamondButton.onClick.RemoveAllListeners();
            _addDiamondButton = null;
            _diamondValueText = null;

            _addGoldButton.onClick.RemoveAllListeners();
            _addGoldButton = null;
            _goldValueText = null;

            _addMoneyButton.onClick.RemoveAllListeners();
            _addMoneyButton = null;
            _moneyValueText = null;

            _userLevelText = null;
            _userStaminaSlider = null;
            _userExpSlider = null;

            _mainCharacterImage = null;

            _worldMapButton.onClick.RemoveAllListeners();
            _worldMapButton_3D.onClick.RemoveAllListeners();
            _worldMapButton = null;
            _storyButton.onClick.RemoveAllListeners();
            _storyButton_3D.onClick.RemoveAllListeners();
            _storyButton = null;
            _guildButton.onClick.RemoveAllListeners();
            _guildButton_3D.onClick.RemoveAllListeners();
            _guildButton = null;
            _presentButton.onClick.RemoveAllListeners();
            _presentButton_3D.onClick.RemoveAllListeners();
            _presentButton = null;
            _dictionaryButton.onClick.RemoveAllListeners();
            _dictionaryButton_3D.onClick.RemoveAllListeners();
            _dictionaryButton = null;
            _inventoryButton.onClick.RemoveAllListeners();
            _inventoryButton_3D.onClick.RemoveAllListeners();
            _inventoryButton = null;

            _characterButton.onClick.RemoveAllListeners();
            _characterButton = null;
            _equipButton.onClick.RemoveAllListeners();
            _equipButton = null;
            _stellaButton.onClick.RemoveAllListeners();
            _stellaButton = null;
            _shopButton.onClick.RemoveAllListeners();
            _shopButton = null;
            _summonButton.onClick.RemoveAllListeners();
            _summonButton = null;
            _missionButton.onClick.RemoveAllListeners();
            _missionButton = null;

            _noticeButton.onClick.RemoveAllListeners();
            _noticeButton = null;
            _postButton.onClick.RemoveAllListeners();
            _postButton = null;
            _optionButton.onClick.RemoveAllListeners();
            _optionButton = null;
        }

        protected override void OnEnter()
        {
            _mainCharacterIndex = 0;
            _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/100211");

            _diamondValueText.text = "0";
            _moneyValueText.text = "0";
            _goldValueText.text = string.Format("{0:##,##0}", _user.gold);

            OpenFirst();
        }

        protected override void OnExit()
        {

        }

        private void OpenFirst()
        {
            switch (TutorialManager.Singleton.openFirst)
            {
                case Constant.TutorialCallbackType.None:
                    LobbyDialogEnter();
                    break;

                case Constant.TutorialCallbackType.ContentMap:
                case Constant.TutorialCallbackType.MainChapter:
                case Constant.TutorialCallbackType.Dimension:
                case Constant.TutorialCallbackType.Babel:
                case Constant.TutorialCallbackType.Story:
                case Constant.TutorialCallbackType.Arena:
                    RequestDialogExit<LobbyMainDialog>();
                    RequestDialogEnter<LobbyContentDialog>();
                    break;
            }
        }

        private void SetTouchArea()
        {
            _worldMapButton.image.SetTouchArea();//.SetTouchAreaWithChild();
            _storyButton.image.SetTouchArea();//.SetTouchAreaWithChild();
            _guildButton.image.SetTouchArea();//.SetTouchAreaWithChild();
            _presentButton.image.SetTouchArea();//.SetTouchAreaWithChild();
            _dictionaryButton.image.SetTouchArea();//.SetTouchAreaWithChild();
            _inventoryButton.image.SetTouchArea();//.SetTouchAreaWithChild();
            // _characterButton.image.SetTouchAreaWithChild();
            // _equipButton.image.SetTouchAreaWithChild();
            // _stellaButton.image.SetTouchAreaWithChild();
            // _shopButton.image.SetTouchAreaWithChild();
            // _summonButton.image.SetTouchAreaWithChild();
            // _missionButton.image.SetTouchAreaWithChild();
            // _noticeButton.image.SetTouchAreaWithChild();
            // _postButton.image.SetTouchAreaWithChild();
            // _optionButton.image.SetTouchAreaWithChild();
        }

        private void OnClickWorldmap()
        {
            LobbyDialogExit(() =>
            {
                // 로비 메인을 끄고
                RequestDialogExit<LobbyMainDialog>();

                // 컨텐츠맵을 열어준다.
                RequestDialogEnter<LobbyContentDialog>();
            });
        }

        private void OnClickOpenCharacterDialog()
        {
            LobbyDialogExit(() =>
            {
                // 로비 메인을 끄고
                RequestDialogExit<LobbyMainDialog>();

                // 캐릭터관리를 켠다
                RequestDialogEnter<LobbyCharacterDialog>();
            });
        }

        private void OnClickOpenGachaDialog()
        {
            LobbyDialogExit(() =>
            {
                // 로비 메인을 끄고
                RequestDialogExit<LobbyMainDialog>();

                // 캐릭터관리를 켠다
                RequestDialogEnter<LobbyGachaDialog>();
            });
        }

        private void OnClickOpenMissionDialog()
        {
            LobbyDialogExit(() =>
            {
                // 로비 메인을 끄고
                RequestDialogExit<LobbyMainDialog>();

                // 캐릭터관리를 켠다
                RequestDialogEnter<LobbyMissionDialog>();
            });
        }

        private void OnClickOpenEquipDialog()
        {
            LobbyDialogExit(() =>
            {
                // 로비 메인을 끄고
                RequestDialogExit<LobbyMainDialog>();

                // 장비관리를 켠다
                RequestDialogEnter<LobbyEquipDialog>();
            });
        }

        private void OnClickOption()
        {
            Dialog.IDialog.RequestDialogEnter<GlobalLocalizeDialog>();
        }

        private void OnClickUndeveloped()
        {
            _mainCharacterIndex++;
            if (_mainCharacterIndex > 6)
                _mainCharacterIndex = 0;

            switch (_mainCharacterIndex)
            {
                case 0:
                    _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/100211");
                    _mainCharacterImage.rectTransform.anchoredPosition = new Vector2(0f, -330f);
                    break;

                case 1:
                    _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/101031");
                    _mainCharacterImage.rectTransform.anchoredPosition = new Vector2(0f, -330f);
                    break;

                case 2:
                    _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/101131");
                    _mainCharacterImage.rectTransform.anchoredPosition = new Vector2(0f, -330f);
                    break;

                case 3:
                    _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/102831");
                    _mainCharacterImage.rectTransform.anchoredPosition = new Vector2(0f, -330f);
                    break;

                case 4:
                    _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/103031");
                    _mainCharacterImage.rectTransform.anchoredPosition = new Vector2(0f, -330f);
                    break;

                case 5:
                    _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/111001");
                    _mainCharacterImage.rectTransform.anchoredPosition = new Vector2(0f, -230f);
                    break;

                case 6:
                    _mainCharacterImage.sprite = Resources.Load<Sprite>("Texture/Character/1_portrait_full/112001");
                    _mainCharacterImage.rectTransform.anchoredPosition = new Vector2(0f, -330f);
                    break;
            }
        }

        private void LobbyDialogEnter(System.Action callback = null)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coLobbyDialogAnimation("OnEnter", callback));
        }

        private void LobbyDialogExit(System.Action callback = null)
        {
            Message.Send<Lobby.EnterContentDialogMsg>(new Lobby.EnterContentDialogMsg());

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coLobbyDialogAnimation("OnExit", callback));
        }

        private void LobbyDialogEnterImmediately()
        {
            // 즉각적으로 들어오게
            _animators.ForEach(e => e.Play(0, 0, 1f));

            // 필요정보 기입
            _userLevelText.text = _user.level.ToString();
            _curExpText.text = string.Format("{0:##,##0}", _user.curExp);
            _userExpSlider.fillAmount = _user.curExp / (float)_user.maxExp;
            _maxExpText.text = string.Format("{0:##,##0}", _user.maxExp);
            _curStaminaText.text = string.Format("{0:##,##0}", _user.curStamina);
            _userStaminaSlider.fillAmount = _user.curStamina / (float)_user.maxStamina;
            _maxStaminaText.text = string.Format("{0:##,##0}", _user.maxStamina);
        }

        private IEnumerator coLobbyDialogAnimation(string key, System.Action callback = null)
        {
            // 입력 잠금
            Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());

            // OnEnter 시에만 Text 초기값 0
            if (key == "OnEnter")
            {
                _userLevelText.text = _user.level.ToString();
                _curExpText.text = "0";
                _maxExpText.text = string.Format("{0:##,##0}", _user.maxExp);
                _userExpSlider.fillAmount = 0f;
                _curStaminaText.text = "0";
                _maxStaminaText.text = string.Format("{0:##,##0}", _user.maxStamina);
                _userStaminaSlider.fillAmount = 0f;
            }

            float t = 0f;

            // Animators에 트리거 전달
            for (int i = 0; i < _animators.Count; i++)
            {
                _animators[i].SetTrigger(key);
            }

            // 0.5초 대기
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                yield return null;
            }

            // Animators의 재생이 완료될 때까지 대기
            for (int i = 0; i < _animators.Count; i++)
            {
                while (_animators[i].GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    yield return null;
                }
            }

            // Enter 시에만 유저정보 갱신
            if (key == "OnEnter")
            {
                t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime;

                    _curExpText.text = string.Format("{0:##,##0}", (int)Mathf.Lerp(0, _user.curExp, t));
                    _userExpSlider.fillAmount = Mathf.Lerp(0f, _user.curExp / (float)_user.maxExp, t);
                    _curStaminaText.text = string.Format("{0:##,##0}", (int)Mathf.Lerp(0, _user.curStamina, t));
                    _userStaminaSlider.fillAmount = Mathf.Lerp(0f, _user.curStamina / (float)_user.maxStamina, t);

                    yield return null;
                }
            }

            // 입력 잠금 해제
            Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());

            if (callback != null)
            {
                callback();
            }

            if (key == "OnEnter")
            {
                CheckScenario();
            }
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 7)
                PlayScenario_07();
            else if (sidx == 22)
                PlayScenario_22();
            else if (sidx == 26)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_equipButton.transform, OnClickOpenEquipDialog));
            }
            else if (sidx == 29)
                PlayScenario_29();
            else if (sidx == 31)
                PlayScenario_31();
            else if (sidx == 34)
                PlayScenario_34();
            else if (sidx == 36)
                PlayScenario_36();
            else if (sidx == 38)
                PlayScenario_38();
            else if (sidx == 40)
                PlayScenario_40();
            else if (sidx == 45)
                PlayScenario_45();
            else if (sidx == 50)
                PlayScenario_50();
            else if (sidx == 53)    // 장비 관리로 이동
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_equipButton.transform, OnClickOpenEquipDialog));
            }
            else if (sidx == 55)     // 장비강화 후 대신전으로 진입
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
            }
            else if (sidx == 77)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(77, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
                }));
            }
            else if (sidx == 87)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(87, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
                }));
            }
            else if (sidx == 91)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(91, () =>
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(92, () =>
                    {
                        // Application.Quit();
                    }));
                }));
            }
        }

        private void PlayScenario_07()
        {
            // 튜토리얼용으로 배치 초기화
            for (int i = 0; i < Info.My.Singleton.Inventory.storyTeam.Length; i++)
            {
                Info.My.Singleton.Inventory.storyTeam[i / 3, i % 3] = 0;
            }

            // scene 7. scenario -> focusing worldmap button
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(7, () =>
            {
                Message.Send<Global.ForceFocusGuideMsg>(new Global.ForceFocusGuideMsg(_worldMapButton.transform, () =>
                {
                    // scene 8. scenario -> touching worldmap button, open content map
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(8, () =>
                    {
                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, () =>
                        {
                            // 컨텐츠맵 오픈
                            OnClickWorldmap();
                        }));
                    }));
                }));
            }));
        }

        private void PlayScenario_22()
        {
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(22, () =>
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(23, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_characterButton.transform, () =>
                    {
                        OnClickOpenCharacterDialog();
                    }));
                }));
            }));
        }

        private void PlayScenario_29()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_summonButton.transform, () =>
            {
                OnClickOpenGachaDialog();
            }));
        }

        private void PlayScenario_31()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
        }

        private void PlayScenario_34()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_characterButton.transform, OnClickOpenCharacterDialog));
        }

        private void PlayScenario_36()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_missionButton.transform, OnClickOpenMissionDialog));
        }

        private void PlayScenario_38()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
        }

        private void PlayScenario_40()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
        }

        private void PlayScenario_45()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
        }

        private void PlayScenario_50()
        {
            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_worldMapButton.transform, OnClickWorldmap));
        }
    }
}