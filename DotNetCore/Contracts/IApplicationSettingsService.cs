namespace DotNetCore.Contracts
{
    public interface IApplicationSettingsService
    {
        int PageSize { get; }

        int MaxArticleFeedbackAttempts { get; }
    }
}
