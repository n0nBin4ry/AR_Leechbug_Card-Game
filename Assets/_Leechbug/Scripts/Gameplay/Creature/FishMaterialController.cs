using System.Collections;
//using FMODUnity;
using UnityEngine;

public class FishMaterialController
{
    private Renderer _renderer;
    //Basic Material References
    private Material normalMat;
    private Health healthComp;
    private Fish _fish;

    //Enum for basic materials
    public enum BasicMaterial
    {
        Normal,
        Damaged,
        Defeated
    };

    public void Initialize(Fish fish)
    {
        _fish = fish;
        _renderer = fish.GetComponentInChildren<SkinnedMeshRenderer>();
        if (!_renderer)
            _renderer = fish.GetComponentInChildren<MeshRenderer>();
        normalMat = _renderer.material; // set default material at start
        
        healthComp = fish.Health;
        healthComp.OnModHealth += ModHealth;
    }
    
    public void ChangeTexture(Material newMat)
    {
        _renderer.material = newMat;
    }

    void ModHealth(int amount)
    {
        if (healthComp.IsZeroHealth) 
            OnEmptyHealth();
        else if (healthComp.IsFullHealth)
            OnHealthFullyRestored();
        if (amount < 0 && !healthComp.IsZeroHealth)
        {
            if(_fish)
            {
                _fish.StartCoroutine(ColorChange());
            }

        }
    }

    void OnEmptyHealth()
    {
        ChangeTexture(BasicMaterial.Defeated);
    }

    void OnHealthFullyRestored()
    {
        ChangeTexture(BasicMaterial.Normal);
    }
    
    public void ChangeTexture(BasicMaterial basicMaterial)
    {
        if(basicMaterial == BasicMaterial.Normal)
        {
            _renderer.material.color = Color.white;
        } else if(basicMaterial == BasicMaterial.Damaged)
        {
            _renderer.material.color = Color.red;
        } else if(basicMaterial == BasicMaterial.Defeated)
        {
            _renderer.material.color = Color.blue;
        }
    }

    IEnumerator ColorChange()
    {
        ChangeTexture(BasicMaterial.Damaged);
        yield return new WaitForSeconds(.5f);
        if (!healthComp.IsZeroHealth)
            ChangeTexture(BasicMaterial.Normal);
    }
}
