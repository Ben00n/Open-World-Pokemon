using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonUIManager : MonoBehaviour
{
    PokemonStatsCalculator pokemonStatsCalculator;
    PlayerManager playerManager;
    GameObject projected;

    private float distanceFromPlayer;
    public Vector3 Offset = new Vector3(0, 1, 0);
    public GameObject levelText;

    private void Awake()
    {
        pokemonStatsCalculator = GetComponent<PokemonStatsCalculator>();
        playerManager = FindObjectOfType<PlayerManager>();
    }
    private void Start()
    {
        projected = Instantiate(levelText, transform.position, Quaternion.identity, transform);
        projected.transform.localPosition += Offset;
    }


    private void Update()
    {
        CheckDistanceFromPlayer();
        projected.GetComponent<TextMesh>().text = pokemonStatsCalculator.Level.ToString();
    }

    private void CheckDistanceFromPlayer()
    {
        distanceFromPlayer = Vector3.Distance(playerManager.transform.position, transform.position);
        if(distanceFromPlayer >= 15)
        {
            projected.SetActive(false);
        }
        else
        {
            projected.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        projected.transform.LookAt(Camera.main.transform);
        projected.transform.Rotate(0, 180, 0);
    }
}
