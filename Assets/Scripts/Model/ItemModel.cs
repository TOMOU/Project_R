using System.Collections.Generic;

public class ItemModel : Model
{
    public class Data
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public int index { get; private set; }
        public int equip { get; private set; }
        public int spec_1 { get; private set; }
        public int spec_2 { get; private set; }
        public int spec_3 { get; private set; }
        public int spec_4 { get; private set; }
        public int spec_5 { get; private set; }
        public int spec_6 { get; private set; }
        public int spec_7 { get; private set; }
        public int spec_8 { get; private set; }
        public int spec_9 { get; private set; }
        public int spec_10 { get; private set; }
        public int spec_11 { get; private set; }
        public int spec_12 { get; private set; }
        public int value1_type { get; private set; }
        public int value1 { get; private set; }
        public int value2_type { get; private set; }
        public int value2 { get; private set; }
        public int value3_type { get; private set; }
        public int value3 { get; private set; }
        public int value4_type { get; private set; }
        public int value4 { get; private set; }
        public int buy_type { get; private set; }
        public int buy_price { get; private set; }
        public int sell_type { get; private set; }
        public int sell_price { get; private set; }
        public int apart_type { get; private set; }
        public int apart_value { get; private set; }
        public int icon { get; private set; }
        public Data(int id, string name, int index, int equip, int spec_1, int spec_2, int spec_3, int spec_4, int spec_5, int spec_6, int spec_7, int spec_8, int spec_9, int spec_10, int spec_11, int spec_12, int value1_type, int value1, int value2_type, int value2, int value3_type, int value3, int value4_type, int value4, int buy_type, int buy_price, int sell_type, int sell_price, int apart_type, int apart_value, int icon)
        {
            this.id = id;
            this.name = name;
            this.index = index;
            this.equip = equip;
            this.spec_1 = spec_1;
            this.spec_2 = spec_2;
            this.spec_3 = spec_3;
            this.spec_4 = spec_4;
            this.spec_5 = spec_5;
            this.spec_6 = spec_6;
            this.spec_7 = spec_7;
            this.spec_8 = spec_8;
            this.spec_9 = spec_9;
            this.spec_10 = spec_10;
            this.spec_11 = spec_11;
            this.spec_12 = spec_12;
            this.value1_type = value1_type;
            this.value1 = value1;
            this.value2_type = value2_type;
            this.value2 = value2;
            this.value3_type = value3_type;
            this.value3 = value3;
            this.value4_type = value4_type;
            this.value4 = value4;
            this.buy_type = buy_type;
            this.buy_price = buy_price;
            this.sell_type = sell_type;
            this.sell_price = sell_price;
            this.apart_type = apart_type;
            this.apart_value = apart_value;
            this.icon = icon;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/table_Item");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);
            
            idx = 0;
            
            data = new Data(row.GetInt(idx++), row.GetString(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));
            
            _list.Add(data);
        }
    }
}
