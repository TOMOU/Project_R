// ==================================================
// ILogic.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2020 (c) ChronoWares All Rights Reserved.
// ==================================================

using UnityEngine;

namespace Logic
{
    public class ILogic : MonoBehaviour
    {
        public void Setup()
        {
            gameObject.name = GetType().Name;
            Message.Send<Global.TransformAttachMsg>(new Global.TransformAttachMsg(Constant.BehaviourType.Logic, this.transform));

            Initialize();

            // 이 직후는 각 씬에서 출력되는 메인 다이얼로그가 출력되니 잠금을 해제
            Message.Send<Global.InputUnlockMsg>(new Global.InputUnlockMsg());
        }

        public void ReleaseLogic()
        {
            Release();
        }

        protected virtual void Initialize() { }
        protected virtual void Release() { }
    }
}