using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClumsyBat.UI
{
    public class TooltipUI : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private Text dialogueText;

        [SerializeField] private Canvas dialogueOverlay;
        [SerializeField] private RectTransform dialogueScroll;
        [SerializeField] private Animator dialogueScrollAnimator;
        [SerializeField] private RectTransform resumeNextImage;
        [SerializeField] private RectTransform resumePlayImage;
        [SerializeField] private RectTransform nomee;
#pragma warning restore 649

        private TooltipHandler tooltipHandler;

        public enum NextDialogueImages
        {
            ResumeGame, NextDialogue
        }

        private enum States
        {
            Closed, Open, Closing, Opening
        }
        private States state;

        private void Start()
        {
            tooltipHandler = FindObjectOfType<TooltipHandler>();
            dialogueOverlay = GetComponent<Canvas>();
            CloseImmediate();
        }

        public void ContinuePressed()
        {
            tooltipHandler.InputReceived();
        }

        public void SetText(string text, NextDialogueImages nextDialogueImage)
        {
            StartCoroutine(SetTextRoutine(text, nextDialogueImage));
        }

        public void Close()
        {
            if (state == States.Closed || state == States.Closing) return;

            state = States.Closing;
            StartCoroutine(CloseDialogueWindow());
        }

        public void Open(float yPos)
        {
            if (state == States.Open || state == States.Opening) return;

            Vector3 pos = dialogueScroll.position;
            pos.y = yPos;
            dialogueScroll.position = pos;

            state = States.Opening;
            StartCoroutine(OpenDialogueWindow());
        }

        public void CloseImmediate()
        {
            state = States.Closed;
            dialogueOverlay.enabled = false;
        }

        private IEnumerator SetTextRoutine(string text, NextDialogueImages nextDialogueImage)
        {
            while (state != States.Open)
            {
                if (state == States.Closing || state == States.Closed)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }

            dialogueText.text = text;
            UIObjectAnimator.Instance.PopInObject(dialogueText.GetComponent<RectTransform>());

            if (nextDialogueImage == NextDialogueImages.NextDialogue)
            {
                UIObjectAnimator.Instance.PopInObject(resumeNextImage);
            }
            else
            {
                UIObjectAnimator.Instance.PopInObject(resumePlayImage);
            }
        }

        private IEnumerator OpenDialogueWindow()
        {
            dialogueText.text = "";
            nomee.gameObject.SetActive(false);
            resumePlayImage.gameObject.SetActive(false);
            resumeNextImage.gameObject.SetActive(false);

            dialogueOverlay.enabled = true;

            dialogueScroll.gameObject.SetActive(true);
            dialogueScrollAnimator.Play("TooltipScrollClosed", 0, 0f);
            yield return StartCoroutine(UIObjectAnimator.Instance.PopInObjectRoutine(dialogueScroll));
            dialogueScrollAnimator.Play("TooltipScrollOpen", 0, 0f);
            yield return new WaitForSecondsRealtime(0.2f);
            UIObjectAnimator.Instance.PopInObject(nomee);
            state = States.Open;
        }

        private IEnumerator CloseDialogueWindow()
        {
            resumePlayImage.gameObject.SetActive(false);
            resumeNextImage.gameObject.SetActive(false);

            UIObjectAnimator.Instance.PopOutObject(dialogueText.GetComponent<RectTransform>());
            yield return StartCoroutine(UIObjectAnimator.Instance.PopOutObjectRoutine(nomee));
            dialogueScrollAnimator.Play("TooltipScrollClose", 0, 0f);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return StartCoroutine(UIObjectAnimator.Instance.PopOutObjectRoutine(dialogueScroll));

            state = States.Closed;
            dialogueOverlay.enabled = false;
        }

    }
}
