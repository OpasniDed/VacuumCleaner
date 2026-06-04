using CustomPlayerEffects;
using Exiled.API.Features;
using HintServiceMeow.Core.Utilities;
using MEC;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System.Collections.Generic;
using UnityEngine;
using VacuumCleaner.Handlers;
using HSM = HintServiceMeow.Core.Models.Hints;

namespace VacuumCleaner.Helpers
{
    public static class VacuumHelper
    {
        private static Config _config => Plugin.Instance.Config;

        public static readonly Dictionary<Player, SchematicWithAudio> VacuumCleaners = new();
        public static bool CreateVacuum(Player player)
        {
            if (VacuumCleaners.ContainsKey(player))
                DeleteVacuum(player);

            SchematicObject schematicObject = ObjectSpawner.SpawnSchematic(_config.SchematicName, player.Position - _config.SchematicMove, new Vector3(0, player.Rotation.eulerAngles.y, 0));

            if (schematicObject == null)
            {
                Log.Error("Schematic does not exist");
                return false;
            }

            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
            player.Health = _config.Health;
            player.ClearInventory();

            schematicObject.transform.parent = player.Transform;

            AudioPlayer audioPlayer = AudioHelper.CreateAndPlayAudio(_config.SoundName, $"Vacuum_{UnityEngine.Random.Range(0, 999999)}", true, 
                schematicObject.Position, false, schematicObject.transform, true, _config.MaxDistance, 5, _config.Volume);

            player.ClearBroadcasts();
            player.Broadcast(5, _config.Broadcast);
            player.CustomName = $"{_config.VacuumName} | ({player.Nickname})";
            player.EnableEffect<Fade>(255);

            foreach (ItemType item in _config.StartingItems)
                player.AddItem(item);

            if (!VacuumCleaners.TryGetValue(player, out SchematicWithAudio data))
            {
                data = new();
                VacuumCleaners[player] = data;
            }

            data.SchematicObject = schematicObject;
            data.AudioPlayer = audioPlayer;

            Timing.RunCoroutine(CreateVacuumHud(player));

            return true;
        }

        public static bool DeleteVacuum(Player player)
        {
            player.CustomName = string.Empty;

            if (VacuumCleaners.TryGetValue(player, out SchematicWithAudio data))
            {
                data.SchematicObject?.Destroy();
                data.AudioPlayer?.Destroy();

                player.DisableEffect<Fade>();

                VacuumCleaners.Remove(player);

                return true;
            }

            return false;
        }

        private static IEnumerator<float> CreateVacuumHud(Player player)
        {
            HSM.Hint hint = new HSM.Hint()
            {
                AutoText = _ =>
                {
                    if (VacuumHandler.PlayerVault.TryGetValue(player, out List<ItemType> items))
                    {
                        return string.Format(Plugin.Instance.Translation.ShowingVaultHud, $"{items.Count}/{_config.LimitForItems}");
                    }
                    return string.Format(Plugin.Instance.Translation.ShowingVaultHud, $"0/{_config.LimitForItems}");
                },
                Alignment = HintServiceMeow.Core.Enum.HintAlignment.Center,
                YCoordinate = 30,
                SyncSpeed = HintServiceMeow.Core.Enum.HintSyncSpeed.Fast,
                FontSize = 25
            };

            PlayerDisplay.Get(player)?.AddHint(hint);

            while (player.IsConnected && VacuumCleaners.ContainsKey(player))
            {
                yield return Timing.WaitForSeconds(0.5f);
            }

            PlayerDisplay.Get(player)?.RemoveHint(hint);

            yield break;
        }
    }
}
