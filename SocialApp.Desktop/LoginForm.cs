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
        Font = new Font("Segoe UI", 11F);
        ClientSize = new Size(420, 320);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(new Label { Text = "Username:", Left = 30, Top = 28, AutoSize = true });
        _usernameInput = new TextBox { Left = 30, Top = 54, Width = 350 };
        Controls.Add(_usernameInput);

        Controls.Add(new Label { Text = "Password:", Left = 30, Top = 96, AutoSize = true });
        // PasswordChar - bichij bui nuuts ug haragdahgui
        _passwordInput = new TextBox { Left = 30, Top = 122, Width = 350, PasswordChar = '*' };
        Controls.Add(_passwordInput);

        // Nas zowhon sign up horm deer haragdana
        _ageLabel = new Label { Text = "Age:", Left = 30, Top = 164, AutoSize = true };
        Controls.Add(_ageLabel);
        _ageInput = new TextBox { Left = 30, Top = 190, Width = 350 };
        Controls.Add(_ageInput);

        _primaryButton = new Button { Left = 30, Top = 236, Width = 170, Height = 38 };
        _primaryButton.Click += (_, _) => Submit();
        Controls.Add(_primaryButton);

        _switchButton = new Button { Left = 210, Top = 236, Width = 170, Height = 38 };
        _switchButton.Click += (_, _) => SetMode(!_signUpMode);
        Controls.Add(_switchButton);

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
