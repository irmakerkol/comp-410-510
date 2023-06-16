using System.Collections;
using UnityEngine;

public class ShieldSkill : Skill
{
    protected override IEnumerator Skill_CR()
    {

        manager.player.isShieldOn = true;
        this.transform.position += new Vector3(0f, 0f, 0f);
        float barTime = skillDuration;
        while (barTime >= 0)
        {
            if (!manager.player.isShieldOn) break;
            this.transform.position = manager.player.gameObject.transform.position + new Vector3(0f, 0f, 0f);
            barTime -= Time.deltaTime; // update elapsed time
            yield return null; // wait for the next frame
        }
        manager.player.isShieldOn = false;
        gameObject.SetActive(false); // disable the object    }
    }

}
