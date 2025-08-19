using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 3.0f;      //�ړ��X�s�[�h
    int direction = 0;              //�ړ�����
    float axisH;                    //����
    float axisV;                    //�c��
    public float angleZ = -90.0f;   //��]����
    Rigidbody2D rbody;              //Rigidbody2D
    Animator animator;              //Animator
    bool isMoving = false;          //�ړ����t���O

    //�_���[�W�Ή�
    public static int hp = 3;       //�v���C���[��HP
    public static string gameState; //�Q�[���̏��
    bool inDamage = false;          //�_���[�W���t���O

    //p1����p2�̊p�x��Ԃ�
    float GetAngle(Vector2 p1, Vector2 p2)
    {
        float angle;
        if(axisH != 0 || axisV != 0)
        {
            //�ړ����Ȃ�p�x���X�V
            //p1����p2�ւ̍����i���_���O�ɂ��邽�߁j
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;

            //�A�[�N�^���W�F���g�Q�֐��Ŋp�x�i���W�A���j�����߂�
            float rad = Mathf.Atan2(dy, dx);

            //���W�A����x�ɕϊ����ĕԂ�
            angle = rad * Mathf.Rad2Deg;
        }
        else
        {
            //��~���Ȃ�ȑO�̊p�x���ێ�
            angle = angleZ;
        }
            return angle;
    }

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();    //Rigidbody2D���擾
        animator = GetComponent<Animator>();    //Animator���擾

        //�Q�[���̏�Ԃ��v���C���ɂ���
        gameState = "playing";
    }

    void Update()
    {
        //�Q�[�����ȊO�ƃ_���[�W���͉������Ȃ�
        if(gameState != "playing" || inDamage)
        {
            return;
        }

        if(isMoving == false)
        {
            axisH = Input.GetAxisRaw("Horizontal");     //���E�L�[����
            axisV = Input.GetAxisRaw("Vertical");     //�㉺�L�[����
        }

        //�L�[���͂̊p�x�����߂�
        Vector2 fromPt = transform.position;
        Vector2 toPt = new Vector2(fromPt.x + axisH, fromPt.y + axisV);
        angleZ = GetAngle(fromPt, toPt);

        //�ړ��p�x��������Ă�������ƃA�j���[�V�����X�V
        int dir;
        if(angleZ >= -45 && angleZ < 45)
        {
            //�E����
            dir = 3;
        }
        else if(angleZ >= 45 && angleZ <= 135)
        {
            //�����
            dir = 2;
        }
        else if (angleZ >= -135 && angleZ <= -45)
        {
            //������
            dir = 0;
        }
        else
        {
            //������
            dir = 1;
        }
        /*��̏�������𐮗�
        if(angleZ >= -45 && angleZ < 45)
        {
            //�E����
            dir = 3;
        }
        else if(angleZ >= 45 && angleZ <= 135)
        {
            //�����
            dir = 2;
        }
        else if(angleZ >= -135 && angleZ <= 135)
        {
            //������
            dir = 1;
        }
        else if(angleZ >= -135 && angleZ <= -45)
        {
            //������
            dir = 0;
        }
         */
        if (dir != direction)
        {
            direction = dir;
            animator.SetInteger("Direction", direction);
        }
    }

    void FixedUpdate()
    {
        //�Q�[�����ȊO�͉������Ȃ�
        if(gameState != "playing")
        {
            return;
        }
        if (inDamage)
        {
            //�_���[�W���_�ł�����
            float val = Mathf.Sin(Time.time * 50);
            if(val > 0)
            {
                //�X�v���C�g��\��
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                //�X�v���C�g���\��
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            return; //�_���[�W���͑���ɂ��ړ��������Ȃ�
        }

        //�ړ����x���X�V����
        rbody.linearVelocity = new Vector2(axisH, axisV).normalized * speed;
    }

    public void SetAxis(float h, float v)
    {
        axisH = h;
        axisV = v;
        if(axisV == 0 && axisV == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }

    //�ڐG
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            GetDamage(collision.gameObject);
        }
    }

    //�_���[�W
    void GetDamage(GameObject enemy)
    {
        if(gameState == "playing")
        {
            hp--;   //HP�����炷
            if(hp > 0)
            {
                //�ړ���~
                rbody.linearVelocity = new Vector2(0, 0);
                //�G�L�����̔��Ε����Ƀq�b�g�o�b�N������
                Vector3 v = (transform.position - enemy.transform.position).normalized;
                rbody.AddForce(new Vector2(v.x * 4, v.y * 4), ForceMode2D.Impulse);

                //�_���[�W�t���OON
                inDamage = true;
                Invoke("DamageEnd", 0.25f);
            }
            else
            {
                //�Q�[���I�[�o�[
                GameOver();
            }
        }
    }

    //�_���[�W�I��
    void DamageEnd()
    {
        inDamage = false;                                           //�_���[�W�t���OOFF
        gameObject.GetComponent<SpriteRenderer>().enabled = true;   //�X�v���C�g�����ɖ߂�
    }

    //�Q�[���I�[�o�[
    void GameOver()
    {
        gameState = "gameover";
        //�Q�[���I�[�o�[���o
        GetComponent<CircleCollider2D>().enabled = false;       //�v���C���[�����������
        rbody.linearVelocity = new Vector2(0, 0);               //�ړ��C��
        rbody.gravityScale = 1;                                 //�d�͂�߂�
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); //�v���C���[��������ɒ��ˏグ��
        animator.SetBool("IsDead", true);                       //�A�j���[�V������؂�ւ���
        Destroy(gameObject, 1.0f);                              //1�b��Ƀv���C���[������
    }
}
