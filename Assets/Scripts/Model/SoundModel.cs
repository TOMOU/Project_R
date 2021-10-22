using System.Collections.Generic;

public class SoundModel : Model
{
    public class Sound
    {
        public int index { get; private set; }
        public Constant.SoundType type { get; private set; }
        public Constant.SoundName name { get; private set; }
        public float volumm { get; private set; }
        public bool isLoop { get; private set; }
        public Sound (int index, Constant.SoundType type, Constant.SoundName name, float volumm, bool isLoop)
        {
            this.index = index;
            this.type = type;
            this.name = name;
            this.volumm = volumm;
            this.isLoop = isLoop;
        }
    }
    private List<Sound> _soundTableList = new List<Sound> ();
    public List<Sound> soundTable { get { return _soundTableList; } }
    public void Setup ()
    {
        CSVReader reader = CSVReader.Load ("Table/SoundTable");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Sound sound = null;

        for (int i = 2; i < maxCount; i++)
        {
            row = reader.GetRow (i);
            
            idx = 0;
            
            sound = new Sound (row.GetInt(idx++), (Constant.SoundType)System.Enum.Parse(typeof(Constant.SoundType), row.GetString(idx++)), (Constant.SoundName)System.Enum.Parse(typeof(Constant.SoundName), row.GetString(idx++)), row.GetFloat(idx++), row.GetBool(idx++));
            
            _soundTableList.Add (sound);
        }
    }
}
