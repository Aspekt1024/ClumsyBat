using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipHandler : MonoBehaviour {

    public bool IsPausedForTooltip;

    private PlayerController _playerControl;
    private InputManager _inputManager;
    private TooltipController _tooltipControl;
    
    private readonly Dictionary<DialogueId, TooltipId[]> _dialogueSet = new Dictionary<DialogueId, TooltipId[]>();
    private readonly Dictionary<TooltipId, string> dialogueDict = new Dictionary<TooltipId, string>();

    private bool _bFirstDialogue;
    private bool _bLastDialogue;
    private WaitType _waitType;

    // DialogueID is used for the DialogueSet Dictionary
    // Referenced in the editor
    public enum DialogueId
    {
        FirstDeath,
        FirstJump,
        SecondJump,
        FirstMoth,
        AllOnYourOwn,
        StalLevel,
        NoMoreStals,
        ActuallyMoreStals,
        FirstStalDrop,
        ThatGotReal,
        HypersonicVillagePt1,
        HypersonicVillagePt2,
    }

    // TooltipID references the individual pieces of dialogue defined in the DialogueSet
    public enum TooltipId
    {
        FirstDeath,
        FirstJump,
        SecondJump,
        FirstMoth,
        AllOnYourOwn1,
        AllOnYourOwn2,
        StalLevel,
        StalDrop1,
        StalDrop2,
        StalDrop3,
        NoMoreStals,
        ActuallyMoreStals,
        ThatGotReal,
        HypersonicVillagePt1,
        HypersonicVillagePt2T1,
        HypersonicVillagePt2T2,
    }

    public enum WaitType
    {
        InGamePause,
        InGameNoPause,
        VillageSpeech
    }

    private void OnEnable()
    {
        EventListener.OnDeath += RemoveTooltips;
        EventListener.OnLevelWon += RemoveTooltips;
    }
    private void OnDisable()
    {
        EventListener.OnDeath -= RemoveTooltips;
        EventListener.OnLevelWon -= RemoveTooltips;
    }

    private void Awake()
    {
        GameObject toolTipOverlay = (GameObject)Instantiate(Resources.Load("ToolTipOverlay"));
        _tooltipControl = toolTipOverlay.GetComponent<TooltipController>();

        SetDialogueIDs();
        SetDialogueText();

        if (!Toolbox.Instance.TooltipCompletionPersist)
        {
            Toolbox.Instance.ResetTooltips();
        }
    }

    private void SetDialogueIDs()
    {
        _dialogueSet.Add(DialogueId.FirstDeath, new [] { TooltipId.FirstDeath });
        _dialogueSet.Add(DialogueId.FirstJump, new[] { TooltipId.FirstJump });
        _dialogueSet.Add(DialogueId.SecondJump, new[] { TooltipId.SecondJump });
        _dialogueSet.Add(DialogueId.FirstMoth, new[] { TooltipId.FirstMoth } );
        _dialogueSet.Add(DialogueId.AllOnYourOwn, new[] { TooltipId.AllOnYourOwn1, TooltipId.AllOnYourOwn2 } );
        _dialogueSet.Add(DialogueId.StalLevel, new[] { TooltipId.StalLevel } );
        _dialogueSet.Add(DialogueId.FirstStalDrop, new[] { TooltipId.StalDrop1, TooltipId.StalDrop2, TooltipId.StalDrop3 } );
        _dialogueSet.Add(DialogueId.NoMoreStals, new[] { TooltipId.NoMoreStals });
        _dialogueSet.Add(DialogueId.ActuallyMoreStals, new[] { TooltipId.ActuallyMoreStals });
        _dialogueSet.Add(DialogueId.ThatGotReal, new[] { TooltipId.ThatGotReal });
        _dialogueSet.Add(DialogueId.HypersonicVillagePt1, new[] { TooltipId.HypersonicVillagePt1 });
        _dialogueSet.Add(DialogueId.HypersonicVillagePt2, new[] { TooltipId.HypersonicVillagePt2T1, TooltipId.HypersonicVillagePt2T2,  });
        //_dialogueSet.Add(DialogueId , new[] { TooltipId });
    }

    private void SetDialogueText()
    {
        dialogueDict.Add(TooltipId.FirstDeath, "Don't worry, we can try again!");
        dialogueDict.Add(TooltipId.FirstJump, "Tap anywhere to flap!");
        dialogueDict.Add(TooltipId.SecondJump, "Keep tapping to keep Clumsy in the air.");
        dialogueDict.Add(TooltipId.FirstMoth, "It's getting dark! Collect moths to fuel the lantern.");
        dialogueDict.Add(TooltipId.AllOnYourOwn1, "You made it! I mean, of course you made it!");
        dialogueDict.Add(TooltipId.AllOnYourOwn2, "The path to the village is just through here.");
        dialogueDict.Add(TooltipId.StalLevel, "This is as far as I've ever been. Be careful!");
        dialogueDict.Add(TooltipId.StalDrop1, "Did you see that!?");
        dialogueDict.Add(TooltipId.StalDrop2, "Oh right, you're a bat. Of course you didn't.");
        dialogueDict.Add(TooltipId.StalDrop3, "Watch for falling objects!" );
        dialogueDict.Add(TooltipId.NoMoreStals, "Whew, we got through it! Wasn't that easy!?");
        dialogueDict.Add(TooltipId.ActuallyMoreStals, "... I was wrong. I think it's going to get a lot harder.");
        dialogueDict.Add(TooltipId.ThatGotReal, "Well that got real! Keep going, we're not far away.");
        dialogueDict.Add(TooltipId.HypersonicVillagePt1 , "You made it! Here's a thing.");
        dialogueDict.Add(TooltipId.HypersonicVillagePt2T1, "Gold moths will now activate hypersonic");
        dialogueDict.Add(TooltipId.HypersonicVillagePt2T2, "By the way, we're being attacked by an evil bat. Make him go away.");
        //_dialogueDict.Add(TooltipId, "");
    }

    private void Start()
    {
        _playerControl = FindObjectOfType<PlayerController>();
        _inputManager = _playerControl.GetInputManager();
    }
    
    public void ShowDialogue(DialogueId eventId, WaitType waitType)
    {
        if (!_playerControl.ThePlayer.IsAlive() || Toolbox.Instance.TooltipCompleted(eventId)) { return; }
        _waitType = waitType;
        Toolbox.Instance.SetTooltipComplete(eventId);
        TooltipId[] dialogue = _dialogueSet[eventId];
        StartCoroutine("SetupDialogue", dialogue);
    }

    public void ShowDialogue(string text, float duration, bool pauses = false)
    {
        if (!_playerControl.ThePlayer.IsAlive()) { return; }
        if (setupTooltipCoroutine != null) StopCoroutine(setupTooltipCoroutine);
        setupTooltipCoroutine = StartCoroutine(SetupTooltip(text, duration, pauses));
    }

    private Coroutine setupTooltipCoroutine;
    private IEnumerator SetupTooltip(string text, float duration, bool pauses = false)
    {
        _waitType = pauses ? WaitType.InGamePause : WaitType.InGameNoPause;
        if (_waitType == WaitType.InGamePause) {
            _playerControl.WaitForTooltip(true);
            IsPausedForTooltip = true;
        }
        _tooltipControl.RestoreOriginalScale();
        
        yield return StartCoroutine(_tooltipControl.OpenTooltip());
        yield return StartCoroutine(ShowTooltip(text));
        
        if (_waitType == WaitType.InGamePause)
        {
            IsPausedForTooltip = false;
            _playerControl.WaitForTooltip(false);
            _playerControl.TooltipResume();
        }
        else
        {
            yield return StartCoroutine(KeepTooltipOnScreen(duration));
        }
        _tooltipControl.StartCoroutine("CloseTooltip");
    }

    public IEnumerator SetupDialogue(TooltipId[] dialogue)
    {
        if (_waitType == WaitType.InGamePause) { _playerControl.WaitForTooltip(true); }
        if (_waitType == WaitType.VillageSpeech) { _playerControl.WaitForVillageSpeech(); }
        _bFirstDialogue = true;
        _bLastDialogue = false;
        int numItems = dialogue.Length;
        foreach (TooltipId speech in dialogue)
        {
            numItems--;
            if (numItems == 0)
            {
                _bLastDialogue = true;
            }
            if (_bFirstDialogue)
            {
                _tooltipControl.RestoreOriginalScale();
                yield return _tooltipControl.StartCoroutine("OpenTooltip");
            }
            yield return StartCoroutine(ShowTooltip(dialogueDict[speech]));
            _bFirstDialogue = false;
        }
        if (_waitType == WaitType.InGamePause)
        {
            _playerControl.WaitForTooltip(false);
            _playerControl.TooltipResume();
        }
        else if (_waitType == WaitType.VillageSpeech)
        {
            _playerControl.WaitForVillageSpeech();  // TODO only need to clear the input
        }
        _tooltipControl.StartCoroutine("CloseTooltip");
        EventListener.TooltipActioned();
    }

    private IEnumerator ShowTooltip(string text)
    {
        if (_bFirstDialogue)
        {
            _tooltipControl.SetText(text);
            _tooltipControl.StartCoroutine("ShowText", true);
        }
        else
        {
            yield return _tooltipControl.StartCoroutine("ShowText", false);
            _tooltipControl.SetText(text);
            _tooltipControl.StartCoroutine("ShowText", true);
        }

        if (_waitType == WaitType.InGamePause || _waitType == WaitType.VillageSpeech)
            yield return StartCoroutine("WaitForTooltip");
    }

    private IEnumerator WaitForTooltip()
    {
        const float tooltipPauseDuration = 0.3f;
        yield return new WaitForSeconds(tooltipPauseDuration);

        if (_bLastDialogue)
        {
            _tooltipControl.ShowTapToPlay();
        }
        else
        {
            _tooltipControl.ShowTapToResume();
        }

        _inputManager.ClearInput();
        while (!_inputManager.TapRegistered())
        {
            yield return null;
        }
        _tooltipControl.HideResumeImages();
    }

    private IEnumerator KeepTooltipOnScreen(float durationOnScreen)
    {
        float timeOnScreen = 0f;
        while (timeOnScreen < durationOnScreen)
        {
            if (!Toolbox.Instance.GamePaused)
                timeOnScreen += Time.deltaTime;
            yield return null;
        }
    }

    private void RemoveTooltips()
    {
        StopAllCoroutines();
        _tooltipControl.gameObject.SetActive(false);
    }
}
