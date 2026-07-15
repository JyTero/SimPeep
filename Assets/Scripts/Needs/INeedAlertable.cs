using UnityEngine;

public interface INeedAlertable
{
    void RegisterToNeedAlert();
    void UpdateData(Need need);
    void DeRegisterFromNeedAlert();
}
