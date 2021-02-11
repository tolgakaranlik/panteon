using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public bool PathStarted = false;

    private NavMeshAgent Agent;
    private Rigidbody RB;
    private Animator Anim;
    private float OldSpeed;
    private int InvisibilityTime;

    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();

        OldSpeed = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region Invisibility
        // When you hit an obstacle, you are given a short time to recover. During this period of time,
        // your character blinks and no collision is happening. I could do this with an Animator component,
        // but there are examples of Animator component in the project already. So I wanted to demonstrate
        // other methods as well. This is why I have coded this section
        if (InvisibilityTime != -1)
        {
            if (++InvisibilityTime < 150)
            {
                if (InvisibilityTime % 15 == 0)
                {
                    // no toggle effect
                }
            }
            else
            {
                InvisibilityTime = -1;
                RB.useGravity = true;
                GetComponent<BoxCollider>().enabled = true;
            }
        }
        #endregion

        if (!Agent.pathPending && Agent.enabled && PathStarted)
        {
            float dist = Agent.remainingDistance;

            if (Agent.pathStatus == NavMeshPathStatus.PathComplete && Agent.remainingDistance <= 0.01)
            {
                // agent is done
                Debug.Log("Agent is done");

                Agent.enabled = false;
                Anim.SetBool("isStanding", true);
                Anim.SetBool("isRunning", false);
                Anim.SetBool("isDead", false);

                Anim.CrossFade("Stand", 0.01f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            OldSpeed = Agent.speed;
            Agent.speed = 0f;
            Anim.SetBool("isDead", true);
            Anim.SetBool("isRunning", false);
            Anim.CrossFade("Fall", 0.01f);

            RB.useGravity = false;
            GetComponent<BoxCollider>().enabled = false;

            StartCoroutine(BackFromDead());
        }
        else if (collision.collider.tag == "Rotating")
        {
            //BiasX = 0.0015f;
        }
        else if (collision.collider.tag == "Environment")
        {
            transform.rotation = Quaternion.identity;
            //BiasX = 0f;
        }
    }

    IEnumerator BackFromDead()
    {
        yield return new WaitForSeconds(3f);

        Anim.SetBool("isDead", false);
        Anim.SetBool("isRunning", true);
        Anim.CrossFade("Run", 0.01f);

        Agent.Move(new Vector3(0, 0, -(transform.position.z + 0.25f)));

        InvisibilityTime = 0;
        Agent.speed = OldSpeed;
        transform.rotation = Quaternion.identity;
    }
}
