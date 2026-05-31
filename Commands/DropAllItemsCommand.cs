using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using System;
using System.Linq;
using VacuumCleaner.Handlers;
using VacuumCleaner.Helpers;

namespace VacuumCleaner.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DropAllItemsCommand : ICommand
    {
        public string Command => "dropallitems";
        public string[] Aliases => new[] { "dropitems", "di", "dropi", "drop" };
        public string Description => Plugin.Instance.Translation.DropItemsDescription;

        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = Plugin.Instance.Translation.CantUseCommand;
                return false;
            }
            if (!VacuumHelper.VacuumCleaners.ContainsKey(player))
            {
                response = Plugin.Instance.Translation.YouAreNotVacuumCleaner;
                return false;
            }

            if (VacuumHandler.PlayerVault[player].Count > 0)
            {
                foreach (ItemType item in VacuumHandler.PlayerVault[player].ToList())
                {
                    Pickup pickup = Pickup.CreateAndSpawn(item, player.Position);
                    VacuumHandler.PlayerVault[player].Remove(pickup.Type);
                }
                response = Plugin.Instance.Translation.DroppedAllItems;
                return true;
            }

            response = Plugin.Instance.Translation.DontHaveItems;
            return false;
        }
    }
}
