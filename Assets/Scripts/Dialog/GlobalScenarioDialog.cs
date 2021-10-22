// ==================================================
// GlobalScenarioDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Febucci.UI;
using TMPro;

namespace Dialog
{
    public class GlobalScenarioDialog : IDialog
    {
        [Header("Panel Object")]
        /// <summary>
        /// 하단 시나리오 텍스트 패널
        /// </summary>
        [SerializeField] private GameObject _floatTextObj;
        /// <summary>
        /// 전체화면 텍스트 패널
        /// </summary>
        [SerializeField] private GameObject _fullTextObj;

        [Header("FloatText Panel Element")]
        [SerializeField] private Image _background;
        /// <summary>
        /// 캐릭터의 이름 표시 UI Object
        /// </summary>
        [SerializeField] private GameObject _charNameObj;
        /// <summary>
        /// 화자 캐릭터 포트레이트
        /// </summary>
        [SerializeField] private List<Image> _charPortrait_Body;
        [SerializeField] private List<Image> _charPortrait_Face;
        /// <summary>
        /// 캐릭터의 이름이 들어가는 UI.Text
        /// </summary>
        [SerializeField] private Text _charNameText;
        [SerializeField] private TextAnimatorPlayer _charMessageAnim;
        //// / <summary>
        //// / 캐릭터의 대사가 들어가는 UI.Text
        //// / </summary>
        // [SerializeField] private TextMeshProUGUI _charMessageText;
        /// <summary>
        /// 화면 터치영역. 텍스트를 넘기는 동작을 수행.
        /// </summary>
        [SerializeField] private Button _touchArea;
        /// <summary>
        /// 대화 스킵 버튼
        /// </summary>
        [SerializeField] private Button _skipButton;
        /// <summary>
        /// 모든 시나리오 스킵 기능 (Direct로 로비로 이동)
        /// </summary>
        [SerializeField] private Button _skipButton_Total;
        [SerializeField] private RectTransform _feedbackNext;

        [Header("FullText Panel Element")]
        /// <summary>
        /// 전체화면 대화 뒷배경
        /// </summary>
        [SerializeField] private Image _fullTextBackground;
        [SerializeField] private TextAnimatorPlayer _fullMessageAnim;
        /// <summary>
        /// 전체화면 메세지 텍스트
        /// </summary>
        [SerializeField] private TextMeshProUGUI _fullMessageText;
        [SerializeField] private Button _touchArea_Full;
        [SerializeField] private RectTransform _feedbackNext_Full;
        /// <summary>
        /// 유닛 리스트 (캐릭터 이름 및 포트레이트 참조용)
        /// </summary>
        private List<UnitModel.Unit> _unitList = new List<UnitModel.Unit>();
        /// <summary>
        /// 현재 재생할 시나리오의 리스트
        /// </summary>
        private List<ScenarioModel.Scenario> _curScenario;

        [SerializeField] private int _sceneIndex = 0;
        [SerializeField] private int _talkIndex = 0;
        private string _messageOrigin;
        private Coroutine _coroutine = null;
        private System.Action _callback = null;

        protected override void OnLoad()
        {
            // 튜토리얼 매니저에 이 Dialog를 등록
            TutorialManager.Singleton.AddGuideDialog(_dialogView);

            // 유닛 데이터를 먼저 받아준다. (여기선 계속 쓰이니깐)
            var um = Model.First<UnitModel>();
            if (um == null)
            {
                Logger.LogErrorFormat("Can't load model = {0}", um.GetType());
                return;
            }

            _unitList = um.unitTable;

            _touchArea.onClick.AddListener(OnClickNext);
            _skipButton.onClick.AddListener(OnClickSkip);
            _skipButton_Total.onClick.AddListener(OnClickSkip_Total);

            _charMessageAnim.textAnimator.onEvent += OnEvent;
            _fullMessageAnim.textAnimator.onEvent += OnEvent;
            _touchArea_Full.onClick.AddListener(OnClickNext);

            Message.AddListener<Global.ShowScenarioTextMsg>(OnShowScenarioText);
            Message.AddListener<Global.ShowScenarioBackgroundMsg>(OnShowScenarioBackground);
            Message.AddListener<Global.ShowScenarioFadeMsg>(OnShowScenarioFade);
        }

