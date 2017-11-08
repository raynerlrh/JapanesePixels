using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Must have a defalt character and move forward
/// </summary>
/// <typeparam name="T">Vector3 expected</typeparam>
public interface EnemyPawn<T> {
    void moveForward(T forwardvec);
    DefaultCharacter char_stat { get; set; }
}
