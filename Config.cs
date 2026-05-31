using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace VacuumCleaner
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Folder name (folder will create in Exiled/Configs directory)")]
        public string AudioFolder { get; set; } = "Audios";
        [Description("Vacuum cleaner health")]
        public int Health { get; set; } = 200;
        [Description("Schematic name")]
        public string SchematicName { get; set; } = "VacuumCleaner";
        [Description("Sound name for playing")]
        public string SoundName { get; set; } = "VacuumCleaner";
        [Description("Player nickname when he vacuum cleaner")]
        public string VacuumName { get; set; } = "Робот пылесос";
        [Description("Vault limit")]
        public int LimitForItems { get; set; } = 10;
        [Description("Player broadcast")]
        public string Broadcast { get; set; } = "Вы стали Робот пылесосом... Ваша задача подбирать вещи... И все???";
        [Description("Spawn chance")]
        public int Chance { get; set; } = 10;
        [Description("Sound volume")]
        public float Volume { get; set; } = 1f;
        [Description("Sound max distance")]
        public float MaxDistance { get; set; } = 50f;
        [Description("Vault items blacklist")]
        public List<ItemType> ItemsBlacklist { get; set; } = new()
        {
            ItemType.MicroHID,
            ItemType.ParticleDisruptor
        };
        [Description("Starting items in inventory")]
        public List<ItemType> StartingItems { get; set; } = new()
        {
            ItemType.SurfaceAccessPass,
            ItemType.KeycardZoneManager
        };
        [Description("Schematic move position")]
        public Vector3 SchematicMove { get; set; } = new(0, 0.9f, 0);
        [Description("Minimum players for spawn")]
        public int MinimumPlayers { get; set; } = 5;
    }
}
