using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationListObj : MonoBehaviour
{
    public TextMeshProUGUI locationTitle;
  
     
   public void OnDeleteButtonClick(){
         SavedLocationManager.Instance.OnDeleteListButton(locationTitle.text);
    }
     public void OnEditListClick(){
          SavedLocationManager.Instance.OnEditLocationListButton(locationTitle.text);
     }
    public void OnOpenListClick(){
         SavedLocationManager.Instance.OnOpenLocationListClick(locationTitle.text);
    }
}
