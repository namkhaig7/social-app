using SocialApp.Core.Interfaces;

namespace SocialApp.Core.Models;


public class Post : Content, IReactable, ICommentable, IShareable
{
    private readonly List<Comment> _comments = [];

    // Hereglegch buriin ODOOgiin reaction - neg hun neg l reactiontai baina (FB shig).
    // Ug hun deed shineer darval, ehnii reaction ni songosnoor solig dana.
    private readonly Dictionary<int, ReactionType> _userReactions = new();

    public int SharesCount { get; private set; }
    public IReadOnlyList<Comment> Comments => _comments;

    public IReadOnlyDictionary<ReactionType, int> Reactions =>
        Enum.GetValues<ReactionType>().ToDictionary(type => type, type => _userReactions.Values.Count(r => r == type));

    // Zurgiig path-aar ni hadgalna (Core ni Windows/GDI+ -ees hamaarahgui)
    public string? ImagePath { get; }

    public Post(
        int id,
        int authorId,
        string text,
        string? imagePath = null,
        int sharesCount = 0,
        DateTime? createdAt = null)
        : base(id, authorId, text, createdAt)
    {
        ImagePath = imagePath;
        SharesCount = sharesCount;
    }

    public void React(int userId, ReactionType type) => _userReactions[userId] = type;

    public void Share() => SharesCount++;

    public void AddComment(Comment comment) => _comments.Add(comment);

    public override string Preview() => $"Post: \"{Text}\"";
}
