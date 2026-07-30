using UnityEngine;
using static UnityEngine.Rendering.BoolParameter;

public class Simulation : ManagerMono
{
    private ClockTime currentTime;
    public ClockTime CurrentTime { get { return currentTime; } }

    //[SerializeField]
    private int timeScale;
    //private CharacterAIHandler characterAIHandler;
    //private CharacterRouting characterRouting;
    //private InteractionEngine interactionEngine;
    //private NeedsEngine needsEngine;

    protected override void Start()
    {
        //characterAIHandler = FindAnyObjectByType<CharacterAIHandler>();
        //characterRouting = FindAnyObjectByType<CharacterRouting>();
        //interactionEngine = FindAnyObjectByType<InteractionEngine>();
        //needsEngine = FindAnyObjectByType<NeedsEngine>();
        base.Start();
        SetSimulationTimeScale(0);

        currentTime = new ClockTime(0);
 
    }

    private void Update()
    {
        float dt = Time.deltaTime * timeScale;
        AdvanceClock(dt);
        characterAIHandler.MyUpdate(dt);
        characterRouting.MyUpdate(dt);
        interactionEngine.MyUpdate(dt);
        needsEngine.MyUpdate(dt);

    }

    public void SetSimulationTimeScale(int speed)
    {
        timeScale = speed;
    }
    float clockTimer;
    private void AdvanceClock(float dt)
    {
        clockTimer += dt;
        if (clockTimer > OneUnitOfTime)
        {
            bool tooMuchTime = true;
            while (tooMuchTime)
            {
                currentTime.currentTime++;
                currentTime.currentMinute++;
                if (currentTime.currentMinute == 60)
                {
                    currentTime.currentHour++;
                    currentTime.currentMinute = 0;
                    if (currentTime.currentHour == 24)
                    {
                        currentTime.currentDay++;
                        currentTime.currentHour = 0;
                    }
                }
                clockTimer = -OneUnitOfTime;
                if (clockTimer < OneUnitOfTime)
                    tooMuchTime = false;
                //DEBUG
                FindAnyObjectByType<ClockUI>().DisplayTime(currentTime);
            }

        }
    }

    public struct ClockTime
    {
        public int currentTime;
        public int currentMinute;
        public int currentHour;
        public int currentDay;

        public ClockTime(int i)
        {
            currentTime = 0;
            currentMinute = 0;
            currentHour = 0;
            currentDay = 0;
        }
    }
}
