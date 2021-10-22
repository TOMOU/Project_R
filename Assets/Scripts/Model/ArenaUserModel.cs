using System.Collections.Generic;

public class ArenaUserModel : Model
{
    public class Data
    {
        public int idx { get; private set; }
        public int level { get; private set; }
        public string userName { get; private set; }
        public int grade { get; private set; }
        public int arenaPoint { get; private set; }
        public string guildName { get; private set; }
        public uint unit_id_1 { get; private set; }
        public int unit_lv_1 { get; private set; }
        public int unit_g_1 { get; private set; }
        public int unit_pos_1 { get; private set; }
        public uint unit_id_2 { get; private set; }
        public int unit_lv_2 { get; private set; }
        public int unit_g_2 { get; private set; }
        public int unit_pos_2 { get; private set; }
        public uint unit_id_3 { get; private set; }
        public int unit_lv_3 { get; private set; }
        public int unit_g_3 { get; private set; }
        public int unit_pos_3 { get; private set; }
        public uint unit_id_4 { get; private set; }
        public int unit_lv_4 { get; private set; }
        public int unit_g_4 { get; private set; }
        public int unit_pos_4 { get; private set; }
        public uint unit_id_5 { get; private set; }
        public int unit_lv_5 { get; private set; }
        public int unit_g_5 { get; private set; }
        public int unit_pos_5 { get; private set; }
        public Data(int idx, int level, string userName, int grade, int arenaPoint, string guildName, uint unit_id_1, int unit_lv_1, int unit_g_1, int unit_pos_1, uint unit_id_2, int unit_lv_2, int unit_g_2, int unit_pos_2, uint unit_id_3, int unit_lv_3, int unit_g_3, int unit_pos_3, uint unit_id_4, int unit_lv_4, int unit_g_4, int unit_pos_4, uint unit_id_5, int unit_lv_5, int unit_g_5, int unit_pos_5)
        {
            this.idx = idx;
            this.level = level;
            this.userName = userName;
            this.grade = grade;
            this.arenaPoint = arenaPoint;
            this.guildName = guildName;
            this.unit_id_1 = unit_id_1;
            this.unit_lv_1 = unit_lv_1;
            this.unit_g_1 = unit_g_1;
            this.unit_pos_1 = unit_pos_1;
            this.unit_id_2 = unit_id_2;
            this.unit_lv_2 = unit_lv_2;
            this.unit_g_2 = unit_g_2;
            this.unit_pos_2 = unit_pos_2;
            this.unit_id_3 = unit_id_3;
            this.unit_lv_3 = unit_lv_3;
            this.unit_g_3 = unit_g_3;
            this.unit_pos_3 = unit_pos_3;
            this.unit_id_4 = unit_id_4;
            this.unit_lv_4 = unit_lv_4;
            this.unit_g_4 = unit_g_4;
            this.unit_pos_4 = unit_pos_4;
            this.unit_id_5 = unit_id_5;
            this.unit_lv_5 = unit_lv_5;
            this.unit_g_5 = unit_g_5;
            this.unit_pos_5 = unit_pos_5;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/table_ArenaUser");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);
            
            idx = 0;
            
            data = new Data(row.GetInt(idx++), row.GetInt(idx++), row.GetString(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetString(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetUInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));
            
            _list.Add(data);
        }
    }
}
