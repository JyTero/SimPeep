using TMPro;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clockDisplay;
    private int currentMinute;
    private int currentHour;
    private int currentDay;

    public void DisplayTime(Simulation.ClockTime currentTime)
    {
        //Time Displaying
        currentMinute = currentTime.currentMinute;
        currentHour = currentTime.currentHour;
        currentDay = currentTime.currentDay;
        
        clockDisplay.text = $"Day {currentDay}. {currentHour}:{currentMinute}";

        //currentHour = (currentTime / tickPerInGameHour) % 24;
        //if (CurrentTime % tickPerInGameHour % 2 == 1)
        //{
        //}
        //else
        //    currentMinute = 10 * (CurrentTime % tickPerInGameHour / 2);
        //if (currentMinute == 0)
        //{
        //    clockDisplay.text = currentHour.ToString() + ":00";
        //}
        //else
        //{
        //    clockDisplay.text = currentHour.ToString() + ":" + currentMinute.ToString();
        //}
    }
}
