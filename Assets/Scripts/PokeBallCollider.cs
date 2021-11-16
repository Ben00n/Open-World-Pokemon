using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeBallCollider : MonoBehaviour
{
    public GameObject impactParticles;
    public GameObject pokeBallParticles;
    public GameObject muzzleParticles;
    public GameObject ringParticles;

    Rigidbody rigidBody;
    PokemonPartyManager pokemonPartyManager;

    bool hasCollided = false;

    Vector3 impactNormal; // used to rotate the impact particles

    private void Awake()
    {
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        pokeBallParticles = Instantiate(pokeBallParticles, transform.position, transform.rotation);
        pokeBallParticles.transform.parent = transform;

        if(muzzleParticles)
        {
            muzzleParticles = Instantiate(muzzleParticles, transform.position, transform.rotation);
            Destroy(muzzleParticles, 2f); //how long particles muzzle last
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Pokemon")
        {
            if (!hasCollided)
            {
                hasCollided = true;
                PokemonStatsCalculator pokemonStatsCalculator = collision.transform.GetComponent<PokemonStatsCalculator>();
                PokemonManager pokemonManager = collision.transform.GetComponent<PokemonManager>();

                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                rigidBody.AddForce(Vector3.up * 200, ForceMode.Impulse); // jump pokeball on hit
                StartCoroutine(PokeBallChannel());

                pokemonManager.shrinking = true;

                StartCoroutine(HidePokemon(pokemonStatsCalculator.transform.gameObject));
                pokemonPartyManager.pokemons.Add(collision.gameObject);


                Destroy(pokeBallParticles);
                Destroy(impactParticles, 4f);
                //Destroy(gameObject, 0.01f);
            }
        }
        else if(collision.transform.tag == "Terrain")
        {
            if(!hasCollided)
            {
                hasCollided = true;
                Destroy(pokeBallParticles);
                Destroy(gameObject, 0.5f);
            }
        }
    }

    IEnumerator PokeBallChannel()
    {
        ringParticles = Instantiate(ringParticles, transform.position, new Quaternion(90, 0, 0, 90));
        yield return new WaitForSeconds(2);
        rigidBody.AddForce(Vector3.up * 100, ForceMode.Impulse);
        yield return new WaitForSeconds(2);
        rigidBody.AddForce(Vector3.up * 100, ForceMode.Impulse);
        yield return new WaitForSeconds(2);
        rigidBody.AddForce(Vector3.up * 100, ForceMode.Impulse);
        Destroy(ringParticles, 2f);
        Destroy(gameObject, 2f);
    }

    IEnumerator HidePokemon(GameObject pokemon)
    {
        yield return new WaitForSeconds(1f);
        foreach (var poke in pokemonPartyManager.pokemons)
        {
            poke.GetComponent<PokemonStatsCalculator>().isWild = false;
            poke.GetComponent<PokemonManager>().shrinking = false;

        }
        pokemon.tag = "PartyPokemon";
        pokemon.gameObject.SetActive(false);

    }
}
