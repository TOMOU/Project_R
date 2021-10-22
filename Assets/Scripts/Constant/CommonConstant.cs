namespace Constant
{
    public enum GameScene
    {
        None,
        Title,
        Tutorial,
        Lobby,
        Battle,
        Defence,
    }

    public enum BehaviourType
    {
        Manager,
        Logic,
        Scene,
        Content,
        UIDialog,
        GlobalDialog,
    }

    public enum UISibling
    {
        // ==============================
        // TitleScene
        // ==============================
        TitleMainDialog,

        // ==============================
        // TutorialScene
        // ==============================
        TutorialNicknameDialog,

        // ==============================
        // LobbyScene
        // ==============================
        LobbyMainDialog,
        LobbyCharacterDialog,
        LobbyGachaDialog,
        LobbyContentDialog,
        LobbyMissionDialog,
        LobbyMainChapDialog,
        LobbyStoryDialog,
        LobbyDimensionDialog,
        LobbyEquipDialog,
        LobbyBabelDialog,
        LobbyArenaDialog,
        LobbyBossDialog,
        LobbyStageInfoDialog,
        LobbyFormationDialog,

        // ==============================
        // BattleNormalScene
        // ==============================
        NormalMainDialog,
        NormalResultDialog,
        NormalPauseDialog,

        // ==============================
        // BattleRaidScene
        // ==============================
        RaidMainDialog,
        RaidResultDialog,
        RaidPauseDialog,

        // ==============================
        // BattleDefenceScene
        // ==============================
        DefenceMainDialog,
        DefencePauseDialog,
        DefenceResultDialog,

        // ==============================
        // Global
        // ==============================
        GlobalRewardDialog = 1001,
        GlobalGuideDialog,
        GlobalScenarioDialog,
        GlobalLocalizeDialog,
        GlobalMessageDialog,
        GlobalLoadingDialog,
        GlobalDebugDialog,
        Max
    }

    /// <summary>
    /// Logger 클래스에서 어느 레벨까지 출력할 건지에 대한 지표
    /// </summary>
    public enum LogLevel
    {
        All,        // 모든 로그를 출력한다.
        Warning,    // Warning 이상만 출력한다.
        Error,      // Error만 출력한다.
        Nothing,    // 아무것도 출력하지 않는다.
    }

    public enum TutorialCallbackType
    {
        None,
        MainChapter,
        Dimension,
        Boss,
        Babel,
        Arena,
        ContentMap,
        Story,
    }

    public delegate void OnComplete();

    public enum ImageFilterMode : int
    {
        Nearest = 0,
        Biliner = 1,
        Average = 2
    }

    public enum Locale
    {
        Korean,
        English,
        Japanese,
        France,
        German,
        ChineseSimplified,
        ChineseTraditional,
    }
}