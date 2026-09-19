using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SocialApp.Desktop;

// Custom panel with rounded corners and shadow effect
public class RoundedPanel : Panel
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int CornerRadius { get; set; } = 15;
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColor { get; set; } = Color.FromArgb(220, 220, 220);
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int BorderWidth { get; set; } = 1;

    public RoundedPanel()
    {
        DoubleBuffered = true;
        BackColor = Color.White;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Draw shadow
        using (var shadowPath = GetRoundedRectPath(new Rectangle(2, 2, Width - 4, Height - 4), CornerRadius))
        using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
        {
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }

        // Draw background
        using (var path = GetRoundedRectPath(new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius))
        using (var brush = new SolidBrush(BackColor))
        {
            e.Graphics.FillPath(brush, path);
        }

        // Draw border
        if (BorderWidth > 0)
        {
            using (var path = GetRoundedRectPath(new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius))
            using (var pen = new Pen(BorderColor, BorderWidth))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }
    }

    private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
