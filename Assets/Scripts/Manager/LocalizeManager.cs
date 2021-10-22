// ==================================================
// LocalizeManager.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections.Generic;
using UnityEngine;

public class LocalizeManager : MonoSingleton<LocalizeManager>
{
    public Constant.Locale locale = Constant.Locale.Korean;
    private List<LocalizeModel.Data> _localizeTable;

    protected override void Init()
    {
        //TODO : 로컬라이즈 테이블 로드
        var lm = Model.First<LocalizeModel>();

        //TODO : 로컬라이즈 리소스 로드
        if (lm != null)
        {
            _localizeTable = lm.Table;
        }
        else
        {
            Logger.LogErrorFormat("로컬라이즈 테이블 로드 실패");
        }

    }

    protected override void Release()
    {
        if (_localizeTable != null)
        {
            _localizeTable.Clear();
            _localizeTable = null;
        }
    }

    public string GetString(int code)
    {
        if (_localizeTable == null || _localizeTable.Count == 0)
        {
            Logger.LogWarning("_localizeTable이 null입니다.");
            return string.Format("<color=#ff00FFff>{0} ERROR</color>", code);
        }

        var data = _localizeTable.Find(e => e.code == code);
        if (data == null)
        {
            Logger.LogWarningFormat("_localizeTalbe에 code={0}의 데이터가 없습니다.", code);
            return code.ToString();
        }

        string result = string.Empty;

        switch (locale)
        {
            case Constant.Locale.Korean:
                result = data.korean;
                break;

            case Constant.Locale.English:
                result = data.english;
                break;

            case Constant.Locale.ChineseSimplified:
                result = data.chineseSimplified;
                break;

            case Constant.Locale.ChineseTraditional:
                result = data.chineseTraditional;
                break;

            case Constant.Locale.Japanese:
                result = data.japanese;
                break;

            case Constant.Locale.France:
                result = data.france;
                break;

            case Constant.Locale.German:
                result = data.german;
                break;
        }

        return Replace(result);
    }

    private string Replace(string str)
    {
        if (string.IsNullOrEmpty(str) == true)
        {
            return "<color=#ff00FFff>EMPTY</color>";
        }

        string result = str;
        result = str.Replace("\\n", "\n");

        return result;
    }
}