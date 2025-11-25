using LabApi.Features.Wrappers;
using SCPRP.Entities;
using SCPRP.Extensions;
using SCPRP.Modules.Players;

namespace SCPRP.Modules.Entities
{
    public class Money : BaseModule
    {
     
        public static BaseEntity DropMoney(UnityEngine.Vector3 Position, UnityEngine.Quaternion Rotation, long amount)
        {
            var dropped_money = SCPRP.Singleton.Entities.CreateEntity("spawned_money");
            ((spawned_money)dropped_money).Amount = amount;
            dropped_money.Spawn(Position, Rotation);
            return dropped_money;
        }

        public static BaseEntity DropMoney(Player player, long amount)
        {
            if (player.GetMoney() < amount)
            {
                HUD.Notify(player, "<color=red>Insufficient funds!</color>");
                return null;
            }
            player.AddMoney(-amount);
            HUD.Notify(player, $"<color=green>Dropped ${amount}</color>");
            return DropMoney(player.Camera.position + (player.Camera.forward * 0.8f), UnityEngine.Quaternion.Euler(player.Rotation.eulerAngles.x, 0, 0), amount);
        }

        public override void Load()
        {
           
        }

        public override void Tick()
        {
            
        }

        public override void Unload()
        {
            
        }
    }
}
