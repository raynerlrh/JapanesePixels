using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerSkill 
{
    string skillName { get; set; }

    void ExecuteSkill();
}
