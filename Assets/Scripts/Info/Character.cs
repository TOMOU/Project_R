// ==================================================
// Character.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

namespace Info
{
    public class Character
    {
        public uint idx;
        public int code;
        public int level;
        public int grade;
        public int exp;
        public int nexp;
        public uint[] equip;

        public Character(uint idx, int code, int level, int grade)
        {
            this.idx = idx;
            this.code = code;
            this.level = level;
            this.grade = grade;
            this.nexp = GetNeedExp();
            this.exp = 0;

            this.equip = new uint[8];

            Message.AddListener<Lobby.AddCharacterExpMsg>(OnAddCharacterExp);
            Message.AddListener<Lobby.EvolveCharacterMsg>(OnEvolveCharacter);
        }

        public void Release()
        {
            Message.RemoveListener<Lobby.AddCharacterExpMsg>(OnAddCharacterExp);
            Message.RemoveListener<Lobby.EvolveCharacterMsg>(OnEvolveCharacter);
        }

        public UnitStatus GetStatus()
        {
            // 테이블에서 유닛데이터 로드
            UnitModel um = Model.First<UnitModel>();
            if (um == null)
            {
                Logger.LogErrorFormat("UnitModel을 로드할 수 없음.");
                return null;
            }

            UnitModel.Unit unit = um.unitTable.Find(e => e.code == code);
            if (unit == null)
            {
                Logger.LogErrorFormat("code={0}인 캐릭터를 찾을 수 없음.");
                return null;
            }

            return new UnitStatus(unit, idx, level, grade);
        }

        private int GetNeedExp()
        {
            CharExpModel m = Model.First<CharExpModel>();
            if (m == null)
            {
                Logger.LogError("CharExpModel 로드 실패");
                return 0;
            }

            CharExpModel.Exp exp = m.expTable.Find(e => e.level == level);
            if (exp == null)
            {
                Logger.LogErrorFormat("CharExpModel 테이블에서 level={0}인 필드를 찾는 데 실패", level);
                return 0;
            }


            return exp.exp;
        }

        private void OnAddCharacterExp(Lobby.AddCharacterExpMsg msg)
        {
            // 나한테 온 메세지가 아니니 무시한다.
            if (msg.Target != idx)
                return;

            // 경험치를 더한다.
            exp += msg.AddExp;

            // 경험치가 필요경험치보다 올라가면 다음레벨로 상승
            if (level < Info.My.Singleton.User.level && exp >= nexp)
            {
                while (exp >= nexp)
                {
                    level++;
                    exp -= nexp;
                    nexp = GetNeedExp();
                }
            }

            Message.Send<Lobby.RefreshCharacterDialogMsg>(new Lobby.RefreshCharacterDialogMsg(true));
        }

        private void OnEvolveCharacter(Lobby.EvolveCharacterMsg msg)
        {
            // 나한테 온 메시지가 아니면 무시한다.
            if (msg.Target != idx)
                return;

            // 성장 가능한 상태인지 확인한다.
            if (grade >= 6)
            {
                Logger.LogWarning("캐릭터가 이미 최대 진화수치라서 더이상 진화가 불가능합니다.");
                return;
            }

            // 캐릭터의 grade를 증가시킨다.
            grade++;

            Message.Send<Lobby.RefreshCharacterDialogMsg>(new Lobby.RefreshCharacterDialogMsg());
        }
    }
}