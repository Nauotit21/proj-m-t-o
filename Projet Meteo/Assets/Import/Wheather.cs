using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.Networking;

public class SnowGlobeWeather : MonoBehaviour
{
    [Header("UI")]
    public GameObject weatherCanvas;
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI cityText;

    [Header("API Config")]
    public string apiKey = "VOTRE_CLE_API"; // OpenWeatherMap
    public string city = "Chicoutimi,CA";

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Abonnement aux événements de grab
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        weatherCanvas.SetActive(true);
        StartCoroutine(FetchWeather());
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        weatherCanvas.SetActive(false);
    }

    IEnumerator FetchWeather()
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather" +
                     $"?q={city}&appid={apiKey}&units=metric&lang=fr";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                WeatherResponse data = JsonUtility.FromJson<WeatherResponse>(request.downloadHandler.text);
                temperatureText.text = $"{Mathf.Round(data.main.temp)}°C";
                descriptionText.text = data.weather[0].description;
                cityText.text = data.name;
            }
            else
            {
                temperatureText.text = "Erreur réseau";
            }
        }
    }
}

// Classes de désérialisation JSON
[System.Serializable]
public class WeatherResponse
{
    public string name;
    public MainData main;
    public WeatherData[] weather;
}

[System.Serializable]
public class MainData { public float temp; public float humidity; }

[System.Serializable]
public class WeatherData { public string description; public string icon; }

