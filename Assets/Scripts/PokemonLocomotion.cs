using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PokemonLocomotion : MonoBehaviour
{
    PokemonManager pokemonManager;
    PokemonAnimatorManager pokemonAnimatorManager;
    NavMeshAgent navmeshAgent;
    public Rigidbody pokemonRigidBody;
    PokemonStatsCalculator pokemonStatsCalculator;
    PokemonPartyManager pokemonPartyManager;

    public float radius;



    private void Awake()
    {
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        pokemonManager = GetComponent<PokemonManager>();
        pokemonAnimatorManager = GetComponentInChildren<PokemonAnimatorManager>();
        navmeshAgent = GetComponent<NavMeshAgent>();
        pokemonRigidBody = GetComponent<Rigidbody>();
        pokemonStatsCalculator = GetComponent<PokemonStatsCalculator>();
    }

    private void Start()
    {
        //navmeshAgent.enabled = false;
        pokemonRigidBody.isKinematic = false;
    }


    public void HandleRandomMovement()
    {
        if(!navmeshAgent.hasPath && navmeshAgent.isStopped == false && pokemonStatsCalculator.isWild)
        {
            pokemonAnimatorManager.animator.SetFloat("Vertical", 0.5f, 0f, Time.deltaTime);

            if (pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Grass || pokemonStatsCalculator.pokemonBase.GetType2 == PokemonType.Grass)
            {
                navmeshAgent.SetDestination(GrassArea.Grass.GetRandomPoint());
            }
            else if(pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Fire || pokemonStatsCalculator.pokemonBase.GetType2 == PokemonType.Fire)
            {
                navmeshAgent.SetDestination(FireArea.Fire.GetRandomPoint());
            }
            else if (pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Water || pokemonStatsCalculator.pokemonBase.GetType2 == PokemonType.Water)
            {
                navmeshAgent.SetDestination(WaterArea.Water.GetRandomPoint());
            }
        }

        if (navmeshAgent.remainingDistance <= 0.1)
        {
            navmeshAgent.isStopped = true;
            pokemonAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            StartCoroutine(Wait());
        }

        CheckIfIsInBattleAndMoveToBattlePoint();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        navmeshAgent.isStopped = false;
    }

    private void CheckIfIsInBattleAndMoveToBattlePoint()
    {
        if (pokemonManager.isInBattle)
        {
            navmeshAgent.SetDestination(pokemonPartyManager.pokemons[0].transform.position + (Vector3.forward * 3));
            if (navmeshAgent.remainingDistance <= 0.1)
            {
                transform.LookAt(pokemonPartyManager.pokemons[0].transform);
                navmeshAgent.isStopped = true; //stop the navmesh from moving
                pokemonAnimatorManager.animator.SetFloat("Vertical", 0, 0f, Time.deltaTime); // go to idle animation
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
