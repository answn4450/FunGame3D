using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Vector3 core = Vector3.zero;
    public Transform coreTransform;
    public Vector3 hover = Vector3.zero;
    public Transform hoverTransform;

    public AudioSource audioSource;
    public AudioClip audioClip;

    public void Update()
    {
        core = coreTransform.position;
        hover = hoverTransform.position;
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("sad");
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
}
