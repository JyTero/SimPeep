using UnityEngine;

public interface INeedAlertable
{
    void RegisterToNeedAlert();
    void UpdateAndRefreshData(Need need);
    void DeRegisterFromNeedAlert();
}
