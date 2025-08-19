using UnityEngine;

public class CamerManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            //プレイヤーの位置と連動させる
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }
    }
}
