using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public abstract class Agent : MonoBehaviour
{
    protected float moveSpeed = 3.1f;
    protected readonly float innerRadius = 6.6f;// 分離領域半径
    protected readonly float separatePower = 1.2f;// 分離力
    protected readonly float alignPower = 3.3f;// 整列力
    protected readonly float cohesionPower = 0.6f;// 結合力
    protected readonly float destinationPower = 0.2f;// 目的地への重視度
    protected float rotationSpeed = 100.0f; // 回転速度の制限値

    protected Rigidbody rb;
    protected List<Agent> outerList = new();// 認識したエージェントのリスト
    protected List<GameObject> objectList = new();// 認識したエージェント以外のオブジェクトxのリスト
    protected Vector3 moveVector;
    public Vector3 GetVelocity { get { return rb.velocity; } }// 現在の速度ベクトル
    protected Vector3 destination = new(200, 20, 200);
    protected Renderer objectRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // ランダムな方向を向く
        this.transform.LookAt(Random.onUnitSphere, Vector3.up);

        objectRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        moveVector = ExecutedMission();

        // 最大速度を制限
        rb.AddForce(moveVector * moveSpeed - rb.velocity);

        // ターゲット方向に向かうための回転を計算
        Quaternion targetRotation = Quaternion.LookRotation(moveVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    // センサー範囲内にColliderオブジェクトが入った時の処理メソッド．
    private void OnTriggerEnter(Collider other)
    {
        // エージェントとそれ以外のオブジェクトのリストをそれぞれ作成する．
        if (other.gameObject.TryGetComponent<Agent>(out var agent) && !outerList.Contains(agent))
        {
            outerList.Add(agent);
        }
        else if (!outerList.Contains(agent))
        {
            objectList.Add(other.gameObject);
        }
    }

    // センサー範囲内からColliderオブジェクトが出た時の処理メソッド．
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Agent>(out var agent) && outerList.Contains(agent))
        {
            outerList.Remove(agent);
        }
    }

    // 他のオブジェクトと衝突時の処理メソッド．
    private void OnCollisionEnter(Collision collision)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.blue;
        }
    }

    public abstract Vector3 ExecutedMission();

}
