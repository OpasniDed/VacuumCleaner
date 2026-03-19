using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HintServiceMeow.Core.Utilities;
using MEC;
using PlayerRoles;
using ProjectMER.Features.Objects;
using System.Collections.Generic;
using VacuumCleaner.Commands;
using static VacuumCleaner.Plugin;

namespace VacuumCleaner
{
    public class vacuumHandler : IModule
    {
        public void Register()
        {
            Exiled.Events.Handlers.Player.Spawned += Spawned;
            Exiled.Events.Handlers.Player.Escaping += PlayerEscaping;
            Exiled.Events.Handlers.Player.Died += PlayerDead;
            Exiled.Events.Handlers.Server.RoundEnded += RoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem += PickingUp;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Spawned -= Spawned;
            Exiled.Events.Handlers.Player.Escaping -= PlayerEscaping;
            Exiled.Events.Handlers.Player.Died -= PlayerDead;
            Exiled.Events.Handlers.Server.RoundEnded -= RoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem -= PickingUp;
        }

        public static readonly Dictionary<Player, List<ItemType>> PlayerVault = new();

        private static Config _config => Plugin.Instance.Config;





        private void PickingUp(PickingUpItemEventArgs ev)
        {
            if (!vacuum.vacuums.ContainsKey(ev.Player)) return;

            ev.IsAllowed = false;

            if (!PlayerVault.TryGetValue(ev.Player, out List<ItemType> values))
            {
                values = new List<ItemType>();
                PlayerVault[ev.Player] = values;
            }
            if (values.Count >= 10)
            {
                ev.Player.ShowHint("У вас лимит в хранилище!\nПропиши команду .drop чтобы освободить хранилище", 5f);
                return;
            }
            if (_config.notAllowedItems.Contains(ev.Pickup.Type))
            {
                ev.Player.ShowHint("Вы не можете подобрать этот предмет", 5f);
                return;
            }
            ev.Pickup.Destroy();
            values.Add(ev.Pickup.Type);
        }

        private void Spawned(SpawnedEventArgs ev)
        {
            Cleanup(ev.Player);
            if (UnityEngine.Random.Range(0, 100) < _config.chance)
            {
                if (Player.List.Count < 5) return;
                if (ev.Player.Role.Type == RoleTypeId.Tutorial) return;
                if (ev.Player.Role.Side == Side.Scp)
                    return;
                Plugin.CreateVacuum(ev.Player);
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
                    if (PlayerVault.TryGetValue(player, out var items))
                    {
                        return $"<color=green><b>Хранилище: {items.Count}/{_config.limitForItems}</b></color>";
                    }
                    return $"<color=green><b>Хранилище: 0/{_config.limitForItems}</b></color>";
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
