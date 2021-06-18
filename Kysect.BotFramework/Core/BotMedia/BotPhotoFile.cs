namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotPhotoFile : IBotMediaFile
    {
        public MediaTypeEnum MediaType => MediaTypeEnum.Photo;
        public string Path { get; }

        public BotPhotoFile(string path)
        {
            Path = path;
        }
    }
}