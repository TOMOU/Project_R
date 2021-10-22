using System.Collections.Generic;

public class LocalizeModel : Model
{
    public class Data
    {
        public int code { get; private set; }
        public string korean { get; private set; }
        public string english { get; private set; }
        public string japanese { get; private set; }
        public string france { get; private set; }
        public string german { get; private set; }
        public string chineseSimplified { get; private set; }
        public string chineseTraditional { get; private set; }
        public Data(int code, string korean, string english, string japanese, string france, string german, string chineseSimplified, string chineseTraditional)
        {
            this.code = code;
            this.korean = korean;
            this.english = english;
            this.japanese = japanese;
            this.france = france;
            this.german = german;
            this.chineseSimplified = chineseSimplified;
            this.chineseTraditional = chineseTraditional;
        }
    }
    private List<Data> _list = new List<Data>();
    public List<Data> Table { get { return _list; } }
    public void Setup()
    {
        txtReader reader = txtReader.Load("Table/table_Localize");

        int maxCount = reader.rowCount;
        int idx = 0;

        txtReader.Row row = null;
        Data data = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow(i);

            idx = 0;

            data = new Data(row.GetInt(idx++), row.GetString(idx++), row.GetString(idx++), row.GetString(idx++), row.GetString(idx++), row.GetString(idx++), row.GetString(idx++), row.GetString(idx++));

            _list.Add(data);
        }
    }
}
