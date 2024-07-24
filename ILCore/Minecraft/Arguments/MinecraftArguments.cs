using ILCore.Model.JsonObject;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ILCore.Model.Enum;

namespace ILCore.Minecraft.Arguments
{
    public class MinecraftArguments
    {
        private readonly string _userName;
        private readonly string _versionName;
        private readonly string _gameDir;
        private readonly string _assetDir;
        private readonly string _assetsIndex;
        private readonly string _uuid;
        private readonly string _accessToken;
        private readonly UserType _userType;
        private readonly string _userProperties;
        private readonly string _versionType;
        public MinecraftArguments(string userName, string versionName, string gameDir, string assetDir, string assetsIndex, string uuid, string accessToken, UserType userType, string userProperties, string versionType)
        {
            _userName = userName;
            _versionName = versionName;
            _gameDir = gameDir;
            _assetDir = assetDir;
            _assetsIndex = assetsIndex;
            _uuid = uuid;
            _accessToken = accessToken;
            _userType = userType;
            _userProperties = userProperties;
            _versionType = versionType;
        }

        public string ToMinecraftArguments(JObject versionObj)
        {
            var gameArguments = "";
            if (versionObj["minecraftArguments"] is not null)
            {
                gameArguments = versionObj["minecraftArguments"].ToString();
                var argumentsStrings = gameArguments.Split(" ");
                gameArguments = PutMinecraftArguments(argumentsStrings);
            }

            var jArgumentsToken = versionObj["arguments"];

            if (jArgumentsToken is not null)
            {
                var arguments = JsonConvert.DeserializeObject<JsonArguments>(jArgumentsToken.ToString());
                gameArguments = PutMinecraftArguments(arguments.Game.OfType<string>().ToArray());
            }
            return gameArguments;
        }

        private string PutMinecraftArguments(IList<string> argumentsStrings)
        {

            for (var i = 0; argumentsStrings.Count > i; i += 2)
            {
                argumentsStrings[i + 1] = argumentsStrings[i + 1] switch
                {
                    "${auth_player_name}" => _userName,
                    "${version_name}" => _versionName,
                    "${game_directory}" => _gameDir,
                    "${game_assets}" => _assetDir,
                    "${assets_root}" => _assetDir,
                    "${assets_index_name}" => _assetsIndex,
                    "${auth_uuid}" => _uuid,
                    "${auth_access_token}" => _accessToken,
                    "${user_type}" => _userType.ToString(),
                    "${user_properties}" => _userProperties,
                    "${version_type}" => _versionType,
                    _ => argumentsStrings[i + 1]
                };
            }

            return string.Join(" ", argumentsStrings);
        }
    }
}
