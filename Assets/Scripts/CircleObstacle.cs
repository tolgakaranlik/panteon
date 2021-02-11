using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class CircleObstacle : MonoBehaviour
{
    public GameObject PartToMove;
    public Vector3 MovementFinal;
    public float SecondsToWait;
    public float SecondsToMoveForward;
    public float SecondsToMoveBackwards;

    Vector3 InitialPosition;
    DateTime PeriodStart;
    bool IsMoving;

    // Start is called before the first frame update
    void Start()
    {
        IsMoving = false;
        PeriodStart = DateTime.Now;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!IsMoving && (DateTime.Now - PeriodStart).TotalSeconds >= SecondsToWait)
        {
            // start the operation
            IsMoving = true;
            InitialPosition = PartToMove.transform.localPosition;
            PartToMove.transform.DOLocalMove(PartToMove.transform.localPosition + MovementFinal, SecondsToMoveForward);
            StartCoroutine(ComeBackToOriginalPosition());
        }
    }

    IEnumerator ComeBackToOriginalPosition()
    {
        yield return new WaitForSeconds(SecondsToMoveForward);

        PartToMove.transform.DOLocalMove(InitialPosition, SecondsToMoveBackwards);
        StartCoroutine(ResetOperation());
    }

    IEnumerator ResetOperation()
    {
        yield return new WaitForSeconds(SecondsToMoveBackwards);
        IsMoving = false;
        PeriodStart = DateTime.Now;
    }
}
