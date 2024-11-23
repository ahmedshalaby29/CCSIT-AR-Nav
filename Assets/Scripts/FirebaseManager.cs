
using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using Firebase.Messaging;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System;



[System.Serializable]
public class Location
{
    public string name; // Name of the location

    public Location(string name)
    {
        this.name = name;
      
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
    public FirebaseFirestore firestore;
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
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageRecieved;
    }

    private void OnMessageRecieved(object sender, MessageReceivedEventArgs e)
    {
        // You can also access custom data from the message:
        if (e.Message.Data != null)
        {
            foreach (KeyValuePair<string, string> kvp in e.Message.Data)
            {
                Debug.Log("Key: " + kvp.Key + ", Value: " + kvp.Value);
            }
        }    
    }


    private IEnumerator GetTokenAsync()
{
    var task = FirebaseMessaging.GetTokenAsync();

    while(!task.IsCompleted)
         yield return new WaitForEndOfFrame();

    Debug.Log("GET TOKEN ASYNC "+ task.Result);
     firestore
            .Collection("users")
            .Document(auth.CurrentUser.UserId)
            .UpdateAsync(new Dictionary<string, object> { { "fcmToken", task.Result } })
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("FCM Token successfully saved to Firestore.");
                }
                else
                {
                    Debug.LogError("Failed to save FCM token: " + task.Exception?.Message);
                }
            });
            
}

public async Task<bool> SignUp(string email, string password)
{
    try
    {
        var userCredential = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
        Debug.Log($"User created: {userCredential.User.Email}");
        
        // Initialize FCM token after successful sign-up
        InitializeFcmToken();
        
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
        
        // Initialize FCM token after successful sign-in
        InitializeFcmToken();
        
        return true;
    }
    catch (FirebaseException ex)
    {
        Debug.LogError($"Sign in failed: {ex.Message}");
        return false;
    }
}

private void InitializeFcmToken()
{
    // Subscribe to the TokenReceived event
    StartCoroutine(GetTokenAsync());
}
public async Task StoreUserData(string userId, string email, string accountType)
{
    DocumentReference docRef = firestore.Collection("users").Document(userId);

    // Prepare the user data
    Dictionary<string, object> userData = new Dictionary<string, object>
    {
        { "email", email },
        { "accountType", accountType },
        { "savedLocationLists", currentUserData != null ? ConvertToFirestoreFormat(currentUserData.savedLocationLists) : new List<Dictionary<string, object>>() }
    };

    // Store in Firestore
    await docRef.SetAsync(userData);

    // Update local user data
    currentUserData = new UserData(email, accountType)
    {
        savedLocationLists = currentUserData?.savedLocationLists ?? new List<LocationList>()
    };

    Debug.Log("User data stored successfully, including location lists.");
}

// Remove a location from a specific location list
public async Task RemoveLocationFromList(string userId, string listTitle, string locationName)
{
    if (currentUserData == null)
    {
        Debug.LogError("User data is not loaded.");
        return;
    }

    // Find the location list
    LocationList targetList = currentUserData.savedLocationLists.Find(list => list.title == listTitle);
    if (targetList == null)
    {
        Debug.LogError($"Location list '{listTitle}' not found.");
        return;
    }

    // Find and remove the location
    Location targetLocation = targetList.locations.Find(location => location.name == locationName);
    if (targetLocation == null)
    {
        Debug.LogError($"Location '{locationName}' not found in list '{listTitle}'.");
        return;
    }
    targetList.locations.Remove(targetLocation);

    // Update Firestore
    DocumentReference docRef = firestore.Collection("users").Document(userId);
    await docRef.UpdateAsync(new Dictionary<string, object>
    {
        { "savedLocationLists", ConvertToFirestoreFormat(currentUserData.savedLocationLists) }
    });

    Debug.Log($"Location '{locationName}' removed from list '{listTitle}'.");
}


