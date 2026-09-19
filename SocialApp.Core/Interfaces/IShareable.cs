namespace SocialApp.Core.Interfaces;

/// Shareable
public interface IShareable
{
    int SharesCount { get; }

    void Share();
}
