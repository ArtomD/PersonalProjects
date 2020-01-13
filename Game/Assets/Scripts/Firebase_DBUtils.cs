using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Firebase_DBUtils : MonoBehaviour
{
    Firebase.Auth.FirebaseAuth auth;

    public Text t_hslist;
    public Text t_hsplayer;
    public Text test_txt;
    public Text t_unchangestatus;
    public InputField if_unchange;
    DatabaseReference reference;

    public string highScores;
    public string playerHighScore;
    public string unstatus;

    private delegate void HSChanged(string s);
    private delegate void playerHSChanged(string s);
    private delegate void UnStatusChanged(string s);
    event HSChanged HSChangedEvent;
    event HSChanged playerHSChangedEvent;
    event UnStatusChanged unStatusChangedEvent;

    void handleHSChanged(string s)
    {
        highScores = s;
    }
    void handlePlayerHSChanged(string s)
    {
        playerHighScore = s;
    }

    void handleUnStatusChanged(string s)
    {
        unstatus = s;
    }

    void Start()
    {


        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonmouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonmouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.Log("User Signed in");

        });
        if (t_hslist == null)
        {
            t_hslist = GetComponent<Text>();
        }
        if (t_hsplayer == null)
        {
            t_hsplayer = GetComponent<Text>();
        }
        if(t_unchangestatus == null)
        {
            t_unchangestatus = GetComponent<Text>();
        }
        if(if_unchange == null)
        {
            if_unchange = GetComponent<InputField>();
        }

        //t_highscores.text = "started";
        highScores = "started";

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://teamagame-484b6.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        this.HSChangedEvent += new HSChanged(handleHSChanged);
        this.playerHSChangedEvent += new HSChanged(handlePlayerHSChanged);
        this.unStatusChangedEvent += new UnStatusChanged(handleUnStatusChanged);

        if(if_unchange != null)
        {
            FirebaseDatabase.DefaultInstance
            .GetReference("users").Child(auth.CurrentUser.UserId)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    HSChangedEvent("error getting scores");
                    Console.WriteLine("error getting scores");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if_unchange.text = snapshot.Child("username").Value.ToString();
                }

            });
        }
        GetHighScores();
    }

    void Update()
    {
        if(t_hslist != null)
        {
            t_hslist.text = highScores;
        }

        if (t_hsplayer != null)
        {
            t_hsplayer.text = "You: " + playerHighScore;
        }

        if(test_txt != null)
        {
            test_txt.text = highScores;
        }

        if(t_unchangestatus != null)
        {
            t_unchangestatus.text = unstatus;
        }
    }

    public void AddHighScore()
    {

    }

    public void test_AddHighScore()
    {
        //HSChangedEvent("adding high score");


        //random score
        int score = (int)UnityEngine.Random.Range(0, 1000);

        //random name
        string name = "";
        int length = (int)(UnityEngine.Random.value * 10) + 1;
        for (int i = 0; i < length; i++)
            name += (char)((int)UnityEngine.Random.Range(33, 126));


        HighScore hs = new HighScore(name, score.ToString());
        string json = JsonUtility.ToJson(hs);

        test_txt.text = json;

        reference.Child("highscores").Child(length.ToString()).SetRawJsonValueAsync(json);
    }

    public async void GetHighScores()
    {
        HSChangedEvent("Getting high scores...");

        await FirebaseDatabase.DefaultInstance
         .GetReference("users").OrderByChild("score").LimitToLast(10)
         .GetValueAsync().ContinueWith(task => {

             HSChangedEvent("getting scores");
             Console.WriteLine("getting scores");

             if (task.IsFaulted)
            {
                 HSChangedEvent("error getting scores");
                 Console.WriteLine("error getting scores");

             }
             else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                 if (snapshot == null)
                 {
                     HSChangedEvent("no scores found");
                     Console.WriteLine("no data found");
                     return;
                 }

                 string s = "";
                 List<DataSnapshot> usersList;
                 var users = snapshot.Children;
                 usersList = new List<DataSnapshot>(users);
                 usersList.Reverse();
                 foreach(var u in usersList)
                 {
                     s += u.Child("username").Value + ": " + u.Child("score").Value + "\n";
                 }


                 HSChangedEvent(s);
                 Console.WriteLine(s);

             }
         });

        //search for player high score
        await FirebaseDatabase.DefaultInstance
        .GetReference("users").Child(auth.CurrentUser.UserId)
        .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    HSChangedEvent("error getting scores");
                    Console.WriteLine("error getting scores");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    float dbScore = 0.0f;
                    bool parsed = float.TryParse(snapshot.Child("score").Value.ToString(), out dbScore);
                    playerHSChangedEvent(dbScore.ToString());
                }

            });


    }

    public async void test_GetHighScores()
    {
        HSChangedEvent("Getting high scores...");

        await FirebaseDatabase.DefaultInstance
         .GetReference("highscores")
         .GetValueAsync().ContinueWith(task => {

             HSChangedEvent("getting scores");
             Console.WriteLine("getting scores");

             if (task.IsFaulted)
             {
                 HSChangedEvent("error getting scores");
                 Console.WriteLine("error getting scores");

             }
             else if (task.IsCompleted)
             {
                 DataSnapshot snapshot = task.Result;

                 if (snapshot == null)
                 {
                     HSChangedEvent("no data found");
                     Console.WriteLine("no data found");
                     return;
                 }

                 string s = "";
                 var scores = snapshot.Children;
                 foreach (var hs in scores)
                 {
                     s += hs.Child("user").Value + ": " + hs.Child("score").Value + "\n";
                 }



                 HSChangedEvent(s);
                 Console.WriteLine(s);

             }
         });

        //search for player high score


    }

    public void changeUsername()
    {
        if (if_unchange == null)
            return;

        if(if_unchange.text != "")
        {
            //unStatusChangedEvent("Changing Username...");
            unStatusChangedEvent("Changing Username to " + if_unchange.text);
            try
            {
                FirebaseDatabase.DefaultInstance
                .GetReference("users").OrderByChild("username").EqualTo(if_unchange.text)
                .GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        HSChangedEvent("error");
                        Console.WriteLine("error");

                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        if(snapshot == null)    //username not found
                        {
                            return;
                        }
                        else
                        {
                            if(snapshot.ChildrenCount == 0)
                            {
                                reference.Child("users").Child(auth.CurrentUser.UserId).Child("username").SetValueAsync(if_unchange.text);
                                unStatusChangedEvent("Username changed!");
                            }
                            else
                            {
                                unStatusChangedEvent("Could not change username, username already taken.");
                            }
                        }

                    }
                });

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            
        }


    }


    [System.Serializable]
    public class HighScore
    {
        public string user;
        public string score;

        public HighScore(string u, string s)
        {
            this.user = u;
            this.score = s;
        }
    }
}
