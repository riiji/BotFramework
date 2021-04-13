namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotPhotoFile :IBotMediaFile
    {
        public string MediaType { get; } = "photo";
        public string Path { get; }

        public BotPhotoFile(string path)
        {
            Path = path;
        }
    }
}