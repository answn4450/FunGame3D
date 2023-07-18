using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrefabManager
{
    private static PrefabManager Instance = null;

    public static PrefabManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new PrefabManager();
        }
        return Instance;
    }

    // ** ������ �����
    private Dictionary<string, GameObject> prototypeObjectList = new Dictionary<string, GameObject>();

    private PrefabManager()
    {
        // ** �����͸� �ҷ��´�.
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");

        // ** �ҷ��� �����͸� Dictionary �� ����
        foreach (GameObject element in prefabs)
            prototypeObjectList.Add(element.name, element);

    }

    // ** �ܺο��� �������� Prefab�� ���� �� �� �ֵ��� Getter�� ����.
    public GameObject GetPrefabByName(string name)
    {
        // ** ���࿡ key�� ���� �Ѵٸ� ���� ��ü�� ��ȯ�ϰ�...
        if (prototypeObjectList.ContainsKey(name))
            return prototypeObjectList[name];
        // ** �׷��� ���������� null
        return null;
    }
}