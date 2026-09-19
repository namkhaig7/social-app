using System.Drawing.Imaging;
using SocialApp.Controls;
using SocialApp.Core.Models;
using SocialApp.Images;
using SocialApp.Core.Repositories;
using SocialApp.Core.Services;

namespace SocialApp.Desktop;

public partial class Form1 : Form
{
    private readonly UserService _userService;
    private readonly PostService _postService;
    private readonly User _currentUser;
    private readonly FlowLayoutPanel _feed;
    private readonly TextBox _postInput;
    private readonly Label _pickedImageLabel;

    // Post bur ijil urgentei baival hurees ni vertical-aar tentsene
    private const int CardWidth = 620;

    // Zurag 512x512 bolgoj hadgalagdsan uchir yag tegeeree haruulna
    private const int PostImageSize = 512;

    // Post hiehees omno songoson zurgiin zam
    private string? _pendingImagePath;

    // Logout darval true bolno - Program.cs butsaad login tsonh haruulna
    public bool LoggedOut { get; private set; }

    public Form1(UserService userService, PostService postService, User currentUser)
    {
        InitializeComponent();

        _userService = userService;
        _postService = postService;
        _currentUser = currentUser;

        // Fixed pixel sizes below assume no auto DPI/font rescaling.
        AutoScaleMode = AutoScaleMode.None;
        Font = new Font("Segoe UI", 10F);
        Text = "SocialApp";
        ClientSize = new Size(760, 900);
        MinimumSize = new Size(700, 500);
        BackColor = Color.FromArgb(240, 242, 245);

        // Baruun deed buland: nevtersen hereglegchiin ner + Logout
        var userBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.White,
            Padding = new Padding(16, 12, 16, 12)
        };
        
