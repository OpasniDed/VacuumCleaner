using Exiled.API.Features;
using Exiled.API.Features.Roles;
using HintServiceMeow.Core.Models.Hints;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using ProjectMER.Commands.ToolGunLike;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VacuumCleaner.Commands;
using static UnityEngine.GraphicsBuffer;

namespace VacuumCleaner
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "OpasniDed";
        public override string Name => "VacuumCleaner";
        public override string Prefix => "VacuumCleaner";
        private static readonly string AudioFolder = Path.Combine(Paths.Configs, "Audios");

        public static System.Random random = new();
        public static Plugin plugin;

        private List<IModule> modules = new()
        {
            new vacuumHandler()
        };

        public override void OnEnabled()
        {
            plugin = this;
            foreach (var module in modules)
            {
                module.Register();
            }
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            foreach (var module in modules)
            {
                module.Unregister();
            }
            plugin = null;
            base.OnDisabled();
        }

        


        public static void CreateVacuum(Player player)
        {
            if (vacuum.vacuums.TryGetValue(player, out SchematicObject objcet))
            {
                if (vacuum.audious.TryGetValue(objcet, out AudioPlayer audioPlayer))
                {
                    DeleteVacuum(player, objcet, audioPlayer);
                    return;
                }
                Log.Error("Ошибка, хз как вообще может произойти...");
            }
            Vector3 spawnPosition = player.Position + Vector3.down * 0.6f;
            SchematicObject schematicObject = ObjectSpawner.SpawnSchematic($"{Plugin.plugin.Config.schematicName}", spawnPosition, Quaternion.Euler(player.Rotation.eulerAngles));
            if (schematicObject == null)
            {
                Log.Error("Не найдена схематика");
                return;
            }
            if (player.Role is not FpcRole fpcRole)
            {
                //
                return;
            }
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
            vacuum.vacuums.Add(player, schematicObject);
            player.Health = Plugin.plugin.Config.Health;
            Timing.RunCoroutine(AttachSchematic(player, schematicObject));
            ChangeVisible(fpcRole, false);
            CreateAndPlayAudio($"{Plugin.plugin.Config.soundName}", $"Vacuum_{random.Next(0, 99999)}", true, schematicObject.Position, false, schematicObject.transform, true, 50f, 5f, schematicObject, Plugin.plugin.Config.volume);
            player.ClearInventory();
            Timing.CallDelayed(0.1f, () =>
            {
                player.AddItem(ItemType.SurfaceAccessPass);
                player.AddItem(ItemType.KeycardZoneManager);
                Timing.RunCoroutine(vacuumHandler.CreateVacuumHUD(player));
            });
            player.ClearBroadcasts();
            player.Broadcast(5, Plugin.plugin.Config.broadcast);
            player.CustomName = Plugin.plugin.Config.VacuumName;
        }
        public static void DeleteVacuum(Player player, SchematicObject schematic, AudioPlayer audioPlayer)
        {
            vacuum.vacuums.Remove(player);
            vacuum.audious.Remove(schematic);
            audioPlayer.Destroy();
            schematic.Destroy();
            if (player.Role is FpcRole fpcRole)
                ChangeVisible(fpcRole, true);
        }

        public static IEnumerator<float> AttachSchematic(Player player, SchematicObject schematicObject)
        {
            while (true)
            {
                if (!vacuum.vacuums.ContainsKey(player))
                {
                    yield break;
                }

                if (player.Role is FpcRole role)
                    ChangeVisible(role, false);

                Vector3 positionForPlayer = player.Position + (-player.Transform.forward * 0.15f) + (Vector3.down * 1f) + (player.Transform.right * 0.15f);
                schematicObject.Position = positionForPlayer;
                schematicObject.Rotation = player.Rotation;
                yield return Timing.WaitForSeconds(0.1f);
            }
        }


        
        private static void CreateAndPlayAudio(string FileName, string audioPlayerName, bool loop, Vector3 position, bool detgroyOnEnd = false, Transform parent = null, bool isSpatial = false, float maxDistance = 5, float minDistance = 5, SchematicObject schematicObject = null, float volume = 1f)
        {
            var audioPlayer = AudioPlayer.CreateOrGet(audioPlayerName);
            var fullPath = Path.Combine(AudioFolder, FileName);
            if (!audioPlayer.TryGetSpeaker(audioPlayerName, out Speaker speaker))
            {
                speaker = audioPlayer.AddSpeaker(audioPlayerName, isSpatial: isSpatial, maxDistance: maxDistance, minDistance: minDistance, volume: volume);
            }
            if (parent)
            {
                speaker.transform.SetParent(parent);
                speaker.transform.localPosition = Vector3.zero;
                speaker.transform.localRotation = Quaternion.identity;
            }
            else
                speaker.Position = position;
            vacuum.audious.Add(schematicObject, audioPlayer);
            if (!AudioClipStorage.AudioClips.ContainsKey(FileName))
                AudioClipStorage.LoadClip(fullPath, FileName);
            audioPlayer.AddClip(FileName, destroyOnEnd: detgroyOnEnd, loop: loop);
        }

        public static void ChangeVisible(FpcRole fpcRole, bool clear)
        {
            if (clear)
            {
                foreach (var p in fpcRole.IsInvisibleFor)
                {
                    fpcRole.IsInvisibleFor.Remove(p);
                }
            }

            var spectators = Player.List.Where(p => p.Role.Type == RoleTypeId.Spectator || p.Role.Type == RoleTypeId.Overwatch);
            var notspectators = Player.List.Where(p => p.Role.Type != RoleTypeId.Spectator && p.Role.Type != RoleTypeId.Overwatch);
            foreach (var spec in spectators)
            {
                fpcRole.IsInvisibleFor.Remove(spec);
            }
            foreach (var p in notspectators)
            {
                if (!fpcRole.IsInvisibleFor.Contains(p))
                    fpcRole.IsInvisibleFor.Add(p);
            }
        }

        public interface IModule
        {
            void Register();
            void Unregister();
        }
    }
}
