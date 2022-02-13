using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PCPokemonButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private PokemonPartyManager pokemonPartyManager;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 startLocalPosition;

    [SerializeField] private TextMeshProUGUI pokemonNameText = null;
    [SerializeField] private Image pokemonIconImage = null;
    [SerializeField] private TextMeshProUGUI pokemonLevel = null;

    private void Awake()
    {
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        Transform testCanvasTransform = transform;
        do
        {
            testCanvasTransform = testCanvasTransform.parent;
            canvas = testCanvasTransform.GetComponent<Canvas>();
        } while (canvas == null);
    }

    private void Start()
    {
        startLocalPosition = rectTransform.localPosition;
    }

    private PCSystem pcSystem = null;
    private PokemonStatsCalculator pokemon = null;

    public void Initialise(PCSystem pcSystem, PokemonStatsCalculator pokemon)
    {
        this.pcSystem = pcSystem;
        this.pokemon = pokemon;
        pokemonNameText.text = pokemon.pokemonBase.Name;
        pokemonIconImage.sprite = pokemon.pokemonBase.GetSprite;
        pokemonLevel.text = "Lvl " + pokemon.Level.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.localPosition = startLocalPosition;
    }
}
