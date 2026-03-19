using CommandSystem;
using Exiled.API.Features;
using ProjectMER.Features.Objects;
using System;
using System.Collections.Generic;

namespace VacuumCleaner.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class vacuum : ICommand
    {
        public string Command { get; } = "vacuum";
        public string[] Aliases { get; } = {};
        public string Description { get; } = "Превратить игрока в Робот пылесос\nИспользование: vacuum <id игрока>";
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

            if (args.Count == 0)
            {
                response = "Использование: vacuum <id игрока>";
                return false;
            }


            if (!Int32.TryParse(args.At(0), out int ide))
            {
                response = "Введите айди игрока";
                return false;
            }

            Player target = Player.Get(id: ide);
            if (target == null)
            {
                response = "Игрок не найден";
                return false;
            }

            try
            {
                Plugin.CreateVacuum(target);
            }
            catch (Exception ex)
            {
                //
            }
 
            response = $"Успешно применено к игроку {target.Nickname}";
            return true;
        }
        


    }
}
