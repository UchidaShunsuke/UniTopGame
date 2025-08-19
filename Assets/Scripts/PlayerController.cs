using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 3.0f;      //移動スピード
    int direction = 0;              //移動方向
    float axisH;                    //横軸
    float axisV;                    //縦軸
    public float angleZ = -90.0f;   //回転方向
    Rigidbody2D rbody;              //Rigidbody2D
    Animator animator;              //Animator
    bool isMoving = false;          //移動中フラグ

    //ダメージ対応
    public static int hp = 3;       //プレイヤーのHP
    public static string gameState; //ゲームの状態
    bool inDamage = false;          //ダメージ中フラグ

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

        //ゲームの状態をプレイ中にする
        gameState = "playing";
    }

    void Update()
    {
        //ゲーム中以外とダメージ中は何もしない
        if(gameState != "playing" || inDamage)
        {
            return;
        }

        if(isMoving == false)
        {
            axisH = Input.GetAxisRaw("Horizontal");     //左右キー入力
            axisV = Input.GetAxisRaw("Vertical");     //上下キー入力
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
        //ゲーム中以外は何もしない
        if(gameState != "playing")
        {
            return;
        }
        if (inDamage)
        {
            //ダメージ中点滅させる
            float val = Mathf.Sin(Time.time * 50);
            if(val > 0)
            {
                //スプライトを表示
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                //スプライトを非表示
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
            return; //ダメージ中は操作による移動をさせない
        }

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

    //接触
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            GetDamage(collision.gameObject);
        }
    }

    //ダメージ
    void GetDamage(GameObject enemy)
    {
        if(gameState == "playing")
        {
            hp--;   //HPを減らす
            if(hp > 0)
            {
                //移動停止
                rbody.linearVelocity = new Vector2(0, 0);
                //敵キャラの反対方向にヒットバックさせる
                Vector3 v = (transform.position - enemy.transform.position).normalized;
                rbody.AddForce(new Vector2(v.x * 4, v.y * 4), ForceMode2D.Impulse);

                //ダメージフラグON
                inDamage = true;
                Invoke("DamageEnd", 0.25f);
            }
            else
            {
                //ゲームオーバー
                GameOver();
            }
        }
    }

    //ダメージ終了
    void DamageEnd()
    {
        inDamage = false;                                           //ダメージフラグOFF
        gameObject.GetComponent<SpriteRenderer>().enabled = true;   //スプライトを元に戻す
    }

    //ゲームオーバー
    void GameOver()
    {
        gameState = "gameover";
        //ゲームオーバー演出
        GetComponent<CircleCollider2D>().enabled = false;       //プレイヤー当たりを消す
        rbody.linearVelocity = new Vector2(0, 0);               //移動修正
        rbody.gravityScale = 1;                                 //重力を戻す
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); //プレイヤーを少し上に跳ね上げる
        animator.SetBool("IsDead", true);                       //アニメーションを切り替える
        Destroy(gameObject, 1.0f);                              //1秒後にプレイヤーを消す
    }
}
