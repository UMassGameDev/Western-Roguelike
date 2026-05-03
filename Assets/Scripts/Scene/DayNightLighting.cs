/*******************************************************
* Script:      DayNightLighting.cs
* Author(s):   Alexander Art
* 
* Description:
*    Script to update the global lighting in the scene to match the time of day.
*******************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class ColorAndTime
{
    [SerializeField, Tooltip("Color")]
    private Color color;
    [SerializeField, Tooltip("The percentage of the day/night cycle when this color is reached."), Range(0, 1)]
    private float cyclePercentage;

    public ColorAndTime(Color c, float percentage)
    {
        color = c;
        cyclePercentage = percentage;
    }

    public Color GetColor() { return color; }
    public float GetCyclePercentage() { return cyclePercentage; }
}

public class DayNightLighting : MonoBehaviour
{
    private DayNightHandler dayNightHandler;
    private Light2D globalLight;

    [SerializeField, Tooltip("The global lighting will linearly interpolate between these colors throughout the day/night cycle. These must be in the correct order.")]
    private List<ColorAndTime> colors;

    void Awake()
    {
        dayNightHandler = GameObject.FindAnyObjectByType<DayNightHandler>().GetComponent<DayNightHandler>();
        globalLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ColorAndTime prevColor = colors[colors.Count - 1];
        ColorAndTime nextColor = colors[0];
        for (int i = 0; i < colors.Count; i++)
        {
            if (colors[i].GetCyclePercentage() > dayNightHandler.GetCyclePercentage())
            {
                prevColor = colors[(i - 1 + colors.Count) % colors.Count];
                nextColor = colors[i];
                break;
            }
        }
        
        globalLight.color = Color.Lerp(prevColor.GetColor(), nextColor.GetColor(), ((dayNightHandler.GetCyclePercentage() + 1.0f - prevColor.GetCyclePercentage()) % 1.0f) / ((nextColor.GetCyclePercentage() + 1.0f - prevColor.GetCyclePercentage()) % 1.0f));
    }
}