public async Task GetUserData(string userId)
{
    DocumentReference docRef = firestore.Collection("users").Document(userId);
    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

    if (snapshot.Exists)
    {
        // Extract email and account type
        string email = snapshot.GetValue<string>("email");
        string accountType = snapshot.GetValue<string>("accountType");

        // Extract location lists
        List<LocationList> locationLists = new List<LocationList>();
        if (snapshot.ContainsField("savedLocationLists"))
        {
            List<object> rawLists = snapshot.GetValue<List<object>>("savedLocationLists");
            foreach (var rawList in rawLists)
            {
                Dictionary<string, object> listData = rawList as Dictionary<string, object>;
                if (listData != null)
                {
                    string title = listData["title"] as string;
                    List<Location> locations = new List<Location>();

                    if (listData.ContainsKey("locations"))
                    {
                        List<object> rawLocations = listData["locations"] as List<object>;
                        foreach (var rawLocation in rawLocations)
                        {
                            Dictionary<string, object> locationData = rawLocation as Dictionary<string, object>;
                            if (locationData != null)
                            {
                                string name = locationData["name"] as string;
                                locations.Add(new Location(name));
                            }
                        }
                    }

                    locationLists.Add(new LocationList(title) { locations = locations });
                }
            }
        }

        // Update local user data
        currentUserData = new UserData(email, accountType)
        {
            savedLocationLists = locationLists
        };

        Debug.Log($"User data retrieved: email: {email}, accountType: {accountType}, location lists: {locationLists.Count}");
    }
    else
    {
        Debug.Log("No such user data!");
    }
}

    // Add a new location list to the user's data
    public async Task AddLocationList(string userId, string title)
    {
        if (currentUserData == null)
        {
            Debug.LogError("User data is not loaded.");
            return;
        }

        // Create a new location list
        LocationList newList = new LocationList(title);
        currentUserData.savedLocationLists.Add(newList);

        // Update Firestore
        DocumentReference docRef = firestore.Collection("users").Document(userId);
        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "savedLocationLists", ConvertToFirestoreFormat(currentUserData.savedLocationLists) }
        });

        Debug.Log($"Location list '{title}' added.");
    }

    // Add a location to a specific location list
    public async Task AddLocationToList(string userId, string listTitle, string locationName)
    {
        if (currentUserData == null)
        {
            Debug.LogError("User data is not loaded.");
            return;
        }

        // Find the location list
        LocationList targetList = currentUserData.savedLocationLists.Find(list => list.title == listTitle);
        if (targetList == null)
        {
            Debug.LogError($"Location list '{listTitle}' not found.");
            return;
        }

        // Add the new location
        targetList.locations.Add(new Location(locationName));

        // Update Firestore
        DocumentReference docRef = firestore.Collection("users").Document(userId);
        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "savedLocationLists", ConvertToFirestoreFormat(currentUserData.savedLocationLists) }
        });

        Debug.Log($"Location '{locationName}' added to list '{listTitle}'.");
    }

    // Update an existing location list's title
    public async Task UpdateLocationListTitle(string userId, string oldTitle, string newTitle)
    {
        if (currentUserData == null)
        {
            Debug.LogError("User data is not loaded.");
            return;
        }

        // Find the location list
        LocationList targetList = currentUserData.savedLocationLists.Find(list => list.title == oldTitle);
        if (targetList == null)
        {
            Debug.LogError($"Location list '{oldTitle}' not found.");
            return;
        }

        // Update the title
        targetList.title = newTitle;

        // Update Firestore
        DocumentReference docRef = firestore.Collection("users").Document(userId);
        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "savedLocationLists", ConvertToFirestoreFormat(currentUserData.savedLocationLists) }
        });

        Debug.Log($"Location list title updated from '{oldTitle}' to '{newTitle}'.");
    }

    // Delete a location list
    public async Task DeleteLocationList(string userId, string listTitle)
    {
        if (currentUserData == null)
        {
            Debug.LogError("User data is not loaded.");
            return;
        }

        // Remove the location list
        currentUserData.savedLocationLists.RemoveAll(list => list.title == listTitle);

        // Update Firestore
        DocumentReference docRef = firestore.Collection("users").Document(userId);
        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "savedLocationLists", ConvertToFirestoreFormat(currentUserData.savedLocationLists) }
        });

        Debug.Log($"Location list '{listTitle}' deleted.");
    }

    // Convert the currentUserData.savedLocationLists to a format Firestore can store
    private List<Dictionary<string, object>> ConvertToFirestoreFormat(List<LocationList> locationLists)
    {
        List<Dictionary<string, object>> firestoreData = new List<Dictionary<string, object>>();

        foreach (LocationList list in locationLists)
        {
            Dictionary<string, object> listData = new Dictionary<string, object>
            {
                { "title", list.title },
                { "locations", list.locations.ConvertAll(location => new Dictionary<string, object>
                    {
                        { "name", location.name }
                    })
                }
            };

            firestoreData.Add(listData);
        }

        return firestoreData;
    }
    public void Logout()
    {
        try
        {
            // Sign out the current user
            auth.SignOut();
            Debug.Log("User logged out successfully.");

            // Clear any locally stored user data
            currentUserData = null;
            SceneManager.LoadScene(0);
            // Optionally, reset any UI or session-related data
            // You can notify other parts of your app that the user has logged out, if needed.
            
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"Logout failed: {ex.Message}");
        }
    }


}
