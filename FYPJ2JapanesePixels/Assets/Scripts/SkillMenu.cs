using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenu : MonoBehaviour 
{
    public float speed;
    public GameObject buttons;

    Vector2 originalPos;
    bool disabled;

    public SkillActivation.SKILL_TYPE skillType;

    void Awake()
    {
        originalPos = transform.localPosition;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (disabled)
            buttons.SetActive(false);

        PlayerSkillController playerSkills = PlayerSkillController.instance;

        // Set skills 
        if (skillType == SkillActivation.SKILL_TYPE.TYPE_DEFENSIVE)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SkillButton>().SetSkill(playerSkills.defensiveSkills[i]);
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SkillButton>().SetSkill(playerSkills.offensiveSkills[i]);
            }
        }
    }

    void OnDisable()
    {
        disabled = true;
        buttons.SetActive(true);
        transform.localPosition = originalPos;
    }

    void Update()
    {
        // Menu animations
        if (PlayerMoveController.instance.NoMovesLeft())
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, originalPos, speed * Time.deltaTime);

            if (transform.localPosition == new Vector3(originalPos.x, originalPos.y, transform.localPosition.z))
                gameObject.SetActive(false);
        }
        else
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(0, transform.localPosition.y), speed * Time.deltaTime);
        }
    }
}
