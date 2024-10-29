using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : Agent
{
    // 他のエージェントから距離をとるメソッド．
    private Vector3 Separation()
    {
        Vector3 vector = new(0, 0, 0);
        int innerAgentNum = 0;

        foreach (var agent in outerList)
        {
            var distance = (transform.position - agent.transform.position).magnitude;

            if (distance <= innerRadius)
            {
                vector += (transform.position - agent.transform.position).normalized * innerRadius / distance;
                innerAgentNum++;
            }
        }

        if (innerAgentNum > 0) vector /= innerAgentNum;
        vector += Avoid();

        return vector;
    }


    // 視界内のエージェントと向きを合わせるメソッド．
    private Vector3 Align()
    {
        Vector3 vector = new();

        foreach (var agent in outerList)
        {
            vector += agent.GetVelocity;
        }

        // カメラ外に移動しないように上下方向を向かないようにする
        vector.y = 0;

        return vector.normalized;
    }

    // 視界内のエージェントの中心座標を目指すメソッド．
    private Vector3 Cohesion()
    {
        Vector3 totalPosition = new(0, 0, 0);

        foreach (var agent in outerList)
        {
            var targetPosition = agent.gameObject.transform.position;

            if (Vector3.Distance(transform.position, targetPosition) > innerRadius)
            {
                // 基準の範囲内の場合ターゲットとする
                totalPosition += targetPosition - transform.position;
            }
        }

        return totalPosition.normalized;
    }

    // エージェント以外のオブジェクトから距離をとるメソッド．
    private Vector3 Avoid()
    {
        Vector3 vector = new(0, 0, 0);
        Collider myCollider = GetComponent<Collider>();
        int innerAgentNum = 0;

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
                    innerAgentNum++;
                }
            }
        }

        if (innerAgentNum > 0) vector /= innerAgentNum;

        return vector;
    }

    public override Vector3 ExecutedMission()
    {
        return ExecuteForwardMission();
    }

    // 前方へ進むミッションメソッド．
    public Vector3 ExecuteForwardMission()
    {
        Vector3 vector = transform.forward;

        if (outerList.Count > 0 || objectList.Count > 0)
        {
            vector = Separation() * separatePower + Align() * alignPower + Cohesion() * cohesionPower;
            vector += transform.forward * vector.magnitude * destinationPower;
        }

        return vector.normalized;
    }

    // 目的地へ進むミッションメソッド．
    public Vector3 ExecuteTargetMission()
    {
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 vector = direction;

        // 目的地周辺にいる場合は結合と分離のみ
        if (Vector3.Distance(transform.position, destination) < 10)
        {
            vector = Separation() * separatePower + Cohesion() * cohesionPower;
        }
        else if (outerList.Count > 0 || objectList.Count > 0)
        {
            vector = Separation() * separatePower + Align() * alignPower + Cohesion() * cohesionPower;
            vector += direction * vector.magnitude * destinationPower;
        }

        return vector.normalized;
    }

}
