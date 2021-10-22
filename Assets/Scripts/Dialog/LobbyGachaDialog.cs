// ==================================================
// LobbyGachaDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace Dialog
{
    public class LobbyGachaDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _gachaButton_1;
        [SerializeField] private Button _gachaButton_10;

        [SerializeField] private GameObject _gachaPanelObj;
        [SerializeField] private List<GachaCardSlot> _cardSlotList;
        [SerializeField] private Button _gachaPanelButton;
        [SerializeField] private Text _gachaPanelButtonText;
        private int[] _unitCodeArr = { 100211, 101031, 101131, 102831, 103031, 111001, 112001 };
        private Coroutine _coroutine;

        private int _curFlipCount;
        private int _maxFlipCount;

        protected override void OnLoad()
        {
            base.OnLoad();

            _backButton.onClick.AddListener(OnClickBack);
            _homeButton.onClick.AddListener(OnClickHome);
            _closeButton.onClick.AddListener(OnClickClose);

            _gachaButton_1.onClick.AddListener(OnClickGacha_1);
            _gachaButton_10.onClick.AddListener(OnClickGacha_10);

            _gachaPanelButton.onClick.AddListener(OnClickGachaConfirm);
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

            _gachaButton_1.onClick.RemoveAllListeners();
            _gachaButton_10.onClick.RemoveAllListeners();
            _gachaPanelButton.onClick.RemoveAllListeners();
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Message.Send<Global.AddEscapeActionMsg>(new Global.AddEscapeActionMsg(() =>
            {
                RequestDialogExit<LobbyGachaDialog>();
                RequestDialogEnter<LobbyMainDialog>();
            }));

            _gachaPanelObj.SetActive(false);

            RefreshUI();

            CheckScenario();
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private void OnClick()
        {

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

        private void RefreshUI()
        {
            // bool isCompleteGacha = Info.My.Singleton.User.isCompleteGacha;

            // _gachaButton_1.SetGrayScaleWithChild(isCompleteGacha);
            // _gachaButton_10.SetGrayScaleWithChild(isCompleteGacha);
        }

        private void OnClickGacha_1()
        {
            _curFlipCount = 0;
            _maxFlipCount = 1;

            _cardSlotList[0].gameObject.SetActive(true);
            _cardSlotList[0].Init(_unitCodeArr[Random.Range(0, _unitCodeArr.Length)], OnClickCard);

            for (int i = 1; i < _cardSlotList.Count; i++)
            {
                _cardSlotList[i].gameObject.SetActive(false);
            }

            _gachaPanelObj.SetActive(true);
            _gachaPanelButtonText.text = LocalizeManager.Singleton.GetString(5005);

            Message.Send<Global.EscapeLockMsg>(new Global.EscapeLockMsg());
        }

        private void OnClickGacha_10()
        {
            _curFlipCount = 0;
            _maxFlipCount = 10;

            for (int i = 0; i < _cardSlotList.Count; i++)
            {
                _cardSlotList[i].Init(_unitCodeArr[Random.Range(0, _unitCodeArr.Length)], OnClickCard);
                _cardSlotList[i].gameObject.SetActive(true);
            }

            _gachaPanelObj.SetActive(true);
            _gachaPanelButtonText.text = LocalizeManager.Singleton.GetString(5005);

            Message.Send<Global.EscapeLockMsg>(new Global.EscapeLockMsg());
        }

        private void OnClickGachaConfirm()
        {
            if (_curFlipCount < _maxFlipCount)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                _coroutine = StartCoroutine(coFlipAll());

                return;
            }

            _gachaPanelObj.SetActive(false);
            Message.Send<Global.EscapeUnlockMsg>(new Global.EscapeUnlockMsg());
        }

        private IEnumerator coFlipAll()
        {
            for (int i = 0; i < _maxFlipCount; i++)
            {
                _cardSlotList[i].OnClick();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnClickCard()
        {
            _curFlipCount++;

            if (_curFlipCount >= _maxFlipCount)
            {
                _gachaPanelButtonText.text = LocalizeManager.Singleton.GetString(5006);
            }
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 29)
                PlayScenario_29();
        }

        private void PlayScenario_29()
        {
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(29, () =>
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_gachaButton_1.transform, () =>
                {
                    _gachaPanelButtonText.text = LocalizeManager.Singleton.GetString(5005);

                    // 뽑기 강제적용
                    _curFlipCount = 0;
                    _maxFlipCount = 1;

                    _cardSlotList[0].gameObject.SetActive(true);
                    _cardSlotList[0].Init(101131, () =>
                    {
                        OnClickCard();

                        _gachaPanelButtonText.text = LocalizeManager.Singleton.GetString(5006);

                        // 가챠 완료
                        Info.My.Singleton.User.isCompleteGacha = true;
                        Info.My.Singleton.Inventory.AddCharacter(101131, 1, 3);

                        RefreshUI();

                        // 뽑기 후 시나리오 재생
                        Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(30, () =>
                        {
                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_gachaPanelButton.transform, () =>
                            {
                                OnClickGachaConfirm();
                                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_backButton.transform, OnClickBack));
                            }));
                        }));
                    });

                    for (int i = 1; i < _cardSlotList.Count; i++)
                    {
                        _cardSlotList[i].gameObject.SetActive(false);
                    }

                    _gachaPanelObj.SetActive(true);

                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_cardSlotList[0].transform, () =>
                    {
                        _cardSlotList[0].OnClick();
                    }));
                }));
            }));
        }
    }
}