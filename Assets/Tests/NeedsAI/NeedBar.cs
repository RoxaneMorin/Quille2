using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedBar : MonoBehaviour
{
    public Quille.BasicNeed associatedBasicNeed;
    
    [SerializeField] private Image needIcon;
    [SerializeField] private Image needFill;

    
    public void Prepare()
    {
        SetIcon(associatedBasicNeed.NeedIcon);
        UpdateFill(associatedBasicNeed.LevelCurrent);
    }

    private void SetIcon(Sprite iconToUse)
    {
        needIcon.sprite = iconToUse;
    }

    public void UpdateFill(float currentNeedLevel)
    {
        needFill.fillAmount = currentNeedLevel;
    }
}
