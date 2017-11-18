using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerSkill 
{
    bool b_needsUpdate { get; set; }
    string skillName { get; set; }

    void ExecuteSkill();
    void Update();
}
