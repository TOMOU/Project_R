using System.Collections.Generic;

public class StageInfoModel : Model
{
    public class StageInfo
    {
        public int chapter { get; private set; }
        public int stage { get; private set; }
        public Constant.StageType stageType { get; private set; }
        public Constant.MapName mapName { get; private set; }
        public int reqStamina { get; private set; }
        public int exp { get; private set; }
        public int coinMin { get; private set; }
        public int coinMax { get; private set; }
        public int wave1 { get; private set; }
        public int wave2 { get; private set; }
        public int wave3 { get; private set; }
        public StageInfo (int chapter, int stage, Constant.StageType stageType, Constant.MapName mapName, int reqStamina, int exp, int coinMin, int coinMax, int wave1, int wave2, int wave3)
        {
            this.chapter = chapter;
            this.stage = stage;
            this.stageType = stageType;
            this.mapName = mapName;
            this.reqStamina = reqStamina;
            this.exp = exp;
            this.coinMin = coinMin;
            this.coinMax = coinMax;
            this.wave1 = wave1;
            this.wave2 = wave2;
            this.wave3 = wave3;
        }
    }
    private List<StageInfo> _stageinfoTableList = new List<StageInfo> ();
    public List<StageInfo> stageinfoTable { get { return _stageinfoTableList; } }
    public void Setup ()
    {
        CSVReader reader = CSVReader.Load ("Table/StageInfoTable");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        StageInfo stageinfo = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow (i);
            
            idx = 0;
            
            stageinfo = new StageInfo (row.GetInt(idx++), row.GetInt(idx++), (Constant.StageType)System.Enum.Parse(typeof(Constant.StageType), row.GetString(idx++)), (Constant.MapName)System.Enum.Parse(typeof(Constant.MapName), row.GetString(idx++)), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));
            
            _stageinfoTableList.Add (stageinfo);
        }
    }
}
