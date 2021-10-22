using System.Collections.Generic;

public class ScenarioModel : Model
{
    public class Scenario
    {
        public int Index { get; private set; }
        public int Scene { get; private set; }
        public int Talk_ID { get; private set; }
        public int Char_ID { get; private set; }
        public int Image_Index { get; private set; }
        public string Character_name { get; private set; }
        public int stand { get; private set; }
        public int pos { get; private set; }
        public int face { get; private set; }
        public int emot { get; private set; }
        public int talk_type { get; private set; }
        public int talk_style { get; private set; }
        public int back_img { get; private set; }
        public int sound { get; private set; }
        public string talk { get; private set; }
        public Scenario(int Index, int Scene, int talk_id, int Character_id, int imgIndex, string Character_name, int stand, int pos, int face, int emot, int talk_type, int talk_style, int back_img, int sound, string talk)
        {
            this.Index = Index;
            this.Scene = Scene;
            this.Talk_ID = talk_id;
            this.Char_ID = Character_id;
            this.Image_Index = imgIndex;
            this.Character_name = Character_name;
            this.stand = stand;
            this.pos = pos;
            this.face = face;
            this.emot = emot;
            this.talk_type = talk_type;
            this.talk_style = talk_style;
            this.back_img = back_img;
            this.sound = sound;
            this.talk = talk;
        }
    }

    public Dictionary<string, List<Scenario>> scenarioDic = new Dictionary<string, List<Scenario>>();

    public void Setup()
    {
        Load_Scenario_Table("Main");
        Load_Scenario_Table("Chiyou");
    }

    private void Load_Scenario_Table(string key)
    {
        txtReader reader = txtReader.Load(string.Format("Table/Scenario/Scenario_{0}", key));

        int maxCount = reader.rowCount;
        int idx = 0;

        txtReader.Row row = null;
        Scenario scenario = null;

        List<Scenario> list = new List<Scenario>();

        for (int i = 0; i < maxCount; i++)
        {
            row = reader.GetRow(i);

            idx = 0;

            scenario = new Scenario(row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetString(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetString(idx++));

            list.Add(scenario);
        }

        scenarioDic.Add(key, list);
    }
}
