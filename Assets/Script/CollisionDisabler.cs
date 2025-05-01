using UnityEngine;

public class CollisionDisabler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

	//�Փ˂��邩�ǂ����A�ΏۃI�u�W�F�N�g1�A�ΏۃI�u�W�F�N�g2���w��
	public static void SetActiveCollision(bool isCollide, GameObject targetObj1, GameObject targetObj2)
	{
		var colliders1 = targetObj1.GetComponentsInChildren<Collider>();
		var colliders2 = targetObj2.GetComponentsInChildren<Collider>();

		foreach (var col1 in colliders1)
		{
			foreach (var col2 in colliders2)
			{
				Physics.IgnoreCollision(col1, col2, !isCollide);
			}
		}
	}
}
