namespace SocialApp.Core.Models;
//Yamarvaa content edgeer zuilstei

public abstract class Content
{
    public int Id { get; }
    public int AuthorId { get; }
    public string Text { get; }
    public DateTime CreatedAt { get; }

    // createdAt - DB-ees unshihad tsagiig ni butsaaj tavihad heregtei.
    // Shineer uusgehed null ugvul odoogiin tsag avna.
    protected Content(int id, int authorId, string text, DateTime? createdAt = null)
    {
        Id = id;
        AuthorId = authorId;
        Text = text;
        CreatedAt = createdAt ?? DateTime.Now;
    }

    public abstract string Preview();
}
