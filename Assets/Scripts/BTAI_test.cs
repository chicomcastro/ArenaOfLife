using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

public class BTAI_test : MonoBehaviour
{
    public bool testBool;
    Root aiRoot = BT.Root();
    float speed;

    void OnEnable()
    {
        aiRoot.OpenBranch(
            BT.While(TestVisibleTarget).OpenBranch(
                BT.Call(Aim),
                BT.Call(Shoot)
            ),
            BT.Sequence().OpenBranch(
                    BT.Call(Walk),
                    BT.Wait(5.0f),
                    BT.Call(Turn),
                    BT.Wait(1.0f)
            )
        );
    }

    // Update is called once per frame
    void Update()
    {
        aiRoot.Tick();
        transform.position += (Vector3)new Vector2(speed * Time.deltaTime, 0)*Mathf.Sign(transform.localScale.x);
    }

    private bool TestVisibleTarget()
    {
        return testBool;
    }

    private void Aim()
    {
        print("Aimming");
    }

    private void Shoot()
    {
        print("Shooting");
    }

    private void Walk()
    {
        speed = 10f;
    }

    private void Turn()
    {
        speed = 0;
        Vector3 scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }
}
