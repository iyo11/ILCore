using ILCore.OAuth;

const string clientId = "288ec5dd-6736-4d4b-9b96-30e083a8cad2";
const string redirectUri = "http://localhost:29116/authentication-response";
//string _scope = "XboxLive.signin offline_access";

var minecraftOAuth2 = new MicrosoftOAuth2(clientId:clientId,redirectUri:redirectUri);
var userProfile = await minecraftOAuth2.AuthorizeAsync();
Console.WriteLine(userProfile.Id);
