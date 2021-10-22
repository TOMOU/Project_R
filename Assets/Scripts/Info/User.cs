// ==================================================
// User.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System;
using System.Collections.Generic;

namespace Info
{
    public class User
    {
        public uint idx;
        public string nickName;
        public int level;
        public int curExp;
        public int maxExp;
        public int curStamina;
        public int maxStamina;

        public int maxClearedMainstory;
        public int maxClearedDimension;
        public int maxClearedBabel;
        public int maxClearedStory;

        public int MainstoryChapter { get { return maxClearedMainstory / 4 + 1; } }
        public int MainstoryStage { get { return maxClearedMainstory % 4 + 1; } }

        public int babelDevelopmentLevel;
        public List<BabelDispatchState> babelDispatchSlotList;
        public List<BabelDevState> babelDevSlotList;

        public int gold;

        public bool isCompleteLocalize;
        public bool isCompleteArena;
        public bool isCompleteGacha;

        // 유저 데이터 초기화
        public User()
        {
            idx = 106832579;
            nickName = string.Empty;
            level = 1;
            curExp = 0;
            maxExp = 1000;
            curStamina = 30;
            maxStamina = 30;

            maxClearedMainstory = 0;
            maxClearedDimension = 0;
            maxClearedBabel = 0;
            maxClearedStory = 0;

            babelDispatchSlotList = new List<BabelDispatchState>();
            for (int i = 0; i < 5; i++)
            {
                babelDispatchSlotList.Add(new BabelDispatchState(i, 0, 0, DateTime.MinValue));
            }

            babelDevelopmentLevel = 7;
            babelDevSlotList = new List<BabelDevState>();
            for (int i = 0; i < 4; i++)
            {
                if (i < 2)
                    babelDevSlotList.Add(new BabelDevState(i, 1, DateTime.MinValue));
                else
                    babelDevSlotList.Add(new BabelDevState(i, 0, DateTime.MinValue));
            }

            isCompleteLocalize = false;
            isCompleteArena = false;
            isCompleteGacha = false;

            // babelDispatchSlotList[0].state = 1;
            // babelDispatchSlotList[0].character = 3;
            // babelDispatchSlotList[0].endTime = DateTime.Now;

            gold = 100000;
        }

        public void Release()
        {
            babelDispatchSlotList.Clear();
            babelDispatchSlotList = null;

            babelDevSlotList.Clear();
            babelDevSlotList = null;
        }

        public bool IsExistInDispatch(uint character)
        {
            return babelDispatchSlotList.Find(e => e.character == character) != null;
        }

        public class BabelDevState
        {
            public int idx;
            public int state;
            public DateTime endTime;

            public BabelDevState(int idx, int state, DateTime end)
            {
                this.idx = idx;
                this.state = state;
                this.endTime = end;
            }
        }

        public class BabelDispatchState
        {
            public int idx;
            public int state;
            public uint character;
            public DateTime endTime;

            public BabelDispatchState(int idx, int state, uint character, DateTime end)
            {
                this.idx = idx;
                this.state = state;
                this.character = character;
                this.endTime = end;
            }
        }
    }
}