        protected override void OnUnload()
        {
            _floatTextObj = null;
            _fullTextObj = null;
            _background = null;
            _charNameObj = null;

            if (_charPortrait_Body != null)
            {
                _charPortrait_Body.Clear();
                _charPortrait_Body = null;
            }

            _charNameText = null;
            // _charMessageText = null;
            _touchArea.onClick.RemoveAllListeners();
            _touchArea = null;
            _skipButton.onClick.RemoveAllListeners();
            _skipButton = null;
            _skipButton_Total.onClick.RemoveAllListeners();
            _skipButton_Total = null;
            _fullTextBackground = null;
            _fullMessageText = null;

            _charMessageAnim.textAnimator.onEvent -= OnEvent;
            _fullMessageAnim.textAnimator.onEvent -= OnEvent;
            _touchArea_Full.onClick.RemoveAllListeners();
            _touchArea_Full = null;

            if (_unitList != null)
            {
                _unitList.Clear();
                _unitList = null;
            }

            _curScenario = null;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (_callback != null)
            {
                _callback = null;
            }

            Message.RemoveListener<Global.ShowScenarioTextMsg>(OnShowScenarioText);
            Message.RemoveListener<Global.ShowScenarioBackgroundMsg>(OnShowScenarioBackground);
            Message.RemoveListener<Global.ShowScenarioFadeMsg>(OnShowScenarioFade);
        }

        protected override void OnEnter()
        {
            _feedbackNext.gameObject.SetActive(false);
            _feedbackNext_Full.gameObject.SetActive(false);
        }

        protected override void OnExit()
        {
            if (_dialogView != null)
                _dialogView.SetActive(false);

            _curScenario = null;
            _sceneIndex = 0;
            _talkIndex = 1;

            // 시나리오 BGM 재생
            SoundManager.Singleton.PlayScenarioBGM(false);

            Message.Send<Global.ReloadSpeedMsg>(new Global.ReloadSpeedMsg());

            if (_callback != null)
                _callback();
        }

        private void OnEvent(string message)
        {
            if (message == "delay")
            {
                _fullMessageText.text = "";
                ShowScenario();
                // _fullMessageText.text = string.Format("<fade>{0}</fade><waitfor=3> <?next>", _fullMessageText.text);
            }
            else if (message == "next")
            {
                _fullMessageText.text = "";
                ShowScenario();
            }
            else if (message == "feedback")
            {
                _feedbackNext.gameObject.SetActive(true);
            }
            else if (message == "feedback_full")
            {
                _feedbackNext_Full.gameObject.SetActive(true);
            }
        }

        private void OnClickNext()
        {
            if (_floatTextObj.activeSelf == true && _charMessageAnim.textAnimator.allLettersShown == false)
            {
                _charMessageAnim.SkipTypewriter();
                _feedbackNext.gameObject.SetActive(true);
            }
            else if (_fullTextObj.activeSelf == true && _fullMessageAnim.textAnimator.allLettersShown == false)
            {
                _fullMessageAnim.SkipTypewriter();
                _feedbackNext_Full.gameObject.SetActive(true);

                // if (_coroutine != null)
                // {
                //     StopCoroutine(_coroutine);
                //     _coroutine = null;
                // }

                // _coroutine = StartCoroutine(coDelayFullText());
            }
            else
            {
                // if (_coroutine != null)
                // {
                //     StopCoroutine(_coroutine);
                //     _coroutine = null;
                // }

                ShowScenario();
            }
        }

        private IEnumerator coDelayFullText()
        {
            yield return new WaitForSeconds(2f);
            ShowScenario();
        }

        private void OnClickSkip()
        {
            var s = _curScenario.Find(e => e.Talk_ID > _talkIndex && e.talk_type != 0);
            if (s != null)  // 전체화면 뿌려줄 게 남았다...g
            {
                _talkIndex = s.Talk_ID;
                ShowScenario();
            }
            else    // 더이상의 전체화면 텍스트 뿌려줄게 없다...그냥 건너뛰자
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                CheckTutorial();
                OnExit();
            }
        }

