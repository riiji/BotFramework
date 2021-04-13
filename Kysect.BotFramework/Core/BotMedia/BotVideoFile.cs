namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotVideoFile : IBotMediaFile
    {
        public string MediaType { get; } = "video";
        public string Path { get; }

        public BotVideoFile(string path)
        {
            Path = path;
        }
    }
}