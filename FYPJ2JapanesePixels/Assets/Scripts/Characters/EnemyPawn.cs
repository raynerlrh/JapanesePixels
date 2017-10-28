using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyPawn<T> {
    void moveForward(T forwardvec);
}
