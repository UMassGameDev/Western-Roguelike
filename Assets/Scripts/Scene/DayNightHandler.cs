/*******************************************************
* Script:      DayNightHandler.cs
* Author(s):   Alexander Art
* 
* Description:
*    Script to keep track of and calculate the day/night cycle.
*******************************************************/

using UnityEngine;

public class DayNightHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Time in seconds for a full day/night cycle.")]
    private int maxTime;

    public static DayNightHandler instance;
    private float time = 0; // Current time of day in seconds
    private int day = 1; // Current day count (each day that passes increments this number by 1)

    private void Awake()
    {
        // Have only 1 DayNightHandler at a time and have it persist between scenes
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= maxTime)
        {
            time -= maxTime;
            ++day;
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

    public int GetDayCount()
    {
        return day;
    }

    public void SetDayCount(int newDay)
    {
        day = newDay;
    }

    public bool IsDay()
    {
        return time < maxTime / 2.0f;
    }

    public bool IsNight()
    {
        return !IsDay();
    }

    public float GetCyclePercentage()
    {
        return time / maxTime;
    }
}
