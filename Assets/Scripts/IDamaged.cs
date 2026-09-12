using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamaged
{
    void OnDamaged(int damage, Vector3 ent);
}
