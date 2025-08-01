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
            DontDestroyOnLoad(gameObject); // �V�[�����܂����ł��ێ�����ꍇ
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Bomb�̉���
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
