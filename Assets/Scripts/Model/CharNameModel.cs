// ==================================================
// CharNameModel.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using System.Collections.Generic;

public class CharNameModel : Model
{
    public class CharName
    {
        public int Index { get; private set; }
        public string Name { get; private set; }
        public CharName(int index, string name)
        {
            this.Index = index;
            this.Name = name;
        }
    }

    private List<CharName> _charNameTableList = new List<CharName>();
    public List<CharName> charNameTable { get { return _charNameTableList; } }

    public void Setup()
    {
        txtReader reader = txtReader.Load("Table/CharacterName");

        int maxCount = reader.rowCount;
        int idx = 0;

        txtReader.Row row = null;
        CharName charName = null;

        for (int i = 0; i < maxCount; i++)
        {
            row = reader.GetRow(i);

            idx = 0;

            charName = new CharName(row.GetInt(idx++), row.GetString(idx++));

            _charNameTableList.Add(charName);
        }
    }
}
