// ==================================================
// ArenaTeamSlot.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;
using UnityEngine.UI;

public class ArenaTeamSlot : MonoBehaviour
{
    public Image characterIcon;
    public GameObject[] gradeObj;

    public void Init(Info.Character character)
    {
        if (character == null)
        {
            return;
        }

        characterIcon.sprite = Resources.Load<Sprite>(string.Format("Texture/Character/3_portrait_facesize/{0}", character.code));

        for (int i = 0; i < gradeObj.Length; i++)
        {
            gradeObj[i].SetActive(i < character.grade);
        }
    }
}
