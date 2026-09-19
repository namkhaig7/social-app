namespace SocialApp.Core.Interfaces;

/// <summary>Something that can be liked (post, comment, ...).</summary>
public interface ILikeable
{
    int LikesCount { get; }

    void Like();
    void Unlike();
}
