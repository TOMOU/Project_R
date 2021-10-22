using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Text;
using Febucci.UI;

namespace Dialog
{
    public class TitleMainDialog : IDialog
    {
        [Header("CI 로고")]
        [SerializeField] private Image _ciImage;

        [SerializeField] private Image _titleImage;
        [SerializeField] private Image _titleLogoImage;
        [SerializeField] private Animator _titleLogoAnim;

        [Header("타이틀 로고")]
        [SerializeField] private Button _titleButton;

        [SerializeField] private TextAnimatorPlayer _touchTextAnim;

        [Header("- Change Locale")]
        [SerializeField] private Button _changeLocaleButton;

        private Coroutine _coroutine = null;

        protected override void OnLoad()
        {
            _ciImage.gameObject.SetActive(false);
            _titleImage.gameObject.SetActive(false);
            _titleLogoImage.gameObject.SetActive(false);
            _titleButton.gameObject.SetActive(false);

            _titleButton.onClick.AddListener(OnClickScreen);
            _changeLocaleButton.onClick.AddListener(() =>
            {
                Dialog.IDialog.RequestDialogEnter<GlobalLocalizeDialog>();
            });
        }

        protected override void OnUnload()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _ciImage = null;
            _titleImage = null;
            _titleLogoImage = null;
            _titleButton.onClick.RemoveAllListeners();
            _titleButton = null;
        }

        protected override void OnEnter()
        {
            // Title 연출 중에는 Input을 잠금
            Message.Send<Global.InputLockMsg>(new Global.InputLockMsg());

            ShowCompanyLogo();
        }

        protected override void OnExit()
        {

        }

        #region Company Logo
        private void ShowCompanyLogo()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coShowCompanyLogo());
        }

        private IEnumerator coShowCompanyLogo()
        {
            _ciImage.gameObject.SetActive(true);

            float t = 0f;
            Color black = Color.black;
            Color white = Color.white;

            // 1초동안 이미지 색상을 검은색->흰색으로
            while (t < 1f)
            {
                t += Time.deltaTime;

                _ciImage.SetColorWithChild(black, white, t);

                yield return null;
            }

            t = 0f;

            while (t < 2f)
            {
                t += Time.deltaTime;
                yield return null;
            }

            t = 0f;

            // 1초동안 이미지 색상을 흰색->검은색으로
            while (t < 1f)
            {
                t += Time.deltaTime;

                _ciImage.SetColorWithChild(white, black, t);

                yield return null;
            }

            ShowTitleVideo();
        }
        #endregion

        #region Main Title
        private void ShowTitleVideo()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(coPrepareVideo());
        }

        private IEnumerator coPrepareVideo()
        {
            // 비디오 오브젝트 선행 활성화
            _ciImage.gameObject.SetActive(false);
            _titleImage.gameObject.SetActive(true);
            _titleLogoImage.gameObject.SetActive(true);

            // 오브젝트 컬러값 1초동안 변경
            float t = 0f;

            // Opacity 변경을 위한 변수
            Color black = Color.black;
            Color white = Color.white;

            while (t < 1f)
            {
                t += Time.deltaTime;
                _titleImage.SetColorWithChild(black, white, t);
                _titleLogoImage.SetColorWithChild(black, white, t);
                yield return null;
            }

            _titleButton.gameObject.SetActive(true);
            _titleLogoAnim.SetTrigger("isEnter");

            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => _touchTextAnim.textAnimator.allLettersShown == true);

            Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());
        }

        private void OnClickScreen()
        {
            if (BuildOptionDefine.isPlayTutorial == false)
            {
                // 캐릭터 전원 참전
                Info.My.Singleton.Inventory.AddCharacter(101131, 1, 3); // 진시
                Info.My.Singleton.Inventory.storyTeam[2, 0] = 000004;
                Info.My.Singleton.Inventory.AddCharacter(103031, 1, 3); // 판도라
                Info.My.Singleton.Inventory.storyTeam[2, 2] = 000005;

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

                Info.My.Singleton.User.nickName = "크로노웨어즈";
                TutorialManager.Singleton.curTutorialIndex = 999;
                Info.My.Singleton.User.maxClearedMainstory = 13;
                Info.My.Singleton.User.maxClearedDimension = 4;
                Info.My.Singleton.User.maxClearedBabel = 3;
                Info.My.Singleton.User.maxClearedStory = 4;
                Scene.IScene.LoadScene<Scene.LobbyScene>();

                Info.My.Singleton.User.isCompleteArena = true;

                Message.Send<Global.FastSpeedMsg>(new Global.FastSpeedMsg());
                Message.Send<Global.EnableAutoModeMsg>(new Global.EnableAutoModeMsg());
            }
            else
            {
                if (string.IsNullOrEmpty(Info.My.Singleton.User.nickName) == true)
                    Scene.IScene.LoadScene<Scene.TutorialScene>();
                else
                {
                    TutorialManager.Singleton.curTutorialIndex = 99999;
                    Scene.IScene.LoadScene<Scene.LobbyScene>();
                }
            }
        }
        #endregion
    }
}