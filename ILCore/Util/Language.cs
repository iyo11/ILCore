using System.Reflection;
using System.Xml;
using ILCore.Languages;

namespace ILCore.Util;

public class XmlDictionaryResource
{
    public Dictionary<string, string> XmlDictionary { get; } = [];
}


public static class Language
{
    private const string LanguagesNamespace = "ILCore.Languages.Langs";
    
    private static readonly Dictionary<string, XmlDictionaryResource> LanguageResource = [];
    private static AvailableLanguages _currentLanguage = AvailableLanguages.en_US;
    
    
    static Language()
    => SetLanguage(Enum.TryParse<AvailableLanguages>(EnvironmentRuntime.Lang, out var currentLanguage)
            ? currentLanguage
            : AvailableLanguages.en_US);
    

    private static XmlDictionaryResource LoadXmlLanguage(AvailableLanguages availableLanguages)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{LanguagesNamespace}.{availableLanguages}.xml");
        if (stream == null) return null;
        using var reader = new StreamReader(stream);
        var xmlContent = reader.ReadToEnd();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlContent);
        var xmlDictionaryResource = new XmlDictionaryResource();
        var nodes = xmlDoc.SelectNodes("//Resource");
        if (nodes == null) return null;
        foreach (XmlNode node in nodes)
        {
            if (node.Attributes?["Key"] == null) continue;
            var key = node.Attributes["Key"].Value;
            var value = node.InnerText;
            xmlDictionaryResource.XmlDictionary.Add(key, value);
        }
        return xmlDictionaryResource;
    }

    public static void SetLanguage(AvailableLanguages languageName)
    {
        if (!LanguageResource.TryAdd(languageName.ToString(), LoadXmlLanguage(languageName)))
        {
            LanguageResource.TryAdd("en-US", LoadXmlLanguage(AvailableLanguages.en_US));
            _currentLanguage = AvailableLanguages.en_US;
        }
        LanguageResource.Remove(_currentLanguage.ToString());
        _currentLanguage = languageName;
    }

    public static string GetValue(string key, params object[] args)
    {
        if (!LanguageResource.TryGetValue(_currentLanguage.ToString(), out var xmlDictionaryResource))
            xmlDictionaryResource = LoadXmlLanguage(AvailableLanguages.en_US);
        return xmlDictionaryResource.XmlDictionary.TryGetValue(key, out var value) ? string.Format(value, args) : $"{key}";
    }
}