using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    public GameObject Path = null;
    public int StartFrom = 0;
    public float Speed = 0.1f;
    public float Closeness = 0.1f;

    [HideInInspector]
    public bool AutoPlay;
    Transform[] PathList;
    bool Paused;

    // Start is called before the first frame update
    void Start()
    {
        AutoPlay = false;
        Paused = false;
        PathList = Path.GetComponentsInChildren<Transform>();
        if(PathList.Length > 0)
        {
            transform.position = PathList[0].position;
        }
    }

    public void SetAutoPlay()
    {
        AutoPlay = true;
    }

    public void StopAutoPlay()
    {
        AutoPlay = false;
        Paused = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(AutoPlay)
        {
            return;
        }

        if(Path == null || Paused)
        {
            return;
        }

        try
        {
            int next = StartFrom + 1;
            if(next >= PathList.Length)
            {
                next = 0;
            }

            transform.position = Vector3.MoveTowards(transform.position, PathList[next].position, Time.deltaTime * Speed);

            float dist = Vector3.Distance(transform.position, PathList[next].position);
            if(Math.Abs(dist) <= Closeness)
            {
                // destination has been reached
                StartFrom = (StartFrom + 1) % PathList.Length;
            }
        } catch(Exception ex)
        {
            Debug.LogError("Failure on move to path: " + ex.StackTrace);
        }
    }

    public void Pause()
    {
        Paused = true;
    }

    public void Resume()
    {
        Paused = false;
    }
}
