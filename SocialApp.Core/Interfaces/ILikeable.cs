namespace SocialApp.Core.Interfaces;


//Like darj boloh interface
public interface ILikeable
{
    int LikesCount { get; }

    void Like();
    void Unlike();
}
