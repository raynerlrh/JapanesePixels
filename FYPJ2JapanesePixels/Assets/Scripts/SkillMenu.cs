using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour 
{
    public float speed;
    public GameObject buttons;
    public Transform defensiveSkills;
    public Transform offensiveSkills;
    public Text availableMovesText;

    Vector2 originalPos;
    bool disabled;

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
        for (int i = 0; i < defensiveSkills.childCount; i++)
        {
            defensiveSkills.GetChild(i).GetComponent<SkillButton>().AttachSkill(playerSkills.defensiveSkills[i]);
        }
        for (int i = 0; i < offensiveSkills.childCount; i++)
        {
            offensiveSkills.GetChild(i).GetComponent<SkillButton>().AttachSkill(playerSkills.offensiveSkills[i]);
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
        availableMovesText.text = PlayerMoveController.instance.numAvailableMoves.ToString();

        // Menu animations
        //if (PlayerMoveController.instance.NoMovesLeft())
        //{
        //    transform.localPosition = Vector2.MoveTowards(transform.localPosition, originalPos, speed * Time.deltaTime);

        //    if (transform.localPosition == new Vector3(originalPos.x, originalPos.y, transform.localPosition.z))
        //        gameObject.SetActive(false);

        //    GameModeManager.instance.question.text.fontSize = 200;
        //    Camera.main.GetComponent<CameraController>().switchMode(CamSettings.CamMode.E_PCENTERED);
        //    PlayerMoveController.instance.e_playstate = PlayerMoveController.PlayState.E_NONCOMBAT;
        //}
        //else
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(0, transform.localPosition.y), speed * Time.deltaTime);
        }
    }
}
