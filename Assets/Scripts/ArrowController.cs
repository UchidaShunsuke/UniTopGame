using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float deleteTime = 2;    //�폜����

    void Start()
    {
        Destroy(gameObject, deleteTime);    //��莞�Ԃŏ���
    }

    void Update()
    {
        
    }

    //�Q�[���I�u�W�F�N�g�ɐڐG
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.SetParent(collision.transform);           //�ڐG�����Q�[���I�u�W�F�N�g�̎q�ɂ���
        GetComponent<CircleCollider2D>().enabled = false;   //������𖳌�������
        GetComponent<Rigidbody2D>().simulated = false;      //�����V���~���[�V�����𖳌��ɂ���
    }
}
