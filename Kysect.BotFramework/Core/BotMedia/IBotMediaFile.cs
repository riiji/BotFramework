namespace Kysect.BotFramework.Core.BotMedia
{
    public interface IBotMediaFile
    {
        MediaTypeEnum MediaType { get; }
        string Path { get; }
    }
}