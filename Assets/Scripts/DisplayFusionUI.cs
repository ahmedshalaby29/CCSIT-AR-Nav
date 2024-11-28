using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using TMPro;

public class DisplayFusionUI : MonoBehaviour
{
    public TextMeshProUGUI fusionText; // Assign this in the Unity Inspector

    void Start()
    {
        FusionProviderType fusionProvider = VuforiaRuntimeUtilities.GetActiveFusionProvider();
        fusionText.text = "Fusion Provider: " + fusionProvider.ToString();
    }
}
