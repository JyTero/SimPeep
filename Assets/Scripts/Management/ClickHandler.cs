using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : ManagementCore
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (debugLog)
                    Debug.Log("On UI");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Interactable intreractable = hit.collider.gameObject.GetComponent<Interactable>();
                if (intreractable != null)
                {
                    if (debugLog)
                        Debug.Log("On Item");

                    UIController.ShowListOfInteractions(intreractable);
                    return;
                }



            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (debugLog)
                    Debug.Log("On UI");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var chara = hit.collider.gameObject.GetComponent<Character>();
                if (chara != null)
                {
                    if (debugLog)
                        Debug.Log("On Character");

                    UIController.ChangeSelectCharacter(chara);
                    return;
                }
            }
        }
    }
}
