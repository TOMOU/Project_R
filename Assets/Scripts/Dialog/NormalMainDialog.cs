// ==================================================
// NormalMainDialog.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections;
using System.Collections.Generic;
using Battle.Normal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Spine.Unity;

namespace Dialog
{
    public class NormalMainDialog : IDialog
    {
        [Header("- Top")]
        [SerializeField] private Text _stageNameText;
        [SerializeField] private Text _waveCountText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _itemCountText;
        [SerializeField] private Text _itemCountText2;
        [SerializeField] private Button _pauseButton = null;

        [Header("- UnitList")]
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private List<UnitCardSlot_Raid> _slotList;

        [Header("- Bottom")]
        [SerializeField] private Button _fastModeButton;
        [SerializeField] private GameObject[] _fastModeObj;
        [SerializeField] private Button _autoModeButton;
        [SerializeField] private GameObject[] _autoModeObj;

        [Header("- CutScene")]
        [SerializeField] private GameObject _cutSceneObj;
        [SerializeField] private Animator _animator;
        [SerializeField] private Image _cutSceneCharacterImage;
        [SerializeField] private SkeletonGraphic _cutSceneGlass;

        [SerializeField] private Button _instantKillButton;



        // ========================================
        // 전추화면 (데미지표기)
        // ========================================
        private List<Damage> _damagePool;
        private GameObject _prefab;

        // ========================================
        // UB연출
        // ========================================
        // [SerializeField] private GameObject _videoRoot = null;
        // [SerializeField] private VideoPlayer _videoPlayer;
        // [SerializeField] private RawImage _videoScreen;
        private Coroutine _coroutine = null;
        private Coroutine _skillCoroutine = null;
        private int _unitCode = 0;
        private System.Action _callback = null;
        private int _curPhase = 0;
        private Battle.Normal.OnTimeProcess progress;
        private Battle.Normal.OnInstantKillProcess instantKillProcess;

        public void Test()
        {
            _dialogView = transform.Find("View").gameObject;

            // 데미지표기 캐싱
            if (_damagePool == null)
                _damagePool = new List<Damage>();

            _prefab = Resources.Load<GameObject>("UI/Battle/DamageObject");
            if (_prefab == null)
            {
                Logger.LogError("UI/Battle/DamageObject 오브젝트가 없습니다.");
                return;
            }

            Damage dmg;

            for (int i = 0; i < 10; i++)
            {
                dmg = GameObject.Instantiate(_prefab).GetComponent<Damage>();
                dmg.transform.SetParent(_dialogView.transform.Find("Damage"));
                dmg.transform.localRotation = Quaternion.identity;
                dmg.transform.localScale = Vector3.one;

                _damagePool.Add(dmg);
            }

            Message.AddListener<UnitSlotInitMsg>(OnUnitSlotInit);
            Message.AddListener<ShowDamageMsg>(OnShowDamage);
            Message.AddListener<ShowHealMsg>(OnShowHeal);
            Message.AddListener<PlayUseSkillMsg>(OnPlayUseSkill);
        }

