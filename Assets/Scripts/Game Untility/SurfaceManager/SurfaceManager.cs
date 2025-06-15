using System.Collections.Generic;
using UnityEngine;

/**
 * SurfaceManager is a singleton class that handles impact effects on different surfaces
 * User defines surfaces and their textures and apply effects based on these surfaces
 */

public class SurfaceManager : MonoBehaviour
{
    private static SurfaceManager _instance;

    public static SurfaceManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField] private List<SurfaceType> surfaces = new();
    [SerializeField] private int defaultPoolSize = 10;
    [SerializeField] private Surface defaultSurface;

    // Handle each type of impact on different game objects
    public void HandleImpact(GameObject hitObject, Vector3 hitPoint, Vector3 hitNormal, ImpactType impact, int triangleIndex)
    {
        if (hitObject.TryGetComponent<Terrain>(out Terrain terrain))
        {
            // Handle terrain impact
            // cbf to implement terrain impact effects
            return;
        }
        else if (hitObject.TryGetComponent<Renderer>(out Renderer renderer))
        {
            Texture activeTexture = GetActiveTextureFromRenderer(renderer, triangleIndex);

            // Find the surface type based on texture
            // Then play the corresponding effects
            SurfaceType surfaceType = surfaces.Find(surface => surface.Albedo == activeTexture);
            if (surfaceType != null)
            {
                foreach (Surface.SurfaceImpactTypeEffect typeEffect in surfaceType.Surface.impactTypeEffects)
                {
                    if (typeEffect.impactType == impact)
                    {
                        foreach (EffectBase effect in typeEffect.effects)
                        {
                            effect.CreateInstance(hitPoint, hitNormal);
                        }
                    }
                }
            }
            // Default surface effect
            else
            {
                foreach (Surface.SurfaceImpactTypeEffect typeEffect in defaultSurface.impactTypeEffects)
                {
                    if (typeEffect.impactType == impact)
                    {
                        foreach (EffectBase effect in typeEffect.effects)
                        {
                            effect.CreateInstance(hitPoint, hitNormal);
                        }
                    }
                }
            }
        }
    }

    // Get the active texture from the renderer based on the triangle index
    private Texture GetActiveTextureFromRenderer(Renderer renderer, int triangleIndex)
    {
        if (renderer.TryGetComponent(out MeshFilter meshFilter))
        {
            Mesh mesh = meshFilter.mesh;

            if (mesh.subMeshCount > 1)
            {
                // Get the triangle hit from the mesh
                int[] hitTriangleIndices = new int[]
                {
                    mesh.triangles[triangleIndex * 3],
                    mesh.triangles[triangleIndex * 3 + 1],
                    mesh.triangles[triangleIndex * 3 + 2]
                };

                // Check each submesh for the triangle indices
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    int[] submeshTriangles = mesh.GetTriangles(i);
                    for (int j = 0; j < submeshTriangles.Length; j += 3)
                    {
                        if (submeshTriangles[j] == hitTriangleIndices[0]
                            && submeshTriangles[j + 1] == hitTriangleIndices[1]
                            && submeshTriangles[j + 2] == hitTriangleIndices[2])
                        {
                            // Found the triangle in the submesh, return the texture
                            return renderer.sharedMaterials[i].mainTexture;
                        }
                    }
                }
            }
            else
            {
                // If there's only one submesh, return the main texture
                return renderer.sharedMaterial.mainTexture;
            }
        }

        // No mesh filter or no triangles found, return null will result in default surface -> effect
        return null;
    }
}
