using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStats : MonoBehaviour 
{
    public float damage;
    
    protected void DestroySelf()
    {
        Destroy(this.gameObject);
    }

}
