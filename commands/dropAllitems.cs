using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using ProjectMER.Features.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VacuumCleaner.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class dropAllitems : ICommand
    {
        public string Command { get; } = "dropallitems";
        public string[] Aliases { get; } = { "dropitems", "di", "dropi", "drop" };
        public string Description { get; } = "Выкинуть все предметы с хранилища";
        public bool SanitizeResponse => false;

        public static readonly Dictionary<Player, SchematicObject> vacuums = new();
        public static readonly Dictionary<SchematicObject, AudioPlayer> audious = new();

        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (player == null)
            {
                response = "Игрока не существует";
                return false;
            }
            if (!vacuum.vacuums.ContainsKey(player))
            {
                response = "Вы не робот пылесос";
                return false;
            }

            if (vacuumHandler.PlayerVault[player].Count > 0)
            {
                foreach (var item in vacuumHandler.PlayerVault[player].ToList())
                {
                    Pickup pickup = Pickup.CreateAndSpawn(item, player.Position);
                    vacuumHandler.PlayerVault[player].Remove(pickup.Type);
                }
                response = "Успешно";
                return true;
            }



            response = $"У вас нет предметов в хранищиле";
            return false;
        }



    }
}
