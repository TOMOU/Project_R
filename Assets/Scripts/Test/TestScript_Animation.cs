using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class TestScript_Animation : MonoBehaviour
{
    private List<SkeletonAnimation> _skelList;
    private int _skelIdx = 0;
    private SkeletonAnimation _skel;
    public List<Texture2D> texture2Ds;
    private Texture2D _curTex;
    private Renderer _curRen;

    private int _skinIndex = 0;
    [SerializeField] private int _skinMax = 0;

    private void Start()
    {
        Application.targetFrameRate = 60;

        GameModel gm = new GameModel();
        gm.Setup();

        SpineManager.Singleton.Setup();

        // 하위에 있는 SkeletonAnimation을 List에 추가.
        _skelList = new List<SkeletonAnimation>();
        foreach (Transform child in transform)
        {
            SkeletonAnimation skel = child.GetComponent<SkeletonAnimation>();
            if (skel != null)
            {
                _skelList.Add(skel);
                skel.gameObject.SetActive(false);
            }
            else
                Logger.LogErrorFormat("{0} 오브젝트에 SkeletonAnimation 컴포넌트가 없습니다.", child.name);
        }

        // 사용가능한 SkeletonAnimation이 없을때를 대비한 예외처리.
        if (_skelList.Count == 0)
        {
            Logger.LogError("사용 가능한 SkeletonAnimation이 없습니다. 이 오브젝트의 하위에 SkeletonAnimation 컴포넌트를 가진 차일드를 추가해주세요.");
            return;
        }

        ChangeModel(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale == 1f ? 0.1f : 1f;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Prev();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            Next();
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeSkinPrev();
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeSkinNext();
        else if (Input.GetKeyDown(KeyCode.Alpha1)) // 대기
            Idle();
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 이동
            Run();
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 공격
            Attack();
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 스킬0
            Hit();
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // 스킬0
            Skill0();
        else if (Input.GetKeyDown(KeyCode.Alpha6)) // 스킬1
            Skill1();
        else if (Input.GetKeyDown(KeyCode.Alpha7)) // 스킬2
            Skill2();
        else if (Input.GetKeyDown(KeyCode.Alpha8)) // 승리
            Victory();
        else if (Input.GetKeyDown(KeyCode.Alpha9)) // 사망
            Die();
    }

    private void Prev()
    {
        ChangeModel(-1);
    }

    private void Next()
    {
        ChangeModel(1);
    }

    private void ChangeSkinPrev()
    {
        if (_skinMax == 1)
            return;

        _skinIndex--;
        if (_skinIndex == 0)
            _skinIndex = _skinMax;

        ChangeSkin();
    }

    private void ChangeSkinNext()
    {
        if (_skinMax == 1)
            return;

        _skinIndex++;
        if (_skinIndex == _skinMax)
            _skinIndex = 1;

        ChangeSkin();
    }

    private void ChangeSkin()
    {
        Logger.LogFormat("curSkinIndex = {0}\nmaxSkinIndex = {1}", _skinIndex, _skinMax);
        _skel.skeleton.SetSkin(_skel.skeleton.Data.Skins.Items[_skinIndex]);
    }

    private void ChangeModel(int idx)
    {
        if (_skel != null)
        {
            _skel.gameObject.SetActive(false);
            _skel.UpdateComplete -= UpdateComplete;
        }

        _skelIdx += idx;
        if (_skelIdx < 0)
            _skelIdx = _skelList.Count - 1;
        else if (_skelIdx > _skelList.Count - 1)
            _skelIdx = 0;

        _skel = _skelList[_skelIdx];
        _skel.gameObject.SetActive(true);

        _skinMax = _skel.skeleton.Data.Skins.Count;
        if (_skinMax > 1)
        {
            _skinIndex = 1;
            _skel.skeleton.SetSkin(_skel.skeleton.Data.Skins.Items[_skinIndex]);
        }

        Idle();

        SpineManager.Singleton.spineTextureDic.TryGetValue(_skel.gameObject.name, out _curTex);
        _curRen = _skel.GetComponent<Renderer>();

        _skel.UpdateComplete += UpdateComplete;
    }

    void UpdateComplete(Spine.Unity.ISkeletonAnimation anim)
    {
        if (_curTex != null && _curRen.material.mainTexture != _curTex)
        {
            _curRen.material.mainTexture = _curTex;
        }
    }

    #region Animation Function
    //todo 대기 애니메이션을 재생한다.
    private void Idle()
    {
        // 대기 애니메이션 루프 재생
        _skel.AnimationState.SetAnimation(0, "idle", true);
    }

    //todo 달리기 애니메이션을 재생한다.
    private void Run()
    {
        // 이동 애니메이션 루프 재생
        _skel.AnimationState.SetAnimation(0, "run", true);
    }

    //todo 공격 애니메이션을 재생하고 대기 애니메이션을 큐에 추가한다.
    private void Attack()
    {
        // 공격 애니메이션 재생 후
        _skel.AnimationState.SetAnimation(0, "attack", false);

        // 대기 애니메이션으로 돌아온다 (애니메이션 큐에 추가)
        _skel.AnimationState.AddAnimation(0, "idle", true, 0f);
    }

    //todo 타격 애니메이션을 재생하고 대기 애니메이션을 큐에 추가.
    private void Hit()
    {
        _skel.AnimationState.SetAnimation(0, "hit", false);

        _skel.AnimationState.AddAnimation(0, "idle", true, 0f);
    }

    //todo 스킬0 애니메이션을 재생하고 대기 애니메이션을 큐에 추가한다.
    private void Skill0()
    {
        // 스킬 애니메이션 재생 후
        _skel.AnimationState.SetAnimation(0, "skill3", false);

        // 2단 연출이 있는 경우
        if (_skel.skeleton.Data.Animations.Find(e => e.Name == "skill3_2") != null)
            _skel.AnimationState.AddAnimation(0, "skill3_2", false, 0);

        // 대기 애니메이션으로 돌아온다 (애니메이션 큐에 추가)
        _skel.AnimationState.AddAnimation(0, "idle", true, 0f);

        if (_skel.name == "alexander")
        {
            StartCoroutine(coSkill());
        }
    }


    private IEnumerator coSkill()
    {
        yield return new WaitForSpineEvent(_skel, "start1");

        Vector3 p1 = _skel.transform.localPosition;
        Vector3 p2 = p1 + new Vector3(0, 20f, 20f);
        Vector3 p3 = new Vector3(-22f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-13f, 1.5f, 1.5f);

        float t = 0f;

        // 상단으로 이동
        while (t < 1f)
        {
            t += Time.deltaTime * 2.5f;
            _skel.transform.localPosition = Vector3.Lerp(p1, p2, t);
            yield return null;
        }

        // 화면 좌측에서 등장
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2.5f;
            _skel.transform.localPosition = Vector3.Lerp(p3, p4, t);
            yield return null;
        }

        yield return new WaitForSpineEvent(_skel, "start2");

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            _skel.transform.localPosition = Vector3.Lerp(p4, p1, t);
            yield return null;
        }

        yield return null;
    }

    //todo 스킬1 애니메이션을 재생하고 대기 애니메이션을 큐에 추가한다.
    private void Skill1()
    {
        // 스킬 애니메이션 재생 후
        _skel.AnimationState.SetAnimation(0, "skill1", false);

        // 대기 애니메이션으로 돌아온다 (애니메이션 큐에 추가)
        _skel.AnimationState.AddAnimation(0, "idle", true, 0f);
    }

    //todo 스킬2 애니메이션을 재생하고 대기 애니메이션을 큐에 추가한다.
    private void Skill2()
    {
        // 스킬 애니메이션 재생 후
        _skel.AnimationState.SetAnimation(0, "skill2", false);

        // 대기 애니메이션으로 돌아온다 (애니메이션 큐에 추가)
        _skel.AnimationState.AddAnimation(0, "idle", true, 0f);

        Vector3 p0 = _skel.transform.position;
        Vector3 p1 = _skel.transform.position + Vector3.up * 4.5f;
        Vector3 p2 = _skel.transform.position + Vector3.up * 4.5f + Vector3.right * 12f;
        Vector3 p3 = _skel.transform.position + Vector3.right * 12f;

        for (int i = 0; i < 10; i++)
        {
            Debug.DrawLine(p0 + Vector3.up * 0.5f * i, p3 + Vector3.up * 0.5f * i, Color.red, 1f);

            for (int j = 0; j < 26; j++)
            {
                Debug.DrawLine(p0 + Vector3.right * 0.5f * j, p1 + Vector3.right * 0.5f * j, Color.red, 1f);
            }
        }
    }

    //todo 승리 애니메이션을 재생한다.
    private void Victory()
    {
        // 승리 애니메이션 재생
        _skel.AnimationState.SetAnimation(0, "victory", false);
    }

    //todo 사망 애니메이션을 재생한다.
    private void Die()
    {
        // 사망 애니메이션 재생
        _skel.AnimationState.SetAnimation(0, "die", false);
        // Debug.LogFormat("{0}\n{1}", _skel.AnimationState.GetCurrent(0).TrackTime, _skel.AnimationState.GetCurrent(0).TrackEnd);
        // _skel.AnimationState.GetCurrent(0).TrackTime = _skel.AnimationState.GetCurrent(0).TrackEnd * 0.5f;

        // 2단 연출이 있는 경우
        if (_skel.skeleton.Data.Animations.Find(e => e.Name == "die2") != null)
            _skel.AnimationState.AddAnimation(0, "die2", true, 0);
    }
    #endregion

    private string _guiMessage = "[← →] 스파인 전환\n[Keypad]\n1: idle\n2: run\n3: attack\n4: hit\n5: skill3\n6: skill1\n7: skill2\n8: victory\n9: die";
    private float _deltaTime = 0f;
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(0, 0, 200, 100), _guiMessage, style);

        Spine.TrackEntry track = _skel.AnimationState.GetCurrent(0);
        GUI.Label(new Rect(0, Screen.height - 100, 200, 100), string.Format("{0:000}", (track.AnimationTime / track.Animation.Duration) * 100), style);

        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        style.fontSize = 40;
        style.normal.textColor = Color.red;

        GUI.Label(new Rect(0, 300, 200, 50), string.Format("{0:0.0}ms  {1:0.}fps", _deltaTime * 1000f, 1f / _deltaTime), style);
    }
}