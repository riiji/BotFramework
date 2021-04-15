namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotVideoFile : IBotMediaFile
    {
        public MediaTypeEnum MediaType { get; } = MediaTypeEnum.Video;
        public string Path { get; }

        public BotVideoFile(string path)
        {
            Path = path;
        }
    }
}