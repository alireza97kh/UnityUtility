using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DobeilLoginManager : MonoSingleton<DobeilLoginManager>
{
    // User information
    protected string userId;
    protected string userName;
    protected string userEmail;
    protected string userProfilePicURL;
    protected string userPassword;

    // Events for login and logout

    public abstract void Login(Action<string, string, string> onDoneLoginEvent, Action<string> onErrorLoginEvent);
    public abstract void Logout(Action onDoneLogOutEvent, Action<string> onErrorLogOutEvent);
    public abstract bool IsLoggedIn();

    // Get user information
    public string GetUserId()
    {
        return userId;
    }

    public string GetUserName()
    {
        return userName;
    }

    public string GetUserEmail()
    {
        return userEmail;
    }

    public string GetUserProfilePicURL()
    {
        return userProfilePicURL;
    }

    public string GetUserPassword()
	{
        return userPassword;
	}

    // Common functions
    protected void SetUserData(string id, string name, string email, string profilePicURL, string passWord)
    {
        userId = id;
        userName = name;
        userEmail = email;
        userProfilePicURL = profilePicURL;
        userPassword = passWord;
    }

}
