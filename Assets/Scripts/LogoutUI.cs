using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutUI : MonoBehaviour
{
    public void OnLogoutClick(){
        FirebaseManager.Instance.Logout();
    }
}
