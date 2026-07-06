using UnityEngine;
using UnityEngine.UI;

public class Bot : MonoBehaviour
{
    public Image TowerPic;

    private int towerId;

    public int TowerId => towerId;

    public void Init(TowerInfo info)
    {
        if (info == null)
        {
            return;
        }

        towerId = info.id;

        if (TowerPic == null)
        {
            return;
        }

        Sprite sprite = Resources.Load<Sprite>("TowerPic/" + info.id);
        if (sprite != null)
        {
            TowerPic.sprite = sprite;
        }
    }
}
