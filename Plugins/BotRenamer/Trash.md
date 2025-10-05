Server.PrintToChatAll($"Бот {player.PlayerName} присоединился к игре!");

int counter = 0;
string newName;
do
{
    newName = botNames[random.Next(botNames.Count)];
    counter++;
}
while (counter <= botNames.Count && Utilities.GetPlayers().Any(x => x.IsBot && x.PlayerName == newName));

player.Clan       = $"[A]";
CCSBot? bot = player.Pawn.Value?.As<CCSBot>();
Server.PrintToChatAll($"Is bot null? {bot == null}!");
if (bot != null)
{
bot.Name = "test" + new Random().Next(0, 100);
}
Utilities.SetStateChanged(player, "CSSBot", "m_name");
Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
Utilities.SetStateChanged(player, "CCSPlayerController", "");
Server.PrintToChatAll($"Новое его имя {newName}!");

CCSBot? bot = player.Pawn.Value?.As<CCSBot>();
if (bot != null)
{
    Utilities.SetStateChanged(bot.As<CBaseEntity>(), "CCSBot", "m_name");
}
