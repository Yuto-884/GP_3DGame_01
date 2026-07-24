using UnityEngine;

public class AttackObject : MonoBehaviour
{
    [SerializeField] public int power = 1;

    [SerializeField] int hp = 2;

    // Ç±ÇÃíeÇåÇÇ¡ÇΩêl
    public GameObject owner;

    private void OnCollisionStay(Collision collision)
    {
        var attackObj = collision.gameObject.GetComponent<AttackObject>();
        if (attackObj != null)
        {
            hp -= attackObj.power;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}