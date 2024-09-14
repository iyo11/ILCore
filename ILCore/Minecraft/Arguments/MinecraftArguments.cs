using ILCore.Launch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Arguments;

public class MinecraftArguments(
    string userName,
    string versionName,
    string gameDir,
    string assetsDir,
    string assetsIndex,
    string uuid,
    string accessToken,
    UserType userType,
    string userProperties,
    string versionType)
{
    public string ToMinecraftArguments(JObject versionObj)
    {
        var gameArguments = "";
        if (versionObj["minecraftArguments"] is not null)
        {
            gameArguments = versionObj["minecraftArguments"].ToString();
            var argumentsStrings = gameArguments.Split(" ");
            gameArguments = FormatMinecraftArguments(argumentsStrings);
        }

        var jArgumentsToken = versionObj["arguments"];

        if (jArgumentsToken is null) return gameArguments;
        var arguments = JsonConvert.DeserializeObject<JsonArguments>(jArgumentsToken.ToString());
        gameArguments = FormatMinecraftArguments(arguments.Game.OfType<string>().ToArray());
        return gameArguments;
    }

    private string FormatMinecraftArguments(IList<string> argumentsStrings)
    {
        for (var i = 0; argumentsStrings.Count > i; i += 2)
            argumentsStrings[i + 1] = argumentsStrings[i + 1] switch
            {
                "${auth_player_name}" => userName,
                "${version_name}" => versionName,
                "${game_directory}" => gameDir,
                "${game_assets}" => assetsDir,
                "${assets_root}" => assetsDir,
                "${assets_index_name}" => assetsIndex,
                "${auth_uuid}" => uuid,
                "${auth_access_token}" => accessToken,
                "${user_type}" => userType.ToString(),
                "${user_properties}" => userProperties,
                "${version_type}" => versionType,
                _ => argumentsStrings[i + 1]
            };

        return string.Join(" ", argumentsStrings);
    }
}