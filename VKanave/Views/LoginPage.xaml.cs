﻿using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using VKanave.Networking;
using VKanave.Networking.NetMessages;

namespace VKanave.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        Current = this;
        textboxUsername.TextChanged += (s, e) =>
        {
            frameUsername.BorderColor = Color.FromRgba(0, 0, 0, 0);
            textboxUsername.TextColor = Colors.White;
            button1.IsEnabled = true;
            framePassword.BorderColor = Color.FromRgba(0, 0, 0, 0);
            textboxPassword.TextColor = Colors.White;
            button1.IsEnabled = true;
        };
        textboxPassword.TextChanged += (s, e) =>
        {
            frameUsername.BorderColor = Color.FromRgba(0, 0, 0, 0);
            textboxUsername.TextColor = Colors.White;
            button1.IsEnabled = true;
            framePassword.BorderColor = Color.FromRgba(0, 0, 0, 0);
            textboxPassword.TextColor = Colors.White;
            button1.IsEnabled = true;
        };
    }

    private void Button_SignIn_Clicked(object sender, EventArgs e)
    {
        if (!CheckInputs1())
        {
            MarkUsername();
            Toast.Make("Username too long or too short.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            return;
        }
        if (!CheckInputs2())
        {
            MarkPassword();
            Toast.Make("Password too long or too short.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            return;
        }
        if (!CheckInputs3())
        {
            Toast.Make("Username contains invalid charachers.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            return;
        }
        button1.IsEnabled = false;
        textboxUsername.IsReadOnly = true;
        textboxPassword.IsReadOnly = true;
        if (AuthorizationMode)
            Networking.Networking.Send(new NMAuth(textboxUsername.Text, textboxPassword.Text));
        else
            Networking.Networking.Send(new NMReg(textboxUsername.Text, textboxPassword.Text));
    }

    private bool CheckInputs1()
    {
        if (string.IsNullOrEmpty(textboxUsername.Text))
            return false;
        if (textboxUsername.Text.Length <= 3 || textboxUsername.Text.Length > 20)
            return false;
        return true;
    }

    private bool CheckInputs2()
    {
        if (string.IsNullOrEmpty(textboxPassword.Text))
            return false;
        if (textboxPassword.Text.Length <= 3 || textboxPassword.Text.Length > 20)
            return false;
        return true;
    }

    private bool CheckInputs3()
    {
        bool b = true;
        textboxPassword.Text.ToLower().ToCharArray().ToList().ForEach((x) =>
        {
            if (!allowedChars.Contains(x))
            {
                b = false;
                return;
            }
        });
        return b;
    }

    public void SignIn(string username, string token, long id, int reg, string displayName, string bio)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            textboxUsername.IsReadOnly = false;
            textboxPassword.IsReadOnly = false;
            if (string.IsNullOrEmpty(token))
            {
                MarkUsername();
                MarkPassword();
                Toast.Make("Incorect username or password.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
            }
            else
            {
                // auth successfully :)
                LocalUser.NewUser(username, token, id, reg, displayName, bio);
                Navigation.PopModalAsync();
            }
        });
    }

    private void MarkUsername()
    {
        frameUsername.BorderColor = Color.FromRgb(255, 0, 0);
        textboxUsername.TextColor = Color.FromRgb(255, 0, 0);
    }

    private void MarkPassword()
    {
        framePassword.BorderColor = Color.FromRgb(255, 0, 0);
        textboxPassword.TextColor = Color.FromRgb(255, 0, 0);
    }

    public void SignUp(int code)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            textboxUsername.IsReadOnly = false;
            textboxPassword.IsReadOnly = false;
            button1.IsEnabled = true;
            if (code == 0)
                RegEvent_UnknownError();
            else if (code == 1)
                RegEvent_Successfully();
            else if (code == 2)
                RegEvent_AlreadyExists();
        });
    }

    private void RegEvent_UnknownError()
    {
        Toast.Make("Unknown error. Try again later.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
    }

    private void RegEvent_Successfully()
    {
        Toast.Make("Thank you for registrating.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
        if (!AuthorizationMode)
            TapGestureRecognizer_Tapped(null, null);
    }

    public void RegEvent_AlreadyExists()
    {
        MarkUsername();
        Toast.Make("A user with that username already exists.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
    }

    protected override void OnDisappearing()
    {
        if (LocalUser.Id != 0)
        {
            Networking.Networking.Send(new NMChats() { localUserId = LocalUser.Id });
            Navigation.PopModalAsync();
        }
        base.OnDisappearing();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (!_animation)
        {
            _animation = true;
            AuthorizationMode = !AuthorizationMode;
            if (AuthorizationMode)
            {
                var t1 = label1.RotateXTo(90, 200, Easing.Linear);
                var t2 = label2.TranslateTo(label2.TranslationX, label2.TranslationY - 50, 200, Easing.Linear);
                await Task.WhenAll(t1, t2);
                label1.Text = "Authorization";
                await label1.RotateXTo(0, 200, Easing.Linear);
            }
            else
            {
                var t1 = label1.RotateXTo(90, 200, Easing.Linear);
                var t2 = label2.TranslateTo(label2.TranslationX, label2.TranslationY + 50, 200, Easing.Linear);
                await Task.WhenAll(t1, t2);
                label1.Text = "Registration";
                await label1.RotateXTo(0, 200, Easing.Linear);
            }
            _animation = false;
        }
    }

    public bool AuthorizationMode
    {
        get => _auth;
        set
        {
            _auth = value;
            if (value)
            {
                //label1.Text = "Authorization";
                //label2.IsVisible = true;
                label3.Text = "Sign up";
                button1.Text = "Sign in";
            }
            else
            {
                //label1.Text = "Registration";
                //label2.IsVisible = false;
                label3.Text = "Sign in";
                button1.Text = "Sign up";
            }
        }
    }

    public static LoginPage Current;

    private readonly List<char> allowedChars = new List<char>()
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ', '_',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    private bool _auth = true;
    private bool _animation = false;
}