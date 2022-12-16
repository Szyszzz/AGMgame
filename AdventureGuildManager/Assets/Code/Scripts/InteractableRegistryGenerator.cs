using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRegistryGenerator : MonoBehaviour
{
    public GameObject ChairParentObject;
    public GameObject CounterParentObject;

    public List<GameObject> InteractablesRegistry;

    void Awake()
    {
        FillRegistryWithChildrenObjects(ChairParentObject);
        FillRegistryWithChildrenObjects(CounterParentObject);
    }

    private void FillRegistryWithChildrenObjects(GameObject parent)
    {
        for(int i = 0; i < parent.transform.childCount; i++)
        {
            InteractablesRegistry.Add(parent.transform.GetChild(i).gameObject);
        }
    }
}
