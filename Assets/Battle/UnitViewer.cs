﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Enums;

public class UnitViewer : MonoBehaviour {

    Image unitImage;
    Text nameText;
    Image classImage;
    Image elementImage;
    Image celestialImage;
    
    Text hpText;
    Image hpBarImage;

    Text apText;
    Image apBarImage;
    
    // FIXME : 버프/디버프는 아직 미구현.

    public void UpdateUnitViewer(GameObject unit)
    {
        Unit unitInfo = unit.GetComponent<Unit>();
        unitImage.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        nameText.text = unitInfo.GetName();
        SetClassImage(unitInfo.GetUnitClass());
        SetElementImage(unitInfo.GetElement());
        SetCelestialImage(unitInfo.GetCelestial());
        hpText.text = unitInfo.GetCurrentHealth() + " / " + unitInfo.GetMaxHealth();
        apText.text = unitInfo.GetCurrentActivityPoint() + " (+" + unitInfo.GetActualDexturity() + ")";        
    }

    void SetClassImage(UnitClass unitClass)
    {
        if (unitClass == UnitClass.Melee)
            classImage.sprite = Resources.Load("Icon/meleeClass", typeof(Sprite)) as Sprite;
        else if (unitClass == UnitClass.Magic)
            classImage.sprite = Resources.Load("Icon/magicClass", typeof(Sprite)) as Sprite;
        else
            classImage.sprite = Resources.Load("Icon/transparent", typeof(Sprite)) as Sprite;
    }

    void SetElementImage(Element element)
    {
        if (element == Element.Fire)
            elementImage.sprite = Resources.Load("Icon/fire", typeof(Sprite)) as Sprite;
        else if (element == Element.Water)
            elementImage.sprite = Resources.Load("Icon/water", typeof(Sprite)) as Sprite;
        else if (element == Element.Plant)
            elementImage.sprite = Resources.Load("Icon/plant", typeof(Sprite)) as Sprite;
        else if (element == Element.Metal)
            elementImage.sprite = Resources.Load("Icon/metal", typeof(Sprite)) as Sprite;
        else
            elementImage.sprite = Resources.Load("Icon/transparent", typeof(Sprite)) as Sprite;
    }

    void SetCelestialImage(Celestial celestial)
    {
        if (celestial == Celestial.Sun)
            celestialImage.sprite = Resources.Load("Icon/sun", typeof(Sprite)) as Sprite;
        else if (celestial == Celestial.Moon)
            celestialImage.sprite = Resources.Load("Icon/moon", typeof(Sprite)) as Sprite;
        else if (celestial == Celestial.Earth)
            celestialImage.sprite = Resources.Load("Icon/earth", typeof(Sprite)) as Sprite;
        else
            celestialImage.sprite = Resources.Load("Icon/transparent", typeof(Sprite)) as Sprite;
    }

	// Use this for initialization
	void Start () {
        unitImage = transform.Find("UnitImage").GetComponent<Image>();
        nameText = transform.Find("NameText").GetComponent<Text>();
        classImage = transform.Find("ClassImage").GetComponent<Image>();
        elementImage = transform.Find("ElementImage").GetComponent<Image>();
        celestialImage = transform.Find("CelestialImage").GetComponent<Image>();
    
        hpText = GameObject.Find("HPText").GetComponent<Text>();;
        hpBarImage = GameObject.Find("HPBarImage").GetComponent<Image>();

        apText = GameObject.Find("APText").GetComponent<Text>();;
        apBarImage = GameObject.Find("APBarImage").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}