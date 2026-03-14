using UnityEngine;

public interface IParryable
{
    void OnParried(UnityEngine.Vector3 parryDirection, float parryForce, float upwardForce, float torqueForce);
}
