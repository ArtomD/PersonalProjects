using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseUtils : MonoBehaviour
{
    public bool firebaseIsSafe;
    private FirebaseApp app;
    public Firebase.Auth.FirebaseAuth auth;
    public Firebase.Auth.FirebaseUser user;

    private string email;
    private string password;

    public Text t_status;

    

    private void Start()
    {


        if (t_status == null)
        {
            t_status = GetComponent<Text>();
        }

        t_status.text = "testing";

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        email = "";
        password = "";
    }

    public void passwordChanged(InputField if_password)
    {
        password = if_password.text;
    }

    public void emailChanged(InputField if_email)
    {
        email = if_email.text;
    }

    void checkDependencies()
    {
        firebaseIsSafe = false;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to firebase app
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                firebaseIsSafe = true;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                firebaseIsSafe = false;
                t_status.text = "Firebase unavailable";
            }
        });
    }

    public void auth_signUp()
    {

        if (email == "" || password == "")
        {
            t_status.text = "Please enter a value in both fields";
            return ;
        }

        t_status.text = "signing up";

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                t_status.text = "Could not sign up.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                t_status.text = "Could not sign up.";
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            t_status.text = "Account created";

        });
    }

    void signIn()
    {

    }
}
