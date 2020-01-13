using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Authenticate : MonoBehaviour
{

    Firebase.Auth.FirebaseAuth auth;

    // Start is called before the first frame update
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
       {
           if(task.IsCanceled)
           {
               Debug.LogError("SignInAnonmouslyAsync was canceled.");
               return;
           }
           if(task.IsFaulted)
           {
               Debug.LogError("SignInAnonmouslyAsync encountered an error: " + task.Exception);
               return;
           }

           Firebase.Auth.FirebaseUser newUser = task.Result;
           Debug.Log("User Signed in");

       });
    }

}
