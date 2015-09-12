using UnityEngine;
using System.Collections;

public class TestNumAi : MonoBehaviour
{
    public GameObject Target;
    public int MaxNum;
	// Use this for initialization
	void Start ()
	{
	    StartCoroutine(GenerateAi());
	}

    private IEnumerator GenerateAi()
    {
        Target.SetActive(false);
        for (int i = 0; i < MaxNum; i++)
        {
            GameObject go = Instantiate(Target);
            go.name = i.ToString();
            go.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