        protected override void OnLoad()
        {
            bool fastMode = TimeManager.Singleton.FastMode;
            _fastModeButton.onClick.AddListener(OnClickFastMode);
            _fastModeObj[0].SetActive(fastMode == false);
            _fastModeObj[1].SetActive(fastMode == true);

            bool isAutoMode = BattleManager.Singleton.AutoSkill;
            _autoModeButton.onClick.AddListener(OnClickAutoMode);

            if (BattleManager.Singleton.battleType != 6)
            {
                _autoModeObj[0].SetActive(isAutoMode == false);
                _autoModeObj[1].SetActive(isAutoMode == true);
            }
            else
            {
                _autoModeObj[0].SetActive(false);
                _autoModeObj[1].SetActive(true);
            }

            _pauseButton.onClick.AddListener(OnClickPause);

            // 데미지표기 캐싱
            if (_damagePool == null)
                _damagePool = new List<Damage>();

            _prefab = Resources.Load<GameObject>("UI/Battle/DamageObject");
            if (_prefab == null)
            {
                Logger.LogError("UI/Battle/DamageObject 오브젝트가 없습니다.");
                return;
            }

            Damage dmg;

            for (int i = 0; i < 10; i++)
            {
                dmg = GameObject.Instantiate(_prefab).GetComponent<Damage>();
                dmg.transform.SetParent(_dialogView.transform.Find("Damage"));
                dmg.transform.localRotation = Quaternion.identity;
                dmg.transform.localScale = Vector3.one;

                _damagePool.Add(dmg);
            }

            _instantKillButton.onClick.AddListener(OnClickInstantKill);

            _slotList.ForEach(e => e.Init(null));

            _cutSceneObj.SetActive(false);

            SetTitleName();

            Message.AddListener<UnitSlotInitMsg>(OnUnitSlotInit);
            Message.AddListener<ShowDamageMsg>(OnShowDamage);
            Message.AddListener<PlayUseSkillMsg>(OnPlayUseSkill);
            Message.AddListener<StartBattleMsg>(OnStartBattle);
            Message.AddListener<ShowHealMsg>(OnShowHeal);
            Message.AddListener<AttachStageTimeMsg>(OnAttachStageTime);
            Message.AddListener<AttachInstantKillMsg>(OnAttachInstantKill);
            Message.AddListener<AttachStageWaveMsg>(OnAttachStageWave);
        }

        protected override void OnUnload()
        {
            if (_slotList != null)
            {
                _slotList.Clear();
                _slotList = null;
            }

            _fastModeButton.onClick.RemoveAllListeners();
            _fastModeButton = null;
            _fastModeObj = null;

            _autoModeButton.onClick.RemoveAllListeners();
            _autoModeButton = null;
            _autoModeObj = null;

            _pauseButton.onClick.RemoveAllListeners();
            _pauseButton = null;

            if (_damagePool != null)
            {
                _damagePool.Clear();
                _damagePool = null;
            }

            // _videoRoot = null;
            // _videoPlayer.Stop();
            // _videoPlayer = null;
            // _videoScreen = null;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (_skillCoroutine != null)
            {
                StopCoroutine(_skillCoroutine);
                _skillCoroutine = null;
            }

            if (_callback != null)
            {
                _callback = null;
            }

            _instantKillButton.onClick.RemoveAllListeners();

            Message.RemoveListener<UnitSlotInitMsg>(OnUnitSlotInit);
            Message.RemoveListener<ShowDamageMsg>(OnShowDamage);
            Message.RemoveListener<PlayUseSkillMsg>(OnPlayUseSkill);
            Message.RemoveListener<StartBattleMsg>(OnStartBattle);
            Message.RemoveListener<ShowHealMsg>(OnShowHeal);
            Message.RemoveListener<AttachStageTimeMsg>(OnAttachStageTime);
            Message.RemoveListener<AttachInstantKillMsg>(OnAttachInstantKill);
            Message.RemoveListener<AttachStageWaveMsg>(OnAttachStageWave);
        }

        protected override void OnEnter()
        {
            // _fastModeButton.gameObject.SetActive(TutorialManager.Singleton.curTutorialIndex > 18);
            // _autoModeButton.gameObject.SetActive(TutorialManager.Singleton.curTutorialIndex > 19);
            // _pauseButton.gameObject.SetActive(TutorialManager.Singleton.curTutorialIndex > 21);

            // _videoRoot.SetActive(false);

            // 
            _autoModeButton.interactable = BattleManager.Singleton.battleType != 6;
        }

        protected override void OnExit()
        {

        }

