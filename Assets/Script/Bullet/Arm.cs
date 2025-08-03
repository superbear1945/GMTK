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
        //使手部重新可见
        if (Player.Instance.armCount == 1) //捡起左手
        {
            Player.Instance.leftArm.SetActive(true); // 显示捡起的左手
        }
        else if (Player.Instance.armCount == 0) //捡起右手
        {
            Player.Instance.rightArm.SetActive(true); // 显示捡起的右手
        }

        Player.Instance.AddArm();
        UIManager.Instance.UpdateCountUI(); // 更新UI显示
        
    }
}
