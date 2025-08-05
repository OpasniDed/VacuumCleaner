using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HintServiceMeow.Core.Utilities;
using MEC;
using ProjectMER.Features.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using VacuumCleaner.Commands;
using static VacuumCleaner.Plugin;

namespace VacuumCleaner
{
    public class vacuumHandler : IModule
    {
        public void Register()
        {
            Exiled.Events.Handlers.Player.ChangingRole += ChangingRole;
            Exiled.Events.Handlers.Player.Escaping += PlayerEscaping;
            Exiled.Events.Handlers.Player.Died += PlayerDead;
            Exiled.Events.Handlers.Server.RoundEnded += RoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem += PickingUp;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= ChangingRole;
            Exiled.Events.Handlers.Player.Escaping -= PlayerEscaping;
            Exiled.Events.Handlers.Player.Died -= PlayerDead;
            Exiled.Events.Handlers.Server.RoundEnded -= RoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem -= PickingUp;
        }

        private static System.Random random = new();
        public static readonly Dictionary<Player, List<ItemType>> playerVault = new();




        private void PickingUp(PickingUpItemEventArgs ev)
        {
            if (!vacuum.vacuums.ContainsKey(ev.Player)) return;

            ev.IsAllowed = false;

            if (!playerVault.TryGetValue(ev.Player, out List<ItemType> values))
            {
                values = new List<ItemType>();
                playerVault[ev.Player] = values;
            }
            if (values.Count >= 10)
            {
                ev.Player.ShowHint("У вас лимит в хранилище!\nПропиши команду .drop чтобы освободить хранилище", 5f);
                return;
            }
            if (Plugin.plugin.Config.notAllowedItems.Contains(ev.Pickup.Type))
            {
                ev.Player.ShowHint("Вы не можете подобрать этот предмет", 5f);
                return;
            }
            ev.Pickup.Destroy();
            values.Add(ev.Pickup.Type);
        }

        private void ChangingRole(ChangingRoleEventArgs ev)
        {
            Cleanup(ev.Player);
            if (random.Next(0, 100) < Plugin.plugin.Config.chance)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    if (ev.Player.Role.Side == Side.Scp)
                        return;
                    Plugin.CreateVacuum(ev.Player);
                });
            }
        }
        private void PlayerEscaping(EscapingEventArgs ev)
        {
            Cleanup(ev.Player);
        }
        private void PlayerDead(DiedEventArgs ev)
        {
            Cleanup(ev.Player);
        }
        private void RoundEnded(RoundEndedEventArgs ev)
        {
            vacuum.vacuums.Clear();
            vacuum.audious.Clear();
        }
        private void Cleanup(Player player)
        {
            if (vacuum.vacuums.TryGetValue(player, out SchematicObject schematicObject))
            {
                if (vacuum.audious.TryGetValue(schematicObject, out AudioPlayer audioPlayer))
                {
                    vacuum.vacuums.Remove(player);
                    vacuum.audious.Remove(schematicObject);
                    audioPlayer.Destroy();
                    schematicObject.Destroy();
                } 
            }
        }

        public static IEnumerator<float> CreateVacuumHUD(Player player)
        {
            var hint = new HintServiceMeow.Core.Models.Hints.Hint()
            {
                AutoText = _ =>
                {
                    if (playerVault.TryGetValue(player, out var items))
                    {
                        return $"<color=green><b>Хранилище: {items.Count}/{Plugin.plugin.Config.limitForItems}</b></color>";
                    }
                    return $"<color=green><b>Хранилище: 0/{Plugin.plugin.Config.limitForItems}</b></color>";
                },
                Alignment = HintServiceMeow.Core.Enum.HintAlignment.Center,
                YCoordinate = 30,
                SyncSpeed = HintServiceMeow.Core.Enum.HintSyncSpeed.Fast,
                FontSize = 25
            };

            PlayerDisplay.Get(player)?.AddHint(hint);

            while (player.IsConnected && vacuum.vacuums.ContainsKey(player))
            {
                yield return Timing.WaitForSeconds(0.5f);
            }
            PlayerDisplay.Get(player)?.RemoveHint(hint);
        }
    }
}
