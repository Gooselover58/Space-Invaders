using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{ // Speed at which the color transitions through the rainbow
    public float colorChangeSpeed = 1f;

    // A renderer component to access the material's color (e.g., SpriteRenderer, MeshRenderer)
    private Renderer objectRenderer;

    // Hue value (0 to 1)
    private float hue = 0f;

    void Start()
    {
        // Get the Renderer component from the GameObject
        objectRenderer = GetComponent<Renderer>();

        // Initialize the object's color
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }

    void Update()
    {
        // Increment the hue value over time, looping back to 0 after reaching 1
        hue += Time.deltaTime * colorChangeSpeed;
        if (hue > 1f)
        {
            hue = 0f;
        }

        // Convert the hue to an RGB color and apply it to the object's material color
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}
