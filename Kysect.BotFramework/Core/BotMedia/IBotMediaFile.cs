namespace Kysect.BotFramework.Core.BotMedia
{
    public interface IBotMediaFile
    {
        public MediaTypeEnum MediaType { get; }
        public string Path { get; }
    }
}