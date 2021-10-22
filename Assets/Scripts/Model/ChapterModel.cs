using System.Collections.Generic;

public class ChapterModel : Model
{
    public class Data
    {
        public int index { get; private set; }
        public int type { get; private set; }
        public int cha_id { get; private set; }
        public int chapter { get; private set; }
        public int reward_count { get; private set; }
        public int star_value_1 { get; private set; }
        public int item_1_id { get; private set; }
        public int item_1_value { get; private set; }
        public int star_value_2 { get; private set; }
        public int item_2_id { get; private set; }
        public int item_2_value { get; private set; }
        public int star_value_3 { get; private set; }
        public int item_3_id { get; private set; }
        public int item_3_value { get; private set; }
        public int star_value_4 { get; private set; }
        public int item_4_id { get; private set; }
        public int item_4_value { get; private set; }
        public int star_value_5 { get; private set; }
        public int item_5_id { get; private set; }
        public int item_5_value { get; private set; }
        public Data(int index, int type, int cha_id, int chapter, int reward_count, int star_value_1, int item_1_id, int item_1_value, int star_value_2, int item_2_id, int item_2_value, int star_value_3, int item_3_id, int item_3_value, int star_value_4, int item_4_id, int item_4_value, int star_value_5, int item_5_id, int item_5_value)
        {
            this.index = index;
            this.type = type;
            this.cha_id = cha_id;
            this.chapter = chapter;
            this.reward_count = reward_count;
            this.star_value_1 = star_value_1;
            this.item_1_id = item_1_id;
            this.item_1_value = item_1_value;
            this.star_value_2 = star_value_2;
            this.item_2_id = item_2_id;
            this.item_2_value = item_2_value;
            this.star_value_3 = star_value_3;
            this.item_3_id = item_3_id;
            this.item_3_value = item_3_value;
            this.star_value_4 = star_value_4;
            this.item_4_id = item_4_id;
            this.item_4_value = item_4_value;
            this.star_value_5 = star_value_5;
            this.item_5_id = item_5_id;
            this.item_5_value = item_5_value;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/table_Chapter");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);

            idx = 0;

            data = new Data(row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));

            _list.Add(data);
        }
    }
}
