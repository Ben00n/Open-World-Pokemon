using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,IDropHandler
{
    PokemonPartyManager pokemonPartyManager;
    PartyMemberUI partyMember;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startLocalPosition;

    public int pokemonIndex;

    private void Awake()
    {
        partyMember = GetComponent<PartyMemberUI>();
        pokemonPartyManager = FindObjectOfType<PokemonPartyManager>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startLocalPosition = rectTransform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        rectTransform.localPosition = startLocalPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            PartyMemberUI partyMemberUI = eventData.pointerDrag.GetComponent<PartyMemberUI>();
            if(partyMemberUI != null)
            {
                pokemonIndex = partyMemberUI.index;
                pokemonPartyManager.SwapPokemon(pokemonIndex, partyMember.GetPokemonIndex());
            }
        }

    }
}
