using Unity.VisualScripting;
using UnityEngine;

public class UIPanel : ManagerMono
{
    protected bool isPanelActive = false;
    public bool IsPanelActive { get { return isPanelActive; } }

    protected UIController uiController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        base.Start();
        uiController = FindAnyObjectByType<UIController>();
    }

    public virtual void OnSelectCharacterChange()
    {
        if (!isPanelActive)
            return;
        OCSS();

    }

    protected virtual void OCSS()
    {
        //PUT NOTHING HERE, USE OnSelectCharacterChange() for common functionality at this step
    }

    public virtual void ActivatePanel()
    {
        isPanelActive = true;
    }

    public virtual void DisablePanel()
    {
        isPanelActive = false;
    }
}
