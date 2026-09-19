using System.ComponentModel;
using System.Drawing.Drawing2D;
using SocialApp.Core.Models;

namespace SocialApp.Controls;

// FB shig 4 reaction (Like, Haha, Sad, Angry) haruulj, darahad ReactionSelected event gargadag custom control
public class ReactionBar : Control
{
    private static readonly ReactionType[] AllReactions =
    [
        ReactionType.Like,
        ReactionType.Haha,
        ReactionType.Sad,
        ReactionType.Angry
    ];

    private int _iconSize = 36;
    private int _spacing = 6;

    // Property 1: neg reaction iconii diameter (px)
    [DefaultValue(36)]
    public int IconSize
    {
        get => _iconSize;
        set
        {
            _iconSize = Math.Max(12, value);
            UpdateControlSize();
            Invalidate();
        }
    }

    // Property 2: icon buriin hoorondoh zai (px)
    [DefaultValue(6)]
    public int Spacing
    {
        get => _spacing;
        set
        {
            _spacing = Math.Max(0, value);
            UpdateControlSize();
            Invalidate();
        }
    }

    // Custom event - standard Click event bish, yag ene controlyn uildel (aль reaction darsan)
    public event EventHandler<ReactionSelectedEventArgs>? ReactionSelected;

    public ReactionBar()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        Cursor = Cursors.Hand;
        UpdateControlSize();
    }

    private void UpdateControlSize()
    {
        int width = AllReactions.Length * _iconSize + (AllReactions.Length + 1) * _spacing;
        int height = _iconSize + _spacing * 2;
        Size = new Size(width, height);
    }

    private Rectangle GetIconBounds(int index)
    {
        int x = _spacing + index * (_iconSize + _spacing);
        return new Rectangle(x, _spacing, _iconSize, _iconSize);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        for (int i = 0; i < AllReactions.Length; i++)
            DrawFace(e.Graphics, AllReactions[i], GetIconBounds(i));
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        for (int i = 0; i < AllReactions.Length; i++)
        {
            if (GetIconBounds(i).Contains(e.Location))
            {
                ReactionSelected?.Invoke(this, new ReactionSelectedEventArgs(AllReactions[i]));
                return;
            }
        }
    }

    // Reaction buriin ongo - ReactionBar болон busad UI 
    public static Color GetColor(ReactionType reaction) => reaction switch
    {
        ReactionType.Like => Color.FromArgb(66, 133, 244), // blue
        ReactionType.Haha => Color.Gold,                   // yellow
        ReactionType.Sad => Color.MediumPurple,            // purple
        ReactionType.Angry => Color.Red,
        _ => Color.LightGray
    };

    // Neg reaction-ii nuur zurna: tойрог (nuur) + nud + am, turul buriin am/nud ondoo
    private static void DrawFace(Graphics g, ReactionType reaction, Rectangle bounds)
    {
        var faceColor = GetColor(reaction);

        using (var faceBrush = new SolidBrush(faceColor))
            g.FillEllipse(faceBrush, bounds);
        g.DrawEllipse(Pens.Black, bounds);

        int eyeSize = Math.Max(3, bounds.Width / 8);
        int eyeY = bounds.Top + bounds.Height / 3;
        int leftEyeX = bounds.Left + bounds.Width / 4;
        int rightEyeX = bounds.Right - bounds.Width / 4 - eyeSize;

        if (reaction == ReactionType.Angry)
        {
            using var browPen = new Pen(Color.Black, 2);
            g.DrawLine(browPen, leftEyeX - 2, eyeY - 3, leftEyeX + eyeSize + 3, eyeY - 8);
            g.DrawLine(browPen, rightEyeX - 3, eyeY - 8, rightEyeX + eyeSize + 2, eyeY - 3);
        }

        g.FillEllipse(Brushes.Black, leftEyeX, eyeY, eyeSize, eyeSize);
        g.FillEllipse(Brushes.Black, rightEyeX, eyeY, eyeSize, eyeSize);

        if (reaction == ReactionType.Sad)
        {
            using var tearBrush = new SolidBrush(Color.FromArgb(120, 180, 255));
            g.FillEllipse(tearBrush, rightEyeX, eyeY + eyeSize, eyeSize / 2, eyeSize);
        }

        var mouthRect = new Rectangle(
            bounds.Left + bounds.Width / 5,
            bounds.Top + bounds.Height / 2,
            bounds.Width * 3 / 5,
            bounds.Height / 3);

        using var mouthPen = new Pen(Color.Black, 2);
        switch (reaction)
        {
            case ReactionType.Like:
                g.DrawArc(mouthPen, mouthRect, 0, 180); // inaad
                break;
            case ReactionType.Haha:
                g.FillEllipse(Brushes.Black, mouthRect); // ihtei neesen am
                break;
            case ReactionType.Sad:
                g.DrawArc(mouthPen, mouthRect, 180, 180); // gomdson am
                break;
            case ReactionType.Angry:
                g.DrawLine(mouthPen, mouthRect.Left, mouthRect.Top, mouthRect.Right, mouthRect.Top); // shulen am
                break;
        }
    }
}
