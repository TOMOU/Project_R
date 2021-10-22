// ==================================================
// Inventory.cs
// ==================================================
// 이 소스 코드의 권리를 명시하는 주석을 제거하지 마시오.
// 소스 코드에 대한 모든 권리는 (주)크로노웨어즈에 있습니다.
//
// Copyright 2021 (c) ChronoWares All Rights Reserved.
// ==================================================

using System.Collections.Generic;

namespace Info
{
    public class Inventory
    {
        // 경험치 포션
        public int[] potion = new int[4];

        // 진화 재료
        public int material_1 { get; private set; }
        public int material_2 { get; private set; }
        public int material_3 { get; private set; }
        public List<Info.Character> characterList;
        public List<Info.Equipment> equipmentList;
        public uint[,] storyTeam = new uint[3, 3];
        public uint[,] dimensionTeam = new uint[3, 3];
        public uint[,] subTeam = new uint[3, 3];
        public uint[,] babelTeam = new uint[3, 3];
        public uint[,] arenaTeam = new uint[3, 3];
        public uint[] bossTeam = new uint[5];

        public Inventory()
        {
            potion[0] = 100;
            potion[1] = 100;
            potion[2] = 100;
            potion[3] = 100;

            material_1 = 1500;
            material_2 = 2000;
            material_3 = 5000;

            characterList = new List<Character>();
            equipmentList = new List<Equipment>();

            // 18~41까지 랜덤 8개 장비추가
            for (int i = 18; i <= 41; i++)
            {
                equipmentList.Add(new Equipment((uint)(i - 17), i, 0));
            }

            equipmentList.Add(new Equipment(30, 46, 0));

            characterList.Add(new Character(000001, 100211, 1, 1));     // 릴리스
            characterList.Add(new Character(000002, 101031, 1, 2));     // 치우
            characterList.Add(new Character(000003, 102831, 1, 3));     // 알렉산더
            // characterList.Add(new Character(000004, 101131, 1, 3));     // 진시황
            // characterList.Add(new Character(000005, 103031, 1, 3));      // 판도라

            // 아군 팀 리스트 지정
            storyTeam[1, 1] = 000001;   // 릴리스
            storyTeam[0, 0] = 000002;   // 치우
            storyTeam[0, 2] = 000003;   // 알렉산더
            // storyTeam[3] = 000004;
            // storyTeam[4] = 000005;

            Message.AddListener<Lobby.UseExpPotionMsg>(OnCharacterUsePotion);
            Message.AddListener<Lobby.UseEvolveMaterialMsg>(OnUseEvolveMaterial);
        }

        public void Release()
        {
            Message.RemoveListener<Lobby.UseExpPotionMsg>(OnCharacterUsePotion);
            Message.RemoveListener<Lobby.UseEvolveMaterialMsg>(OnUseEvolveMaterial);

            for (int i = 0; i < characterList.Count; i++)
            {
                characterList[i].Release();
            }
            characterList.Clear();
            characterList = null;

            // for (int i = 0; i < enemyList.Count; i++)
            // {
            //     enemyList[i].Release();
            // }
            // enemyList.Clear();
            // enemyList = null;
        }

        private void OnCharacterUsePotion(Lobby.UseExpPotionMsg msg)
        {
            if (msg.Exp == 200)
                potion[0] -= msg.Count;
            else if (msg.Exp == 400)
                potion[1] -= msg.Count;
            else if (msg.Exp == 800)
                potion[2] -= msg.Count;
            else if (msg.Exp == 1600)
                potion[3] -= msg.Count; ;

            Message.Send<Lobby.AddCharacterExpMsg>(new Lobby.AddCharacterExpMsg(msg.Target, msg.Exp * msg.Count));
        }

        private void OnUseEvolveMaterial(Lobby.UseEvolveMaterialMsg msg)
        {
            // 캐릭터 idx를 참조하여 성장재료를 판단
            // msg.Target

            // 성장재료가 부족한지 확인 (메세지가 잘못 들어왔는지)
            if (material_1 < 15 || material_2 < 20 || material_3 < 50)
            {
                Logger.LogWarning("진화재료 부족");
                return;
            }

            // 재료 차감
            material_1 -= 15;
            material_2 -= 20;
            material_3 -= 50;

            // 캐릭터에 메세지 전송
            Message.Send<Lobby.EvolveCharacterMsg>(new Lobby.EvolveCharacterMsg(msg.Target));
        }

        public bool AddCharacter(int code, int level, int grade)
        {
            var cm = Model.First<UnitModel>();
            if (cm != null)
            {
                var character = cm.unitTable.Find(e => e.code == code);
                if (character != null)
                {
                    Info.Character cha = new Character((uint)characterList.Count + 1, code, level, grade);
                    characterList.Add(cha);
                    return true;
                }
            }

            return false;
        }

        public Info.Character GetCharacterByIndex(uint index)
        {
            if (characterList == null)
            {
                Logger.LogWarning("유저 보유 캐릭터리스트가 null입니다.");
                return null;
            }

            return characterList.Find(e => e.idx == index);
        }

        public Info.Equipment GetEquipmentByIndex(uint index)
        {
            if (equipmentList == null)
            {
                return null;
            }

            return equipmentList.Find(e => e.idx == index);
        }

        public List<Info.Character> GetArenaCharacter()
        {
            List<Info.Character> result = new List<Character>();

            for (int i = 0; i < arenaTeam.GetLength(0); i++)
            {
                for (int j = 0; j < arenaTeam.GetLength(1); j++)
                {
                    // 아레나팀에 배치된 캐릭터가 있으면 넣는다.
                    if (arenaTeam[i, j] > 0)
                    {
                        result.Add(GetCharacterByIndex(arenaTeam[i, j]));
                    }
                }
            }

            return result;
        }

        public void AddItem(int id, int value)
        {
            var im = Model.First<ItemModel>();
            ItemModel.Data data = im.Table.Find(e => e.id == id);

            if (data.equip > 0)
            {
                uint idx = (uint)equipmentList.Count + 1;

                Equipment equip = new Equipment(idx, id, 0);

                equipmentList.Add(equip);
            }
        }
    }
}