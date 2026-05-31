using Exiled.API.Features;
using System.IO;
using UnityEngine;

namespace VacuumCleaner.Helpers
{
    public static class AudioHelper
    {
        private static Config _config => Plugin.Instance.Config;

        public static AudioPlayer CreateAndPlayAudio(string FileName, string audioPlayerName, bool loop, Vector3 position, bool destroyOnEnd = false, Transform parent = null, bool isSpatial = false, float maxDistance = 5, float minDistance = 5, float volume = 1f)
        {
            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet(audioPlayerName);

            FileName = FileName += ".ogg";
            string path = Path.Combine(Paths.Configs, _config.AudioFolder);
            string fullPath = Path.Combine(path, FileName);

            if (!audioPlayer.TryGetSpeaker(audioPlayerName, out Speaker speaker))
                speaker = audioPlayer.AddSpeaker(audioPlayerName, isSpatial: isSpatial, maxDistance: maxDistance, minDistance: minDistance, volume: volume);

            if (parent)
            {
                speaker.transform.SetParent(parent);
                speaker.transform.localPosition = Vector3.zero;
                speaker.transform.localRotation = Quaternion.identity;
            }
            else
                speaker.Position = position;

            if (!AudioClipStorage.AudioClips.ContainsKey(FileName))
                AudioClipStorage.LoadClip(fullPath, FileName);

            audioPlayer.AddClip(FileName, destroyOnEnd: destroyOnEnd, loop: loop);

            return audioPlayer;
        }
    }
}
