using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Follower : Agent
{
    private GameObject leader;
    public void SetLeader(GameObject leader)
    {
        this.leader = leader;
    }
    public override Vector3 ExecutedMission()
    {
        var targetPosition = leader.transform.position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 vector = direction;

        vector = Separation() * separatePower + Align() * alignPower + Cohesion() * cohesionPower;

        return vector.normalized;
    }

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

        if (innerAgentNum > 0) {
            vector /= innerAgentNum;
        }
        vector += Avoid();

        return vector;
    }

    private Vector3 Align()
    {
        Vector3 vector = new();
        Rigidbody leaderRB = leader.GetComponent<Rigidbody>();
        vector += leaderRB.velocity;
        return vector.normalized;
    }

    // リーダーの中心座標を目指すメソッド．
    private Vector3 Cohesion()
    {
        Vector3 vector = new(0, 0, 0);
        var targetPosition = leader.gameObject.transform.position;

        if (Vector3.Distance(transform.position, targetPosition) > innerRadius)
        {
            vector += targetPosition - transform.position;
        }

        return vector.normalized;
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

        if (innerAgentNum > 0)
        {
            vector /= innerAgentNum;
        }

        return vector;
    }

}
