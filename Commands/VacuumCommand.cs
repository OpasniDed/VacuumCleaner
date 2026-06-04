using CommandSystem;
using Exiled.API.Features;
using System;
using VacuumCleaner.Helpers;

namespace VacuumCleaner.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class VacuumCommand : ICommand
    {
        public string Command => "vacuum";
        public string[] Aliases => new[] { "vac" };
        public string Description => Plugin.Instance.Translation.VacuumCommandDescription;

        private string _usage = Plugin.Instance.Translation.VacuumCommandUsage;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = Plugin.Instance.Translation.CantUseCommand;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = _usage;
                return false;
            }


            if (!int.TryParse(arguments.At(0), out int id))
            {
                response = Plugin.Instance.Translation.EnterPlayerId;
                return false;
            }

            Player target = Player.Get(id);

            if (target == null)
            {
                response = Plugin.Instance.Translation.PlayerNotFound;
                return false;
            }

            if (VacuumHelper.VacuumCleaners.ContainsKey(target))
            {
                VacuumHelper.DeleteVacuum(target);
                response = string.Format(Plugin.Instance.Translation.Deleted, target.Nickname);
                return true;
            }
            else
            {
                if (VacuumHelper.CreateVacuum(target))
                {
                    response = string.Format(Plugin.Instance.Translation.Created, target.Nickname);
                    return true;
                }
                else
                {
                    response = "Maybe schematic or audio is null";
                    return false;
                }
            }
        }
    }
}
