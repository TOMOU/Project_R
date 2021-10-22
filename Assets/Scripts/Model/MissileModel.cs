using System.Collections.Generic;

public class MissileModel : Model
{
    public class Data
    {
        public int ID { get; private set; }
        public string EffectName { get; private set; }
        public float MoveSpeeed { get; private set; }
        public float ActiveTime { get; private set; }
        public Data(int ID, string EffectName, float MoveSpeeed, float ActiveTime)
        {
            this.ID = ID;
            this.EffectName = EffectName;
            this.MoveSpeeed = MoveSpeeed;
            this.ActiveTime = ActiveTime;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/table_Missile");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);
            
            idx = 0;
            
            data = new Data(row.GetInt(idx++), row.GetString(idx++), row.GetFloat(idx++), row.GetFloat(idx++));
            
            _list.Add(data);
        }
    }
}
