using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Versions;

public interface IVersionsLocal
{

    JObject ToVersionJObject(string json);

}