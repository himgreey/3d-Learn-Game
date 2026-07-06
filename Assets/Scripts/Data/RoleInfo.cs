using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleInfo
{
    public int id;
    public string res;
    public int defaultWeapon;
    public int lockMoney;
    public int atk;
    public int hp;
    public string tips;
    /// <summary>Resources 路径，如 UI/RoleHead/1。留空则按 id 自动拼路径。</summary>
    public string headIcon;
}
