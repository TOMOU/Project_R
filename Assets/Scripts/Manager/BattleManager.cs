using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoSingleton<BattleManager>
{
    /// <summary>
    /// 어느 타입의 전투인지(임시)
    /// <para>0: None</para>
    /// <para>1: 메인스토리</para>
    /// <para>2: 대신전</para>
    /// <para>3: 차원던전</para>
    /// <para>4: 보스전</para>
    /// <para>5: 바벨타워</para>
    /// <para>6: 콜로세움</para>
    /// </summary>
    public int battleType;
    public int selectCharacter;
    public int selectChapter;
    public int selectStage;
    public bool inLobby = false;

    public bool AutoSkill { get; private set; }
    private List<IUnitInfo> _teamBlueList;
    private List<IUnitInfo> _teamRedList;
    private List<IUnitInfo> _tempList;

    protected override void Init()
    {
        AutoSkill = PlayerPrefs.GetInt("enabledAutoSkill", 0) == 0 ? false : true;

        if (_teamBlueList == null)
            _teamBlueList = new List<IUnitInfo>();
        else
            _teamBlueList.Clear();

        if (_teamRedList == null)
            _teamRedList = new List<IUnitInfo>();
        else
            _teamRedList.Clear();

        if (_tempList == null)
            _tempList = new List<IUnitInfo>();
        else
            _tempList.Clear();

        // LoadData();

        Message.AddListener<Global.EnableAutoModeMsg>(OnEnableAutoMode);
        Message.AddListener<Global.DisableAutoModeMsg>(OnDisableAutoMode);
    }

    protected override void Release()
    {
        PlayerPrefs.SetInt("enabledAutoSkill", AutoSkill ? 1 : 0);

        if (_teamBlueList != null)
        {
            _teamBlueList.Clear();
            _teamBlueList = null;
        }

        if (_teamRedList != null)
        {
            _teamRedList.Clear();
            _teamRedList = null;
        }

        if (_tempList != null)
        {
            _tempList.Clear();
            _tempList = null;
        }

        Message.RemoveListener<Global.EnableAutoModeMsg>(OnEnableAutoMode);
        Message.RemoveListener<Global.DisableAutoModeMsg>(OnDisableAutoMode);
    }

    private void OnEnableAutoMode(Global.EnableAutoModeMsg msg)
    {
        AutoSkill = true;
    }

    private void OnDisableAutoMode(Global.DisableAutoModeMsg msg)
    {
        AutoSkill = false;
    }

    public List<T> GetBlueTeam<T>() where T : IUnitInfo
    {
        return _teamBlueList.Cast<T>().ToList();
    }

    public List<T> GetRedTeam<T>() where T : IUnitInfo
    {
        return _teamRedList.Cast<T>().ToList();
    }

    public List<T> GetSummonTeam<T>() where T : IUnitInfo
    {
        return _tempList.Cast<T>().ToList();
    }

    public int GetAliveUnitCount(Constant.Team team)
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).ToList().FindAll(e => e.isDie == false);
        return list.Count;
    }

    //todo 팀에 유닛을 추가한다.
    public void AddUnit(Constant.Team team, IUnitInfo info)
    {
        if (team == Constant.Team.Blue)
            _teamBlueList.Add(info);
        else if (team == Constant.Team.Red)
            _teamRedList.Add(info);
        else
            _tempList.Add(info);
    }

    //todo 팀의 유닛을 제거한다.
    public void RemoveUnit(Constant.Team team, IUnitInfo info)
    {
        if (team == Constant.Team.Blue)
            _teamBlueList.Remove(info);
        else
            _teamRedList.Remove(info);
    }

    public void RemoveTeamUnit(Constant.Team team)
    {
        if (team == Constant.Team.Blue)
            _teamBlueList.Clear();
        else
            _teamRedList.Clear();
    }

    //todo 팀에있는 모든 유닛을 제거한다.
    public void RemoveAllUnit()
    {
        _teamBlueList.ForEach(e => e.Release());
        _teamBlueList.Clear();
        _teamRedList.ForEach(e => e.Release());
        _teamRedList.Clear();
        _tempList.ForEach(e => e.Release());
        _tempList.Clear();
    }

    public IUnitInfo FindUnit(int targetTeam, Vector3 myPos)
    {
        IUnitInfo info = null;
        List<IUnitInfo> lst = null;

        // 타겟 팀에 따라 리스트를 가져온다.
        if (targetTeam == 0)
            lst = _teamBlueList;
        else
            lst = _teamRedList;

        // 리스트 중 가장 가까운 녀석을 뽑는다.
        float distance = 100f;
        for (int i = 0; i < lst.Count; i++)
        {
            if (lst[i].Status.hp <= 0)
                continue;

            if (Mathf.Abs(lst[i].CurrentPosition.x - myPos.x) < distance)
            {
                info = lst[i];
                distance = Mathf.Abs(lst[i].CurrentPosition.x - myPos.x);
            }
        }

        return info;
    }

    public bool CheckUnitIdle()
    {
        foreach (var p in _teamBlueList)
        {
            if ((Constant.UnitState)p.FSM.GetState() != Constant.UnitState.Idle)
                return false;
        }

        foreach (var p in _teamRedList)
        {
            if ((Constant.UnitState)p.FSM.GetState() != Constant.UnitState.Idle)
                return false;
        }

        return true;
    }

    #region 조건에 따른 유닛리스트 검색
    public List<T> FindUnitByHorizontal<T>(Constant.Team team, Vector3 myPos, bool flip) where T : IUnitInfo
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false &&
        myPos.y - 1f < e.CurrentPosition.y &&       // 자기보다 1f 아래 이내
        e.CurrentPosition.y <= myPos.y + 1f &&      // 자기보다 1f 위 이내
        (flip == true ? myPos.x >= e.CurrentPosition.x : myPos.x <= e.CurrentPosition.x)           // 방향에 따라
        );

        return list.Cast<T>().ToList();
    }

    public List<T> FindUnitByHorizontal_Limit<T>(Constant.Team team, Vector3 myPos, float width, bool flip) where T : IUnitInfo
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false &&
        Mathf.Abs(myPos.x - e.CurrentPosition.x) < width &&        // 나와의 x거리가 width 이내
        myPos.y - 1f < e.CurrentPosition.y &&       // 자기보다 1f 아래 이내
        e.CurrentPosition.y <= myPos.y + 1f &&      // 자기보다 1f 위 이내
        (flip == true ? myPos.x >= e.CurrentPosition.x : myPos.x <= e.CurrentPosition.x)           // 방향에 따라
        );

        return list.Cast<T>().ToList();
    }

    public List<T> FindUnitByArea<T>(Constant.Team team, Vector3 targetPos, float range)
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false &&
        Vector3.Distance(e.CurrentPosition, targetPos) <= range);

        return list.Cast<T>().ToList();
    }

    public List<T> FindUnitByArea_Square<T>(Constant.Team team, Vector3 center, Vector2 squareSize)
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false &&
        Mathf.Abs(e.CurrentPosition.x - center.x) <= squareSize.x / 2 &&
        Mathf.Abs(e.CurrentPosition.y - center.y) <= squareSize.y / 2
        );

        return list.Cast<T>().ToList();
    }

    // 해당 팀의 모든 유닛을 검색.
    private List<T> FindUnitByAll<T>(Constant.Team team) where T : IUnitInfo
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false);
        return list.Cast<T>().ToList();
    }

    // 가까운 타겟을 기준으로 범위로 Search
    private List<T> FindUnitByArea<T>(IUnitInfo target, Constant.Team team) where T : IUnitInfo
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false);
        var value = list.FindAll(e => Mathf.Abs(target.CurrentPosition.x - e.CurrentPosition.x) < 3f);

        return value.Cast<T>().ToList();
    }

    // my에서 가까운 순서 중 startIndex 부터 count 개수만큼의 유닛정보 검색.
    private List<T> FindUnitByOrder<T>(IUnitInfo my, Constant.Team team, int startIndex, int count) where T : IUnitInfo
    {
        List<IUnitInfo> list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false);
        var lst = list.ToList();

        lst.Sort((a, b) => SortUnit(my, a, b));

        if (lst == null || lst.Count == 0)
            return null;

        return lst.GetRange(startIndex, count).Cast<T>().ToList();
    }

    // my에서 제일 가까운 캐릭터를 검색.
    public T FindUnitByDistance_X<T>(IUnitInfo my, Constant.Team team, int findIndex = 0) where T : IUnitInfo
    {
        List<IUnitInfo> list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false);
        var lst = list.ToList();

        lst.Sort((a, b) => SortUnit(my, a, b));

        if (lst == null || lst.Count == 0)
            return null;

        if (findIndex > lst.Count - 1)
            return lst[lst.Count - 1] as T;

        return lst[findIndex] as T;
    }

    public T FindUnitByDistance_XY<T>(IUnitInfo my, Constant.Team team, int findIndex = 0) where T : IUnitInfo
    {
        List<IUnitInfo> list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false);
        var lst = list.ToList();

        lst.Sort((a, b) => SortUnit(my, a, b));

        if (lst == null || lst.Count == 0)
            return null;

        if (findIndex > lst.Count - 1)
            return lst[lst.Count - 1] as T;

        return lst[findIndex] as T;
    }

    // 해당 팀에서 체력이 가장 낮은 캐릭터를 검색.
    private T FindUnitByHP<T>(Constant.Team team) where T : IUnitInfo
    {
        var list = (team == Constant.Team.Blue ? _teamBlueList : _teamRedList).FindAll(e => e.isDie == false);

        if (list == null || list.Count == 0)
            return null;

        // 리스트 중 가장 가까운 녀석을 뽑는다.
        int hp = list[0].Status.hp;
        IUnitInfo value = list[0];

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Status.hp < hp)
            {
                value = list[i];
                hp = list[i].Status.hp;
            }
        }
        return value as T;
    }
    #endregion

    // 내 캐릭터, 스킬, (UnitInfo를 담을)targets를 통해 스킬의 i번째 효과가 적용되는 타겟정보를 얻는다.
    public List<T> GetSkillTarget<T>(T my, SkillModel.Skill skill, List<T> targets, int i) where T : IUnitInfo
    {
        List<T> list = new List<T>();
        if (targets == null)
            targets = new List<T>();
        targets.Clear();

        Constant.Team team = my.Team;

        if (skill.teams[i] == Constant.SkillTeam.Ally)
            team = my.Team == Constant.Team.Blue ? Constant.Team.Blue : Constant.Team.Red;
        else
            team = my.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue;

        switch (skill.targets[i])
        {
            case Constant.SkillTarget.None:
                break;
            case Constant.SkillTarget.All:
                list.AddRange(FindUnitByAll<T>(team));
                break;
            case Constant.SkillTarget.Area:
                list.AddRange(FindUnitByArea<T>(FindUnitByDistance_X<T>(my, team), team));
                break;
            case Constant.SkillTarget.One:
                list.Add(FindUnitByDistance_X<T>(my, team));
                break;
            case Constant.SkillTarget.One_Second:
                list.Add(FindUnitByDistance_X<T>(my, team, 1));
                break;
            case Constant.SkillTarget.One_Third:
                list.Add(FindUnitByDistance_X<T>(my, team, 2));
                break;
            case Constant.SkillTarget.One_Fourth:
                list.Add(FindUnitByDistance_X<T>(my, team, 3));
                break;
            case Constant.SkillTarget.One_Fifth:
                list.Add(FindUnitByDistance_X<T>(my, team, 4));
                break;
            case Constant.SkillTarget.Me:
                list.Add(my);
                break;
            case Constant.SkillTarget.HP:
                list.Add(FindUnitByHP<T>(team));
                break;
        }

        // target에 추출한 캐릭터 추가
        foreach (var p in list)
        {
            if (targets.Contains(p) == false)
                targets.Add(p);
        }

        return targets.Cast<T>().ToList();
    }

    public List<T> GetSkillTargetAll<T>(T my, SkillModel.Skill skill, List<T> targets) where T : IUnitInfo
    {
        List<T> list = new List<T>();
        if (targets == null)
            targets = new List<T>();
        targets.Clear();

        Constant.Team team = my.Team;

        for (int i = 0; i < skill.types.Count; i++)
        {
            if (skill.teams[i] == Constant.SkillTeam.Ally)
                team = my.Team == Constant.Team.Blue ? Constant.Team.Blue : Constant.Team.Red;
            else
                team = my.Team == Constant.Team.Blue ? Constant.Team.Red : Constant.Team.Blue;

            switch (skill.targets[i])
            {
                case Constant.SkillTarget.None:
                    break;
                case Constant.SkillTarget.All:
                    list.AddRange(FindUnitByAll<T>(team));
                    break;
                case Constant.SkillTarget.Area:
                    list.AddRange(FindUnitByArea<T>(FindUnitByDistance_X<T>(my, team), team));
                    break;
                case Constant.SkillTarget.One:
                    list.Add(FindUnitByDistance_X<T>(my, team));
                    break;
                case Constant.SkillTarget.One_Second:
                    list.Add(FindUnitByDistance_X<T>(my, team, 1));
                    break;
                case Constant.SkillTarget.Me:
                    list.Add(my);
                    break;
                case Constant.SkillTarget.HP:
                    break;
            }
        }

        // target에 추출한 캐릭터 추가
        foreach (var p in list)
        {
            if (targets.Contains(p) == false)
                targets.Add(p);
        }

        return targets as List<T>;
    }

    public int SortUnit(IUnitInfo my, IUnitInfo a, IUnitInfo b)
    {
        int ret = 0;

        if (Vector3.Distance(a.transform.localPosition, my.transform.localPosition) < Vector3.Distance(b.transform.localPosition, my.transform.localPosition))
            return -1;
        else if (Vector3.Distance(a.transform.localPosition, my.transform.localPosition) > Vector3.Distance(b.transform.localPosition, my.transform.localPosition))
            return 1;
        else if (a.Status.hp < b.Status.hp)
            return -1;
        else if (a.Status.hp > b.Status.hp)
            return 1;
        else if (a.Status.pDef < b.Status.pDef)
            return -1;
        else if (a.Status.pDef > b.Status.pDef)
            return 1;
        else if (a.Status.damage > b.Status.damage)
            return -1;
        else if (a.Status.damage < b.Status.damage)
            return 1;

        return ret;
    }

    public bool IsNewStage()
    {
        bool result = false;

        switch (battleType)
        {
            case 1:
                if ((selectChapter - 1) * 4 + selectStage > Info.My.Singleton.User.maxClearedMainstory)
                    result = true;
                break;

            case 2:
                if ((selectChapter - 1) * 4 + selectStage > Info.My.Singleton.User.maxClearedStory)
                    result = true;
                break;

            case 3:
                if (selectStage > Info.My.Singleton.User.maxClearedDimension)
                    result = true;
                break;

            case 5:
                if (selectStage > Info.My.Singleton.User.maxClearedBabel)
                    result = true;
                break;
        }

        return result;
    }
}