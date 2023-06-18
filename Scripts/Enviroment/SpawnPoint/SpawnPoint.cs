using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint 
{
    public  bool IsEmpty=> _isEmpty;

    private bool _isEmpty;
    private Vector3 _position;

    public Vector3 Position=> _position;

    public SpawnPoint(Vector3 position)
    {
        _isEmpty = true;
        _position = position;
    }

    public void SetEmptiness(bool isEmpty)
    {
        _isEmpty = isEmpty;
    }

}
