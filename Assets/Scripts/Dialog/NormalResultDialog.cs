// ==================================================
// NormalResultDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace Dialog
{
    public class NormalResultDialog : IDialog
    {
        [SerializeField] private Image _bg;
        [SerializeField] private Animator _anim;

        [Header("- Board")]
        [SerializeField] private GameObject _winObj;
        [SerializeField] private GameObject _loseObj;
        [SerializeField] private GameObject _timeOverObj;

        [Header("- Win")]
        [SerializeField] private Text _userNicknameText;
        [SerializeField] private Text _userLevelText;
        [SerializeField] private Image _userExpSlider;
        [SerializeField] private Text _userAddExpText;
        [SerializeField] private List<ResultCharSlot> _charSlotList;

        [Header("- Time Up")]
        [SerializeField] private List<UnitCardSlot> _charSlotList_Time;

        [Header("- Lose")]
        [SerializeField] private List<UnitCardSlot> _charSlotList_Lose;

        [Header(" - Button")]
        [SerializeField] private GridLayoutGroup _buttonGridLayout;
        [SerializeField] private Button _moveLobbyButton;
        [SerializeField] private Button _retryBattleButton;
        [SerializeField] private Button _moveNextButton;

        private Coroutine _coroutine = null;
        private Texture2D _captureTex = null;

        protected override void OnLoad()
        {
            _bg.gameObject.SetActive(false);
            _winObj.SetActive(false);
            _loseObj.SetActive(false);
            _moveLobbyButton.gameObject.SetActive(false);
            _retryBattleButton.gameObject.SetActive(false);
            _moveNextButton.gameObject.SetActive(false);
        }

        protected override void OnUnload()
        {
            _bg = null;
            _winObj = null;
            _loseObj = null;
            _moveLobbyButton.onClick.RemoveAllListeners();
            _moveLobbyButton = null;
            _retryBattleButton.onClick.RemoveAllListeners();
            _retryBattleButton = null;
            _moveNextButton.onClick.RemoveAllListeners();
            _moveNextButton = null;
        }

        protected override void OnEnter()
        {
            // Escape 동작 전부 지운다.
            Message.Send<Global.RemoveEscapeActionAllMsg>(new Global.RemoveEscapeActionAllMsg());

            Message.AddListener<Battle.Normal.BattleDrawMsg>(OnBattleDraw);
            Message.AddListener<Battle.Normal.BattleVictoryMsg>(OnBattleVictory);
            Message.AddListener<Battle.Normal.BattleDefeatMsg>(OnBattleDefeat);
        }

        protected override void OnExit()
        {
            Message.RemoveListener<Battle.Normal.BattleDrawMsg>(OnBattleDraw);
            Message.RemoveListener<Battle.Normal.BattleVictoryMsg>(OnBattleVictory);
            Message.RemoveListener<Battle.Normal.BattleDefeatMsg>(OnBattleDefeat);
        }

        private void OnClickLobby()
        {
            TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
            Scene.IScene.LoadScene<Scene.LobbyScene>();
        }

        private void OnClickRetry()
        {
            Scene.IScene.LoadScene<Scene.BattleNormalScene>();
        }

        private void OnClickNext()
        {
            if (BattleManager.Singleton.battleType == 6)
            {
                Scene.IScene.LoadScene<Scene.LobbyScene>();
            }
            else
            {
                SetChapterAndStage();
                Scene.IScene.LoadScene<Scene.LobbyScene>();
            }
        }

        private void OnBattleDraw(Battle.Normal.BattleDrawMsg msg)
        {
            _winObj.SetActive(false);
            _timeOverObj.SetActive(true);
            _loseObj.SetActive(false);
            _moveLobbyButton.gameObject.SetActive(true);
            _retryBattleButton.gameObject.SetActive(BattleManager.Singleton.battleType != 6);
            _moveNextButton.gameObject.SetActive(true);

            RefreshTimeUp();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coCaptureBackground(() =>
            {
                CheckScenario();
            }));
        }

        private void OnBattleVictory(Battle.Normal.BattleVictoryMsg msg)
        {
            _winObj.SetActive(true);
            _timeOverObj.SetActive(false);
            _loseObj.SetActive(false);
            _moveLobbyButton.gameObject.SetActive(true);
            _retryBattleButton.gameObject.SetActive(BattleManager.Singleton.battleType != 6);
            _moveNextButton.gameObject.SetActive(true);

            RefreshVictory();

            var list = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();
            for (int i = 0; i < _charSlotList.Count; i++)
            {
                if (i < list.Count)
                {
                    _charSlotList[i].Active();
                }
            }

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coCaptureBackground(() =>
            {
                CheckScenario();
            }));
        }

        private void OnBattleDefeat(Battle.Normal.BattleDefeatMsg msg)
        {
            _winObj.SetActive(false);
            _timeOverObj.SetActive(false);
            _loseObj.SetActive(true);
            _moveLobbyButton.gameObject.SetActive(true);
            _retryBattleButton.gameObject.SetActive(BattleManager.Singleton.battleType != 6);
            _moveNextButton.gameObject.SetActive(true);

            RefreshLose();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coCaptureBackground(() =>
            {
                CheckScenario();
            }));
        }

        private void RefreshVictory()
        {
            Info.User user = Info.My.Singleton.User;

            var em = Model.First<CharExpModel>();
            if (em == null)
            {
                Logger.LogError("CharExpModel 로드 실패");
                return;
            }

            CharExpModel.Exp current = em.expTable.Find(e => e.level == user.level);
            CharExpModel.Exp next = em.expTable.Find(e => e.level == user.level + 1);

            _userNicknameText.text = user.nickName.ToString();
            _userLevelText.text = string.Format("Lv. {0}", user.level);
            _userExpSlider.fillAmount = user.curExp / (float)user.maxExp;
            _userAddExpText.text = string.Format("+ {0:##,##}", 300);

            var list = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();

            for (int i = 0; i < _charSlotList.Count; i++)
            {
                if (i < list.Count)
                {
                    _charSlotList[i].gameObject.SetActive(true);
                    _charSlotList[i].Init(list[i].FSM.Skeleton.SkeletonDataAsset, list[i].Status);
                }
                else
                {
                    _charSlotList[i].gameObject.SetActive(false);
                }
            }
        }

        private void RefreshTimeUp()
        {
            var list = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();

            for (int i = 0; i < _charSlotList_Time.Count; i++)
            {
                if (i < list.Count)
                {
                    _charSlotList_Time[i].gameObject.SetActive(true);
                    _charSlotList_Time[i].Init(list[i].Status);
                }
                else
                {
                    _charSlotList_Time[i].gameObject.SetActive(false);
                }
            }
        }

        private void RefreshLose()
        {
            var list = BattleManager.Singleton.GetBlueTeam<UnitInfo_Normal>();

            for (int i = 0; i < _charSlotList_Lose.Count; i++)
            {
                if (i < list.Count)
                {
                    _charSlotList_Lose[i].gameObject.SetActive(true);
                    _charSlotList_Lose[i].Init(list[i].Status);
                }
                else
                {
                    _charSlotList_Lose[i].gameObject.SetActive(false);
                }
            }
        }

        private IEnumerator coCaptureBackground(System.Action callback)
        {
            Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());

            yield return new WaitForEndOfFrame();

            _captureTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            _captureTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
            _captureTex.Apply();

            Sprite sprite = Sprite.Create(_captureTex, new Rect(0, 0, Screen.width, Screen.height), Vector2.zero);
            _bg.sprite = sprite;

            _bg.gameObject.SetActive(true);

            _anim.SetTrigger("FadeIn");

            yield return new WaitForSeconds(0.2f);

            while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;

            if (_winObj.activeSelf == true)
            {
                for (int i = 0; i < _charSlotList.Count; i++)
                {
                    if (_charSlotList[i].gameObject.activeSelf == true)
                        _charSlotList[i].Victory();
                }
            }

            yield return new WaitForSeconds(0.5f);
            Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());

            if (callback != null)
            {
                callback();
            }
        }

        private void SetChapterAndStage()
        {
            switch (BattleManager.Singleton.battleType)
            {
                case 1:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;

                    StageModel sm = Model.First<StageModel>();
                    StageModel.Data data = sm.Table.Find(e => e.chapter == BattleManager.Singleton.selectChapter && e.stage == BattleManager.Singleton.selectStage + 1);
                    if (data == null)
                    {
                        BattleManager.Singleton.selectChapter++;
                        BattleManager.Singleton.selectStage = 1;
                    }
                    else
                    {
                        BattleManager.Singleton.selectStage++;
                    }

                    break;

                case 2:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Story;
                    break;

                case 3:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Dimension;
                    break;

                case 4:
                    // TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;
                    break;

                case 5:
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Babel;
                    break;
            }
        }

        private IEnumerator coNext()
        {
            yield return new WaitForSeconds(2f);

            Scene.IScene.LoadScene<Scene.LobbyScene>();
        }

        private void CheckScenario()
        {
            int sidx = TutorialManager.Singleton.curTutorialIndex;

            if (sidx == 21)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(21, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveLobbyButton.transform, _buttonGridLayout, () =>
                    {
                        TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                        Scene.IScene.LoadScene<Scene.LobbyScene>();
                    }));
                }));
            }
            else if (sidx == 33)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(33, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveLobbyButton.transform, _buttonGridLayout, () =>
                    {
                        TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                        Scene.IScene.LoadScene<Scene.LobbyScene>();
                    }));
                }));
            }
            else if (sidx == 40)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;
                    SetChapterAndStage();
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 42)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    Message.Send<Battle.Normal.Scenario_PandoraMsg>(new Battle.Normal.Scenario_PandoraMsg());
                }));
            }
            else if (sidx == 50)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;
                    SetChapterAndStage();
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 52)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(52, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveLobbyButton.transform, _buttonGridLayout, () =>
                    {
                        TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                        Scene.IScene.LoadScene<Scene.LobbyScene>();
                    }));
                }));
            }
            else if (sidx == 60 || sidx == 62 || sidx == 65)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;
                    SetChapterAndStage();
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 66)
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(66, () =>
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(67, () =>
                    {
                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                        {
                            TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.ContentMap;
                            Scene.IScene.LoadScene<Scene.LobbyScene>();
                        }));
                    }));
                }));
            }
            else if (sidx == 70)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Babel;
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 75)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.MainChapter;
                    SetChapterAndStage();
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 77)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveLobbyButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 87)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveLobbyButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 88)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Arena;
                    Info.My.Singleton.User.isCompleteArena = true;
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 91)
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveLobbyButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.None;
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
            }
            else if (sidx == 55)
            {
                if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage < 4)
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                {
                    TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Story;
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }));
                }
                else if (BattleManager.Singleton.battleType == 2 && BattleManager.Singleton.selectChapter == 1 && BattleManager.Singleton.selectStage == 4)
                {
                    Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(8, () =>
                    {
                        Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_moveNextButton.transform, _buttonGridLayout, () =>
                        {
                            TutorialManager.Singleton.openFirst = Constant.TutorialCallbackType.Story;
                            Scene.IScene.LoadScene<Scene.LobbyScene>();
                        }));
                    }, "Chiyou"));
                }
            }
            else
            {
                _moveLobbyButton.onClick.AddListener(OnClickLobby);
                _retryBattleButton.onClick.AddListener(OnClickRetry);
                _moveNextButton.onClick.AddListener(OnClickNext);
            }
        }
    }
}