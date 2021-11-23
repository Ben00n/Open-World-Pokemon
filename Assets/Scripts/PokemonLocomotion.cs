using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PokemonLocomotion : MonoBehaviour
{
    BattleManager battleManager;
    PokemonManager pokemonManager;
    PokemonAnimatorManager pokemonAnimatorManager;
    NavMeshAgent navmeshAgent;
    public Rigidbody pokemonRigidBody;
    PokemonStatsCalculator pokemonStatsCalculator;
    PokemonPartyManager pokemonPartyManager;

    public float radius;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
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
        if(pokemonManager.isFainted)
        {
            pokemonAnimatorManager.animator.SetFloat("Vertical", 0f, 0f, Time.deltaTime);
            navmeshAgent.isStopped = true;
        }
        else
        {
            if (!navmeshAgent.hasPath && navmeshAgent.isStopped == false && pokemonStatsCalculator.isWild && !pokemonManager.isFainted)
            {
                pokemonAnimatorManager.animator.SetFloat("Vertical", 0.5f, 0f, Time.deltaTime);

                if (pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Grass)
                {
                    navmeshAgent.SetDestination(GrassArea.Grass.GetRandomPoint());
                }
                else if (pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Fire)
                {
                    navmeshAgent.SetDestination(FireArea.Fire.GetRandomPoint());
                }
                else if (pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Water)
                {
                    navmeshAgent.SetDestination(WaterArea.Water.GetRandomPoint());
                }
                else if(pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Normal)
                {
                    navmeshAgent.SetDestination(NormalArea.Normal.GetRandomPoint());
                }
                else if (pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Bug)
                {
                    navmeshAgent.SetDestination(BugArea.Bug.GetRandomPoint());
                }
                else if(pokemonStatsCalculator.pokemonBase.GetType1 == PokemonType.Electric)
                {
                    navmeshAgent.SetDestination(ElectricArea.Electric.GetRandomPoint());
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
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        navmeshAgent.isStopped = false;
    }

    public void CheckIfIsInBattleAndMoveToBattlePoint()
    {
        if (pokemonManager.isInBattle)
        {
            var healthyPokemonInParty = battleManager.playerPokemonManager.gameObject;
            navmeshAgent.SetDestination(healthyPokemonInParty.transform.position + (Vector3.forward * 3));
            if (navmeshAgent.remainingDistance <= 0.1)
            {
                transform.LookAt(healthyPokemonInParty.transform);
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
