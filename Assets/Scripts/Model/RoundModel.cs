using System.Collections.Generic;

public class RoundModel : Model
{
    public class Data
    {
        public int index { get; private set; }
        public int stage_id { get; private set; }
        public uint mob_id_11 { get; private set; }
        public int mob_lv_11 { get; private set; }
        public uint mob_id_12 { get; private set; }
        public int mob_lv_12 { get; private set; }
        public uint mob_id_13 { get; private set; }
        public int mob_lv_13 { get; private set; }
        public uint mob_id_21 { get; private set; }
        public int mob_lv_21 { get; private set; }
        public uint mob_id_22 { get; private set; }
        public int mob_lv_22 { get; private set; }
        public uint mob_id_23 { get; private set; }
        public int mob_lv_23 { get; private set; }
        public uint mob_id_31 { get; private set; }
        public int mob_lv_31 { get; private set; }
        public uint mob_id_32 { get; private set; }
        public int mob_lv_32 { get; private set; }
        public uint mob_id_33 { get; private set; }
        public int mob_lv_33 { get; private set; }
        public int reward_id { get; private set; }
        public int reward_rate { get; private set; }
        public int reward_max { get; private set; }
        public Data(int index, int stage_id, uint mob_id_11, int mob_lv_11, uint mob_id_12, int mob_lv_12, uint mob_id_13, int mob_lv_13, uint mob_id_21, int mob_lv_21, uint mob_id_22, int mob_lv_22, uint mob_id_23, int mob_lv_23, uint mob_id_31, int mob_lv_31, uint mob_id_32, int mob_lv_32, uint mob_id_33, int mob_lv_33, int reward_id, int reward_rate, int reward_max)
        {
            this.index = index;
            this.stage_id = stage_id;
            this.mob_id_11 = mob_id_11;
            this.mob_lv_11 = mob_lv_11;
            this.mob_id_12 = mob_id_12;
            this.mob_lv_12 = mob_lv_12;
            this.mob_id_13 = mob_id_13;
            this.mob_lv_13 = mob_lv_13;
            this.mob_id_21 = mob_id_21;
            this.mob_lv_21 = mob_lv_21;
            this.mob_id_22 = mob_id_22;
            this.mob_lv_22 = mob_lv_22;
            this.mob_id_23 = mob_id_23;
            this.mob_lv_23 = mob_lv_23;
            this.mob_id_31 = mob_id_31;
            this.mob_lv_31 = mob_lv_31;
            this.mob_id_32 = mob_id_32;
            this.mob_lv_32 = mob_lv_32;
            this.mob_id_33 = mob_id_33;
            this.mob_lv_33 = mob_lv_33;
            this.reward_id = reward_id;
            this.reward_rate = reward_rate;
            this.reward_max = reward_max;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/table_Round");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);
            
            idx = 0;
            
            data = new Data(row.GetInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));
            
            _list.Add(data);
        }
    }
}
