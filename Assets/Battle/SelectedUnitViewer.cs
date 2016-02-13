﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Enums;

public class SelectedUnitViewer : MonoBehaviour {

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

    public void UpdateUnitViewer(GameObject unitObject)
    {
        Unit unit = unitObject.GetComponent<Unit>();
        unitImage.sprite = unit.GetDefaultSprite();
        nameText.text = unit.GetName();
        SetClassImage(unit.GetUnitClass());
        SetElementImage(unit.GetElement());
        SetCelestialImage(unit.GetCelestial());
        UpdateHp(unit);
        UpdateAp(unit);
    }

    void UpdateHp(Unit unit)
    {
        hpText.text = unit.GetCurrentHealth() + " / " + unit.GetMaxHealth();
        hpBarImage.transform.localScale = new Vector3(((float)unit.GetCurrentHealth())/((float)unit.GetMaxHealth()), 1, 1);
    }
    
    void UpdateAp(Unit unit)
    {
        apText.text = unit.GetCurrentActivityPoint() + " (+" + unit.GetActualDexturity() + ")";
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
    
    void Awake () {
        unitImage = transform.Find("UnitImage").GetComponent<Image>();
        nameText = transform.Find("NameText").GetComponent<Text>();
        classImage = transform.Find("ClassImage").GetComponent<Image>();
        elementImage = transform.Find("ElementImage").GetComponent<Image>();
        celestialImage = transform.Find("CelestialImage").GetComponent<Image>();
    
        hpText = transform.Find("HP").transform.Find("HPText").GetComponent<Text>();;
        hpBarImage = transform.Find("HP").transform.Find("HPBarImage").GetComponent<Image>();

        apText = transform.Find("AP").transform.Find("APText").GetComponent<Text>();;
        apBarImage = transform.Find("AP").transform.Find("APBarImage").GetComponent<Image>();
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
