using System.Collections.Generic;

public class RewardModel : Model
{
    public class Data
    {
        public int index { get; private set; }
        public int reward_id { get; private set; }
        public int type { get; private set; }
        public int item_1_id { get; private set; }
        public int item_1_value { get; private set; }
        public float item_1_rate { get; private set; }
        public int item_2_id { get; private set; }
        public int item_2_value { get; private set; }
        public float item_2_rate { get; private set; }
        public int item_3_id { get; private set; }
        public int item_3_value { get; private set; }
        public float item_3_rate { get; private set; }
        public int item_4_id { get; private set; }
        public int item_4_value { get; private set; }
        public float item_4_rate { get; private set; }
        public int item_5_id { get; private set; }
        public int item_5_value { get; private set; }
        public float item_5_rate { get; private set; }
        public int item_6_id { get; private set; }
        public int item_6_value { get; private set; }
        public float item_6_rate { get; private set; }
        public int item_7_id { get; private set; }
        public int item_7_value { get; private set; }
        public float item_7_rate { get; private set; }
        public int item_8_id { get; private set; }
        public int item_8_value { get; private set; }
        public float item_8_rate { get; private set; }
        public int item_9_id { get; private set; }
        public int item_9_value { get; private set; }
        public float item_9_rate { get; private set; }
        public Data(int index, int reward_id, int type, int item_1_id, int item_1_value, float item_1_rate, int item_2_id, int item_2_value, float item_2_rate, int item_3_id, int item_3_value, float item_3_rate, int item_4_id, int item_4_value, float item_4_rate, int item_5_id, int item_5_value, float item_5_rate, int item_6_id, int item_6_value, float item_6_rate, int item_7_id, int item_7_value, float item_7_rate, int item_8_id, int item_8_value, float item_8_rate, int item_9_id, int item_9_value, float item_9_rate)
        {
            this.index = index;
            this.reward_id = reward_id;
            this.type = type;
            this.item_1_id = item_1_id;
            this.item_1_value = item_1_value;
            this.item_1_rate = item_1_rate;
            this.item_2_id = item_2_id;
            this.item_2_value = item_2_value;
            this.item_2_rate = item_2_rate;
            this.item_3_id = item_3_id;
            this.item_3_value = item_3_value;
            this.item_3_rate = item_3_rate;
            this.item_4_id = item_4_id;
            this.item_4_value = item_4_value;
            this.item_4_rate = item_4_rate;
            this.item_5_id = item_5_id;
            this.item_5_value = item_5_value;
            this.item_5_rate = item_5_rate;
            this.item_6_id = item_6_id;
            this.item_6_value = item_6_value;
            this.item_6_rate = item_6_rate;
            this.item_7_id = item_7_id;
            this.item_7_value = item_7_value;
            this.item_7_rate = item_7_rate;
            this.item_8_id = item_8_id;
            this.item_8_value = item_8_value;
            this.item_8_rate = item_8_rate;
            this.item_9_id = item_9_id;
            this.item_9_value = item_9_value;
            this.item_9_rate = item_9_rate;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/table_Reward");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);
            
            idx = 0;
            
            data = new Data(row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++));
            
            _list.Add(data);
        }
    }
}
