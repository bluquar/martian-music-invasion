using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AddressingController : MonoBehaviour {

    public uint levelNumber;

    public float sgGrav = 0.1f;
    public float sgFriction = 0.85f;
    public float sgSpring = 0.2f;

    public float swaptime = 1.2f;
    public float flytime = 1.2f;

    public float tutorialFadeTime = 0.2f;

    public bool IsFirstLevel = true;

    public GameObject Superdog;
    public Text SuperdogText;
    public GameObject SuperdogDialogue;
    public Button SuperdogButton;

    public GameObject LevelCompleteObject;
    public GameObject GrayCirclePrefab;
    public GameObject Supergirl;
    public GameObject SupergirlArm;
    public GameObject SupergirlVineCurled;
    public GameObject Vine;
    public GameObject ThrowingVine;
    public BoxCollider2D StaffCollider;
    public AudioSource LastMeasureSource;

    public GameObject[] LevelSteps;

    public GameObject BackgroundParent;
    public GameObject[] Backgrounds;
    private int BackgroundLeft = 0;

    private Vector3 BackgroundDelta;

    private int BackgroundRight { get { return 1 - this.BackgroundLeft; } }
    private bool TransitioningBackgrounds = false;

    private GameObject CurrentStepObject;
    private AddressingStep CurrentStep;
    private GameObject[] IncorrectCircles;
    private GrayCircle CorrectCircle;

    private GameObject NextStepObject;
    private AddressingStep NextStep;

    private int stepsCompleted;

    private Vector3 sgVelocity;

    private static float GetYByAddress(string address)
    { switch (address) {
        case "1st Line": return  0f / 8f;
        case "1st Space": return 1f / 8f;
        case "2nd Line": return  2f / 8f;
        case "2nd Space": return 3f / 8f;
        case "3rd Line": return  4f / 8f;
        case "3rd Space": return 5f / 8f;
        case "4th Line":  return 6f / 8f;
        case "4th Space": return 7f / 8f;
        case "5th Line": return  8f / 8f;
        default:
            Debug.Log("Invalid address: " + address);
            return -0.5f;
    } }

    private void SwapBackgrounds()
    {
        BackgroundLeft = 1 - BackgroundLeft;
    }

    private void PlaceInBox(GameObject obj)
    {
        Bounds bounds = StaffCollider.bounds;
        float boundsAR = bounds.size.x / bounds.size.y;

        SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
        float srAR = sr.bounds.size.x / sr.bounds.size.y;

        float scaleAdjustment;

        if (boundsAR > srAR)
        {
            // Height is limiting factor
            scaleAdjustment = bounds.size.y / sr.bounds.size.y;
        } else
        {
            // Width is limiting factor
            scaleAdjustment = bounds.size.x / sr.bounds.size.x;
        }

        obj.transform.localScale *= scaleAdjustment;

        Vector3 posDelta = bounds.center - sr.gameObject.transform.position;

        var fncb = obj.GetComponent<AddressingStep>().FirstNoteCollider.bounds;
        fncb.center += posDelta;
        obj.transform.position += posDelta;
         
        //sr.gameObject.transform.position = bounds.center;
    }

    private GameObject[] LoadGrayNotes(GameObject stepObj, AddressingStep step, out GrayCircle correct)
    {
        Bounds bounds = step.NotesBox.bounds;
        float yMin = bounds.center.y - bounds.extents.y;
        float yMax = bounds.center.y + bounds.extents.y;
        float xMin = bounds.center.x - bounds.extents.x;
        float xMax = bounds.center.x + bounds.extents.x;

        correct = null;

        GameObject[] circles = new GameObject[step.Notes.Length - 1];
        int j = 0;

        for (int i = 0; i < step.Notes.Length; i++)
        {
            Vector3 position = new Vector3(xMin + (xMax - xMin) * (((float)i) / step.Notes.Length),
                                           yMin + (yMax - yMin) * GetYByAddress(step.Notes[i]), -2);
            GameObject gray = (GameObject)Instantiate(GrayCirclePrefab, position, Quaternion.identity);
            gray.name = step.Notes[i];
            gray.transform.parent = stepObj.transform;
            gray.name = step.Notes[i];
            GrayCircle circ = gray.GetComponent<GrayCircle>();
            bool isCorrect = (i == step.CorrectIndex);
            if (isCorrect)
            {
                circ.IsCorrect = true;
                correct = circ;
            } else
            {
                circ.IsCorrect = false;
                circles[j++] = gray;
            }
            circ.controller = this;
        }
        return circles;
    }

    private IEnumerator ConfigureFirstStep()
    {
        CurrentStepObject = Instantiate(LevelSteps[0]);
        CurrentStep = CurrentStepObject.GetComponent<AddressingStep>();
        CurrentStepObject.transform.parent = this.transform;

        PlaceInBox(CurrentStepObject);
        IncorrectCircles = LoadGrayNotes(CurrentStepObject, CurrentStep, out CorrectCircle);

        firstNoteHeight = CurrentStep.FirstNoteCollider.bounds.center.y;

        SuperdogText.text = "Swing to the " + CurrentStep.Notes[CurrentStep.CorrectIndex] + "!";
        yield return Transition.FadeIn(SuperdogText.gameObject, tutorialFadeTime, false);

        if (!IsFirstLevel && LevelSelection.IsAutoplaying())
        {
            CorrectCircleClicked(CorrectCircle);
        }
    }

    private void UpdateHangingVine(AddressingStep step)
    {
        Vector3 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector3 notePos = step.FirstNoteCollider.bounds.center;

        PlaceVineBetween(Vine, handPos + 0.5f * Vector3.back, notePos + 0.5f * Vector3.back);

        float deltaX = notePos.x - handPos.x;
        float deltaY = notePos.y - handPos.y;

        float mag2 = Vector2.SqrMagnitude(notePos - handPos);
        sgVelocity += sgGrav * (new Vector3(deltaX * deltaY, -deltaX * deltaX) / mag2);

        Vector3 direction = Vector3.Normalize(new Vector3(-deltaY, deltaX, 0f));
        sgVelocity = direction * Vector3.Dot(direction, sgVelocity);

        sgVelocity *= sgFriction;

        Supergirl.transform.position += sgVelocity;
        handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector3 delta = notePos + Mathf.Sqrt(mag2) * (handPos - notePos).normalized - handPos;
        Supergirl.transform.position += delta;
    }

    float vineLength;
    float firstNoteHeight;

    void InitializeVineLength()
    {
        Vector2 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector2 notePos = CurrentStep.FirstNoteCollider.bounds.center;
        vineLength = Vector3.Magnitude(handPos - notePos);
    }

    private string[] TutorialMessages =
    {
        "The Martians have stolen your cape!",
        "Swing through the jungle to find it.",
        "You can swing on notes on the staff above",
        "Some of the notes are booby traps",
        "Follow my hints to swing only on safe notes!"
    };

    public bool InTutorial = false;
    public int TutorialIndex = 0;

    public void TutorialNextButtonPressed()
    {
        Logger.Instance.LogAction(string.Format("Address Level {0}", levelNumber),
            "Tutorial Button Pressed", string.Format("{0}", TutorialIndex));
        StartCoroutine(Tutorial());
    }

    public float Dark = 0.5f;
    public float Bright = 1f;

    private IEnumerator Tutorial()
    {
        if (TutorialIndex == TutorialMessages.Length)
        {
            SuperdogButton.gameObject.SetActive(false);
            StartCoroutine(Transition.TransitionBrightness(gameObject, Superdog, tutorialFadeTime, Dark, Bright));
            yield return Transition.FadeOut(SuperdogText.gameObject, tutorialFadeTime, false);
            SuperdogText.text = "Swing to the " + CurrentStep.Notes[CurrentStep.CorrectIndex] + "!";
            yield return Transition.FadeIn(SuperdogText.gameObject, tutorialFadeTime, false);
            if (LevelSelection.IsAutoplaying())
            {
                CorrectCircleClicked(CorrectCircle);
            }
        } else if (TutorialIndex == 0)
        {
            SuperdogDialogue.SetActive(true);
            SuperdogButton.gameObject.SetActive(true);
            SuperdogText.text = TutorialMessages[TutorialIndex++];
            InTutorial = true;
            StartCoroutine(Transition.FadeIn(SuperdogDialogue, tutorialFadeTime, false));
            yield return Transition.TransitionBrightness(gameObject, Superdog, tutorialFadeTime, Bright, Dark);
            if (LevelSelection.IsAutoplaying())
            {
                TutorialNextButtonPressed();
            }
        } else {
            SuperdogButton.gameObject.SetActive(false);
            yield return Transition.FadeOut(SuperdogText.gameObject, tutorialFadeTime, false);
            SuperdogText.text = TutorialMessages[TutorialIndex++];
            if (TutorialIndex == 3)
            {
                StartCoroutine(Transition.TransitionBrightness(CurrentStepObject, null, tutorialFadeTime, Dark, Bright));
            } else if (TutorialIndex == 4)
            {
                StartCoroutine(Transition.TransitionBrightness(CurrentStepObject, null, tutorialFadeTime, Bright, Dark));
            }
            yield return Transition.FadeIn(SuperdogText.gameObject, tutorialFadeTime, false);
            yield return new WaitForSeconds(tutorialFadeTime);
            if (TutorialIndex == TutorialMessages.Length)
            {
                SuperdogButton.GetComponentInChildren<Text>().text = "Let's go!";
            }
            SuperdogButton.gameObject.SetActive(true);
            if (LevelSelection.IsAutoplaying())
            {
                TutorialNextButtonPressed();
            }
        }
    }

    private IEnumerator InitializeSuperdog()
    {
        if (IsFirstLevel)
        {
            yield return Tutorial();
        } else
        {
            SuperdogButton.gameObject.SetActive(false);
            SuperdogDialogue.SetActive(true);
            yield return Transition.FadeIn(SuperdogDialogue, 0.4f, false);
        }
    }

    void NormalizeVineLength()
    {
        Vector2 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector2 notePos = CurrentStep.FirstNoteCollider.bounds.center;

        float vineGoal = vineLength + (notePos.y - firstNoteHeight);
        Vector2 desiredPos = notePos + vineGoal * (handPos - notePos).normalized;
        Vector3 delta = desiredPos - handPos;

        if (delta.magnitude > sgSpring)
        {
            delta *= (sgSpring / delta.magnitude);
        }

        Supergirl.transform.position += delta;
    }

	protected void Start () {
        BackgroundDelta = (Backgrounds[BackgroundRight].transform.position -
                           Backgrounds[BackgroundLeft].transform.position);
        stepsCompleted = 0;
        StartCoroutine(ConfigureFirstStep());

        InitializeVineLength();
        StartCoroutine(InitializeSuperdog());
        sgVelocity = Vector3.zero;
	}

    protected void Update()
    {
        UpdateHangingVine(CurrentStep);
        NormalizeVineLength();


    }

    public void CorrectCircleClicked(GrayCircle circ)
    {
        if (TransitioningBackgrounds)
        {
            return;
        }
        Logger.Instance.LogAction("Correct Circle", stepsCompleted.ToString(), "");
        if (++stepsCompleted == LevelSteps.Length)
        {
            StartCoroutine(LevelComplete(circ));
        }
        else
        {
            StartCoroutine(LoadNextStep(circ));
        }
    }

    public void IncorrectCircleClicked(GrayCircle circ)
    {
        if (TransitioningBackgrounds)
        {
            return;
        }
        Logger.Instance.LogAction("Incorrect Circle", stepsCompleted.ToString(), circ.name);
        Debug.Log("Incorrect circle clicked");
    }

    private IEnumerator LevelComplete(GrayCircle circ)
    {
        Debug.Log("TODO");
        yield return null;
    }

    private void PlaceVineBetween(GameObject vine, Vector3 start, Vector3 end)
    {
        Transform vt = vine.transform;
        SpriteRenderer vsr = vine.GetComponent<SpriteRenderer>();

        vt.rotation = Quaternion.identity;

        float curWidth = vsr.bounds.size.x;
        float desiredWidth = Vector2.Distance(start, end);

        float deltaX = end.x - start.x;
        float deltaY = end.y - start.y;

        vt.localScale = new Vector3(vt.localScale.x * (desiredWidth / curWidth), vt.localScale.y, 1f);
        vt.position = 0.5f * (start + end);
        vt.position += 0.5f * Vector3.back;
        float rotation = Mathf.Rad2Deg * Mathf.Atan2(deltaY, deltaX);
        vt.Rotate(Vector3.forward, rotation);
    }

    private IEnumerator ThrowVine(Vector3 start, Vector3 dest, float duration)
    {
        float elapsed = 0f;
        Vector3 position;
        do
        {
            position = Vector3.Lerp(start, dest, Transition.SmoothLerp(elapsed / duration));
            if (position != start)
            {
                ThrowingVine.SetActive(true);
                PlaceVineBetween(ThrowingVine, start + 0.5f * Vector3.back, position + 0.5f * Vector3.back);
            }
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        ThrowingVine.SetActive(false);
    }

    private IEnumerator FlySuperdog(float duration)
    {
        yield return Transition.Resize(Superdog.transform, Vector3.one, duration / 4);
        yield return Transition.StandingWave(Superdog.transform, Vector3.up, 0.7f, 1, duration / 2);
        yield return Transition.Resize(Superdog.transform, new Vector3(-1f, 1f, 1f), duration / 4);
    }

    private IEnumerator LoadNextStep(GrayCircle circ)
    {
        bool isLastLevel;
        GameObject[] nextIncorrect;
        AddressingStep OldStep = CurrentStep;
        GameObject OldStepObject = CurrentStepObject;
        GameObject NewStepObject = Instantiate(LevelSteps[stepsCompleted]);
        AddressingStep NextStep = NewStepObject.GetComponent<AddressingStep>();
        GrayCircle NextCorrect = null;
        isLastLevel = NextStep.Notes.Length == 0;
        NewStepObject.transform.parent = this.transform;
        TransitioningBackgrounds = true;

        //PlaceInBox(NewStepObject);

        float currentNoteBoxHeight = OldStep.NotesBox.bounds.size.y;
        float newNoteBoxHeight = NextStep.NotesBox.bounds.size.y;
        NewStepObject.transform.localScale *= (currentNoteBoxHeight / newNoteBoxHeight);
        Vector3 positionDeltaY = Vector3.up * (OldStep.NotesBox.bounds.center.y - NextStep.NotesBox.bounds.center.y);
        NewStepObject.transform.position += positionDeltaY;

        if (!isLastLevel)
        {
            nextIncorrect = LoadGrayNotes(NewStepObject, NextStep, out NextCorrect);
        } else
        {
            nextIncorrect = new GameObject[0];
        }

        Vector3 offset = Vector3.Scale(Vector3.right, circ.gameObject.transform.position - NextStep.FirstNoteCollider.bounds.center);
        Vector3 finalOffset = Vector3.Scale(Vector3.right, circ.gameObject.transform.position - OldStep.FirstNoteCollider.bounds.center);
        NewStepObject.transform.position += offset;

        OldStepObject.transform.position += Vector3.forward * 0.2f;

        SuperdogDialogue.SetActive(false);
        if (!isLastLevel)
        {
            SuperdogText.text = "Swing to the " + NextStep.Notes[NextStep.CorrectIndex] + "!";
        }

        // Stage 1: "Swap"
        StartCoroutine(Transition.FadeOut(IncorrectCircles, swaptime, Transition.FinishType.Destroy));
        StartCoroutine(Transition.FadeOut(circ.gameObject, swaptime, false));
        StartCoroutine(Transition.FadeIn(NewStepObject, swaptime, false));
        yield return Transition.Rotate(SupergirlArm.transform, swaptime / 2, 0f, 160f);

        // Stage 1.5: Throw vine
        SupergirlVineCurled.SetActive(false);
        yield return ThrowVine(SupergirlVineCurled.transform.position, circ.transform.position, swaptime / 2);

        Destroy(circ.gameObject);

        CurrentStep = NextStep;
        CurrentStepObject = NewStepObject;
        IncorrectCircles = nextIncorrect;
        CorrectCircle = NextCorrect;

        if (!isLastLevel)
        {
            StartCoroutine(TransitionBackgrounds());
            StartCoroutine(Transition.Translate(CurrentStepObject.transform, CurrentStepObject.transform.position - finalOffset, flytime));
            StartCoroutine(FlySuperdog(flytime));
        } else
        {
            StartCoroutine(Transition.Translate(CurrentStepObject.transform, Vector3.Scale(CurrentStepObject.transform.position, new Vector3(0f, 1f, 1f)), flytime));
            StartCoroutine(FlySuperdogAway(flytime));
        }
        StartCoroutine(Transition.FadeOut(OldStepObject, flytime, false));
        StartCoroutine(Transition.Translate(OldStepObject.transform, OldStepObject.transform.position - finalOffset, flytime));
        SupergirlVineCurled.SetActive(true);
        StartCoroutine(Transition.Rotate(SupergirlArm.transform, flytime, 160f, 0f));

        yield return new WaitForSeconds(flytime + 0.2f);

        TransitioningBackgrounds = false;

        if (!isLastLevel)
        {
            SuperdogDialogue.SetActive(true);
        }

        Destroy(OldStepObject);

        if (isLastLevel)
        {
            yield return CompleteLevel();
        } else if (LevelSelection.IsAutoplaying())
        {
            CorrectCircleClicked(CorrectCircle);
        }
    }

    private IEnumerator FlySuperdogAway(float duration)
    {
        CircleCollider2D leftCirc = null, rightCirc = null;
        #region Get colliders
        foreach (CircleCollider2D col in Superdog.GetComponentsInChildren<CircleCollider2D>())
        {
            if (leftCirc == null)
            {
                leftCirc = col;
            } else
            {
                if (col.bounds.center.x < leftCirc.bounds.center.x)
                {
                    rightCirc = leftCirc;
                    leftCirc = col;
                } else
                {
                    rightCirc = col;
                }
            }
        }
        #endregion

        Vector3 start = Superdog.transform.position;
        Vector3 left = leftCirc.bounds.center;
        Vector3 right = rightCirc.bounds.center;

        Vector3 startSize = Superdog.transform.localScale;
        Vector3 midSize = new Vector3(0f, 1.3f * startSize.y, 1f);
        Vector3 finalSize = new Vector3(-1.3f * startSize.x, 1.3f * 1.3f * startSize.y, 1f);

        float elapsed = 0f;
        float p;

        Transform t = Superdog.transform;
        Vector3 startScale = t.localScale;

        LevelCompleteObject.SetActive(true);
        StartCoroutine(Transition.FadeIn(LevelCompleteObject, duration, false));

        Vector3 levelCompleteInitialScale = LevelCompleteObject.transform.localScale;

        do
        {
            p = elapsed / duration;
            t.position = Vector3.Lerp(start, left, Transition.SmoothLerp(p));
            t.localScale = Vector3.Lerp(startSize, midSize, p * p);
            foreach (SpriteRenderer sr in LevelCompleteObject.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = new Color(1f, 0.3f, 0.3f, 1f - 0.5f * p);
            }
            LevelCompleteObject.transform.localScale = levelCompleteInitialScale * (1f + 0.3f * p);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);

        elapsed = 0f;

        do
        {
            p = elapsed / duration;
            t.position = Vector3.Lerp(left, right, Transition.SmoothLerp(p));
            t.localScale = Vector3.Lerp(midSize, finalSize, 1 - (1 - p) * (1 - p));
            foreach (SpriteRenderer sr in LevelCompleteObject.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = new Color(1f, 0.3f, 0.3f, 0.5f - 0.5f * p);
            }
            LevelCompleteObject.transform.localScale = levelCompleteInitialScale * (1.3f + 0.3f * p);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);

        Destroy(LevelCompleteObject);
        Destroy(Superdog);
    }

    private IEnumerator CompleteLevel()
    {
        GameObject measureObject = CurrentStepObject.GetComponentInChildren<SpriteRenderer>().gameObject;
        StartCoroutine(Transition.Translate(measureObject.transform, 6f *  Vector3.back, 0.6f));
        yield return Transition.TransitionBrightness(gameObject, measureObject, 0.6f, Bright, Dark);
        LastMeasureSource.time = 0f;
        LastMeasureSource.loop = false;
        LastMeasureSource.Play();

        LevelSelection.AddressingLevelCompleted(this.levelNumber, measureObject.transform, this);
    }

    public void ClearBackground()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator TransitionBackgrounds()
    {
        TransitioningBackgrounds = true;
        Vector3 startPosition = BackgroundParent.transform.position;
        Vector3 endPosition = startPosition - BackgroundDelta;

        yield return Transition.Translate(BackgroundParent.transform, endPosition, flytime);

        SwapBackgrounds();
        Backgrounds[BackgroundRight].transform.position += 2 * BackgroundDelta;
        TransitioningBackgrounds = false;
    }
	
}
