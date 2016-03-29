using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddressingController : MonoBehaviour {

    public float sgGrav = 0.1f;
    public float sgFriction = 0.85f;
    public float sgSpring = 0.2f;

    public float swaptime = 1f;
    public float flytime = 1f;

    public GameObject GrayCirclePrefab;
    public GameObject Supergirl;
    public GameObject Vine;
    public BoxCollider2D StaffCollider;

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

    private void LoadGrayNotes(GameObject stepObj, AddressingStep step)
    {
        Bounds bounds = step.NotesBox.bounds;
        float yMin = bounds.center.y - bounds.extents.y;
        float yMax = bounds.center.y + bounds.extents.y;
        float xMin = bounds.center.x - bounds.extents.x;
        float xMax = bounds.center.x + bounds.extents.x;

        IncorrectCircles = new GameObject[step.Notes.Length - 1];
        int j = 0;

        for (int i = 0; i < step.Notes.Length; i++)
        {
            Vector3 position = new Vector3(xMin + (xMax - xMin) * (((float)i) / step.Notes.Length),
                                           yMin + (yMax - yMin) * GetYByAddress(step.Notes[i]), -2);
            GameObject gray = (GameObject)Instantiate(GrayCirclePrefab, position, Quaternion.identity);
            gray.transform.parent = stepObj.transform;
            gray.name = step.Notes[i];
            GrayCircle circ = gray.GetComponent<GrayCircle>();
            bool isCorrect = (i == step.CorrectIndex);
            if (isCorrect)
            {
                circ.IsCorrect = true;
            } else
            {
                circ.IsCorrect = false;
                IncorrectCircles[j++] = gray;
            }
            circ.controller = this;
        }
    }

    private void ConfigureFirstStep()
    {
        CurrentStepObject = Instantiate(LevelSteps[0]);
        CurrentStep = CurrentStepObject.GetComponent<AddressingStep>();
        CurrentStepObject.transform.parent = this.transform;

        PlaceInBox(CurrentStepObject);
        LoadGrayNotes(CurrentStepObject, CurrentStep);
    }

    private void UpdateHangingVine(AddressingStep step)
    {
        Vector2 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector2 notePos = step.FirstNoteCollider.bounds.center;

        Debug.Log("hand at " + handPos);
        Debug.Log("note at " + notePos);

        Transform vt = Vine.transform;
        SpriteRenderer vsr = Vine.GetComponent<SpriteRenderer>();

        vt.rotation = Quaternion.identity;

        float curWidth = vsr.bounds.size.x;
        float desiredWidth = Vector2.Distance(handPos, notePos);

        float deltaX = notePos.x - handPos.x;
        float deltaY = notePos.y - handPos.y;

        vt.localScale = new Vector3(vt.localScale.x * (desiredWidth / curWidth), vt.localScale.y, 1f);
        vt.position = 0.5f * (handPos + notePos);
        vt.position += 0.5f * Vector3.back;
        float rotation = Mathf.Rad2Deg * Mathf.Atan2(deltaY, deltaX);
        //vt.rotation = new Quaternion(1f, 1f, 1f, rotation);
        vt.Rotate(Vector3.forward, rotation);

        //sgVelocity += Vector3.Normalize(new Vector3(-deltaY, deltaX, 1f)) * (sgGrav * deltaX) / deltaY;

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

    void InitializeVineLength()
    {
        Vector2 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector2 notePos = CurrentStep.FirstNoteCollider.bounds.center;
        vineLength = Vector3.Magnitude(handPos - notePos);
    }

    void NormalizeVineLength()
    {
        Vector2 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector2 notePos = CurrentStep.FirstNoteCollider.bounds.center;

        Vector2 desiredPos = notePos + vineLength * (handPos - notePos).normalized;
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
        ConfigureFirstStep();
        UpdateHangingVine(CurrentStep);

        InitializeVineLength();
        sgVelocity = Vector3.zero;
	}

    protected void Update()
    {
        UpdateHangingVine(CurrentStep);
        NormalizeVineLength();


    }

    public void CorrectCircleClicked(GrayCircle circ)
    {
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
        Debug.Log("Incorrect circle clicked");
    }

    private IEnumerator LevelComplete(GrayCircle circ)
    {
        Debug.Log("TODO");
        yield return null;
    }

    private IEnumerator LoadNextStep(GrayCircle circ)
    {
        StartCoroutine(Transition.FadeOut(IncorrectCircles, swaptime, Transition.FinishType.Destroy));

        AddressingStep OldStep = CurrentStep;
        GameObject OldStepObject = CurrentStepObject;

        CurrentStepObject = Instantiate(LevelSteps[stepsCompleted]);
        CurrentStep = CurrentStepObject.GetComponent<AddressingStep>();
        CurrentStepObject.transform.parent = this.transform;

        PlaceInBox(CurrentStepObject);
        LoadGrayNotes(CurrentStepObject, CurrentStep);

        Vector3 offset = Vector3.Scale(Vector3.right, circ.gameObject.transform.position - CurrentStep.FirstNoteCollider.bounds.center);

        Vector3 finalOffset = Vector3.Scale(Vector3.right, circ.gameObject.transform.position - OldStep.FirstNoteCollider.bounds.center);
        CurrentStepObject.transform.position += offset;

        OldStepObject.transform.position += Vector3.forward * 0.2f;

        StartCoroutine(Transition.FadeOut(circ.gameObject, swaptime, false));
        yield return Transition.FadeIn(CurrentStepObject, swaptime, false);

        Destroy(circ.gameObject);

        StartCoroutine(TransitionBackgrounds());
        StartCoroutine(Transition.FadeOut(OldStepObject, flytime, false));
        StartCoroutine(Transition.Translate(OldStepObject.transform, OldStepObject.transform.position - finalOffset, flytime));
        yield return Transition.Translate(CurrentStepObject.transform, CurrentStepObject.transform.position - finalOffset, flytime);

        Destroy(OldStepObject);


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
