using System.Linq;

namespace ET.Server
{
    [FriendOf(typeof(PlayerComponent))]
    public static partial class PlayerComponentSystem
    {
        public static void Add(this PlayerComponent self, Player player)
        {
            self.dictionary.Add(player.Account, player);
            self.idPlayers.Add(player.Id, player);
        }
        
        public static void Remove(this PlayerComponent self, Player player)
        {
            self.dictionary.Remove(player.Account);
            self.idPlayers.Remove(player.Id);
            player.Dispose();
        }
        
        public static Player GetByAccount(this PlayerComponent self,  string account)
        {
            self.dictionary.TryGetValue(account, out EntityRef<Player> player);
            return player;
        }
        
        public static Player Get(this PlayerComponent self, long id)
        {
            self.idPlayers.TryGetValue(id, out EntityRef<Player> player);
            return player;
        }
    }
}