        private void Update()
        {
            if (_slotList == null)
                return;

            for (int i = 0; i < _slotList.Count; i++)
            {
                _slotList[i].Refresh();
            }

            if (progress != null)
            {
                progress(_timeText);
            }

            if (instantKillProcess != null)
            {
                instantKillProcess(_instantKillButton.gameObject);
            }
        }

        private void OnClickFastMode()
        {
            if (TutorialManager.Singleton.curTutorialIndex < 19)
                return;

            // 현재의 빠르기 상태
            bool isFastMode = TimeManager.Singleton.FastMode;

            // 매니저에 값 전송
            if (isFastMode == true)
            {
                Message.Send<Global.NormalSpeedMsg>(new Global.NormalSpeedMsg());
                isFastMode = false;
            }
            else
            {
                Message.Send<Global.FastSpeedMsg>(new Global.FastSpeedMsg());
                isFastMode = true;
            }

            // 버튼 상태 바꾸기
            _fastModeObj[0].SetActive(isFastMode == false);
            _fastModeObj[1].SetActive(isFastMode == true);
        }

        private void OnClickAutoMode()
        {
            if (TutorialManager.Singleton.curTutorialIndex < 19)
                return;

            // 현재의 auto 상태
            bool isAutoMode = BattleManager.Singleton.AutoSkill;

            // 매니저에 값 전송
            if (isAutoMode == true)
            {
                Message.Send<Global.DisableAutoModeMsg>(new Global.DisableAutoModeMsg());
                isAutoMode = false;
            }
            else
            {
                Message.Send<Global.EnableAutoModeMsg>(new Global.EnableAutoModeMsg());
                isAutoMode = true;
            }

            // 버튼 상태 바꾸기
            _autoModeObj[0].SetActive(isAutoMode == false);
            _autoModeObj[1].SetActive(isAutoMode == true);
        }

        private void OnClickPause()
        {
            RequestDialogEnter<NormalPauseDialog>();
        }

        private void OnUnitSlotInit(UnitSlotInitMsg msg)
        {
            if (msg.index < 0 || msg.index > 4)
            {
                Logger.LogErrorFormat("[UnitSlotInitMsg] UI 인덱스 메세지를 잘못 보냈습니다.\n받은 인덱스 = {0}", msg.index);
                return;
            }

            _slotList[msg.index].Init(msg.status);
        }

        private void OnShowDamage(ShowDamageMsg msg)
        {
            Damage d = _damagePool.Find(e => e.IsActive == false);
            if (d != null)
            {
                d.ActiveDamage(msg.target.transform, msg.damage, msg.isCritical, -1);
                d.transform.SetAsLastSibling();
            }
            else
            {
                d = GameObject.Instantiate(_prefab).GetComponent<Damage>();
                d.transform.SetParent(transform);
                d.transform.localRotation = Quaternion.identity;
                d.transform.localScale = Vector3.one;

                _damagePool.Add(d);

                d.ActiveDamage(msg.target.transform, msg.damage, msg.isCritical, -1);
                d.transform.SetAsLastSibling();
            }
        }

        private void OnShowHeal(ShowHealMsg msg)
        {
            Damage d = _damagePool.Find(e => e.IsActive == false);
            if (d != null)
            {
                d.ActiveHeal(msg.Target.transform, msg.Value);
                d.transform.SetAsLastSibling();
            }
            else
            {
                d = GameObject.Instantiate(_prefab).GetComponent<Damage>();
                d.transform.SetParent(transform);
                d.transform.localRotation = Quaternion.identity;
                d.transform.localScale = Vector3.one;

                _damagePool.Add(d);

                d.ActiveHeal(msg.Target.transform, msg.Value);
                d.transform.SetAsLastSibling();
            }
        }

        private void OnAttachStageTime(AttachStageTimeMsg msg)
        {
            progress = msg.Delegate;
        }

        private void OnAttachInstantKill(AttachInstantKillMsg msg)
        {
            instantKillProcess = msg.Delegate;
        }