        private void OnClickSkip_Total()
        {
            if (_dialogView != null)
                _dialogView.SetActive(false);

            var inven = Info.My.Singleton.Inventory;
            if (inven.characterList.Find(e => e.idx == 101131) == null)
                Info.My.Singleton.Inventory.AddCharacter(101131, 1, 3); // 진시
            if (inven.characterList.Find(e => e.idx == 103031) == null)
                Info.My.Singleton.Inventory.AddCharacter(103031, 1, 3); // 판도라

            if (string.IsNullOrEmpty(Info.My.Singleton.User.nickName) == true)
                Info.My.Singleton.User.nickName = "크로노웨어즈";

            _curScenario = null;
            _sceneIndex = 0;
            _talkIndex = 1;
            _callback = null;
            TutorialManager.Singleton.curTutorialIndex = 999;
            Info.My.Singleton.User.maxClearedMainstory = 13;
            Info.My.Singleton.User.maxClearedStory = 4;
            Info.My.Singleton.User.isCompleteArena = true;

            // 팀설정 동기화
            Info.My.Singleton.Inventory.dimensionTeam = Info.My.Singleton.Inventory.storyTeam;
            Info.My.Singleton.Inventory.subTeam = Info.My.Singleton.Inventory.storyTeam;
            Info.My.Singleton.Inventory.babelTeam = Info.My.Singleton.Inventory.storyTeam;
            Info.My.Singleton.Inventory.arenaTeam = Info.My.Singleton.Inventory.storyTeam;

            Info.My.Singleton.Inventory.bossTeam[0] = 000001;
            Info.My.Singleton.Inventory.bossTeam[1] = 000002;
            Info.My.Singleton.Inventory.bossTeam[2] = 000003;
            Info.My.Singleton.Inventory.bossTeam[3] = 000004;
            Info.My.Singleton.Inventory.bossTeam[4] = 000005;

            Scene.IScene.LoadScene<Scene.LobbyScene>();
        }

        /// <summary>
        /// 시나리오 텍스트를 재생한다
        /// </summary>
        private void ShowScenario()
        {
            var s = _curScenario.Find(e => e.Talk_ID == _talkIndex);

            // 더이상의 talkIndex가 없다. 시나리오 텍스트 종료.
            if (s == null)
            {
                _dialogView.SetActive(false);
                CheckTutorial();
                OnExit();
                return;
            }

            ShowText(s);
            _talkIndex++;
        }

