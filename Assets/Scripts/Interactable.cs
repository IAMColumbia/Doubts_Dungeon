using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    GameObject mesh;

    [SerializeField]
    UnityEvent OnInteract;

    [SerializeField]
    float outlineThickness, flashDuration;

    [SerializeField]
    Color normal;

    [SerializeField]
    Color flash;

    public List<Item> AllItemsInGame = new List<Item>();

    InventoryManager IM = new InventoryManager();
    

    public bool isVendor;

    public bool VendorSetup;

    [SerializeField]
    public GameObject Vendor;

   
    Material HighlightMat;
    // Start is called before the first frame update
    void Start()
    {
        HighlightMat = mesh.GetComponent<MeshRenderer>().material;
        HighlightMat.SetFloat("_Outline_Thickness", 0);
        
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
        if (isVendor == true)
        {
            Vendor.SetActive(false);
        }
      
    }

    
    
    IEnumerator flashColor()
    {
        yield return new WaitForSeconds(0.2f);
        HighlightMat.SetColor("_Outline_Color", normal);
    }



    public void ChestInteraction()
    {
        int randomIndex = Random.Range(0, AllItemsInGame.Count);
        Debug.Log("Chest Interacting");

        IM.AddItem(AllItemsInGame[randomIndex]);
    }
}
