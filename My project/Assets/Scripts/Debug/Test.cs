using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    private GameObject bullet;
    public GameObject a;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        //DontDestroyOnLoad(a);

        a.transform.position = Vector3.zero;
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.Space))
            a.transform.position += Vector3.one * Time.deltaTime;
        if (Input.GetKeyUp(KeyCode.Space))
            SceneManager.LoadScene("Test");
    }
}
