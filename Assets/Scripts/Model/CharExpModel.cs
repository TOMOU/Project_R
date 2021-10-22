// ==================================================
// CharExpModel.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections.Generic;

public class CharExpModel : Model
{
    public class Exp
    {
        public int index { get; private set; }
        public int level { get; private set; }
        public int exp { get; private set; }
        public int total { get; private set; }
        public Exp(int index, int level, int exp, int total)
        {
            this.index = index;
            this.level = level;
            this.exp = exp;
            this.total = total;
        }
    }

    private List<Exp> _expTableList = new List<Exp>();
    public List<Exp> expTable { get { return _expTableList; } }
    public void Setup()
    {
        CSVReader reader = CSVReader.Load("Table/cha_exp");

        int maxCount = reader.rowCount;
        int idx = 0;

        CSVReader.Row row = null;
        Exp exp = null;

        for (int i = 0; i < maxCount; i++)
        {
            row = reader.GetRow(i);

            idx = 0;

            exp = new Exp(row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++), row.GetInt(idx++));

            _expTableList.Add(exp);
        }
    }
}
