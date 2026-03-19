using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace VacuumCleaner
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public int Health { get; set; } = 200;
        public string schematicName { get; set; } = "VacuumCleaner";
        public string soundName { get; set; } = "VacuumCleaner.ogg";
        public string VacuumName { get; set; } = "Робот пылесос";
        public int limitForItems { get; set; } = 10;
        public string broadcast { get; set; } = "Вы стали Робот пылесосом... Ваша задача подбирать вещи... И все???";
        public int chance { get; set; } = 10;
        public float volume { get; set; } = 1f;
        [Description("Запрещенные предметы")]
        public List<ItemType> notAllowedItems { get; set; } = new()
        {
            ItemType.MicroHID,
            ItemType.ParticleDisruptor
        };
    }
}
