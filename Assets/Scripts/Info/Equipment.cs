// ==================================================
// Equipment.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

namespace Info
{
    public class Equipment
    {
        public uint idx;
        public int code;
        public uint characterIdx;  // 장착한 캐릭터의 idx

        public Equipment(uint idx, int code, uint character)
        {
            this.idx = idx;
            this.code = code;
            this.characterIdx = character;
        }

        public ItemModel.Data GetData()
        {
            var im = Model.First<ItemModel>();
            return im.Table.Find(e => e.id == code);
        }
    }
}