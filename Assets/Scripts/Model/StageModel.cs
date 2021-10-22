using System.Collections.Generic;

public class StageModel : Model
{
    public class Data
    {
        public int index { get; private set; }
        public int type { get; private set; }
        public int cha_id { get; private set; }
        public int grade { get; private set; }
        public int chapter { get; private set; }
        public int stage { get; private set; }
        public int stage_name { get; private set; }
        public int round_count { get; private set; }
        public int round_id_1 { get; private set; }
        public int round_id_2 { get; private set; }
        public int round_id_3 { get; private set; }
        public int reward_id { get; private set; }
        public int reward_id_first { get; private set; }
        public Data(int index, int type, int cha_id, int grade, int chapter, int stage, int stage_name, int round_count, int round_id_1, int round_id_2, int round_id_3, int reward_id, int reward_id_first)
        {
            this.index = index;
            this.type = type;
            this.cha_id = cha_id;
            this.grade = grade;
            this.chapter = chapter;
            this.stage = stage;
            this.stage_name = stage_name;
            this.round_count = round_count;
            this.round_id_1 = round_id_1;
            this.round_id_2 = round_id_2;
            this.round_id_3 = round_id_3;
            this.reward_id = reward_id;
            this.reward_id_first = reward_id_first;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/table_Stage");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);
            
            idx = 0;
            
            data = new Data(row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));
            
            _list.Add(data);
        }
    }
}
