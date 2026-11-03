// ---------------------------------------------------------------
//  Eigene Edition
//
//  Traegt beim Serverstart eine selbst zusammengestellte Edition in
//  die Profilvorlagen ein -- zusaetzlich zu Standard, Unheard,
//  „SPT Zero to hero“ und den anderen.
//
//  Warum ein Mod und nicht einfach ein Eintrag in
//  SPT_Data/database/templates/profiles.json:
//  SPT prueft seit 4.1 beim Start die Pruefsummen seiner
//  Datenbankdateien. Eine geaenderte profiles.json bricht den Start
//  mit „Dateiüberprüfung fehlgeschlagen“ ab. Mods duerfen dieselbe
//  Tabelle dagegen zur Laufzeit veraendern -- dafuer sind sie da.
//
//  Die Edition selbst baut das SPT Kontrollzentrum zusammen und legt
//  sie als edition.json neben diese DLL. Dieser Mod liest die Datei
//  nur ein: so bleibt alles Einstellbare in der Oberflaeche, und hier
//  muss nach einem SPT-Update nichts nachgezogen werden.
// ---------------------------------------------------------------
using System.Text.Json;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Utils;

// „Path“ gibt es auch in den EFT-Modellen (Wegpunkte) - Alias setzen.
using Pfad = System.IO.Path;

namespace EigeneEdition;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.kaan.eigeneedition";
    public string Name { get; init; } = "Eigene Edition";
    public string Author { get; init; } = "Kaan";
    public List<string>? Contributors { get; init; } = new();
    public SemanticVersioning.Version Version { get; init; } = new(1, 0, 0);
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1");
    public bool HasPrepatcher { get; init; } = false;
    public List<string>? Incompatibilities { get; init; } = new();
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = new();
    public string? Url { get; init; } = "";
    public string License { get; init; } = "MIT";
}

[Injectable(InjectionType.Singleton)]
public class EditionsLader(
    ISptLogger<EditionsLader> logger,
    TemplateTable vorlagen,
    JsonUtil json) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken abbruch)
    {
        try
        {
            var ordner = Pfad.GetDirectoryName(typeof(EditionsLader).Assembly.Location);
            var datei = Pfad.Combine(ordner ?? ".", "edition.json");
            if (!File.Exists(datei))
            {
                logger.Info("[Eigene Edition] Keine edition.json gefunden - "
                            + "es bleibt bei den mitgelieferten Editionen.");
                return Task.CompletedTask;
            }

            using var papier = JsonDocument.Parse(File.ReadAllText(datei));
            var wurzel = papier.RootElement;
            var name = wurzel.TryGetProperty("name", out var n)
                ? n.GetString() ?? "Eigene Edition"
                : "Eigene Edition";

            if (!wurzel.TryGetProperty("edition", out var roh))
            {
                logger.Error("[Eigene Edition] In der edition.json fehlt der "
                             + "Abschnitt „edition“.");
                return Task.CompletedTask;
            }

            // Ueber JsonUtil, damit dieselben Wandler greifen wie beim
            // Einlesen der Datenbank (MongoId und Konsorten).
            var edition = json.Deserialize<ProfileSides>(roh.GetRawText());
            if (edition is null)
            {
                logger.Error("[Eigene Edition] Die edition.json ließ sich "
                             + "nicht lesen.");
                return Task.CompletedTask;
            }

            if (wurzel.TryGetProperty("beschreibung", out var b))
            {
                var text = b.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    edition.DescriptionLocaleKey = text;
                }
            }

            vorlagen.Profiles[name] = edition;
            logger.Success($"[Eigene Edition] „{name}“ steht zur Auswahl.");
        }
        catch (Exception ex)
        {
            logger.Error("[Eigene Edition] " + ex.Message);
        }

        return Task.CompletedTask;
    }
}
