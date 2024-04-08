using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class eq_slot : MonoBehaviour
{

    [SerializeField]
    SOCat Cat;
    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetCat(SOCat cat)
    {
        this.Cat = cat;
        var tmp = transform.GetChild(0).GetComponent<Image>();
        Cat.SetMaterialColor(tmp);
    }
    public SOCat GetCat()
    {
        return Cat;
    }

    public void Select(bool selected)
    {
        var img = GetComponent<Image>();
        if (selected)
        {
            selected = true;            
            img.color = Color.red;
        }
        else
        {
            selected = true;
            img.color = Color.white;
        }

    }
}
