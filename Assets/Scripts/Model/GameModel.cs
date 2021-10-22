using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : Model
{
    private ModelRef<ArenaUserModel> _arenaUserModel = new ModelRef<ArenaUserModel> ();
    private ModelRef<ChapterModel> _chapterModel = new ModelRef<ChapterModel> ();
    private ModelRef<CharExpModel> _charExpModel = new ModelRef<CharExpModel> ();
    private ModelRef<CharNameModel> _charNameModel = new ModelRef<CharNameModel> ();
    private ModelRef<ItemModel> _itemModel = new ModelRef<ItemModel> ();
    private ModelRef<LocalizeModel> _localizeModel = new ModelRef<LocalizeModel> ();
    private ModelRef<MissileModel> _missileModel = new ModelRef<MissileModel> ();
    private ModelRef<RewardModel> _rewardModel = new ModelRef<RewardModel> ();
    private ModelRef<RoundModel> _roundModel = new ModelRef<RoundModel> ();
    private ModelRef<ScenarioModel> _scenarioModel = new ModelRef<ScenarioModel> ();
    private ModelRef<SkillModel> _skillModel = new ModelRef<SkillModel> ();
    private ModelRef<SoundModel> _soundModel = new ModelRef<SoundModel> ();
    private ModelRef<StageInfoModel> _stageInfoModel = new ModelRef<StageInfoModel> ();
    private ModelRef<StageModel> _stageModel = new ModelRef<StageModel> ();
    private ModelRef<UnitModel> _unitModel = new ModelRef<UnitModel> ();
    public bool loadCompleteGlobalContent = false;

    public void Setup ()
    {
        // ArenaUserModel 로드
        _arenaUserModel.Model = new ArenaUserModel ();
        _arenaUserModel.Model.Setup ();

        // ChapterModel 로드
        _chapterModel.Model = new ChapterModel ();
        _chapterModel.Model.Setup ();

        // CharExpModel 로드
        _charExpModel.Model = new CharExpModel ();
        _charExpModel.Model.Setup ();

        // CharNameModel 로드
        _charNameModel.Model = new CharNameModel ();
        _charNameModel.Model.Setup ();

        // ItemModel 로드
        _itemModel.Model = new ItemModel ();
        _itemModel.Model.Setup ();

        // LocalizeModel 로드
        _localizeModel.Model = new LocalizeModel ();
        _localizeModel.Model.Setup ();

        // MissileModel 로드
        _missileModel.Model = new MissileModel ();
        _missileModel.Model.Setup ();

        // RewardModel 로드
        _rewardModel.Model = new RewardModel ();
        _rewardModel.Model.Setup ();

        // RoundModel 로드
        _roundModel.Model = new RoundModel ();
        _roundModel.Model.Setup ();

        // ScenarioModel 로드
        _scenarioModel.Model = new ScenarioModel ();
        _scenarioModel.Model.Setup ();

        // SkillModel 로드
        _skillModel.Model = new SkillModel ();
        _skillModel.Model.Setup ();

        // SoundModel 로드
        _soundModel.Model = new SoundModel ();
        _soundModel.Model.Setup ();

        // StageInfoModel 로드
        _stageInfoModel.Model = new StageInfoModel ();
        _stageInfoModel.Model.Setup ();

        // StageModel 로드
        _stageModel.Model = new StageModel ();
        _stageModel.Model.Setup ();

        // UnitModel 로드
        _unitModel.Model = new UnitModel ();
        _unitModel.Model.Setup ();
    }
}
