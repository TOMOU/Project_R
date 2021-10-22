// ==================================================
// CSVReader.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

public class CSVReader
{
    public class Row
    {
        public string[] cell;

        public int GetInt(int i)
        {
            int value = 0;
            if (i >= cell.Length)
                return value;

            if (string.IsNullOrEmpty(cell[i]) == false)
                int.TryParse(cell[i], out value);

            return value;
        }

        public uint GetUInt(int i)
        {
            uint value = 0;
            if (i >= cell.Length)
                return value;

            if (string.IsNullOrEmpty(cell[i]) == false)
                uint.TryParse(cell[i], out value);

            return value;
        }

        public float GetFloat(int i)
        {
            float value = 0;

            if (string.IsNullOrEmpty(cell[i]) == false)
                float.TryParse(cell[i], out value);

            return value;
        }

        public long GetLong(int i)
        {
            long value = 0;

            if (string.IsNullOrEmpty(cell[i]) == false)
                long.TryParse(cell[i], out value);

            return value;
        }

        public byte GetByte(int i)
        {
            byte value = 0;

            if (string.IsNullOrEmpty(cell[i]) == false)
                byte.TryParse(cell[i], out value);

            return value;
        }

        public double GetDouble(int i)
        {
            double value = 0;

            if (string.IsNullOrEmpty(cell[i]) == false)
                double.TryParse(cell[i], out value);

            return value;
        }

        public string GetString(int i)
        {
            return cell[i];
        }

        public bool GetBool(int i)
        {
            return cell[i] == "TRUE";
        }
    }

    string fullText;
    Row[] row;

    public int rowCount;
    public int colCount;

    public static CSVReader Load(string path)
    {
        TextAsset asset = Resources.Load(path) as TextAsset;
        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        string data = encoding.GetString(asset.bytes);

        // UTF-8 BOM 제거
        data = data.Trim(new char[] { '\uFEFF', '\u200b' });

        CSVReader reader = new CSVReader();
        reader.fullText = data; //asset.text;

        string[] lines = reader.fullText.Split('\r');

        reader.rowCount = lines.Length - 1;
        reader.row = new Row[reader.rowCount];

        reader.colCount = 0;
        for (int i = 0; i < reader.rowCount; i++)
        {
            reader.row[i] = new Row();

            reader.row[i].cell = lines[i].Split(',');
            if (reader.row[i].cell[0].Length > 0)
            {
                reader.row[i].cell[0] = reader.row[i].cell[0].Replace("\n", "");
            }
            reader.colCount = Mathf.Max(reader.colCount, reader.row[i].cell.Length);
        }

        return reader;
    }

    public static CSVReader Load(TextAsset asset)
    {
        string data = asset.text;

        CSVReader reader = new CSVReader();
        reader.fullText = data;

        string[] lines = reader.fullText.Split('\r');

        reader.rowCount = lines.Length - 1;
        reader.row = new Row[reader.rowCount];

        reader.colCount = 0;
        for (int i = 0; i < reader.rowCount; i++)
        {
            reader.row[i] = new Row();

            reader.row[i].cell = lines[i].Split(',');
            if (reader.row[i].cell[0].Length > 0)
            {
                reader.row[i].cell[0] = reader.row[i].cell[0].Replace("\n", "");
            }
            reader.colCount = Mathf.Max(reader.colCount, reader.row[i].cell.Length);
        }

        return reader;
    }

    public Row GetRow(int i)
    {
        if (row.Length <= i)
            return null;

        return row[i];
    }
}