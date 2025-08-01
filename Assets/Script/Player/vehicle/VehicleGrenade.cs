using UnityEngine;

public class VehicleGrenade : MonoBehaviour
{
    public static VehicleGrenade Instance;

    public int maxBomb = 10;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Bomb‚Ì‰ÁŒ¸
    public void UseBomb()
    {
        if (maxBomb > 0)
        {
            maxBomb--;
        }
    }

    public void AddBomb(int amount)
    {
        maxBomb += amount;
    }

    public int GetCurrentBombCount()
    {
        return maxBomb;
    }
}
