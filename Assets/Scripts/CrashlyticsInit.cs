using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Import Firebase and Crashlytics
using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions; // Needed for ContinueWithOnMainThread

public class CrashlyticsInit : MonoBehaviour {
    // Use this for initialization
     void Awake () {
        // Initialize Firebase
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                // When this property is set to true, Crashlytics will report all
                // uncaught exceptions as fatal events. This is the recommended behavior.
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                LogDeviceInfoToCrashlytics();
                // Set a flag here for indicating that your project is ready to use Firebase.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}",dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void LogDeviceInfoToCrashlytics()
    {
        string deviceModel = SystemInfo.deviceModel;
        string operatingSystem = SystemInfo.operatingSystem;
        
        // Log to Crashlytics
        Crashlytics.Log($"Device Model: {deviceModel}, OS: {operatingSystem}");
        Debug.Log("Device info logged to Crashlytics.");
        // Optional: You can also log a custom non-fatal exception for testing
    }

    // Update is called once per frame
    void Update(){
        // Your update logic
    }
}
