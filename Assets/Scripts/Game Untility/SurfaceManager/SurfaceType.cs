using UnityEngine;

/*
 * Define a new surface type, the texture to check for and
 * the surface to apply effects from based on impact and corresponding effect
 */

[System.Serializable]
public class SurfaceType
{
    public Texture Albedo;
    public Surface Surface;
}
