using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eq_select : MonoBehaviour
{
    private SOCat selectedCat;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: podpiac pod player controller i dla kazdego kota puscisc ta petle
        //for(int i = 0; i< eqCats.count-1; i++)
        //{
        //    var catEqSlot = transform.GetChild(i);
        //    catEqSlot.GetComponent<eq_slot>().SetCat(eqCats[i]);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void OnSelectCat(GameObject cat)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<eq_slot>().Select(false);
        }
        cat.GetComponent<eq_slot>().Select(true);
        selectedCat = cat.GetComponent<eq_slot>().GetCat();

        //TODO: Zmienic teksty z prawej
    }
}
