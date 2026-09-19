namespace SocialApp.Core.Interfaces;

/// <summary>Something that can be shared/reposted.</summary>
public interface IShareable
{
    int SharesCount { get; }

    void Share();
}
