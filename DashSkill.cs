using System.Collections;
using UnityEngine;

public class DashSkill : Skill
{


    protected override IEnumerator Skill_CR()
    {
        float elapsed = 0f; // elapsed time since start of movement
        manager.player.speed *= 5;
        manager.player.isShieldOn = true;
        this.transform.position = manager.player.transform.position - new Vector3(0f, -1.2f, 1.2f);
        while (elapsed < skillDuration)
        {
            manager.player.isShieldOn = true;
            this.transform.position = manager.player.transform.position - new Vector3(0f, -1.2f, 1.2f);
            elapsed += Time.deltaTime; // update elapsed time
            yield return null; // wait for the next frame
        }
        manager.player.speed /= 5;
        manager.player.isShieldOn = false;

        gameObject.SetActive(false); // disable the object
    }

}
