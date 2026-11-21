using UnityEngine;

public class LaneBlurController : MonoBehaviour
{
    private const string BlurDistanceProperty = "_BlurDistance";

    [SerializeField] private Material[] mapMaterials = new Material[3];

    private void OnEnable()
    {
        ResolveMaterials();
        ApplyBlurToLanes(GameState.CurrentLane);
        GameState.CurrentLaneChanged += ApplyBlurToLanes;
    }

    private void OnDisable()
    {
        GameState.CurrentLaneChanged -= ApplyBlurToLanes;
    }

    private void ResolveMaterials()
    {
        if (mapMaterials == null || mapMaterials.Length != 3)
        {
            Material[] resolvedMaterials = new Material[3];
            if (mapMaterials != null)
            {
                for (int lane = 0; lane < Mathf.Min(mapMaterials.Length, resolvedMaterials.Length); lane++)
                {
                    resolvedMaterials[lane] = mapMaterials[lane];
                }
            }
            mapMaterials = resolvedMaterials;
        }

        Renderer[] renderers = FindObjectsOfType<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null)
                {
                    continue;
                }

                if (TryGetLaneIndex(material.name, out int laneIndex) && mapMaterials[laneIndex] == null)
                {
                    mapMaterials[laneIndex] = material;
                }
            }
        }
    }

    private static bool TryGetLaneIndex(string materialName, out int laneIndex)
    {
        for (int lane = 0; lane < 3; lane++)
        {
            if (materialName.StartsWith("map" + lane))
            {
                laneIndex = lane;
                return true;
            }
        }

        laneIndex = 0;
        return false;
    }

    private void ApplyBlurToLanes(int currentLane)
    {
        for (int lane = 0; lane < mapMaterials.Length; lane++)
        {
            Material material = mapMaterials[lane];
            if (material != null)
            {
                material.SetFloat(BlurDistanceProperty, Mathf.Abs(currentLane - lane));
            }
        }
    }
}
