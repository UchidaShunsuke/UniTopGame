using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public float speed = 3.0f;      //移動スピード
    int direction = 0;              //移動方向
    float axisH;                    //横軸
    float axisV;                    //縦軸
    public float angleZ = -90.0f;   //回転方向
    Rigidbody2D rbody;              //Rigidbody2D
    Animator animator;              //Animator
    bool isMoving = false;          //移動中フラグ

    //p1からp2の角度を返す
    float GetAngle(Vector2 p1, Vector2 p2)
    {
        float angle;
        if(axisH != 0 || axisV != 0)
        {
            //移動中なら角度を更新
            //p1からp2への差分（原点を０にするため）
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;

            //アークタンジェント２関数で角度（ラジアン）を求める
            float rad = Mathf.Atan2(dy, dx);

            //ラジアンを度に変換して返す
            angle = rad * Mathf.Rad2Deg;
        }
        else
        {
            //停止中なら以前の角度を維持
            angle = angleZ;
        }
            return angle;
    }

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();    //Rigidbody2Dを取得
        animator = GetComponent<Animator>();    //Animatorを取得
    }

    void Update()
    {
        if(isMoving == false)
        {
            axisH = Input.GetAxisRaw("Horizontal");     //左右キー入力
            axisV = Input.GetAxisRaw("Vartical");     //上下キー入力
        }

        //キー入力の角度を求める
        Vector2 fromPt = transform.position;
        Vector2 toPt = new Vector2(fromPt.x + axisH, fromPt.y + axisV);
        angleZ = GetAngle(fromPt, toPt);

        //移動角度から向いている方向とアニメーション更新
        int dir;
        if(angleZ >= -45 && angleZ < 45)
        {
            //右向き
            dir = 3;
        }
        else if(angleZ >= 45 && angleZ <= 135)
        {
            //上向き
            dir = 2;
        }
        else if (angleZ >= -135 && angleZ <= -45)
        {
            //下向き
            dir = 0;
        }
        else
        {
            //左向き
            dir = 1;
        }
        /*上の条件分岐を整理
        if(angleZ >= -45 && angleZ < 45)
        {
            //右向き
            dir = 3;
        }
        else if(angleZ >= 45 && angleZ <= 135)
        {
            //上向き
            dir = 2;
        }
        else if(angleZ >= -135 && angleZ <= 135)
        {
            //左向き
            dir = 1;
        }
        else if(angleZ >= -135 && angleZ <= -45)
        {
            //下向き
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
        //移動速度を更新する
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
}
