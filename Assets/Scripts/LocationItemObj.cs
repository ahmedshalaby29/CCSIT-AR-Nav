using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocationItemObj : MonoBehaviour
{
   
    public TextMeshProUGUI locationTitle;
  
     
   public void OnDeleteButtonClick(){
         SavedLocationManager.Instance.OnDeleteLocationButton(locationTitle.text);
    }

    public void OnLocationClick(){
         SavedLocationManager.Instance.OnSelectLocationClick(locationTitle.text);
    }
}
