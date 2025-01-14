using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] GameObject CrystalObject;
    [SerializeField] LayerMask crystal;
    [SerializeField] GameObject orientation;


    void Update()
    {
        if (CrystalObject != null)
        { 
            if (Physics.Raycast(orientation.transform.position, orientation.transform.forward, 10, crystal) && Input.GetKeyDown(KeyCode.E))
            {
                CrystalObject.GetComponent<MeshCollider>().enabled = false;
                StartCoroutine(MyCoroutine());
               
            }
        }
    }

   
    IEnumerator MyCoroutine()
    {
        while (CrystalObject != null && Vector3.Distance(CrystalObject.transform.position, transform.position) > 0.2f)
        {
            CrystalObject.transform.position = Vector3.Lerp(CrystalObject.transform.position, transform.position, Time.deltaTime * 3);
            if (Vector3.Distance(CrystalObject.transform.position, transform.position) < 0.2f)
            {
                
                 Destroy(CrystalObject);
            }
            yield return null;
        }
        
    }
}
