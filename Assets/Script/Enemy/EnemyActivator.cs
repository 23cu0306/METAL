using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Renderer rend;
    private bool hasActivated = false;

    public Behaviour[] EnemyComp;

    void Start()
    {
        
        
        rend = GetComponent<Renderer>();
        //GetComponent<GloomVisBoss>().enabled = false;

        foreach (var comp in EnemyComp)
        {
            if( comp != null)
            {
                comp.enabled = false;
            }

        }

        Debug.Log("意気消沈沈");

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasActivated && rend.isVisible)
        {

            hasActivated = true;

            foreach (var comp in EnemyComp)
            { 
                if (comp != null)
                {
                    comp.enabled = true;
                }
            }
            Debug.Log("パターン赤！起動します！");

        }
        
    }
}
