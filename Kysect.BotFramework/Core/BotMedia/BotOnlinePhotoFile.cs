namespace Kysect.BotFramework.Core.BotMedia
{
    public class BotOnlinePhotoFile : IBotOnlineFile
    {
        public MediaTypeEnum MediaType { get; } = MediaTypeEnum.Photo;
        public string Path { get; }
        public string Id { get; }

        public BotOnlinePhotoFile(string path)
        {
            Path = path;
        }
        public BotOnlinePhotoFile(string path, string id)
        {
            Path = path;
            Id = id;
        }
    }
}