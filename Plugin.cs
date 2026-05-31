using Exiled.API.Features;
using System;
using VacuumCleaner.Handlers;

namespace VacuumCleaner
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Author => "OpasniDed";
        public override string Name => "VacuumCleaner";
        public override string Prefix => "VacuumCleaner";
        public override Version Version => new(2, 0, 0);
        public override Version RequiredExiledVersion => new(9, 14, 1);

        private VacuumHandler _handler;

        public static Plugin Instance { get; private set; }

        public override void OnEnabled()
        {
            Instance = this;

            _handler = new();
            _handler.Register();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _handler.Unregister();
            _handler = null;

            Instance = null;

            base.OnDisabled();
        }
    }
}
