using System.Collections;
using System.Collections.Generic;
using Constant;
using Global;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private class ClipCache
    {
        public AudioClip clip { get; private set; }
        public float volume { get; private set; }
        public bool isLoop { get; private set; }
        public ClipCache(AudioClip clip, float volume, bool isLoop)
        {
            this.clip = clip;
            this.volume = volume;
            this.isLoop = isLoop;
        }
    }

    // 원본 사운드 리소스 리스트
    private Dictionary<int, ClipCache> _clipDic = new Dictionary<int, ClipCache>();
    // 사운드 컴포넌트 리스트
    private List<AudioSource> _sourceList = new List<AudioSource>();
    // 사운드 테이블 
    private List<SoundModel.Sound> _soundTable;

    private Coroutine _coroutine = null;
    private AudioSource _curBGM = null;
    private AudioSource _scenarioBGM = null;

    protected override void Init()
    {
        // TODO : 사운드 테이블 로드
        var sm = Model.First<SoundModel>();

        // TODO : 사운드 리소스 로드
        if (sm != null)
        {
            _soundTable = sm.soundTable;
            AudioClip clip = null;
            ClipCache cache = null;

            for (int i = 0; i < _soundTable.Count; i++)
            {
                clip = Resources.Load<AudioClip>(string.Format("Sound/{0}/{1}", _soundTable[i].type, _soundTable[i].name));
                cache = new ClipCache(clip, _soundTable[i].volumm, _soundTable[i].isLoop);
                _clipDic.Add(_soundTable[i].index, cache);
            }
        }
        else
        {
            Logger.LogErrorFormat("사운드 테이블 로드 실패");
        }

        Message.AddListener<PlaySoundMsg>(OnPlaySound);
        Message.AddListener<StopSoundMsg>(OnStopSound);
        Message.AddListener<StopAllSoundMsg>(OnStopAllSound);
    }

    protected override void Release()
    {
        base.Release();

        Message.RemoveListener<PlaySoundMsg>(OnPlaySound);
        Message.RemoveListener<StopSoundMsg>(OnStopSound);
        Message.RemoveListener<StopAllSoundMsg>(OnStopAllSound);
    }

    private void OnPlaySound(PlaySoundMsg msg)
    {
        var source = _sourceList.Find(e => e.isPlaying == false && e.clip.name != "Scene");

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.Stop();
            _sourceList.Add(source);
        }

        ClipCache cache;
        if (_clipDic.TryGetValue((int)msg.soundName, out cache))
        {
            source.clip = cache.clip;
            source.volume = cache.volume;
            source.loop = cache.isLoop;
            source.Play();

            // 현재의 BGM이다.
            if (cache.isLoop == true && cache.clip.name != "Scene")
            {
                _curBGM = source;
            }
        }
        else
        {
            Logger.LogErrorFormat("{0} 사운드가 없습니다.", msg.soundName);
        }
    }

    private void OnStopSound(StopSoundMsg msg)
    {
        ClipCache cache;
        if (_clipDic.TryGetValue((int)msg.soundName, out cache))
        {
            var source = _sourceList.Find(e => e.clip == cache.clip);
            if (source != null && source.isPlaying)
                source.Stop();
        }
        else
        {
            Logger.LogErrorFormat("{0} 사운드가 없습니다.", msg.soundName);
        }
    }

    public void PlayAttackSound(bool isSword)
    {
        var source = _sourceList.Find(e => e.isPlaying == false && e.clip.name != "Scene");

        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.Stop();
            _sourceList.Add(source);
        }

        int sidx = 0;
        if (isSword)
            sidx = Random.Range(4, 7);
        else
            sidx = Random.Range(7, 10);

        ClipCache cache;
        if (_clipDic.TryGetValue(sidx, out cache))
        {
            source.clip = cache.clip;
            source.volume = cache.volume;
            source.loop = cache.isLoop;
            source.Play();
        }
    }

    public void PlayScenarioBGM(bool isScenario)
    {
        // if (_scenarioBGM == null)
        // {
        //     _scenarioBGM = gameObject.AddComponent<AudioSource>();
        //     _scenarioBGM.playOnAwake = false;
        //     _scenarioBGM.Stop();
        //     _sourceList.Add(_scenarioBGM);

        //     ClipCache cache;
        //     if (_clipDic.TryGetValue((int)SoundName.Scene, out cache))
        //     {
        //         _scenarioBGM.clip = cache.clip;
        //         _scenarioBGM.volume = cache.volume;
        //         _scenarioBGM.loop = cache.isLoop;
        //         _scenarioBGM.Stop();
        //     }
        // }

        // if (isScenario == true)
        // {
        //     Logger.LogWarning("시나리오 BGM 재생");

        //     if (_coroutine != null)
        //     {
        //         StopCoroutine(_coroutine);
        //         _coroutine = null;
        //     }

        //     // 시나리오를 재생
        //     _coroutine = StartCoroutine(coFadeBGM(_scenarioBGM, _curBGM));
        // }
        // else
        // {
        //     Logger.LogWarning("기본 BGM 재생");

        //     if (_coroutine != null)
        //     {
        //         StopCoroutine(_coroutine);
        //         _coroutine = null;
        //     }

        //     // 메인을 재생
        //     _coroutine = StartCoroutine(coFadeBGM(_curBGM, _scenarioBGM));
        // }
    }

    private IEnumerator coFadeBGM(AudioSource play, AudioSource stop)
    {
        float volumeA = play.volume;
        float volumeB = stop.volume;
        float t = volumeA;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;

            play.volume = Mathf.Lerp(volumeA, 1f, t);
            stop.volume = Mathf.Lerp(volumeB, 0f, t);

            yield return null;
        }

        play.volume = 1f;
        stop.volume = 0f;

        if (play.isPlaying == false)
            play.Play();
        stop.Stop();
    }

    private void OnStopAllSound(StopAllSoundMsg msg)
    {
        foreach (AudioSource p in _sourceList)
        {
            if (p != null && p.isPlaying)
            {
                p.Stop();
            }
        }
    }
}