        private void ShowText(ScenarioModel.Scenario s)
        {
            // 혹시 남아있을지 모르는 coroutine을 null.
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            // talkType에 따라 패널 오브젝트 활성 & 비활성화.
            _floatTextObj.SetActive(s.talk_type == 0);
            _fullTextObj.SetActive(s.talk_type == 1 || s.talk_type == 2);

            // FloatText를 띄워주어야 하는 경우
            if (s.talk_type == 0)
            {
                _charNameObj.SetActive(s.talk_type == 0);
                if (s.talk_type == 0)
                {
                    // 0번인 경우 유저이름 출력
                    if (s.Char_ID == 0)
                        _charNameText.text = GetMessage("<username>");
                    else
                    {
                        // 유닛이름 테이블 로드
                        var nm = Model.First<CharNameModel>();
                        if (nm != null)
                        {
                            var n = nm.charNameTable.Find(e => e.Index == s.Char_ID);
                            if (n != null)
                            {
                                _charNameText.text = n.Name;
                            }
                            else
                            {
                                Logger.LogErrorFormat("Can't find char_id in charname.txt\ntarget(char_id) = {0}", s.Char_ID);
                                _charNameText.text = string.Empty;
                            }
                        }
                        else
                        {
                            Logger.LogError("테이블 로드 실패");
                            _charNameText.text = string.Empty;
                        }
                    }

                    Sprite[] spr = GetUnitPortrait(s.Image_Index, s.face);

                    for (int i = 0; i < _charPortrait_Body.Count; i++)
                    {
                        // 캐릭터 이미지 못찾았으니 비활성화.
                        if (spr == null)
                        {
                            _charPortrait_Body[i].gameObject.SetActive(false);
                            continue;
                        }

                        _charPortrait_Body[i].gameObject.SetActive(i == s.stand);
                    }

                    if (spr[0] != null || spr[1] != null)
                    {
                        _charPortrait_Body[s.stand].gameObject.SetActive(spr[0] != null);
                        _charPortrait_Face[s.stand].gameObject.SetActive(spr[1] != null);

                        if (spr[0] != null)
                            _charPortrait_Body[s.stand].sprite = spr[0];
                        if (spr[1] != null)
                            _charPortrait_Face[s.stand].sprite = spr[1];

                        // 얼굴 네이티브 사이즈, 위치수정
                        _charPortrait_Face[s.stand].SetNativeSize();

                        Vector2 vec = Vector2.zero;

                        switch (s.Image_Index)
                        {
                            case 1:
                                vec = new Vector2(437f, -166f);
                                break;

                            case 2:
                                vec = new Vector2(456f, -164f);
                                break;

                            case 3:
                                vec = new Vector2(421f, -343f);
                                break;

                            case 4:
                                vec = new Vector2(325f, -186f);
                                break;

                            case 5:
                                vec = new Vector2(372f, -308f);
                                break;
                        }

                        _charPortrait_Face[s.stand].rectTransform.anchoredPosition = vec;

                        // reverse
                        _charPortrait_Body[s.stand].transform.localScale = new Vector3(s.pos == 0 ? 1.5f : -1.5f, 1.5f, 1f);
                    }
                    else
                    {
                        for (int i = 0; i < _charPortrait_Body.Count; i++)
                        {
                            _charPortrait_Body[i].gameObject.SetActive(false);
                        }
                    }
                }

                // _charMessageText.text = GetMessage(s.talk);
                _feedbackNext.gameObject.SetActive(false);
                _charMessageAnim.ShowText(GetMessage(s.talk) + "<?feedback>");
            }
            // FullText를 띄워주어야 하는 경우
            else
            {
                _fullTextBackground.color = (s.talk_type == 1 ? Color.black : Color.white);
                _fullMessageText.color = (s.talk_type == 1 ? Color.white : Color.black);

                _fullMessageAnim.useTypeWriter = true;


                // _fullMessageText.text = ;
                // _fullMessageAnim.ShowText(string.Format("{0}", GetMessage(s.talk, true)));
                _feedbackNext_Full.gameObject.SetActive(false);
                _fullMessageAnim.ShowText(string.Format("{0}<waitfor=0.1f> <?feedback_full>", GetMessage(s.talk, true)));
            }
        }

        private IEnumerator coFade(bool isFade)
        {
            Color s = _background.color;
            Color e = _background.color;
            s.a = (isFade == true ? 0f : 1f);
            e.a = (isFade == true ? 1f : 0f);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                _background.color = Color.Lerp(s, e, t);
                yield return null;
            }

            _dialogView.SetActive(false);
            OnExit();

