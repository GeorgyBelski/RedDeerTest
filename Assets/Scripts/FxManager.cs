using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public GameObject startFumePrefab;
    public static GameObject fumePrefab;
    public static List<FxController> fxList = new List<FxController>();

    private void Start()
    {
        fumePrefab = startFumePrefab;
    }
    public static FxController GetFume() 
    {
        FxController fxController = null;
        foreach (FxController fx in fxList)
        {
            if (!fx.isActive)
            {
                fxController = fx;
                break;
            }
        }
        if (!fxController)
        {
            fxController = Instantiate(fumePrefab).GetComponent<FxController>();
            fxList.Add(fxController);
        }
        return fxController;
    }
}
