using System.Collections.Generic;

public class UnitModel : Model
{
    public class Unit
    {
        public int code { get; private set; }
        public string unitName { get; private set; }
        public string unitName_Kor { get; private set; }
        public Constant.Position position { get; private set; }
        public Constant.BasicAttackType basicProperty { get; private set; }
        public int hp { get; private set; }
        public int damage { get; private set; }
        public int physicalDefence { get; private set; }
        public int magicalDefence { get; private set; }
        public int attackType { get; private set; }
        public float attackRange { get; private set; }
        public float attackSpeed { get; private set; }
        public float attackKeyFrame { get; private set; }
        public float criticalPercentage { get; private set; }
        public float criticalMultiple { get; private set; }
        public float moveSpeed { get; private set; }
        public float hpLevel { get; private set; }
        public float hpGrade { get; private set; }
        public float damageLevel { get; private set; }
        public float damageGrade { get; private set; }
        public float pDefLevel { get; private set; }
        public float pDefGrade { get; private set; }
        public float mDefLevel { get; private set; }
        public float mDefGrade { get; private set; }
        public int skillID_0 { get; private set; }
        public int skillID_1 { get; private set; }
        public int skillID_2 { get; private set; }
        public Unit(int code, string unitName, string unitName_Kor, Constant.Position position, Constant.BasicAttackType basicProperty, int hp, int damage, int physicalDefence, int magicalDefence, int attackType, float attackRange, float attackSpeed, float attackKeyFrame, float criticalPercentage, float criticalMultiple, float moveSpeed, float hpLevel, float hpGrade, float damageLevel, float damageGrade, float pDefLevel, float pDefGrade, float mDefLevel, float mDefGrade, int skillID_0, int skillID_1, int skillID_2)
        {
            this.code = code;
            this.unitName = unitName;
            this.unitName_Kor = unitName_Kor;
            this.position = position;
            this.basicProperty = basicProperty;
            this.hp = hp;
            this.damage = damage;
            this.physicalDefence = physicalDefence;
            this.magicalDefence = magicalDefence;
            this.attackType = attackType;
            this.attackRange = attackRange;
            this.attackSpeed = attackSpeed;
            this.attackKeyFrame = attackKeyFrame;
            this.criticalPercentage = criticalPercentage;
            this.criticalMultiple = criticalMultiple;
            this.moveSpeed = moveSpeed;
            this.hpLevel = hpLevel;
            this.hpGrade = hpGrade;
            this.damageLevel = damageLevel;
            this.damageGrade = damageGrade;
            this.pDefLevel = pDefLevel;
            this.pDefGrade = pDefGrade;
            this.mDefLevel = mDefLevel;
            this.mDefGrade = mDefGrade;
            this.skillID_0 = skillID_0;
            this.skillID_1 = skillID_1;
            this.skillID_2 = skillID_2;
        }
    }
    private List<Unit> _unitTableList = new List<Unit>();
    public List<Unit> unitTable { get { return _unitTableList; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/UnitTable");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Unit unit = null;

        for (int i = 0; i < maxCount; i++)
        {
            row = reader.GetRow(i);

            idx = 0;

            unit = new Unit(row.GetInt(idx++), row.GetString(idx++), row.GetString(idx++), (Constant.Position)System.Enum.Parse(typeof(Constant.Position), row.GetString(idx++)), (Constant.BasicAttackType)System.Enum.Parse(typeof(Constant.BasicAttackType), row.GetString(idx++)), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetFloat(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));

            _unitTableList.Add(unit);
        }
    }
}
