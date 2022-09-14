using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BuffDebuffManager : MonoBehaviour
{
    [SerializeField] private List<BuffSO> buffsList;
    [SerializeField] private float abilitySpawnDuration = 5f;
    [SerializeField] private float abilityMinSpawnRate = 60f;
    [SerializeField] private float abilityMaxSpawnRate = 75f;
    [SerializeField] private AudioClip abilityActivateSound;
    private float randomTime;

    private void Start() 
    {
        Orb.OnRandomAbilityActivate += Orb_OnRandomAbilityActivate;

        randomTime = Random.Range(abilityMinSpawnRate, abilityMaxSpawnRate);
    }

    private void Update() 
    {
        randomTime -= Time.deltaTime;

        if (randomTime <= 0f)
        {
            TriggerRandomAbilitySpawn();
            randomTime = Random.Range(abilityMinSpawnRate, abilityMaxSpawnRate);
        }
    }

    private void Orb_OnRandomAbilityActivate(object sender, System.EventArgs e)
    {
        ActivateRandomBuff();
    }

    [Button("Activate Random Orb")]
    private void TriggerRandomAbilitySpawn()
    {
        List<GridObject> goWithOrbs = LevelGrid.Instance.GetAllGridObjectWithOrbs();

        int randomIndex = Random.Range(0, goWithOrbs.Count - 1);
        Orb randomOrb = goWithOrbs[randomIndex].GetOrb();
        randomOrb.EnableIsAbilityActivated(abilitySpawnDuration);
    }

    private void ActivateRandomBuff()
    {
        int randomIndex = Random.Range(0, buffsList.Count - 1);
        buffsList[randomIndex].Execute();

        AudioManager.Instance.PlaySFX(abilityActivateSound);
        //instantiate UI logo
    }

}
