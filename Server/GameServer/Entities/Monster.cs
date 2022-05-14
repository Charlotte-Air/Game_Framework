using Charlotte.Proto;
using GameServer.Core;

namespace GameServer.Entities
{
    class Monster : CharacterBase
    {
        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {

        }
    }
}
