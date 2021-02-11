using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public bool Running = false;
    public float Speed = 0.1f;
    public float SlideSpeed = 0.05f;
    public SkinnedMeshRenderer Skin;
    public GUIController GUI;
    public Camera MainCamera;
    public EnemySpawner Spawner;
    public Es.InkPainter.InkCanvas InkCanvas;

    bool ObstacleHit;
    bool RunningOld;
    bool MoveToSide;
    bool CanMove;
    float TargetLocation;
    float BiasX;
    int InvisibilityTime;
    int Lives = 3;
    Animator Anim;
    Rigidbody RB;

    const int SOUND_DONG = 0;
    const int SOUND_CHIME = 1;

    void Start()
    {
        BiasX = 0f;
        CanMove = false;
        MoveToSide = false;
        Running = false;
        RunningOld = false;
        ObstacleHit = false;
        InvisibilityTime = -1;

        Anim = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
        GUI.UpdateLives(3);

        Anim.SetBool("isStanding", true);
        Anim.SetBool("isRunning", false);
        Anim.SetBool("isDead", false);

        Anim.CrossFade("Stand", 0.01f);
        StartCoroutine(RoundStartSequence());
    }

    void FixedUpdate()
    {
        #region Invisibility
        // When you hit an obstacle, you are given a short time to recover. During this period of time,
        // your character blinks and no collision is happening. I could do this with an Animator component,
        // but there are examples of Animator component in the project already. So I wanted to demonstrate
        // other methods as well. This is why I have coded this section
        if(InvisibilityTime != -1)
        {
            if(++InvisibilityTime < 150)
            {
                if(InvisibilityTime % 15 == 0)
                {
                    Skin.enabled = !Skin.enabled;
                }
            } else
            {
                Skin.enabled = true;
                InvisibilityTime = -1;
                RB.useGravity = true;
                GetComponent<BoxCollider>().enabled = true;
            }
        }
        #endregion

        GUI.DisplayOrder(Spawner.GetOrder(transform.position.z) + 1);

        if (!ObstacleHit && CanMove)
        {
            // Dead?
            if(transform.position.y < -0.325)
            {
                Die();
                return;
            }

            // Detect state change
            if (RunningOld != Running)
            {
                RunningOld = Running;
                ToggleRunning(RunningOld);
            }

            GUI.UpdateProgress(transform.position.z);

            // Move forward
            transform.position += Vector3.forward * Speed + Vector3.left * BiasX;

            // Move sides
            if (MoveToSide)
            {
                if(TargetLocation < transform.position.x)
                {
                    // move to left
                    if((transform.position + Vector3.left * SlideSpeed).x <= TargetLocation)
                    {
                        transform.position = new Vector3(TargetLocation, transform.position.y, transform.position.z);
                        MoveToSide = false;
                    } else
                    {
                        transform.position += Vector3.left * SlideSpeed;
                    }
                } else
                {
                    // move to right
                    if ((transform.position + Vector3.right * SlideSpeed).x >= TargetLocation)
                    {
                        transform.position = new Vector3(TargetLocation, transform.position.y, transform.position.z);
                        MoveToSide = false;
                    }
                    else
                    {
                        transform.position += Vector3.right * SlideSpeed;
                    }
                }
            }

            // Collect user input to go sides
            if(Input.touches.Length > 0)
            {
                // process touch 1 only, ignore others
                TargetLocation = -0.18f + (Input.touches[0].position.x / Screen.width) * 0.36f;
                MoveToSide = true;
            }

            if(Input.GetMouseButton(0))
            {
                TargetLocation = -0.18f + (Input.mousePosition.x / Screen.width) * 0.36f;
                if(TargetLocation < -0.18f)
                {
                    TargetLocation  = -0.18f;
                }

                if (TargetLocation > 0.18f)
                {
                    TargetLocation = 0.18f;
                }

                MoveToSide = true;
            }
        }
    }

    private void Die()
    {
        CanMove = false;
        GUI.FailUI();
        RB.isKinematic = true;
    }

    private void Succeed()
    {
        MainCamera.nearClipPlane = 0.3f;
        CanMove = false;
        GUI.SuccessUI();
        RB.isKinematic = true;
        GUI.SetTexturePainted(0);
        StartCoroutine(MeasurePaintedParts());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            GetComponents<AudioSource>()[SOUND_DONG].Play();
            ObstacleHit = true;
            Anim.SetBool("isDead", true);
            Anim.SetBool("isRunning", false);
            Anim.CrossFade("Fall", 0.01f);

            RB.useGravity = false;
            GetComponent<BoxCollider>().enabled = false;

            if (--Lives <= 0)
            {
                Die();
            }
            else
            {
                GUI.UpdateLives(Lives);
                StartCoroutine(BackFromDead());
            }
        }
        else if (collision.collider.tag == "Rotating")
        {
            BiasX = 0.0015f;
        }
        else if (collision.collider.tag == "Environment")
        {
            transform.rotation = Quaternion.identity;
            BiasX = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Finish")
        {
            StartCoroutine(FinishLevel());
        }
    }

    private void ToggleRunning(bool value)
    {
        switch(value)
        {
            case true:
                Anim.SetBool("isStanding", false);
                Anim.SetBool("isRunning", true);
                Anim.SetBool("isDead", false);

                Anim.CrossFade("Run", 0.01f);
                break;
            case false:
                Anim.SetBool("isStanding", true);
                Anim.SetBool("isRunning", false);
                Anim.SetBool("isDead", false);

                Anim.CrossFade("Stand", 0.01f);
                break;
        }
    }

    IEnumerator MeasurePaintedParts()
    {
        yield return new WaitForSeconds(0.33f);

        float percent = 0f;
        var texture = InkCanvas.PaintDatas[0].paintMainTexture;
        var newTex = new Texture2D(texture.width, texture.height);
        RenderTexture.active = texture;
        newTex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        newTex.Apply();

        Color[] data = newTex.GetPixels(0, 0, texture.width, texture.height);
        int found = 0;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (data[y * texture.width + x].r == 1 && data[y * texture.width + x].g == 0 && data[y * texture.width + x].b == 0)
                {
                    found++;
                }
            }
        }

        percent = texture.height * texture.width;
        percent = 100f * (float)found / percent;
        GUI.SetTexturePainted(percent);

        if (percent < 100)
        {
            StartCoroutine(MeasurePaintedParts());
        }
    }

    IEnumerator FinishLevel()
    {
        yield return new WaitForSeconds(0.5f);

        CanMove = false;

        Anim.SetBool("isStanding", true);
        Anim.SetBool("isRunning", false);
        Anim.SetBool("isDead", false);

        Succeed();

        ToggleRunning(Running);
        GetComponents<AudioSource>()[SOUND_CHIME].Play();
        Anim.CrossFade("Stand", 0.01f);
    }

    IEnumerator BackFromDead()
    {
        yield return new WaitForSeconds(3f);

        Anim.SetBool("isDead", false);
        Anim.SetBool("isRunning", true);
        Anim.CrossFade("Run", 0.01f);

        transform.position = new Vector3(0, 0.005f, -0.25f);
        InvisibilityTime = 0;
        ObstacleHit = false;
        transform.rotation = Quaternion.identity;
    }

    IEnumerator RoundStartSequence()
    {
        GUI.StartCountDown();

        yield return new WaitForSeconds(4f);

        CanMove = true;
        Running = true;
        GUI.ClearCountDown();
    }
}
