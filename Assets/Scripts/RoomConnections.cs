using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnections : MonoBehaviour
{
    #region variables

    #region private-variables

    #endregion

    #region public-variable

    public List<GameObject> teleportationPoints = new List<GameObject>();
    public int id;
    public GameObject startPoint = null;

    #endregion

    #endregion

    #region functions

    #region private-functions

    private void Awake()
    {
        teleportationPoints = FindTeleportationPoints();
    }

    private void Start()
    {
        
    }

    private List<GameObject> FindTeleportationPoints()
    {
        List<GameObject> list = new List<GameObject>();
        int i = 0;
        foreach (Transform child in transform)
        {
            if(child.tag == "TeleportationPoint")
            {
                child.GetComponent<Teleportation>().id = i;
                list.Add(child.gameObject);
                i++;
            }
        }
        return list;
    }

    #endregion

    #region public-functions

    #endregion

    #endregion
}
