using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int hitDamage = 100;

    [SerializeField] ShieldSkill shield;

    public GameObject mainObject;

    private void OnCollisionEnter(Collision other)
    {
        Player hero = other.gameObject.GetComponent<Player>();

        if (hero.isShieldOn)
        {
            if (hasShield(hero))
            {
                mainObject.SetActive(false);
            }
            hero.isShieldOn = false;
            return;
        }

        hero.Die();
        this.gameObject.GetComponent<Collider>().enabled = false;
    }

    bool hasShield(Player hero)
    {
       return shield.isActiveAndEnabled;
    }

}
