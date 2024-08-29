public class AssetObject
{
    public string Hash { get; set; }

    public string Path => $"{Hash[..2]}/{Hash}";

    public int Size { get; set; }
}

public class JsonAsset
{
    public Dictionary<string, AssetObject> objects { get; set; }
}