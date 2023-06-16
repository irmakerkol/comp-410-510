using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public int t;
    public Transform characterSkinsPosition;
    public Text unlockBtnTxt;
    public Manager manager;
    public RuntimeAnimatorController controllerForRT;

    private void Start()
    {
        SetCharacterSkinsPanel();
    }

    public void ChangeCharacterSkin(bool right)
    {
        if (right)
        {
            t++;
            for (int i = 0; i < manager.characterSkins.Count; i++)
            {
                characterSkinsPosition.GetChild(i).gameObject.SetActive(false);
            }

            if (t >= manager.characterSkins.Count)
                t = 0;

            characterSkinsPosition.GetChild(t).gameObject.SetActive(true);
        }
        else
        {
            t--;
            for (int i = 0; i < manager.characterSkins.Count; i++)
            {
                characterSkinsPosition.GetChild(i).gameObject.SetActive(false);
            }

            if (t < 0)
                t = manager.characterSkins.Count - 1;

            characterSkinsPosition.GetChild(t).gameObject.SetActive(true);
        }

            unlockBtnTxt.text = "Select";
    }

    public void SetCharacterSkin()
    {
        if (PlayerPrefs.GetInt(manager.characterSkins[t].name) == 1)
        {
            PlayerPrefs.SetInt("CharacterSkin", t);
            manager.ClearScene();
            manager.CreateMenuScene();
        }
        else
        {

            PlayerPrefs.SetInt(manager.characterSkins[t].name, 1);
            unlockBtnTxt.text = "Select";
        }
    }

    public void SetCharacterSkinsPanel()
    {
        if (!PlayerPrefs.HasKey("CharacterSkin"))
        {
            PlayerPrefs.SetInt("CharacterSkin", 0);
            PlayerPrefs.SetInt(manager.characterSkins[0].name, 1);
        }

        t = PlayerPrefs.GetInt("CharacterSkin");

        for (int i = 0; i < manager.characterSkins.Count; i++)
        {
            if (!PlayerPrefs.HasKey(manager.characterSkins[i].name))
            {
                PlayerPrefs.SetInt(manager.characterSkins[i].name, 0);
            }

            GameObject character = Instantiate(manager.characterSkins[i]);
            print(character);
            DestroyImmediate(character.GetComponent<Player>());
            DestroyImmediate(character.GetComponent<Rigidbody>());

            character.GetComponent<Animator>().runtimeAnimatorController = controllerForRT;

            character.transform.SetParent(characterSkinsPosition, false);
            SetLayer(character.transform, LayerMask.NameToLayer("CharacterSkins RT"));

            if (i != t)
                character.SetActive(false);
        }

        unlockBtnTxt.text = "Select";
    }

    public virtual void SetLayer(Transform trans, int layer)
    {
        trans.gameObject.layer = layer;
        foreach (Transform child in trans)
            SetLayer(child, layer);
    }
}