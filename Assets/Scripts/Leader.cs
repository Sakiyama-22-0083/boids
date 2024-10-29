using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Leader : Agent
{
    public override Vector3 ExecutedMission()
    {
        // return ExecuteForwardMission();
        return ExecuteTargetMission();
    }


    // エージェント以外のオブジェクトから距離をとるメソッド．
    private Vector3 Avoid()
    {
        Vector3 vector = new(0, 0, 0);
        Collider myCollider = GetComponent<Collider>();
        int innerObjectNum = 0;

        foreach (var obstacle in objectList)
        {
            Collider targetCollider = obstacle.GetComponent<Collider>();

            if (myCollider != null && targetCollider != null)
            {
                // 自身の位置に最も近いターゲットのコライダーの表面上の点を取得
                Vector3 closestPointOnTarget = targetCollider.ClosestPoint(transform.position);
                var distance = Vector3.Distance(transform.position, closestPointOnTarget);

                if (distance <= innerRadius)
                {
                    vector += (transform.position - closestPointOnTarget).normalized * innerRadius / distance;
                    innerObjectNum++;
                }
            }
        }

        if (innerObjectNum > 0)
        {
            vector /= innerObjectNum;
        }

        return vector;
    }

    // 前方へ進むミッションメソッド．
    public Vector3 ExecuteForwardMission()
    {
        Vector3 vector = transform.forward;
        vector.y = 0;
        vector = vector.normalized * alignPower;
        vector += Avoid() * separatePower;
        return vector.normalized;
    }

    // 目的地へ進むミッションメソッド．
    public Vector3 ExecuteTargetMission()
    {
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 vector = direction * alignPower + Avoid() * separatePower;

        if (Vector3.Distance(transform.position, destination) < 10)
        {
            vector = new Vector3(0, 0, 0);
        }

        return vector.normalized;
    }

}