        var userBarFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false
        };
        
        var logoutButton = new ModernButton 
        { 
            Text = "Log out", 
            BackColor = Color.FromArgb(220, 53, 69),
            HoverBackColor = Color.FromArgb(200, 35, 51),
            ForeColor = Color.White,
            Height = 36
        };
        logoutButton.Click += (_, _) =>
        {
            LoggedOut = true;
            Close();
        };
        
        var userLabel = new Label
        {
            Text = $"👤 {_currentUser.Username}",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, 8, 16, 0),
            ForeColor = Color.FromArgb(50, 50, 50)
        };
        
        userBarFlow.Controls.Add(logoutButton);
        userBarFlow.Controls.Add(userLabel);
        userBar.Controls.Add(userBarFlow);

        // Create post panel with modern styling
        var topPanel = new Panel 
        { 
            Dock = DockStyle.Top, 
            Height = 90, 
            BackColor = Color.White,
            Padding = new Padding(16)
        };
        
        var postContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown
        };
        
        var inputRow = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        _postInput = new TextBox 
        { 
            Width = 480,
            Height = 32,
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.FixedSingle,
            Text = "What's on your mind?"
        };
        _postInput.Enter += (_, _) => { if (_postInput.Text == "What's on your mind?") _postInput.Clear(); };
        _postInput.Leave += (_, _) => { if (string.IsNullOrWhiteSpace(_postInput.Text)) _postInput.Text = "What's on your mind?"; };
        
        var imageButton = new ModernButton 
        { 
            Text = "📷 Image", 
            Margin = new Padding(8, 0, 0, 0),
            BackColor = Color.FromArgb(108, 117, 125),
            HoverBackColor = Color.FromArgb(90, 98, 104),
            ForeColor = Color.White,
            Height = 32
        };
        imageButton.Click += (_, _) => PickImage();
        
        var postButton = new ModernButton 
        { 
            Text = "Post", 
            Margin = new Padding(8, 0, 0, 0),
            BackColor = Color.FromArgb(0, 123, 255),
            HoverBackColor = Color.FromArgb(0, 105, 217),
            ForeColor = Color.White,
            Height = 32
        };
        postButton.Click += (_, _) => CreatePost();
        
        inputRow.Controls.Add(_postInput);
        inputRow.Controls.Add(imageButton);
        inputRow.Controls.Add(postButton);
        
        _pickedImageLabel = new Label 
        { 
            AutoSize = true, 
            Margin = new Padding(0, 8, 0, 0), 
            ForeColor = Color.FromArgb(108, 117, 125),
            Font = new Font("Segoe UI", 9F, FontStyle.Italic)
        };
        
        postContainer.Controls.Add(inputRow);
        postContainer.Controls.Add(_pickedImageLabel);
        topPanel.Controls.Add(postContainer);

        _feed = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        // Tsonhnii hemjee soligdvol post-uudiig dahin golluulna
        _feed.Resize += (_, _) => CenterCards();

        // Dock.Top-uudiig Fill-ees omno nemne. Suuld nemegdsen Top ni dooguur bairlana.
        Controls.Add(_feed);
        Controls.Add(topPanel);
        Controls.Add(userBar);

        RefreshFeed();
    }

    // Zurag songood SocialApp.Images sangaar 512x512 bolgono
    private void PickImage()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Zurag songoh",
            Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*"
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;

        using var original = Image.FromFile(dialog.FileName);

        // Lab 2-iin sangaa ashiglaj bui: jijig zurgiig zowshoorohgui, busdiig 512x512 bolgoj tailna
        using var fitted = SquareImageFitter.Fit(original);
        if (fitted is null)
        {
            MessageBox.Show(
                $"Zurag hetevch jijig baina. Hamgiin baga {SquareImageFitter.TargetSize}x{SquareImageFitter.TargetSize} baih ystoi.",
                "Zurag tohiroogui");
            return;
        }

        var imagesDir = Path.Combine(Application.StartupPath, "images");
        Directory.CreateDirectory(imagesDir);
        var savePath = Path.Combine(imagesDir, $"{Guid.NewGuid():N}.png");
        fitted.Save(savePath, ImageFormat.Png);

        _pendingImagePath = savePath;
        _pickedImageLabel.Text = Path.GetFileName(dialog.FileName);
    }

    private void CreatePost()
    {
        // Zurag songoson bol text hooson baisan ch post hiij bolno
        var text = _postInput.Text == "What's on your mind?" ? "" : _postInput.Text;
        if (string.IsNullOrWhiteSpace(text) && _pendingImagePath is null) return;

        _postService.CreatePost(_currentUser.Id, text, _pendingImagePath);
        _postInput.Text = "What's on your mind?";
        _pendingImagePath = null;
        _pickedImageLabel.Text = "";
        RefreshFeed();
    }

    private void RefreshFeed()
    {
        _feed.Controls.Clear();
        foreach (var post in _postService.GetAllPosts().Reverse())
            _feed.Controls.Add(CreatePostControl(post));
        CenterCards();
    }

    // Image.FromFile ni file-iig barij baidag uchir hulguur uusgej avna
    private static Image LoadImageCopy(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var loaded = Image.FromStream(stream);
        return new Bitmap(loaded);
    }

    // Postuudiig hevtee tenhlegeer golluulna
    private void CenterCards()
    {
        int left = Math.Max(0, (_feed.ClientSize.Width - CardWidth) / 2);
        foreach (Control card in _feed.Controls)
            card.Margin = new Padding(left, 8, 0, 8);
    }

    // Neg post iig haruulah panel: text, reaction bar, share, comment
    private Control CreatePostControl(Post post)
    {
        var author = _userService.GetById(post.AuthorId);

        // Modern rounded card with shadow
        var card = new RoundedPanel
        {
            MinimumSize = new Size(CardWidth, 0),
            MaximumSize = new Size(CardWidth, 0),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(8),
            Padding = new Padding(20),
            BackColor = Color.White,
            CornerRadius = 12,
            BorderColor = Color.FromArgb(230, 230, 230)
        };
        
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        // Author name with timestamp
        var authorLabel = new Label
        {
            Text = $"👤 {author?.Username ?? "Unknown"}",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Color.FromArgb(50, 50, 50)
        };
        layout.Controls.Add(authorLabel);
        
        var timeLabel = new Label
        {
            Text = GetRelativeTime(post.CreatedAt),
            Font = new Font("Segoe UI", 8.5F),
            AutoSize = true,
            ForeColor = Color.FromArgb(150, 150, 150),
            Margin = new Padding(0, -4, 0, 12)
        };
        layout.Controls.Add(timeLabel);

        // Post text
        if (!string.IsNullOrWhiteSpace(post.Text))
        {
            layout.Controls.Add(new Label
            {
                Text = post.Text,
                AutoSize = true,
                MaximumSize = new Size(CardWidth - 60, 0),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(70, 70, 70),
                Margin = new Padding(0, 0, 0, 12)
            });
        }

        // Zuragtai post bol zurgiig ni haruulna (SocialApp.Images-eer 512x512 bolgoson)
        if (post.ImagePath is not null && File.Exists(post.ImagePath))
        {
            var pictureBox = new PictureBox
            {
                Image = LoadImageCopy(post.ImagePath),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(PostImageSize, PostImageSize),
                Margin = new Padding(0, 0, 0, 12)
            };
            layout.Controls.Add(pictureBox);
        }
        
        // Separator line
        var separator = new Panel
        {
            Height = 1,
            Width = CardWidth - 60,
            BackColor = Color.FromArgb(230, 230, 230),
            Margin = new Padding(0, 4, 0, 12)
        };
        layout.Controls.Add(separator);

        //  4 emoji picker
        var picker = new ReactionBar { Visible = false, Margin = new Padding(0, 8, 0, 0), AccessibleName = "ReactionPicker" };

        // zadalj haruulah mor - anhandaa haragdahgui
        var breakdownLabel = new Label 
        { 
            AutoSize = true, 
            Visible = false,
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(120, 120, 120),
            Margin = new Padding(0, 4, 0, 0)
        };

        // Comment bichih talbar - "Comment" darahad l haragdana
        var commentInput = new TextBox 
        { 
            Width = 400,
            Font = new Font("Segoe UI", 9.5F),
            Height = 28
        };
        var commentSubmit = new ModernButton 
        { 
            Text = "Submit",
            Height = 28,
            BackColor = Color.FromArgb(40, 167, 69),
            HoverBackColor = Color.FromArgb(33, 136, 56),
            ForeColor = Color.White
        };
        var commentInputRow = new FlowLayoutPanel 
        { 
            AutoSize = true, 
            WrapContents = false, 
            Visible = false, 
            Margin = new Padding(0, 8, 0, 0) 
        };
        commentInputRow.Controls.Add(commentInput);
        commentInputRow.Controls.Add(commentSubmit);

        // Bui comment-uud - dandaa haragdana
        var commentsLabel = new Label 
        { 
            AutoSize = true, 
            Margin = new Padding(0, 8, 0, 0), 
            Text = FormatComments(post),
            Font = new Font("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(80, 80, 80),
            MaximumSize = new Size(CardWidth - 60, 0)
        };

        // Action buttons with modern styling
        var reactionButton = new ModernButton 
        { 
            Height = 32,
            Margin = new Padding(0, 0, 8, 0)
        };
        var reactionCountLink = new LinkLabel 
        { 
            AutoSize = true, 
            Margin = new Padding(0, 8, 16, 0),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            LinkColor = Color.FromArgb(100, 100, 100),
            ActiveLinkColor = Color.FromArgb(0, 123, 255)
        };
        var commentButton = new ModernButton 
        { 
            Text = $"💬 Comment ({post.Comments.Count})",
            Height = 32,
            Margin = new Padding(0, 0, 8, 0)
        };
        var shareButton = new ModernButton 
        { 
            Text = $"🔄 Share ({post.SharesCount})",
            Height = 32
        };

        void RefreshReactionButton()
        {
            // Show current user's reaction if they reacted, otherwise show default "Like"
            var userReaction = post.GetUserReaction(_currentUser.Id);
            ReactionType displayReaction;
            int displayCount;
            
            if (userReaction.HasValue)
            {
                // User has reacted - show their reaction
                displayReaction = userReaction.Value;
                displayCount = post.Reactions[displayReaction];
            }
            else
            {
                // User hasn't reacted - show default "Like" button
                displayReaction = ReactionType.Like;
                displayCount = post.Reactions[ReactionType.Like];
            }
                
            var emoji = displayReaction switch
            {
                ReactionType.Like => "👍",
                ReactionType.Haha => "😂",
                ReactionType.Sad => "😢",
                ReactionType.Angry => "😠",
                _ => "👍"
            };
            
            // Button shows user's reaction or default Like
            reactionButton.Text = $"{emoji} {displayReaction}";
            reactionButton.ForeColor = ReactionBar.GetColor(displayReaction);
            
            // Link shows total reaction count across all types
            var totalReactions = post.Reactions.Values.Sum();
            reactionCountLink.Text = $"{totalReactions} reactions";
            
            // Breakdown shows all reactions with counts > 0
            breakdownLabel.Text = string.Join("  •  ", post.Reactions.Where(r => r.Value > 0).Select(r => $"{r.Key}: {r.Value}"));
        }
        RefreshReactionButton();

        // "Like" darahad 4 emoji haragdah/aldana
        reactionButton.Click += (_, _) => picker.Visible = !picker.Visible;

        // Toon deer darahad ali hediig emote baigaa iig zadlaj haruulna
        reactionCountLink.Click += (_, _) => breakdownLabel.Visible = !breakdownLabel.Visible;

        // Picker deerh 4 emoji-ii negiig songohod reaction bolgood picker-iig haana
        picker.ReactionSelected += (_, e) =>
        {
            _postService.ReactPost(post.Id, _currentUser.Id, e.Reaction);
            RefreshFeed();
        };

        shareButton.Click += (_, _) =>
        {
            _postService.SharePost(post.Id);
            RefreshFeed();
        };

        // "Comment" darahad text oruulah tal haragdana or alga bolno
        commentButton.Click += (_, _) => commentInputRow.Visible = !commentInputRow.Visible;
        commentSubmit.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(commentInput.Text)) return;
            _postService.CommentOnPost(post.Id, _currentUser.Id, commentInput.Text);
            RefreshFeed();
        };

        var dock = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 0, 0) };
        dock.Controls.Add(reactionButton);
        dock.Controls.Add(commentButton);
        dock.Controls.Add(shareButton);
        
        var statsRow = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 8, 0, 0) };
        statsRow.Controls.Add(reactionCountLink);

        layout.Controls.Add(dock);
        layout.Controls.Add(statsRow);
        layout.Controls.Add(picker);
        layout.Controls.Add(breakdownLabel);
        layout.Controls.Add(commentInputRow);
        layout.Controls.Add(commentsLabel);
        
        card.Controls.Add(layout);
        return card;
    }

    //comment haragdah baidal
    private string FormatComments(Post post)
    {
        if (post.Comments.Count == 0) return "(No comments yet)";

        var lines = post.Comments.Select(c =>
        {
            var author = _userService.GetById(c.AuthorId)?.Username ?? "Unknown";
            return $"💬 {author}: {c.Text}";
        });
        return string.Join("\n", lines);
    }
    
    // Relative time display (e.g., "5 minutes ago")
    private string GetRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        if (timeSpan.TotalMinutes < 1)
            return "just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minutes ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hours ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} days ago";
        
        return dateTime.ToString("MMM dd, yyyy");
    }
}
