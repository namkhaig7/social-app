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
        Font = new Font("Segoe UI", 11F);
        Text = "SocialApp";
        ClientSize = new Size(760, 900);
        MinimumSize = new Size(700, 500);

        // Baruun deed buland: nevtersen hereglegchiin ner + Logout
        var userBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 44,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(8, 6, 8, 0)
        };
        var logoutButton = new Button { Text = "Log out", AutoSize = true };
        logoutButton.Click += (_, _) =>
        {
            LoggedOut = true;
            Close();
        };
        var userLabel = new Label
        {
            Text = _currentUser.Username,
            Font = new Font(Font, FontStyle.Bold),
            AutoSize = true,
            Margin = new Padding(0, 6, 8, 0)
        };
        userBar.Controls.Add(logoutButton);
        userBar.Controls.Add(userLabel);

        // FlowLayoutPanel
        var topPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, WrapContents = false, Padding = new Padding(8) };
        _postInput = new TextBox { Width = 330 };
        var imageButton = new Button { Text = "Add Image", AutoSize = true, Margin = new Padding(8, 0, 0, 0) };
        imageButton.Click += (_, _) => PickImage();
        var postButton = new Button { Text = "Post", AutoSize = true, Margin = new Padding(8, 0, 0, 0) };
        postButton.Click += (_, _) => CreatePost();
        _pickedImageLabel = new Label { AutoSize = true, Margin = new Padding(8, 6, 0, 0), ForeColor = Color.Gray };
        topPanel.Controls.Add(_postInput);
        topPanel.Controls.Add(imageButton);
        topPanel.Controls.Add(postButton);
        topPanel.Controls.Add(_pickedImageLabel);

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
        if (string.IsNullOrWhiteSpace(_postInput.Text) && _pendingImagePath is null) return;

        _postService.CreatePost(_currentUser.Id, _postInput.Text, _pendingImagePath);
        _postInput.Clear();
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

        // Min/Max urgeniig tentsuu bolgood urgenii ni tsoolno - ingesneer post bur ijil
        // urguntei bolj huree ni vertical-aar tentsene. Undur ni aguulgaasaa hamaarna
        var layout = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(CardWidth, 0),
            MaximumSize = new Size(CardWidth, 0),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(8),
            Padding = new Padding(10)
        };

        layout.Controls.Add(new Label
        {
            Text = author?.Username ?? "Unknown",
            Font = new Font(Font, FontStyle.Bold),
            AutoSize = true
        });

        layout.Controls.Add(new Label
        {
            Text = post.Text,
            AutoSize = true,
            MaximumSize = new Size(CardWidth - 40, 0)
        });

        // Zuragtai post bol zurgiig ni haruulna (SocialApp.Images-eer 512x512 bolgoson)
        if (post.ImagePath is not null && File.Exists(post.ImagePath))
        {
            layout.Controls.Add(new PictureBox
            {
                Image = LoadImageCopy(post.ImagePath),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(PostImageSize, PostImageSize),
                Margin = new Padding(0, 6, 0, 6)
            });
        }

        //  4 emoji picker
        var picker = new ReactionBar { Visible = false, Margin = new Padding(0, 4, 0, 0), AccessibleName = "ReactionPicker" };

        // zadalj haruulah mor - anhandaa haragdahgui
        var breakdownLabel = new Label { AutoSize = true, Visible = false };

        // Comment bichih talbar - "Comment" darahad l haragdana
        var commentInput = new TextBox { Width = 400 };
        var commentSubmit = new Button { Text = "Submit", AutoSize = true };
        var commentInputRow = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Visible = false, Margin = new Padding(0, 4, 0, 0) };
        commentInputRow.Controls.Add(commentInput);
        commentInputRow.Controls.Add(commentSubmit);

        // Bui comment-uud - dandaa haragdana
        var commentsLabel = new Label { AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = FormatComments(post) };

        // Dock: [Like N] [N] [Comment N] [Share N]
        var reactionButton = new Button { AutoSize = true };
        var reactionCountLink = new LinkLabel { AutoSize = true, Margin = new Padding(0, 6, 12, 0) };
        var commentButton = new Button { AutoSize = true, Text = $"Comment {post.Comments.Count}" };
        var shareButton = new Button { AutoSize = true, Text = $"Share {post.SharesCount}" };

        void RefreshReactionButton()
        {
            var dominant = post.Reactions.OrderByDescending(r => r.Value).First();
            reactionButton.Text = dominant.Key.ToString();
            reactionButton.ForeColor = ReactionBar.GetColor(dominant.Key);
            reactionCountLink.Text = dominant.Value.ToString();
            breakdownLabel.Text = string.Join("   ", post.Reactions.Select(r => $"{r.Key}: {r.Value}"));
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
            RefreshReactionButton();
            picker.Visible = false;
        };

        shareButton.Click += (_, _) =>
        {
            _postService.SharePost(post.Id);
            shareButton.Text = $"Share {post.SharesCount}";
        };

        // "Comment" darahad text oruulah tal haragdana or alga bolno
        commentButton.Click += (_, _) => commentInputRow.Visible = !commentInputRow.Visible;
        commentSubmit.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(commentInput.Text)) return;
            _postService.CommentOnPost(post.Id, _currentUser.Id, commentInput.Text);
            commentsLabel.Text = FormatComments(post);
            commentButton.Text = $"Comment {post.Comments.Count}";
            commentInput.Clear();
        };

        var dock = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 8, 0, 0) };
        dock.Controls.Add(reactionButton);
        dock.Controls.Add(reactionCountLink);
        dock.Controls.Add(commentButton);
        dock.Controls.Add(shareButton);

        layout.Controls.Add(dock);
        layout.Controls.Add(picker);
        layout.Controls.Add(breakdownLabel);
        layout.Controls.Add(commentInputRow);
        layout.Controls.Add(commentsLabel);

        return layout;
    }

    //comment haragdah baidal
    private string FormatComments(Post post)
    {
        if (post.Comments.Count == 0) return "(No comments yet)";

        var lines = post.Comments.Select(c =>
        {
            var author = _userService.GetById(c.AuthorId)?.Username ?? "Unknown";
            return $"{author} : {c.Text}";
        });
        return string.Join("\n", lines);
    }
}
