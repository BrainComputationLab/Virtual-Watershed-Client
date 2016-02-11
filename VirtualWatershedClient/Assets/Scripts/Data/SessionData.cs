﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using VTL.ListView;
using SimpleJson;

[Serializable]
public class SessionObjectStructure
{
    public string Name;
    public string DataLocation;
    public SerialVector3 GameObjectPosition;
}

[Serializable]
public class SessionDataStructure
{
    public string Location;
    public SerialVector3 PlayerPosition;
    public SerialVector3 PlayerRotation;
    public float PlayerAngle;
    public Dictionary<string,SessionObjectStructure> GameObjects;
}

/// <summary>
/// The Session Data class is meant to hold any data that is added and *loaded* into the Unity Session.
/// </summary>
public class SessionData
{
    ListViewManager listview;
    private GameObject Go;
    GameObject PlayerController
    {
        get
        {
            if (!Go)
            {
                Go = GameObject.Find("/UIContainer/PlayerController/ControlScripts");
            }
            return Go;
        }
    }
    public Vector3 PlayerPosition
    {
        get
        {
            return PlayerController.transform.position;
        }
    }

    public Vector3 PlayerRotation
    {
        get
        {
            Vector3 rot;
            float angle;
            PlayerController.transform.rotation.ToAngleAxis(out angle, out rot);
            return rot;
        }
    }


    public float PlayerAngle
    {
        get
        {
            Vector3 rot;
            float angle;
            Go.transform.rotation.ToAngleAxis(out angle, out rot);
            return angle;
        }
    }
    public string Location
    {
        get
        {
            return GlobalConfig.Location;
        }
        
    }

    // json specification -- that may be turned into a schema later.
    // {
    // Player Position
    // Location
    // list of data in scene
    // [
    // Gameobject
    // {
    // name -- name of the data to be placed into the session
    // position -- where was this data located
    // original dataset local: url or file
    // }
    //
    // }

    // Store everything by variable or model run name your choice
    public Dictionary<string, WorldObject> SessionObjects = new Dictionary<string,WorldObject>();

    public SessionData()
    {

    }

    public void SaveSessionData(string filename)
    {
        SessionDataStructure dataStructure = new SessionDataStructure();
        dataStructure.PlayerPosition = PlayerPosition;
        dataStructure.PlayerRotation = PlayerRotation;
        dataStructure.PlayerAngle = PlayerAngle;
        dataStructure.Location = Location;
        dataStructure.GameObjects = new Dictionary<string, SessionObjectStructure>();
        foreach (var pair in SessionObjects)
        {
            Debug.LogError(pair.Key);
            Debug.LogError(pair.Value != null);
             dataStructure.GameObjects[pair.Key] = pair.Value.saveSessionData();
        }
        string jsonstring = Newtonsoft.Json.JsonConvert.SerializeObject(dataStructure);

        // Save the file out
        String pathDownload = filename;
        using (StreamWriter file = new StreamWriter(@pathDownload))
        {
            file.Write(jsonstring);
        }
    }

    public int NumberOfObjects()
    {
        return SessionObjects.Count;
    }

    public void InsertSessionData(WorldObject worldObject)
    {
        if (!SessionObjects.ContainsKey(worldObject.record.name))
        {
            Debug.LogError("===================================== INSERT");
            SessionObjects[worldObject.record.name] = worldObject;
        }
    }

    public void Clear()
    {
        SessionObjects.Clear();
    }


    public WorldObject GetSessionObject(string name)
    {
        return SessionObjects[name];
    }

}
