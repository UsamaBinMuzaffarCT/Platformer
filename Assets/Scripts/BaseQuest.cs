using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseQuest : MonoBehaviour
{
    #region variables

    #region protected-variables

    protected int questRoomID;
    protected Enumirators.QuestType questType;
    protected GameObject questRoom;
    protected MapGeneration mapGeneration;
    protected bool questCompletionState = false;
    protected MapVisualization mapVisualization;
    protected GameManager gameManager;

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
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    public abstract void CreateQuest();

    #endregion

    #region public-functions


    #endregion

    #endregion
}
