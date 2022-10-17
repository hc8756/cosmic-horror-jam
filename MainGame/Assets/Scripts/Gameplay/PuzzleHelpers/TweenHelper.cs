using UnityEngine;
using DG.Tweening;

// Easily use DOTween from UnityEvents. I just set up the ones that seem useful.
// Kind of a hacky solution but it's very useful
// http://dotween.demigiant.com/documentation.php
public class TweenHelper : MonoBehaviour
{
    public void TransformDOMove(Transform t, Vector3 endValue, float duration) => t.DOMove(endValue, duration);
    public void TransformDORotate(Transform t, Vector3 endValue, float duration) => t.DORotate(endValue, duration);
    public void TransformDOLocalMove(Transform t, Vector3 endValue, float duration) => t.DOLocalMove(endValue, duration);
    public void TransformDOJump(Transform t, Vector3 endValue, float jumpPower, int numJumps, float duration) 
        => t.DOJump(endValue, jumpPower, numJumps, duration);
    public void TransformDOShakePosition(Transform t, float duration, float strength) => t.DOShakePosition(duration, strength);

    // Light
    public void LightDOColor(Light light, Color to, float duration) => light.DOColor(to, duration);
    public void LightDOShadowStrength(Light light, float to, float duration)=> light.DOShadowStrength(to, duration);

    // Rigidbody
    public void RigidbodyDOMove(Rigidbody r, Vector3 to, float duration) => r.DOMove(to, duration);
    public void RigidbodyDOJump(Rigidbody r, Vector3 to, float power, int numJumps, float duration) => r.DOJump(to, power, numJumps, duration);
    public void RigidbodyDORotate(Rigidbody r, Vector3 to, float duration) => r.DORotate(to, duration);
    public void RigidbodyDOLookAt(Rigidbody r, Vector3 towards, float duration) => r.DOLookAt(towards, duration);
    public void RigidbodyDOPath(Rigidbody r, Vector3[] path, float duration) => r.DOPath(path, duration);

    public void Test(int test, string asdf) => Debug.Log("Test");
}
