using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseQuest : MonoBehaviour
{
    #region variables

    #region protected-variables

    [SerializeField] protected int questRoomID;
    [SerializeField] protected Enumirators.QuestType questType;
    [SerializeField] protected GameObject questRoom;
    [SerializeField] protected MapGeneration mapGeneration;
    protected bool questCompletionState = false;
    [SerializeField] protected MapVisualization mapVisualization;

    #endregion

    #region public-variables

    #endregion

    #endregion

    #region functions

    #region protected-functions

    protected virtual void Awake()
    {
        mapGeneration = GameObject.FindWithTag("Map").GetComponent<MapGeneration>();
        mapVisualization = GameObject.FindWithTag("MapUI").GetComponent<MapVisualization>();
    }

    protected abstract void CreateQuest();

    #endregion

    #region public-functions


    #endregion

    #endregion
}
