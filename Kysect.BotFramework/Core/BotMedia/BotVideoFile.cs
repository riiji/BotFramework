namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotVideoFile : IBotMediaFile
    {
        public BotVideoFile(string path)
        {
            Path = path;
        }

        public MediaTypeEnum MediaType => MediaTypeEnum.Video;
        public string Path { get; }
    }
}