            _coroutine = null;
        }

        /// <summary>
        /// n번 시나리오를 재생 시작하라는 메세지
        /// </summary>
        /// <param name="msg"></param>
        private void OnShowScenarioText(Global.ShowScenarioTextMsg msg)
        {
            _sceneIndex = msg.sceneIndex;
            _talkIndex = 1;
            _callback = msg.callback;

            if (_dialogView != null)
                _dialogView.SetActive(true);

            _curScenario = GetScenario(_sceneIndex, msg.scenarioName);
            ShowScenario();

            // 시나리오 BGM 재생
            SoundManager.Singleton.PlayScenarioBGM(true);
        }

        private void OnShowScenarioBackground(Global.ShowScenarioBackgroundMsg msg)
        {
            if (_dialogView != null)
                _dialogView.SetActive(true);

            _fullTextObj.SetActive(false);
            _floatTextObj.SetActive(false);

            _background.gameObject.SetActive(msg.isEnabled);
            _background.color = Color.black;
        }

        private void OnShowScenarioFade(Global.ShowScenarioFadeMsg msg)
        {
            if (_dialogView != null)
                _dialogView.SetActive(true);

            _callback = msg.Callback;

            _fullTextObj.SetActive(false);
            _floatTextObj.SetActive(false);

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coFade(msg.isFade));
        }

        /// <summary>
        /// sceneIndex에 해당하는 시나리오 리스트 로드
        /// </summary>
        /// <param name="sceneIndex">시나리오 인덱스</param>
        /// <returns></returns>
        private List<ScenarioModel.Scenario> GetScenario(int sceneIndex, string key)
        {
            // 모델 로드.
            var sm = Model.First<ScenarioModel>();
            if (sm == null)
            {
                Logger.LogError("ScenarioModel을 찾지 못하였습니다.");
                return null;
            }

            // 시나리오 로드.
            List<ScenarioModel.Scenario> scenarios = null;
            sm.scenarioDic.TryGetValue(key, out scenarios);
            var s = scenarios.FindAll(e => e.Scene == sceneIndex);
            if (s == null)
            {
                Logger.LogErrorFormat("index가 {0}인 시나리오를 찾지 못하였습니다.");
                return null;
            }

            // 혹시 모르니 talkIndex 순서대로 정렬.
            s.Sort((a, b) => a.Talk_ID.CompareTo(b.Talk_ID));

            return s;
        }

        /// <summary>
        /// code에 해당하는 캐릭터의 이름을 불러옴
        /// </summary>
        /// <param name="code">캐릭터의 코드값</param>
        /// <returns></returns>
        private string GetUnitName(int code)
        {
            // 추후 추가될 ??? 인덱스
            // if (code == 10000)
            //     return string.Empty;

            var unit = _unitList.Find(e => e.code == code);
            if (unit == null)
                return string.Empty;

            return unit.unitName_Kor;
        }

        private Sprite[] GetUnitPortrait(int index, int face)
        {
            Sprite[] spr = new Sprite[2];
            spr[0] = Resources.Load<Sprite>(string.Format("Texture/Character/Scenario/Resize/{0}_body", index));
            spr[1] = Resources.Load<Sprite>(string.Format("Texture/Character/Scenario/Resize/{0}_face_{1}", index, face));
            return spr;
        }

        private Sprite[] GetUnitPortrait(string name)
        {
            Sprite[] spr = new Sprite[2];
            spr[0] = Resources.Load<Sprite>(string.Format("Texture/Character/Scenario/Resize/{0}_body", name));
            spr[1] = Resources.Load<Sprite>(string.Format("Texture/Character/Scenario/Resize/{0}_face_0", name));
            return spr;
        }

        /// <summary>
        /// Scenario의 Message를 특수기호문자를 통해 출력해야할 문자열로 변환
        /// </summary>
        /// <param name="str">Scenario의 Message</param>
        /// <returns></returns>
        private string GetMessage(string str, bool delay = false)
        {
            string result = str;

            // 쉼표 변환
            result = result.Replace("$", ",");

            // 줄바꿈 변환
            if (delay == false)
                result = result.Replace("<br>", "\n");
            else
                result = result.Replace("<br>", "<waitfor=1>\n");

            // 유저이름 변환
            result = result.Replace("<username>", Info.My.Singleton.User.nickName);

            return result;
        }

        private void CheckTutorial()
        {
            // // 튜토리얼 가이드 컴포넌트가 있는지 확인한다.
            // var tuto = TutorialManager.Singleton.GetGuideButtonInfo(_sceneIndex);

            // // 해당 씬의 가이드가 있으면 표시한다.
            // if (tuto != null)
            // {

            //     int nextScene = _sceneIndex + 1;

            //     Message.Send<Global.ShowButtonGuideMsg>(new Global.ShowButtonGuideMsg(tuto.scene, tuto.activeType, null));
            // }
        }
    }
}