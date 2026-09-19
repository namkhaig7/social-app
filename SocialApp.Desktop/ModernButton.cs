using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SocialApp.Desktop;

// Modern flat button with hover effects
public class ModernButton : Button
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color HoverBackColor { get; set; } = Color.FromArgb(240, 240, 240);
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color PressedBackColor { get; set; } = Color.FromArgb(220, 220, 220);
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int CornerRadius { get; set; } = 8;

    private bool _isHovered;
    private bool _isPressed;

    public ModernButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.FromArgb(245, 245, 245);
        ForeColor = Color.FromArgb(50, 50, 50);
        Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        Cursor = Cursors.Hand;
        Padding = new Padding(12, 6, 12, 6);
        AutoSize = true;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        _isPressed = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        base.OnMouseDown(mevent);
        _isPressed = true;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        base.OnMouseUp(mevent);
        _isPressed = false;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        var g = pevent.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, Width, Height);
        Color bgColor = _isPressed ? PressedBackColor : (_isHovered ? HoverBackColor : BackColor);

        using (var path = GetRoundedRectPath(rect, CornerRadius))
        using (var brush = new SolidBrush(bgColor))
        {
            g.FillPath(brush, path);
        }

        // Draw text
        TextRenderer.DrawText(g, Text, Font, rect, ForeColor, 
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
