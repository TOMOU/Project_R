﻿// ==================================================
// BuildOptionDefine.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

// #define TEST

public class BuildOptionDefine
{
#if TEST
    public static bool isResizeSpineTexture = false;
    public static bool isPlayTutorial = false;
#else
    public static bool isResizeSpineTexture = true;
    public static bool isPlayTutorial = true;
#endif
}
