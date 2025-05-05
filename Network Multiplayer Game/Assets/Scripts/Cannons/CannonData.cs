using UnityEngine;

[CreateAssetMenu(fileName = "CannonData", menuName = "Scriptable Objects/CannonData")]
public class CannonData : ScriptableObject
{

    [SerializeField] private string cannonName;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float launchAngle; //will say in inspector
    [SerializeField] private float rangeZ;  //will say in inspector
    [SerializeField] private float heightY;
    [SerializeField] private float timeTaken;
    [SerializeField] private float lifespan;
    [SerializeField] private float damage;

    public GameObject BallPrefab => ballPrefab;
    public float LaunchAngle => launchAngle;
    public float RangeZ => rangeZ;
    public float HeightY => heightY;
    public float TimeTaken => timeTaken;
    public float Lifespan => lifespan;
    public float Damage => damage;


}
