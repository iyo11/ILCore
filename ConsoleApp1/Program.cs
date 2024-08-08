﻿/*
const string clientId = "288ec5dd-6736-4d4b-9b96-30e083a8cad2";
const string redirectUri = "http://localhost:29116/authentication-response";
const string scope = "XboxLive.signin offline_access";

var minecraftOAuth2 = new MicrosoftOAuth2(clientId:clientId,redirectUri:redirectUri,new RedirectMessage(),scope);
var userProfile = await minecraftOAuth2.AuthorizeAsync();
Console.WriteLine(userProfile.MinecraftToken);
*/


using ILCore.Languages;
using ILCore.Launch;
using ILCore.Minecraft.Libraries;
using ILCore.OAuth;
using ILCore.OAuth.MinecraftOAuth;
using ILCore.OAuth.RedirectUri;
using ILCore.Util;

const string javaPath = @"C:\Program Files\Java\jdk1.8.0_202\jre\bin\javaw.exe";
//const string javaPath = @"C:\Program Files\Java\jdk-11\bin\javaw.exe";
const string minecraftPath = @"G:\Minecraft\.minecraft";
//Fail


//Success

const string versionName = "1.12.2-Forge_14.23.5.2859";


const string maxMemory = "4096";
const string jvmArgs = " -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true";

const string clientId = "288ec5dd-6736-4d4b-9b96-30e083a8cad2";
const string redirectUri = "http://localhost:29116/authentication-response";
const string scope = "XboxLive.signin offline_access";

var minecraftOAuth2 = new MicrosoftOAuth2(clientId:clientId,uri:redirectUri,new RedirectMessage());
//var userProfile = await minecraftOAuth2.AuthorizeAsync();

var userProfile = new LegacyUserProfile { Name = "IYO" };


LaunchArgs launchArgs = new();
var info = new LaunchInfo
{
    VersionName = versionName,
    AbsoluteVersion = true,
    MinecraftPath = minecraftPath,
    MaxMemory = maxMemory,
    JvmArgs = jvmArgs,
    UserProfile = userProfile,
    LauncherName = "ILCore",
    LauncherVersion = "001",
    CustomArgs = "ILCore",
    Fullscreen = false,
    WindowWidth = 1280,
    WindowHeight = 760
};
var launchArg = await launchArgs.PrepareArguments(info);

await new Natives().Extract(minecraftPath, versionName);
var minecraftProcess = new MinecraftProcessBuilder().BuildProcess(javaPath,launchArg);

minecraftProcess.MinecraftLogOutPut += (sender, minecraftLog) =>
{
    //Console.WriteLine(minecraftLog.ToString());
};

minecraftProcess.MinecraftLogCrash += (sender, errors) => 
{
    foreach (var log in errors)
    {
        Console.WriteLine(log);
    }
};

await minecraftProcess.Start();

