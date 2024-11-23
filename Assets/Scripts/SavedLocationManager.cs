using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavedLocationManager : MonoBehaviour
{        

        public GameObject backButtonObj;
        public GameObject closeButtonObj;
    public GameObject savedListsObj;
        public GameObject savedLocationsObj;
    public GameObject locationListPrefab; // Prefab for displaying location lists
    public GameObject locationPrefab; // Prefab for displaying individual locations
    public Transform listsParent; // Parent object to hold location list prefabs
    public Transform locationsParent; // Parent object to hold location prefabs

    public TMP_InputField newListTitleInput; // Input field for new list title
    public TMP_Dropdown newLocationDropdown; // Input field for new location name
    public static SavedLocationManager Instance { get; private set; }
    public string selectedListTitle;
    private string currentUserId;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;        
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    void Start()
    {
        currentUserId = FirebaseManager.Instance.auth.CurrentUser.UserId;

        // Load and populate the user's saved location lists
        PopulateLocationLists();
    }

    public void OnAddNewListButton()
    {
        string newListTitle = newListTitleInput.text;

        if (string.IsNullOrEmpty(newListTitle))
        {
            Debug.LogWarning("List title cannot be empty!");
            return;
        }

        StartCoroutine(AddNewLocationList(newListTitle));
    }

    public void OnAddNewLocationButton()
    {
        string newLocationName = newLocationDropdown.options[newLocationDropdown.value].text;


        StartCoroutine(AddLocationToSelectedList(selectedListTitle, newLocationName));
    }

    public void OnDeleteListButton(string listTitle)
    {

        StartCoroutine(DeleteLocationList(listTitle));
    }
      public void OnDeleteLocationButton(string locationTitle)
    {

        StartCoroutine(RemoveLocationFromList(locationTitle));
    }

    public void OnOpenLocationListClick(string listTitle){
        selectedListTitle = listTitle;
        PopulateLocations(listTitle);
        savedListsObj.SetActive(false);
        savedLocationsObj.SetActive(true);
        backButtonObj.SetActive(true);
        closeButtonObj.SetActive(false);
    }

    public void OnSelectLocationClick(string locationTitle){
        savedListsObj.SetActive(true);
        savedLocationsObj.SetActive(false);
        backButtonObj.SetActive(false);
        closeButtonObj.SetActive(true);
        gameObject.SetActive(false);
          List<string> options = new List<string>();

        // Loop through the navigation targets and add their names to the options list
        foreach (var target in  NavigationManager.Instance.navigationTargets)
        {
            // Assuming NavigationTarget has a property called "name"
            options.Add(target.name); // or target.ToString() if applicable
        }
        NavigationManager.Instance.targetDropdown.value = options.IndexOf(locationTitle);
       // NavigationManager.Instance.selectedTarget = NavigationManager.Instance.navigationTargets.Find((target) => target.name == locationTitle);
    }

    private IEnumerator AddNewLocationList(string listTitle)
    {
        yield return FirebaseManager.Instance.AddLocationList(currentUserId, listTitle);
        PopulateLocationLists();
    }

    private IEnumerator AddLocationToSelectedList(string listTitle, string locationName)
    {
        yield return FirebaseManager.Instance.AddLocationToList(currentUserId, listTitle, locationName);
        PopulateLocations(listTitle);
    }
    private IEnumerator RemoveLocationFromList(string locationTitle)
    {
        yield return FirebaseManager.Instance.RemoveLocationFromList(currentUserId, selectedListTitle,locationTitle);
        PopulateLocations(selectedListTitle);
    }
    private IEnumerator DeleteLocationList(string listTitle)
    {
        yield return FirebaseManager.Instance.DeleteLocationList(currentUserId, listTitle);
        PopulateLocationLists();
    }

    private async void PopulateLocationLists()
    {
        await FirebaseManager.Instance.GetUserData(currentUserId);

        // Clear current UI
        foreach (Transform child in listsParent)
        {
            Destroy(child.gameObject);
        }

        //listsDropdown.ClearOptions();

        List<LocationList> locationLists = FirebaseManager.Instance.currentUserData.savedLocationLists;
        

        foreach (LocationList list in locationLists)
        {
            // Instantiate and configure the location list prefab
            GameObject newList = Instantiate(locationListPrefab, listsParent);
            newList.GetComponentInChildren<LocationListObj>().locationTitle.text = list.title;
           
        }

        // Update dropdown

        Debug.Log("Location lists populated.");
    }

    private void PopulateLocations(string listTitle)
    {
        // Clear current UI
        foreach (Transform child in locationsParent)
        {
            Destroy(child.gameObject);
        }

        LocationList selectedList = FirebaseManager.Instance.currentUserData.savedLocationLists
            .Find(list => list.title == listTitle);

        if (selectedList == null)
        {
            Debug.LogWarning($"List '{listTitle}' not found.");
            return;
        }

        foreach (Location location in selectedList.locations)
        {
            // Instantiate and configure the location prefab
            GameObject newLocation = Instantiate(locationPrefab, locationsParent);
           newLocation.GetComponentInChildren<LocationItemObj>().locationTitle.text = location.name;

        }

        Debug.Log($"Locations for '{listTitle}' populated.");
    }
}
