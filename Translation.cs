using Exiled.API.Interfaces;
using System.ComponentModel;

namespace VacuumCleaner
{
    public class Translation : ITranslation
    {
        [Description("Max vault limit message")]
        public string VaultLimit { get; set; } = "Your vault is max.\n Enter .drop command for drop all items";
        [Description("Item blacklist message")]
        public string ItemBlacklist { get; set; } = "You cant pickup this item";
        [Description("Vault show in hud")]
        public string ShowingVaultHud { get; set; } = "<color=green><b>Vault: {0}</b></color>";
        [Description("Vacuum command description")]
        public string VacuumCommandDescription { get; set; } = "Spawn player as Vacuum cleaner. Enter command again to remove Vacuum cleaner for player";
        [Description("Vacuum command usage text")]
        public string VacuumCommandUsage { get; set; } = "Usage: vacuum <player id>";
        [Description("Cant use command text")]
        public string CantUseCommand { get; set; } = "You cant use this command";
        [Description("Enter player ID text")]
        public string EnterPlayerId { get; set; } = "Enter player ID";
        [Description("Player not found text")]
        public string PlayerNotFound { get; set; } = "Player not found";
        [Description("Deleted vacuum cleaner text")]
        public string Deleted { get; set; } = "Deleted vacuum cleaner for player {0}";
        [Description("Created vacuum cleaner text")]
        public string Created { get; set; } = "Created vacuum cleaner for player {0}";
        [Description("Drop items command description")]
        public string DropItemsDescription { get; set; } = "Drop all items from vault";
        [Description("You are not vacuum cleaner text")]
        public string YouAreNotVacuumCleaner { get; set; } = "You're not a vacuum cleaner";
        [Description("Dropped all items text")]
        public string DroppedAllItems { get; set; } = "You successfully dropped all items";
        [Description("Dont have items in vault text")]
        public string DontHaveItems { get; set; } = "You dont have any items in the vault";
    }
}
