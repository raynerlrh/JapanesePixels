using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerSkill : PlayerSkill 
{
    public bool b_needsUpdate { get; set; }
    public string skillName { get; set; }

    public TestPlayerSkill()
    {
        skillName = "Test Skill";
    }

    public void ExecuteSkill()
    {
        Debug.Log("Executed Skill: " + skillName);
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
