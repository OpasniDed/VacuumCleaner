using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using System.Collections.Generic;
using VacuumCleaner.Helpers;

namespace VacuumCleaner.Handlers
{
    public class VacuumHandler
    {
        public void Register()
        {
            Exiled.Events.Handlers.Player.Spawned += Spawned;
            Exiled.Events.Handlers.Player.Escaping += Escaping;
            Exiled.Events.Handlers.Player.Died += Died;
            Exiled.Events.Handlers.Server.RoundEnded += RoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem += PickingUp;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Spawned -= Spawned;
            Exiled.Events.Handlers.Player.Escaping -= Escaping;
            Exiled.Events.Handlers.Player.Died -= Died;
            Exiled.Events.Handlers.Server.RoundEnded -= RoundEnded;
            Exiled.Events.Handlers.Player.PickingUpItem -= PickingUp;
        }

        private static Config _config => Plugin.Instance.Config;


        private static Dictionary<Player, List<ItemType>> _playerVault = new();

        public static Dictionary<Player, List<ItemType>> PlayerVault => _playerVault;

        private void PickingUp(PickingUpItemEventArgs ev)
        {
            if (!VacuumHelper.VacuumCleaners.ContainsKey(ev.Player)) 
                return;

            ev.IsAllowed = false;

            if (!_playerVault.TryGetValue(ev.Player, out List<ItemType> items))
            {
                items = new List<ItemType>();
                PlayerVault[ev.Player] = items;
            }

            if (items.Count >= 10)
            {
                ev.Player.ShowHint(Plugin.Instance.Translation.VaultLimit, 5f);
                return;
            }

            if (_config.ItemsBlacklist.Contains(ev.Pickup.Type))
            {
                ev.Player.ShowHint(Plugin.Instance.Translation.ItemBlacklist, 5f);
                return;
            }

            ev.Pickup.Destroy();

            items.Add(ev.Pickup.Type);
        }

        private void Spawned(SpawnedEventArgs ev)
        {
            VacuumHelper.DeleteVacuum(ev.Player);

            if (UnityEngine.Random.Range(0, 100) < _config.Chance)
            {
                if (Player.List.Count < _config.MinimumPlayers) 
                    return;
                if (ev.Player.Role.Type is RoleTypeId.Tutorial or RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.None or RoleTypeId.Destroyed) 
                    return;

                if (ev.Player.Role.Side is Side.Scp)
                    return;

                VacuumHelper.CreateVacuum(ev.Player);
            }
        }

        private void Escaping(EscapingEventArgs ev)
        {
            VacuumHelper.DeleteVacuum(ev.Player);
            ev.Player.Role.Set(RoleTypeId.Spectator);
        }

        private void Died(DiedEventArgs ev)
        {
            VacuumHelper.DeleteVacuum(ev.Player);
        }

        private void RoundEnded(RoundEndedEventArgs ev) => VacuumHelper.VacuumCleaners.Clear();
    }
}
