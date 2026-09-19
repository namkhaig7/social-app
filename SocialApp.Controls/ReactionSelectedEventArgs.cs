using SocialApp.Core.Models;

namespace SocialApp.Controls;

// Hereglegch ali reaction darsan iig damjuulna
public class ReactionSelectedEventArgs(ReactionType reaction) : EventArgs
{
    public ReactionType Reaction { get; } = reaction;
}
