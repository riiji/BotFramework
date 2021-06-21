namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotPhotoFile : IBotMediaFile
    {
        public BotPhotoFile(string path)
        {
            Path = path;
        }

        public MediaTypeEnum MediaType => MediaTypeEnum.Photo;
        public string Path { get; }
    }
}