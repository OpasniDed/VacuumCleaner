using CustomPlayerEffects;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VacuumCleaner.Commands;

namespace VacuumCleaner
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "OpasniDed";
        public override string Name => "VacuumCleaner";
        public override string Prefix => "VacuumCleaner";
        private static readonly string AudioFolder = Path.Combine(Paths.Configs, "Audios");

        public static Plugin Instance { get; private set; }

        private List<IModule> _modules = new()
        {
            new vacuumHandler()
        };

        private static Config _config => Instance.Config;

        public override void OnEnabled()
        {
            Instance = this;
            foreach (var module in _modules)
            {
                module.Register();
            }
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            foreach (var module in _modules)
            {
                module.Unregister();
            }
            Instance = null;
            base.OnDisabled();
        }

        public static void CreateVacuum(Player player)
        {
            if (DeleteVacuum(player)) return;

            SchematicObject schematicObject = ObjectSpawner.SpawnSchematic($"{_config.schematicName}", player.Position - new Vector3(0, 0.9f, 0), new Vector3(0, player.Rotation.eulerAngles.y, 0));
            if (schematicObject == null)
            {
                Log.Error("Не найдена схематика");
                return;
            }
            schematicObject.transform.parent = player.Transform;
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);
            vacuum.vacuums.Add(player, schematicObject);
            player.Health = _config.Health;
            CreateAndPlayAudio($"{_config.soundName}", $"Vacuum_{UnityEngine.Random.Range(0, 99999)}", true, schematicObject.Position, false, schematicObject.transform, true, 50f, 5f, schematicObject, _config.volume);
            player.ClearInventory();
            Timing.CallDelayed(0.1f, () =>
            {
                player.AddItem(ItemType.SurfaceAccessPass);
                player.AddItem(ItemType.KeycardZoneManager);
                Timing.RunCoroutine(vacuumHandler.CreateVacuumHUD(player));
            });
            player.ClearBroadcasts();
            player.Broadcast(5, _config.broadcast);
            player.CustomName = $"{_config.VacuumName} ({player.Nickname})";
            player.EnableEffect<Fade>(255);
        }
        public static bool DeleteVacuum(Player player)
        {
            player.CustomName = null;
            if (vacuum.vacuums.TryGetValue(player, out SchematicObject objcet))
            {
                vacuum.vacuums.Remove(player);
                objcet.Destroy();
                if (vacuum.audious.TryGetValue(objcet, out AudioPlayer audioPlayer))
                {
                    vacuum.audious.Remove(objcet);
                    audioPlayer.Destroy();
                    player.DisableEffect<Fade>();
                    return true;
                }
                return true;
            }
            return false;
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
        public interface IModule
        {
            void Register();
            void Unregister();
        }
    }
}
