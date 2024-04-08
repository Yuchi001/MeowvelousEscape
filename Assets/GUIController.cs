using Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{

    [SerializeField]
    List<HpBars> Bars;

    [SerializeField]
    TextMeshProUGUI CatScore;
    private void ChangeColorUI()
    {
        var colors = PlayerManager.Instance.KeyColors;
        for (int i = 0; i< transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            for(int j = 0; j < colors.Count; j++)
            {
                if (colors[j].key.ToString() == child.name)
                {
                    var grandChilds = child.transform.GetComponentsInChildren<Image>();
                    foreach (var item in grandChilds)
                    {
                        if(item.name == "Frame")
                        {
                            item.color = colors[j].color;
                        }
                    }
                    var grandChildsText = child.transform.GetComponentsInChildren<TextMeshProUGUI>();
                    foreach (var item in grandChildsText)
                    {
                        if (item.name == "Text" || item.name == "Level")
                        {
                            item.GetComponent<TextMeshProUGUI>().color = colors[j].color;
                        }
                    }
                }
            }
        }
    }

    private void SetActiveCats()
    {
        var cats = PlayerManager.Instance.TeamMembers;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            for(int j = 0; j< cats.Count; j++)
            {
                if (cats[j].attackKey.ToString() == child.name)
                {
                    var grandChilds = child.transform.GetComponentsInChildren<Image>();
                    for (int k = 0;k< grandChilds.Length; k++)
                    {
                        if (grandChilds[k].name == "Cat")
                        {
                            cats[j].member.GetCat().cat.SetMaterialColor(grandChilds[k]);
                        }
                    }
                    var grandChildsText = child.transform.GetComponentsInChildren<TextMeshProUGUI>();
                    for (int k = 0; k < grandChildsText.Length; k++)
                    {
                        if (grandChildsText[k].name == "Level")
                        {
                            grandChildsText[k].SetText("Level " + cats[j].member.GetCat().level);
                        }
                    }
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetActiveCats();
        ChangeColorUI();
    }

    // Update is called once per frame
    void Update()
    {
        var cats = PlayerManager.Instance.TeamMembers;
        foreach (var cat in cats)
        {
            foreach (var bar in Bars)
            {
                if (cat.attackKey != bar.key) continue;
                
                var activeCat = cat.member.GetCat();
                var catInfo = cat.member.GetCat().cat.GetSpecificInfo(activeCat.level);
                
                bar.Image.fillAmount = activeCat.health / (catInfo.maxHealth != 0 ? catInfo.maxHealth : 1);
                //bar.ColdDown.fillAmount = catInfo.cooldown;
            }

        }
        CatScore.SetText(PlayerManager.Instance.RescuedCatsCount.ToString());
        //TODO Przerobic na Event
        SetActiveCats();
    }



    [System.Serializable]
    public struct HpBars
    {
        public KeyCode key;
        public Image Image;
        public Image ColdDown;
    }
}
