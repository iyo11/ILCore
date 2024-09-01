namespace ILCore.Minecraft.Assets;

public class AssetObject
{
    public string Hash { get; set; }

    public string Path => $"{Hash[..2]}";
    public string Url => $"{Hash[..2]}/{Hash}";

    public int Size { get; set; }
}

public class JsonAsset
{
    public Dictionary<string, AssetObject> Objects { get; set; }
}