using UnityEngine;
using System.Collections;

public class Transition
{
    public enum FinishType
    {
        None,
        Destroy,
        Deactivate,
        Activate
    }

    private static float SmoothLerp(float t)
    {
        //smoothstep
        return t * t * (3f - 2f * t);
        //smootherstep (https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/)
        // return t * t * t * (t * (6f * t - 15f) + 10f);
    }

    public static IEnumerator Translate(Transform t, Vector3 destination, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = t.position;

        do
        {
            t.position = Vector3.Lerp(startPosition, destination, SmoothLerp(elapsed / duration));
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = destination;
    }

    public static IEnumerator Resize(Transform t, Vector3 destScale, float duration)
    {
        float elapsed = 0f;
        Vector3 startScale = t.localScale;

        do
        {
            t.localScale = Vector3.Lerp(startScale, destScale, SmoothLerp(elapsed / duration));
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.localScale = destScale;
    }

    public static IEnumerator TranslateResize(GameObject obj, Vector3 destPos, Vector2 destSize, float duration)
    {
        float elapsed = 0f;
        float p;

        Transform t = obj.transform;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Vector2 startSize = sr.bounds.size;

        Vector3 startScale = t.localScale;
        Vector3 destScale = new Vector3(startScale.x * (destSize.x / startSize.x),
            startScale.y * (destSize.y / startSize.y), t.localScale.z);

        Vector3 startPos = new Vector3(t.position.x, t.position.y, destPos.z);

        do
        {
            p = SmoothLerp(elapsed / duration);
            t.position = Vector3.Lerp(startPos, destPos, p);
            t.localScale = Vector3.Lerp(startScale, destScale, p);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = destPos;
        t.localScale = destScale;
    }

    public static IEnumerator TranslateMovingTarget(Transform t, Vector3 destination, Transform target, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = t.position;

        Vector3 initialTarget = target.position;

        do
        {
            Vector3 currentTarget = target.position;
            Vector3 dest = destination + (currentTarget - initialTarget);

            t.position = Vector3.Lerp(startPosition, dest, SmoothLerp(elapsed / duration));
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = destination;
    }

    public static void SetColor(Color c, GameObject root, GameObject exclude, bool enableSr)
    {
        foreach (SpriteRenderer sr in root.GetComponentsInChildren<SpriteRenderer>())
        {
            if (exclude != null && sr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            sr.color = c;
            if (enableSr)
            {
                sr.enabled = true;
            }
        }
        foreach (CanvasRenderer cr in root.GetComponentsInChildren<CanvasRenderer>())
        {
            if (exclude != null && cr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            cr.SetColor(c);
        }
    }

    public static void SetColor(Color c, GameObject root, GameObject exclude)
    {
        SetColor(c, root, exclude, false);
    }

    public static IEnumerator TransitionColor(GameObject root, GameObject exclude, float duration, 
        Color startColor, Color destColor, bool enableSr)
    {
        if (root == null)
        {
            yield return null;
        }

        float t;
        float elapsed = 0f;

        while (elapsed <= duration)
        {
            t = SmoothLerp(elapsed / duration);
            Color c = Color.Lerp(startColor, destColor, t);
            SetColor(c, root, exclude, enableSr);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        SetColor(destColor, root, exclude, enableSr);

        yield return null;
    }

    public static IEnumerator FinishAction(GameObject[] objs, FinishType finish)
    {
        switch (finish)
        {
            case FinishType.None:
                yield return null;
                break;
            case FinishType.Activate:
                foreach (GameObject obj in objs)
                {
                    obj.SetActive(true);
                }
                yield return null;
                break;
            case FinishType.Deactivate:
                foreach (GameObject obj in objs)
                {
                    obj.SetActive(false);
                }
                yield return null;
                break;
            case FinishType.Destroy:
                foreach (GameObject obj in objs)
                {
                    GameObject.Destroy(obj);
                }
                yield return null;
                break;
            default:
                yield return null;
                break;
        }
    }

    public static IEnumerator TransitionColor(GameObject[] objs, float duration, FinishType finish, Color initial, Color final)
    {
        float t;
        float elapsed = 0f;

        while (elapsed <= duration)
        {
            t = SmoothLerp(elapsed / duration);
            Color c = Color.Lerp(initial, final, t);
            foreach (GameObject obj in objs)
            {
                SetColor(c, obj, null);
            }
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        foreach (GameObject obj in objs)
        {
            SetColor(final, obj, null);
        }

        yield return FinishAction(objs, finish);

        yield return null;
    }

    public static IEnumerator TransitionColor(GameObject obj, float duration,
        Color startColor, Color destColor, bool enableSr)
    {
        yield return TransitionColor(obj, null, duration, startColor, destColor, enableSr);
    }

    public static IEnumerator TransitionColor(GameObject obj, float duration, Color startColor, Color destColor)
    {
        yield return TransitionColor(obj, null, duration, startColor, destColor, false);
    }



    public static IEnumerator TransitionAlpha(GameObject obj, float duration, float initial, float final, bool enableSr)
    {
        Color32 startColor = new Color(1f, 1f, 1f, initial);
        Color32 destColor = new Color(1f, 1f, 1f, final);

        yield return TransitionColor(obj, duration, startColor, destColor, enableSr);
    }

    public static IEnumerator TransitionAlpha(GameObject[] objs, float duration, FinishType finish, float initial, float final)
    {
        Color32 startColor = new Color(1f, 1f, 1f, initial);
        Color32 destColor = new Color(1f, 1f, 1f, final);

        yield return TransitionColor(objs, duration, finish, startColor, destColor);
    }

    public static IEnumerator TransitionBrightness(GameObject root, GameObject exclude, float duration, float initial, float final)
    {
        Color startColor = new Color(initial, initial, initial, 1f);
        Color finalColor = new Color(final, final, final, 1f);

        yield return TransitionColor(root, exclude, duration, startColor, finalColor, false);
    }

    public static IEnumerator FadeIn(GameObject obj, float duration, bool enableSr)
    {
        yield return TransitionAlpha(obj, duration, 0f, 1f, enableSr);
    }

    public static IEnumerator FadeOut(GameObject obj, float duration, bool enableSr)
    {
        yield return TransitionAlpha(obj, duration, 1f, 0f, enableSr);
    }

    public static IEnumerator FadeOut(GameObject[] objs, float duration, FinishType finish)
    {
        yield return TransitionAlpha(objs, duration, finish, 1f, 0f);
    }
}
