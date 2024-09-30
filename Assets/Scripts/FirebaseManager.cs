
using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;



[System.Serializable]
public class Location
{
    public string name; // Name of the location
    public float latitude; // Latitude
    public float longitude; // Longitude

    public Location(string name, float latitude, float longitude)
    {
        this.name = name;
        this.latitude = latitude;
        this.longitude = longitude;
    }
}

[System.Serializable]
public class LocationList
{
    public string title; // Title of the location list
    public List<Location> locations; // List of locations

    public LocationList(string title)
    {
        this.title = title;
        this.locations = new List<Location>(); // Initialize the list
    }
}

[System.Serializable]
public class UserData
{
    public string email;
    public string accountType;
    public List<LocationList> savedLocationLists; // List of location lists


    public UserData(string _email, string _accountType)
    {
        savedLocationLists = new List<LocationList>(); // Initialize the list
        email = _email;
        accountType = _accountType;
    }
}


public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public FirebaseAuth auth;
    private FirebaseFirestore firestore;
    public UserData currentUserData; // Store user data here

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Preserve this object across scenes
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public async Task<bool> SignUp(string email, string password)
    {
        try
        {
            var userCredential = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            Debug.Log($"User created: {userCredential.User.Email}");
            return true;
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Sign up failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SignIn(string email, string password)
    {
        try
        {
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log($"User signed in: {userCredential.User.Email}");
            return true;
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Sign in failed: {ex.Message}");
            return false;
        }
    }

    public async Task StoreUserData(string userId, string email, string accountType)
    {
        DocumentReference docRef = firestore.Collection("users").Document(userId);
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", email },
            { "accountType", accountType }
        };

        await docRef.SetAsync(userData);
        currentUserData = new UserData(email, accountType); // Store user data locally
        Debug.Log("User data stored successfully.");
    }

    public async Task GetUserData(string userId)
    {
        DocumentReference docRef = firestore.Collection("users").Document(userId);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            string email = snapshot.GetValue<string>("email");
            string accountType = snapshot.GetValue<string>("accountType");
            currentUserData = new UserData(email, accountType); // Update current user data
            Debug.Log($"User Data: email: {email}, accountType: {accountType}");
        }
        else
        {
            Debug.Log("No such user data!");
        }
    }
}