        private void OnAttachStageWave(AttachStageWaveMsg msg)
        {
            _waveCountText.text = string.Format("{0}/{1}", msg.Current, msg.Max);
        }

        private void OnPlayUseSkill(PlayUseSkillMsg msg)
        {
            if (msg.Sender.Status.code == 110001)
            {
                if (msg.action != null)
                    msg.action();
                return;
            }

            if (_skillCoroutine != null)
            {
                StopCoroutine(_skillCoroutine);
                _skillCoroutine = null;
            }

            _skillCoroutine = StartCoroutine(coPlayCutscene(msg.Sender.Status.code, msg.action));
        }

        private IEnumerator coPlayCutscene(int code, System.Action callback)
        {
            Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(Constant.SoundName.Scene_Skill));

            _cutSceneCharacterImage.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/4_cutscene/{0}", code));

            switch (code)
            {
                case 100211:
                case 101031:
                case 102831:
                    _cutSceneCharacterImage.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                    break;
                case 101131:
                case 103031:
                    _cutSceneCharacterImage.transform.localScale = new Vector3(-1.2f, 1.2f, 1);
                    break;
            }

            _cutSceneObj.SetActive(true);
            _cutSceneGlass.Initialize(true);
            _cutSceneGlass.AnimationState.SetAnimation(0, "scene", false);

            yield return new WaitForSeconds(0.5f);

            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                yield return null;

            _cutSceneObj.SetActive(false);

            if (callback != null)
                callback();
        }

        private void OnStartBattle(StartBattleMsg msg)
        {
            _curPhase++;

            if (TutorialManager.Singleton.curTutorialIndex != 14)
                return;

            if (_curPhase == 1)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }

                _coroutine = StartCoroutine(coTutorialProcess());
            }
        }

        // private void PrepareVideo()
        // {
        //     if (_coroutine != null)
        //     {
        //         StopCoroutine(_coroutine);
        //         _coroutine = null;
        //     }

        //     _coroutine = StartCoroutine(coPrepareVideo());
        // }

        // private IEnumerator coPrepareVideo()
        // {
        //     // 비디오 준비
        //     _videoPlayer.Prepare();

        //     // 비디오가 준비되는 것을 기다림
        //     while (!_videoPlayer.isPrepared)
        //     {
        //         yield return null;
        //     }

        //     // VideoPlayer의 출력 texture를 RawImage의 texture로 설정한다
        //     _videoScreen.texture = _videoPlayer.texture;

        //     // 비디오 재생
        //     _videoPlayer.playbackSpeed = Time.timeScale;
        //     _videoPlayer.Play();

        //     // 사운드 재생
        //     PlaySound("cutin");

        //     while (_videoPlayer.isPlaying)
        //         yield return null;

        //     _videoRoot.SetActive(false);

        //     if (_callback != null)
        //         _callback();

        //     PlaySound("ub");
        // }

        private void PlaySound(string state)
        {
            string msg = string.Format("vo_{0}_{1}", _unitCode, state);
            Constant.SoundName e = (Constant.SoundName)System.Enum.Parse(typeof(Constant.SoundName), msg);
            Message.Send<Global.PlaySoundMsg>(new Global.PlaySoundMsg(e));
        }

        private void OnClickInstantKill()
        {
            BattleManager.Singleton.GetRedTeam<UnitInfo_Normal>().ForEach(e =>
            {
                Message.Send<Battle.Normal.SendDamageMsg>(new Battle.Normal.SendDamageMsg(e, 99999, true));
            });
        }

        private void SetTitleName()
        {
            int chapter = BattleManager.Singleton.selectChapter;
            int stage = BattleManager.Singleton.selectStage;

            switch (BattleManager.Singleton.battleType)
            {
                case 0:
                    _stageNameText.text = "???";
                    break;

                case 1:
                    _stageNameText.text = string.Format("{0}  {1} - {2}", LocalizeManager.Singleton.GetString(8002 + chapter - 1), chapter, stage);
                    break;

                case 2:
                    _stageNameText.text = string.Format("{0}  {1} - {2}", LocalizeManager.Singleton.GetString(10002), chapter, stage);
                    break;

                case 3:
                    _stageNameText.text = string.Format("{0} {1}", LocalizeManager.Singleton.GetString(9001), LocalizeManager.Singleton.GetString(9002 + stage - 1));
                    break;

                case 4:
                    _stageNameText.text = LocalizeManager.Singleton.GetString(13001);
                    break;

                case 5:
                    _stageNameText.text = string.Format(LocalizeManager.Singleton.GetString(11033), stage);
                    break;

                case 6:
                    _stageNameText.text = LocalizeManager.Singleton.GetString(12001);
                    break;
            }
        }

        private IEnumerator coTutorialProcess()
        {
            var slot = _slotList.Find(e => e.Status.unitName == "chiyou");

            _grid.enabled = false;

            Message.Send<Battle.Normal.LockInstantKillMsg>(new LockInstantKillMsg());

            // 14. 라운드 시작 시
            Message.Send<Global.EnablePauseMsg>(new Global.EnablePauseMsg());
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(14, () =>
            {
                Message.Send<Global.DisablePauseMsg>(new Global.DisablePauseMsg());
            }));

            // 2초 대기
            yield return new WaitForSeconds(2f);

            // 15. 치우 정보 포커싱
            Message.Send<Global.EnablePauseMsg>(new Global.EnablePauseMsg());
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(15, () =>
            {
                Message.Send<Global.ForceFocusGuideMsg>(new Global.ForceFocusGuideMsg(slot.transform, () =>
                {
                    Message.Send<Global.DisablePauseMsg>(new Global.DisablePauseMsg());
                }));
            }));

            Message.Send<Battle.Normal.UnlockInstantKillMsg>(new UnlockInstantKillMsg());

            // 2라운드 넘어가기 전까지 대기
            yield return new WaitUntil(() => _curPhase > 1);

            Message.Send<Battle.Normal.LockInstantKillMsg>(new LockInstantKillMsg());

            // 16. 치우 정보 포커싱 및 AP Max
            slot.Status.mp = 1000;

            Message.Send<Global.EnablePauseMsg>(new Global.EnablePauseMsg());
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(16, () =>
            {
                Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(slot.transform, () =>
                {
                    Message.Send<Battle.Normal.GenericUseSkillMsg>(new GenericUseSkillMsg(slot.Status.idx));
                    Message.Send<Global.DisablePauseMsg>(new Global.DisablePauseMsg());
                }));
            }));

            Message.Send<Battle.Normal.UnlockInstantKillMsg>(new UnlockInstantKillMsg());

            // 3라운드 넘어가기 전까지 대기
            yield return new WaitUntil(() => _curPhase > 2);

            Message.Send<Battle.Normal.LockInstantKillMsg>(new LockInstantKillMsg());

            Message.Send<Global.EnablePauseMsg>(new Global.EnablePauseMsg());
            Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(17, () =>
            {
                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(18, () =>
                {
                    Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_autoModeButton.transform, () =>
                    {
                        OnClickAutoMode();
                        Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(19, () =>
                        {
                            Message.Send<Global.ForceButtonGuideMsg>(new Global.ForceButtonGuideMsg(_fastModeButton.transform, () =>
                            {
                                OnClickFastMode();
                                Message.Send<Global.ShowScenarioTextMsg>(new Global.ShowScenarioTextMsg(20, () =>
                                {
                                    Message.Send<Global.DisablePauseMsg>(new Global.DisablePauseMsg());
                                }));
                            }));
                        }));
                    }));
                }));
            }));

            Message.Send<Battle.Normal.UnlockInstantKillMsg>(new UnlockInstantKillMsg());

            yield return null;
        }
    }
}