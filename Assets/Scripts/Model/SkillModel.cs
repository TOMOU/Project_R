using System.Collections;
using System.Collections.Generic;
using Constant;
using UnityEngine;

public class SkillModel : Model
{
    public class Skill
    {
        public int id;
        public string name;
        public float keyFrame;

        public List<SkillType> types;
        public List<SkillTeam> teams;
        public List<SkillTarget> targets;
        public List<SkillProperty> properties;
        public List<float> values;

        public Skill (int id, string name, float keyFrame,
            List<SkillType> types, List<SkillTeam> teams, List<SkillTarget> targets, List<SkillProperty> properties, List<float> values)
        {
            this.id = id;
            this.name = name;
            this.keyFrame = keyFrame;
            this.types = types;
            this.teams = teams;
            this.targets = targets;
            this.properties = properties;
            this.values = values;
        }
    }

    private List<Skill> _skillTableList = new List<Skill> ();
    public List<Skill> skillTable { get { return _skillTableList; } }

    public void Setup ()
    {
        CSVReader reader = CSVReader.Load ("Table/SkillTable");

        int maxCount = reader.rowCount;
        int idx = 0;
        CSVReader.Row row = null;

        Skill skill = null;

        List<SkillType> types = new List<SkillType> ();
        List<SkillTeam> teams = new List<SkillTeam> ();
        List<SkillTarget> targets = new List<SkillTarget> ();
        List<SkillProperty> properties = new List<SkillProperty> ();
        List<float> values = new List<float> ();

        for (int i = 1; i < maxCount; i++)
        {
            row = reader.GetRow (i);

            idx = 0;

            types = new List<SkillType> ();
            teams = new List<SkillTeam> ();
            targets = new List<SkillTarget> ();
            properties = new List<SkillProperty> ();
            values = new List<float> ();

            for (int j = 3; j < reader.colCount; j += 5)
            {
                SkillType e1 = (SkillType) System.Enum.Parse (typeof (SkillType), row.GetString (j));
                if (e1 != SkillType.None)
                {
                    SkillTeam e2 = (SkillTeam) System.Enum.Parse (typeof (SkillTeam), row.GetString (j + 1));
                    SkillTarget e3 = (SkillTarget) System.Enum.Parse (typeof (SkillTarget), row.GetString (j + 2));
                    SkillProperty e4 = (SkillProperty) System.Enum.Parse (typeof (SkillProperty), row.GetString (j + 3));
                    float e5 = row.GetFloat (j + 4);

                    types.Add (e1);
                    teams.Add (e2);
                    targets.Add (e3);
                    properties.Add (e4);
                    values.Add (e5);
                }
            }

            skill = new Skill (row.GetInt (idx++), row.GetString (idx++), row.GetFloat (idx++), types, teams, targets, properties, values);

            _skillTableList.Add (skill);
        }
    }
}