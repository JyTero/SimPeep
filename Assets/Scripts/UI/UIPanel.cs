using Unity.VisualScripting;
using UnityEngine;

public class UIPanel : ManagerMono
{
    protected UIController uiController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        base.Start();
        uiController = FindAnyObjectByType<UIController>();
    }
}
