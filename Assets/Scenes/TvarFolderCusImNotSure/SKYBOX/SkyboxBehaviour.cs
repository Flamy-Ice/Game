using UnityEngine;

public enum MapAtmosphere
{
    Bizzare,
    Chaos
}

public class SkyboxController : MonoBehaviour
{
    [Header("Skybox Materials")]
    public Material bizzareSkybox;
    public Material chaosSkybox;

    public void ChangeSkybox(MapAtmosphere newState)
    {
        if (newState == MapAtmosphere.Bizzare && bizzareSkybox != null)
        {
            RenderSettings.skybox = bizzareSkybox;
            DynamicGI.UpdateEnvironment(); 
            Debug.Log("Skybox material changed to the bizzare ver");
        }
        else if (newState == MapAtmosphere.Chaos && chaosSkybox != null)
        {
            RenderSettings.skybox = chaosSkybox;
            DynamicGI.UpdateEnvironment(); 
            Debug.Log("Skybox material changed to the chaos ver");
        }
    }
}