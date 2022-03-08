using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PCPokemonButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IDropHandler
{
    private PokemonPartyManager pokemonPartyManager;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 startLocalPosition;
    public int index;
    PCPokemonButton pcPokemonButton;

    private PCSystem pcSystem = null;
    private PokemonStatsCalculator pokemon = null;

    [SerializeField] private TextMeshProUGUI pokemonNameText = null;
    [SerializeField] private Image pokemonIconImage = null;
    [SerializeField] private TextMeshProUGUI pokemonLevel = null;

    private void Awake()
    {
        pcPokemonButton = GetComponent<PCPokemonButton>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startLocalPosition = rectTransform.localPosition;
    }

    public void Initialise(PCSystem pcSystem, PokemonStatsCalculator pokemon,int index)
    {
        this.pcSystem = pcSystem;
        this.pokemon = pokemon;
        this.index = index;
        pokemonNameText.text = pokemon.pokemonBase.Name;
        pokemonIconImage.sprite = pokemon.pokemonBase.GetSprite;
        pokemonLevel.text = "Lvl " + pokemon.Level.ToString();
    }

    public int GetPokemonIndex()
    {
        return index;
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
        rectTransform.anchoredPosition = startLocalPosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            PCPokemonButton currentDrag = eventData.pointerDrag.GetComponent<PCPokemonButton>();
            if (currentDrag != null)
            {
                pokemonPartyManager.SwapPcPokemons(index, currentDrag.GetPokemonIndex());
                pcSystem.SetPcData(pcSystem.pcData);
            }
        }
    }
}
