using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerSkill
{
    bool b_needsUpdate { get; set; }
    string skillName { get; set; }
    int cellsAffected { get; set; }
    float damage { get; set; }
    float movesTaken { get; set; }
    void ExecuteSkill();
    void Update();
}
