using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : BulletBase
{
    public override void OnHit(Collider2D collision)
    {
        _isLand = true; // 设置为已落地状态
        _rb2d.velocity = Vector2.zero; // 停止移动            
    }

    public override void OnPickUp()
    {
        GameManager.CurrentPlayer.GetComponent<Shoot>().AddArm();
    }
}
