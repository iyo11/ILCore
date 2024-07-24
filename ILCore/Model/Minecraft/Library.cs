using ILCore.Model.Enum;
using ILCore.Model.JsonObject;

namespace ILCore.Model.Minecraft
{
    public class Library
    {
        public string Name { get; set; }

        public LibraryType Type { get; set; }

        public string Path { get; set; }

        public int Size { get; set; }

        public string Sha1 { get; set; }

        public string Url { get; set; }

        public string[] Exclude { get; set; }

        public JsonRule[] Rules { get; set; }
    }
}
