using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MinimapSettings : MonoBehaviour
{
    #region Custom Variables

    [SerializeField] private Transform destination;
    [SerializeField] private Transform player;

    private NavMeshPath path;
    [SerializeField] private LineRenderer lineRenderer;

    #endregion

    #region Built-In Methods

    void Start()
    {
        path = new NavMeshPath();
    }

    private void Update()
    {
        if (NavMesh.CalculatePath(player.position, destination.position, NavMesh.AllAreas, path))
        {
            lineRenderer.positionCount = path.corners.Length;

            for (int i = 0; i < path.corners.Length; i++)
            {
                lineRenderer.SetPosition(i, path.corners[i]);
            }
        }
    }

    #endregion

    #region Custom Methods
    #endregion
}
