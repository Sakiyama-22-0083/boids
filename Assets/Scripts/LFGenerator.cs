using UnityEngine;

public class LFGenerator : MonoBehaviour
{
    public GameObject leader;
    public GameObject follower;
    public int followerNum = 5;
    public Vector3 range = new Vector3(10, 10, 10);// ランダムな座標範囲

    void Start()
    {
        SpawnLeader(leader);
        SpawnFollower(follower, followerNum);
    }

    // leaderを生成するメソッド
    void SpawnLeader(GameObject leader)
    {
        // ランダムな座標を生成
        Vector3 randomPosition = new Vector3(
            Random.Range(-range.x, range.x),
            Random.Range(1, range.y),
            Random.Range(-range.z, range.z)
        );

        // オブジェクトを生成
        this.leader = Instantiate(leader, randomPosition, Quaternion.identity);
    }

    // followerを生成するメソッド
    void SpawnFollower(GameObject follower, int followerNum)
    {
        for (int i = 0; i < followerNum; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-range.x, range.x),
                Random.Range(1, range.y),
                Random.Range(-range.z, range.z)
            );

            GameObject followerInstance = Instantiate(follower, randomPosition, Quaternion.identity);
            Follower followerScript = followerInstance.GetComponent<Follower>();
            followerScript.SetLeader(this.leader);
        }
    }
}
