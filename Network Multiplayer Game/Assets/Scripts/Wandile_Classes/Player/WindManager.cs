using UnityEngine;

public class WindManager : MonoBehaviour
{
    #region Custom Variables

    public static WindManager instance;

    public Vector3 windDirection;

    #endregion

    #region Built-In Methods

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    #endregion

    #region Custom Methods

    public Vector3 GetWindDirection()
    {
        return windDirection;
    }

    #endregion
}
