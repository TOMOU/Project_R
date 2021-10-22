using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ExcelTool
{
    public class CSVTool : MonoBehaviour
    {
        [MenuItem("Assets/CSVTool/To Constant", false)]
        public static bool ReadFile_Constant()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Editor is Playing");
                return false;
            }

            TextAsset selected = Selection.activeObject as TextAsset;
            if (selected == null)
            {
                Debug.LogWarning("Selected is null");
                return false;
            }

            ExcelParser parser = new ExcelParser();
            parser.ParseToConstant(selected);
            return true;
        }

        [MenuItem("Assets/CSVTool/To Model", false)]
        public static bool ReadFile_Model()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Editor is Playing");
                return false;
            }

            TextAsset selected = Selection.activeObject as TextAsset;
            if (selected == null)
            {
                Debug.LogWarning("Selected is null");
                return false;
            }

            ExcelParser parser = new ExcelParser();
            parser.ParseToModel(selected);
            return true;
        }
    }

    public class ExcelParser
    {
        public void ParseToConstant(TextAsset asset)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            string data = encoding.GetString(asset.bytes);

            // UTF-8 BOM 제거
            data = data.Trim(new char[] { '\uFEFF', '\u200b' });

            string[] lines = data.Split('\r');

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace Constant");
            sb.AppendLine("{");

            string enumName = string.Empty;
            string id = string.Empty;
            string name = string.Empty;
            string comment = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                string[] texts = lines[i].Split(',');
                for (int j = 0; j < texts.Length; j++)
                {
                    string t = texts[j].Replace("\n", "");

                    // 아무 문자 없으면 건너뛴다.
                    if (string.IsNullOrEmpty(t))
                    {
                        if (j < 2)
                        {
                            id = string.Empty;
                            name = string.Empty;
                            comment = string.Empty;
                        }
                        else
                        {
                            comment = string.Empty;
                        }
                        continue;
                    }

                    // Enun 제목을 파싱한다.
                    if (string.IsNullOrEmpty(enumName))
                    {
                        enumName = t;
                        if (enumName != "CONST")
                        {
                            sb.AppendLine("    public enum " + t);
                            sb.AppendLine("    {");
                        }
                        continue;
                    }
                    // Enum 종료지점을 파싱한다.
                    else if (t == "END")
                    {
                        if (enumName != "CONST")
                        {
                            sb.AppendLine("    }");
                            sb.AppendLine("");
                        }
                        enumName = string.Empty;
                        id = string.Empty;
                        name = string.Empty;
                        comment = string.Empty;
                        continue;
                    }
                    else if (j == 0) // id
                    {
                        id = t;
                    }
                    else if (j == 1) // tag
                    {
                        name = t;
                    }
                    else if (j == 2) // name
                    {
                        comment = t;
                    }
                }

                if (string.IsNullOrEmpty(id) == false && string.IsNullOrEmpty(name) == false)
                {
                    if (enumName != "CONST")
                    {
                        if (string.IsNullOrEmpty(comment))
                            sb.AppendLine("        " + name + " = " + id + ",");
                        else
                            sb.AppendLine("        " + name + " = " + id + ",  // " + comment);
                    }
                    else
                    {
                        sb.AppendLine("public const " + id + " " + name + " = " + comment + ";");
                    }
                }
            }

            sb.AppendLine("}");
            Save(Application.dataPath + "/Scripts/Constant/", asset.name, sb);
        }

        public void ParseToModel(TextAsset asset)
        {
            // string modelName = asset.name.Remove (asset.name.Length - 5, 5);
            string modelName = GetClassName(asset.name.Remove(0, 6));


            CSVReader reader = CSVReader.Load(asset);
            CSVReader.Row paramType = reader.GetRow(0);
            CSVReader.Row paramName = reader.GetRow(1);

            if (IsManualModel(paramType.GetString(0)) == false)
            {
                Debug.LogWarningFormat("{0}Model의 파라미터형이 잘못 입력되어 있거나, 수동으로 스크립트를 만들어야 합니다.", modelName);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("");
            sb.AppendLine("public class " + modelName + "Model : Model");
            sb.AppendLine("{");
            sb.AppendLine("    public class Data");
            sb.AppendLine("    {");
            for (int i = 0; i < paramType.cell.Length; i++)
            {
                if (string.IsNullOrEmpty(paramType.GetString(i)) == true)
                    continue;

                sb.AppendLine("        public " + paramType.GetString(i) + " " + paramName.GetString(i) + " { get; private set; }");
            }
            sb.Append("        public Data("); //! 인자값 추가
            for (int i = 0; i < paramType.cell.Length; i++)
            {
                if (string.IsNullOrEmpty(paramType.GetString(i)) == true)
                    continue;

                if (i > 0)
                    sb.Append(", ");
                sb.Append(paramType.GetString(i) + " " + paramName.GetString(i)); //! 인자값 추가
            }
            sb.AppendLine(")");
            sb.AppendLine("        {");
            for (int i = 0; i < paramType.cell.Length; i++)
            {
                if (string.IsNullOrEmpty(paramType.GetString(i)) == true)
                    continue;

                sb.AppendLine("            this." + paramName.GetString(i) + " = " + paramName.GetString(i) + ";");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("    private List<Data> _list = new List<Data>();");
            sb.AppendLine("    public List<Data> Table { get { return _list; } }");
            sb.AppendLine("    public void Setup()");
            sb.AppendLine("    {");
            sb.AppendLine("        CSVReader reader = CSVReader.Load(\"Table/table_" + modelName + "\");");
            sb.AppendLine("");
            sb.AppendLine("        int maxCount = reader.rowCount;");
            sb.AppendLine("        int idx = 0;");
            sb.AppendLine("");
            sb.AppendLine("        CSVReader.Row row = null;");
            sb.AppendLine("        Data data = null;");
            sb.AppendLine("");
            sb.AppendLine("        for (int i = 2; i < maxCount; i++)");
            sb.AppendLine("        {");
            sb.AppendLine("            row = reader.GetRow(i);");
            sb.AppendLine("            ");
            sb.AppendLine("            idx = 0;");
            sb.AppendLine("            ");
            sb.Append("            data = new Data(");
            for (int i = 0; i < paramType.cell.Length; i++)
            {
                if (string.IsNullOrEmpty(paramType.GetString(i)) == true)
                    continue;

                if (i > 0)
                    sb.Append(", ");
                sb.Append(ParseParamType(paramType.GetString(i)));
            }
            sb.AppendLine(");");
            sb.AppendLine("            ");
            sb.AppendLine("            _list.Add(data);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            Save(Application.dataPath + "/Scripts/Model/", modelName + "Model", sb);
            SyncGameModel();
        }

        private string GetClassName(string origin)
        {
            return origin[0].ToString().ToUpper() + origin.Substring(1);
        }

        private string ParseParamType(string paramType)
        {
            string s = string.Empty;

            if (paramType == "int")
                s = "row.GetInt(idx++)";
            else if (paramType == "uint")
                s = "row.GetUInt(idx++)";
            else if (paramType == "float")
                s = "row.GetFloat(idx++)";
            else if (paramType == "long")
                s = "row.GetLong(idx++)";
            else if (paramType == "double")
                s = "row.GetDouble(idx++)";
            else if (paramType == "byte")
                s = "row.GetByte(idx++)";
            else if (paramType == "string")
                s = "row.GetString(idx++)";
            else if (paramType == "bool")
                s = "row.GetBool(idx++)";
            else
                s = "(" + paramType + ")" + "System.Enum.Parse(typeof(" + paramType + "), row.GetString(idx++))";

            return s;
        }

        private bool IsManualModel(string paramType)
        {
            if (paramType == "int" || paramType == "uint" || paramType == "float" || paramType == "long" ||
                paramType == "double" || paramType == "byte" || paramType == "string" || paramType == "bool" || paramType.Contains("Constant"))
                return true;

            return false;
        }

        private void SyncGameModel()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("");
            sb.AppendLine("public class GameModel : Model");
            sb.AppendLine("{");

            string modelPath = Application.dataPath + "/Scripts/Model";
            DirectoryInfo di = new DirectoryInfo(modelPath);
            if (di.Exists == false)
            {
                Debug.LogError("Model 폴더 경로가 잘못되었습니다.");
                return;
            }

            foreach (FileInfo fi in di.GetFiles())
            {
                string name = Path.GetFileNameWithoutExtension(fi.FullName);
                if (name.Contains(".meta") || name.Contains(".cs"))
                    continue;

                if (name == "GameModel")
                    continue;

                // str1.substring(0, 1).toUpperCase()+str1.substring(1);
                sb.AppendLine("    private ModelRef<" + name + "> _" + name.Substring(0, 1).ToLower() + name.Substring(1) + " = new ModelRef<" + name + "> ();");
            }

            sb.AppendLine("    public bool loadCompleteGlobalContent = false;");
            sb.AppendLine("");
            sb.AppendLine("    public void Setup ()");
            sb.AppendLine("    {");

            int cnt = 0;

            foreach (FileInfo fi in di.GetFiles())
            {
                string name = Path.GetFileNameWithoutExtension(fi.FullName);
                if (name.Contains(".meta") || name.Contains(".cs"))
                    continue;

                if (name == "GameModel")
                    continue;

                if (cnt > 0)
                    sb.AppendLine("");

                sb.AppendLine("        // " + name + " 로드");
                sb.AppendLine("        _" + name.Substring(0, 1).ToLower() + name.Substring(1) + ".Model = new " + name + " ();");
                sb.AppendLine("        _" + name.Substring(0, 1).ToLower() + name.Substring(1) + ".Model.Setup ();");

                cnt++;
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            Save(modelPath + "/", "GameModel", sb);
        }

        private void Save(string path, string fileName, StringBuilder sb)
        {
            string name = fileName + ".cs ";

            StreamWriter sw = new StreamWriter(path + name);
            sw.Write(sb.ToString());
            sw.Close();

            AssetDatabase.Refresh();

            Debug.LogFormat("{0} 경로에 {1} 파일이 생성되었습니다.", path, name);
        }
    }
}