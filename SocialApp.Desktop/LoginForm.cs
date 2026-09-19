using SocialApp.Core.Models;
using SocialApp.Core.Services;

namespace SocialApp.Desktop;

// Sign in / Sign up tsonh.
// Sign in horm: zowhon username + password.
// Sign up horm deer l nemeed nas asuuna.
public class LoginForm : Form
{
    private readonly UserService _userService;
    private readonly TextBox _usernameInput;
    private readonly TextBox _passwordInput;
    private readonly TextBox _ageInput;
    private readonly Label _ageLabel;
    private readonly Button _primaryButton;
    private readonly Button _switchButton;

    private bool _signUpMode;

    // Amjilttai nevtersen hereglegch (amjiltgui bol null)
    public User? SignedInUser { get; private set; }

    public LoginForm(UserService userService)
    {
        _userService = userService;

        AutoScaleMode = AutoScaleMode.None;
        Font = new Font("Segoe UI", 10F);
        ClientSize = new Size(450, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(240, 242, 245);
        
        // Main container with padding
        var container = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(40, 30, 40, 30),
            BackColor = Color.Transparent
        };
        
        // Title
        var titleLabel = new Label
        {
            Text = "SocialApp",
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            AutoSize = true,
            Left = 40,
            Top = 30,
            ForeColor = Color.FromArgb(0, 123, 255)
        };
        container.Controls.Add(titleLabel);

        Controls.Add(new Label 
        { 
            Text = "Username", 
            Left = 40, 
            Top = 90, 
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(70, 70, 70)
        });
        _usernameInput = new TextBox 
        { 
            Left = 40, 
            Top = 115, 
            Width = 370,
            Height = 32,
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.FixedSingle
        };
        container.Controls.Add(_usernameInput);

        Controls.Add(new Label 
        { 
            Text = "Password", 
            Left = 40, 
            Top = 160, 
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(70, 70, 70)
        });
        _passwordInput = new TextBox 
        { 
            Left = 40, 
            Top = 185, 
            Width = 370,
            Height = 32,
            Font = new Font("Segoe UI", 10F),
            PasswordChar = '●',
            BorderStyle = BorderStyle.FixedSingle
        };
        container.Controls.Add(_passwordInput);

        _ageLabel = new Label 
        { 
            Text = "Age", 
            Left = 40, 
            Top = 230, 
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(70, 70, 70)
        };
        container.Controls.Add(_ageLabel);
        
        _ageInput = new TextBox 
        { 
            Left = 40, 
            Top = 255, 
            Width = 370,
            Height = 32,
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.FixedSingle
        };
        container.Controls.Add(_ageInput);

        _primaryButton = new ModernButton 
        { 
            Left = 40, 
            Top = 305, 
            Width = 180, 
            Height = 40,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            BackColor = Color.FromArgb(0, 123, 255),
            HoverBackColor = Color.FromArgb(0, 105, 217),
            ForeColor = Color.White
        };
        _primaryButton.Click += (_, _) => Submit();
        container.Controls.Add(_primaryButton);

        _switchButton = new ModernButton 
        { 
            Left = 230, 
            Top = 305, 
            Width = 180, 
            Height = 40,
            Font = new Font("Segoe UI", 10F),
            BackColor = Color.FromArgb(108, 117, 125),
            HoverBackColor = Color.FromArgb(90, 98, 104),
            ForeColor = Color.White
        };
        _switchButton.Click += (_, _) => SetMode(!_signUpMode);
        container.Controls.Add(_switchButton);
        
        Controls.Add(container);

        AcceptButton = _primaryButton;
        SetMode(false);
    }

    // Horm soligdohod ali talbar haragdahiig tohtooj ugne
    private void SetMode(bool signUpMode)
    {
        _signUpMode = signUpMode;

        Text = signUpMode ? "SocialApp - Sign Up" : "SocialApp - Sign In";
        _primaryButton.Text = signUpMode ? "Sign Up" : "Sign In";
        _switchButton.Text = signUpMode ? "Back to Sign In" : "Create account";

        _ageLabel.Visible = signUpMode;
        _ageInput.Visible = signUpMode;
    }

    private void Submit()
    {
        var username = _usernameInput.Text.Trim();
        var password = _passwordInput.Text;

        if (username.Length == 0 || password.Length == 0)
        {
            MessageBox.Show("Username bolon password-oo oruulna uu.");
            return;
        }

        if (_signUpMode) SignUp(username, password);
        else SignIn(username, password);
    }

    private void SignIn(string username, string password)
    {
        var user = _userService.SignIn(username, password);
        if (user is null)
        {
            MessageBox.Show("Username esvel password buruu baina.");
            return;
        }
        Finish(user);
    }

    private void SignUp(string username, string password)
    {
        if (_userService.FindByUsername(username) is not null)
        {
            MessageBox.Show($"'{username}' hediin burtgeltei baina. Sign In hiine uu.");
            return;
        }

        if (!byte.TryParse(_ageInput.Text.Trim(), out var age))
        {
            MessageBox.Show("Nasaa zov oruulna uu (jishee: 20).");
            return;
        }

        Finish(_userService.Register(username, age, $"{username}@mail.com", password));
    }

    private void Finish(User user)
    {
        SignedInUser = user;
        DialogResult = DialogResult.OK;
        Close();
    }
}
