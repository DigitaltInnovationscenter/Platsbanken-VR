using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SimulateGrab : MonoBehaviour
{
    private int count;
    private GameObject currentOrb;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        count++;
        if (count == 400) {
            currentOrb = OrbsManager.Instance.orbLevel0GameObject;
            OrbsManager.Instance.ActivateOrb(currentOrb);
        }
        if (count == 800) {
            SelectFromNextLevel();
        }
        if (count == 1200) {
            SelectFromNextLevel();
        }
        if (count == 1600) {
            SelectFromNextLevel();
        }
    }

    private void SelectFromNextLevel() {
        var lastOrb = currentOrb;
        var root = currentOrb.transform.Find("OrbChildsRoot");
        if(root != null && root.childCount > 0) {
            currentOrb = root.GetChild(0).gameObject;
            OrbsManager.Instance.ActivateOrb(currentOrb);
            currentOrb.transform.parent = null;
            Destroy(lastOrb);
        }
    }

}
