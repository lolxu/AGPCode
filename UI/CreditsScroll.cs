using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class CreditsScroll : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;

    [SerializeField] private RectTransform contentsPosition;

    [SerializeField] private float initialY, finalY, duration;

    private Tween scroll;
    public void ScrollThruCredits()
    {
        if (scroll != null) { scroll.Kill(false); }
        contentsPosition.gameObject.transform.localPosition = new Vector2(0, initialY);
        Debug.Log("Scrolling");
        scroll = contentsPosition.DOLocalMoveY(finalY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                ResetList();
                mainMenu.CloseCredits();
            });

    }
    private void ResetList()
    {
        if (scroll != null) { scroll.Kill(false); }
        contentsPosition.gameObject.transform.localPosition = new Vector2(0, initialY);
    }

    private void OnEnable()
    {
        ScrollThruCredits(); 
    }
    private void OnDisable()
    {
        ResetList();
    }

}
