using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using UnityModManagerNet;

namespace ModTagEx;

internal static class Updater
{
    // link to content database blueprint file
    private const string BLUEPRINT_DB_SOURCE_URL = "https://raw.githubusercontent.com/alterasc/alterasc.github.io/main/all_blueprints.txt";

    // api link to check last commit
    private const string BLUEPRINT_DB_CHECK_URL = "https://api.github.com/repos/alterasc/alterasc.github.io/commits?path=all_blueprints.txt&sha=main&per_page=1";

    public static void Update(UnityModManager.ModEntry modEntry)
    {
        if (!Main.Settings.AutoUpdateDB)
        {
            return;
        }
        var logger = modEntry.Logger;
        long timeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // once per day check
        if (Main.Settings.LastUpdateTS > 0 && timeNow - Main.Settings.LastUpdateTS < 24 * 60 * 60)
        {
            logger.Log($"Skipping update check. Last check:{Main.Settings.LastUpdateTS}, now {timeNow}");
            return;
        }
        Main.Settings.LastUpdateTS = timeNow;
        Main.Settings.Save(modEntry);

        try
        {
            using var web = new TimeoutWebClient();
            web.Headers[HttpRequestHeader.UserAgent] = "WotR-ModTagEx";
            var modDir = Path.GetDirectoryName(modEntry.Path);
            var file = new FileInfo(Path.Combine(modDir, "all_blueprints_upd"));
            if (file.Exists)
            {
                file.Delete();
            }
            var definition = new[] { new { sha = "" } };
            var raw = web.DownloadString(BLUEPRINT_DB_CHECK_URL);
            var result = JsonConvert.DeserializeAnonymousType(raw, definition);
            var lastCommitSha = result[0].sha;
            logger.Log($"Last database commit sha: {lastCommitSha}");
            if (lastCommitSha != Main.Settings.DatabaseCommit)
            {
                web.DownloadFile(BLUEPRINT_DB_SOURCE_URL, file.FullName);
                var dbLocation = Path.Combine(modDir, "all_blueprints.txt");
                if (File.Exists(dbLocation))
                {
                    file.Replace(dbLocation, null);
                }
                else
                {
                    file.MoveTo(dbLocation);
                }
                Main.Settings.DatabaseCommit = lastCommitSha;
                Main.Settings.Save(modEntry);
                logger.Log($"Updated database");
            }
            else
            {
                logger.Log($"Database is up to date");
            }
        }
        catch (Exception ex)
        {
            logger.Log($"Couldn't update database:\n{ex}");
        }
    }
}

public class TimeoutWebClient : WebClient
{
    public int Timeout { get; set; } = 5000; // default 5 seconds

    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = base.GetWebRequest(address);
        request.Timeout = Timeout;
        return request;
    }
}