namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotOnlinePhotoFile : IBotOnlineFile
    {
        public BotOnlinePhotoFile(string path)
        {
            Path = path;
        }

        public BotOnlinePhotoFile(string path, string id)
        {
            Path = path;
            Id = id;
        }

        public MediaTypeEnum MediaType => MediaTypeEnum.Photo;
        public string Path { get; }
        public string Id { get; }
    }
}