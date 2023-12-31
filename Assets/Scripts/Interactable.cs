using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    GameObject mesh;

    [SerializeField]
    UnityEvent OnInteract;

    [SerializeField]
    float outlineThickness, flashDuration;

    [SerializeField]
    Color normal, flash;

    Material HighlightMat;
    // Start is called before the first frame update
    void Start()
    {
        HighlightMat = mesh.GetComponent<MeshRenderer>().material;
        HighlightMat.SetFloat("_Outline_Thickness", 0);
        HighlightMat.SetColor("_Outline_Color", normal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        HighlightMat.SetColor("_Outline_Color", flash);
        OnInteract.Invoke();
        StartCoroutine(flashColor());
    }

    public void EnterTrigger()
    {
        HighlightMat.SetFloat("_Outline_Thickness", outlineThickness);
    }

    public void ExitTrigger()
    {
        HighlightMat.SetFloat("_Outline_Thickness", 0);
    }
    
    IEnumerator flashColor()
    {
        yield return new WaitForSeconds(0.2f);
        HighlightMat.SetColor("_Outline_Color", normal);
    }
}
