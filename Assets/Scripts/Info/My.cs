// ==================================================
// My.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

namespace Info
{
    public class My : MonoSingleton<My>
    {
        public Info.User User { get; private set; }
        public Info.Inventory Inventory { get; private set; }

        protected override void Init()
        {
            base.Init();

            User = new User();
            Inventory = new Inventory();
        }

        protected override void Release()
        {
            base.Release();

            User.Release();
            User = null;

            Inventory.Release();
            Inventory = null;
        }
    }
}