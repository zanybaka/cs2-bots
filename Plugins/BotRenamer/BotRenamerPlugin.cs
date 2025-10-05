using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Timers;
using Microsoft.Extensions.Logging;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace BotRenamerPlugin
{
    [MinimumApiVersion(80)]
    public class BotRenamer : BasePlugin
    {
        private List<string> allBotNames = new();
        private Random random = new();
        private Dictionary<int, Timer> playerTimers = new();
        private HashSet<string> availableBotNames = new();

        public override string ModuleName => "Bot Renamer";
        public override string ModuleAuthor => "ZanyBaka";
        public override string ModuleDescription => "Randomly rename bots on join using JSON list";
        public override string ModuleVersion => "1.0";

        public override void Load(bool hotReload)
        {
            Logger.LogInformation($"[{ModuleName}] Loaded.");
            LoadBotNames();

            RegisterListener<Listeners.OnClientPutInServer>(OnPlayerConnect);
            RegisterListener<Listeners.OnClientDisconnect>(OnPlayerDisconnect);
        }

        private void LoadBotNames()
        {
            try
            {
                string path = Path.Combine(ModuleDirectory, "BotNames.json");
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var names = JsonSerializer.Deserialize<List<string>>(json);
                    if (names != null && names.Count > 0)
                    {
                        allBotNames       = names;
                        availableBotNames = new HashSet<string>(allBotNames);
                        Logger.LogInformation($"[{ModuleName}] Loaded {allBotNames.Count} bot names.");
                    }
                    else
                    {
                        Logger.LogWarning($"[{ModuleName}] BotNames.json is empty.");
                    }
                }
                else
                {
                    Logger.LogWarning($"[{ModuleName}] BotNames.json not found at {path}.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"[{ModuleName}] Failed to load BotNames.json: {e.Message}");
            }
        }

        private void OnPlayerDisconnect(int playerSlot)
        {
            ClearTimer(playerSlot);
            ReturnBackAvailableBotName(playerSlot);
        }

        private void OnPlayerConnect(int playerSlot)
        {
            try
            {
                var player = Utilities.GetPlayerFromSlot(playerSlot);
                if (player == null)
                {
                    Logger.LogWarning($"[{ModuleName}] Player at slot {playerSlot} not found.");
                    return;
                }

                if (!player.IsBot)
                {
                    Server.PrintToChatAll($"[Achtung] {player.PlayerName} теперь в игре. Partisanen puf-puf!");
                    return;
                }

                if (allBotNames.Count == 0)
                {
                    Logger.LogWarning($"[{ModuleName}] No bot names loaded, check BotRenamer.json file. Skipping rename.");
                    return;
                }

                if (availableBotNames.Count == 0)
                {
                    Logger.LogWarning($"[{ModuleName}] No available bot names. Skipping rename.");
                    return;
                }

                ClearTimer(playerSlot);

                var list = availableBotNames.ToList();
                var newName = list[random.Next(list.Count)];
                availableBotNames.Remove(newName);

                var timer = AddTimer(5f, () =>
                {
                    var p = Utilities.GetPlayerFromSlot(playerSlot);
                    if (p == null || !p.IsValid)
                    {
                        ClearTimer(playerSlot);
                        ReturnBackAvailableBotName(newName);
                    }
                    else
                    {
                        player.PlayerName = newName;
                    }
                }, TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);

                playerTimers[playerSlot] = timer;
                Logger.LogDebug($"[{ModuleName}] Bot in slot {playerSlot} renamed to {newName}.");
            }
            catch (Exception e)
            {
                Logger.LogError($"[{ModuleName}] {e}");
            }
        }

        private bool ClearTimer(int playerSlot)
        {
            if (!playerTimers.TryGetValue(playerSlot, out Timer? oldTimer))
            {
                return false;
            }

            oldTimer.Kill();
            playerTimers.Remove(playerSlot);
            return true;

        }

        private void ReturnBackAvailableBotName(string name)
        {
            availableBotNames.Add(name);
        }

        private void ReturnBackAvailableBotName(int playerSlot)
        {
            var player = Utilities.GetPlayerFromSlot(playerSlot);
            if (player != null)
            {
                availableBotNames.Add(player.PlayerName);
            }
        }
    }
}
