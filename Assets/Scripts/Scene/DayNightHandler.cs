/*******************************************************
* Script:      DayNightHandler.cs
* Author(s):   Alexander Art
* 
* Description:
*    Very simple script to keep track of and calculate the day/night cycle.
*    Also controls global day/night lighting, but this should probably be moved to a different script.
*******************************************************/

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Time in seconds for a full day/night cycle.")]
    private int maxTime;

    private float time = 0; // Current time of day in seconds
    private int day = 1; // Current day number (each day that passes increments this number by 1)

    private Light2D globalLight;

    void Awake()
    {
        globalLight = GetComponent<Light2D>();
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time >= maxTime)
        {
            time -= maxTime;
            ++day;
        }

        if (IsDay())
        {
            globalLight.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else {
            globalLight.color = new Color(0.8f, 0.8f, 0.9f, 1.0f);
        }
    }

    public float GetTime()
    {
        return time;
    }

    public void SetTime(float newTime)
    {
        time = newTime;
    }

    public bool IsDay()
    {
        return time < maxTime / 2.0f;
    }

    public bool IsNight()
    {
        return !IsDay();
    }

    public int GetDayNumber()
    {
        return day;
    }

    public void SetDayNumber(int newDay)
    {
        day = newDay;
    }
}
