using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class UIGauge : UISubItem
{
    enum Objects{
        GaugeBar,
        GaugeBox,
    }

    [SerializeField] private int maxBlocks = 50; // 최대 블록 수
    private List<Image> blocks = new List<Image>(); // 생성된 블록 리스트

    public override bool Init()
    {
        if(base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));

        GameObject gagueBox = GetObject((int)Objects.GaugeBox);
        blocks.Add(gagueBox.GetComponent<Image>());
        GameObject gaugeBar = GetObject((int)Objects.GaugeBar);

        HorizontalLayoutGroup gaugeBarHorizontalLayoutGroup = gaugeBar.GetOrAddComponent<HorizontalLayoutGroup>();
        gaugeBarHorizontalLayoutGroup.childControlWidth = true;

        Logger.Log("Init");
        for(int i = 0; i < maxBlocks; i++){
            blocks.Add(Managers.Resource.Instantiate(gagueBox, gaugeBar.transform).GetComponent<Image>());
        }

        return true;
    }

    public void SetGauge(float value)
    {
        for(int i = maxBlocks; i >= 0; i--){
            if((float)i/maxBlocks <= value){
                blocks[i].DOFade(1f, 0.2f);
            }
            else{
                blocks[i].DOFade(0.1f, 0.2f);
            }
        }
    }


}
