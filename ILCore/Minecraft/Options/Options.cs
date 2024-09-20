namespace ILCore.Minecraft.Options;

public class Options
{
    public async Task SetOptions(string minecraftPath, string versionName, Dictionary<string, string> newOptions)
    {
        var optionsFile = Path.Combine(minecraftPath, "versions", versionName, "options.txt");
        if (!File.Exists(optionsFile))
            File.Create(optionsFile);
        var optionsText = await File.ReadAllTextAsync(optionsFile);
        var options = ToOptions(optionsText);
        foreach (var item in newOptions) options[item.Key] = item.Value;
        optionsText = ToText(options);
        await File.WriteAllTextAsync(optionsFile, optionsText);
    }


    public Dictionary<string, string> ToOptions(string optionText)
    {
        if (string.IsNullOrWhiteSpace(optionText)) return new Dictionary<string, string>();
        var items = new Dictionary<string, string>();
        using var reader = new StringReader(optionText);
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(':');
            if (parts.Length != 2) continue;
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            items.Add(key, value);
        }

        return items;
    }

    public string ToText(Dictionary<string, string> options)
    {
        return string.Join("\n", options.Select(x => $"{x.Key}:{x.Value}"));
    }
}