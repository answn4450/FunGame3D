using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllManager : MonoBehaviour
{
    const int optionStructPos = 0;
    const int optionStructUse = 1;
    const int optionAttackType = 2;

    int usingOption;

    void Start()
    {
        usingOption = 0;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
		{
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                usingOption--;
            if (Input.GetKeyDown(KeyCode.RightArrow))
                usingOption++;

            usingOption = (usingOption - 1) % 3 + 1;
        }

    }
}
