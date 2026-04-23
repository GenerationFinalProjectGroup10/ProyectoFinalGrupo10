using UnityEngine;
using System.Collections;

public class SceneSequenceManager : MonoBehaviour
{
    public DialogueManager dialogueManager;

    [Header("Personajes")]
    public Transform madre;
    public Transform hija;

    public Animator madreAnim;
    public Animator hijaAnim;

    [Header("Targets Iniciales")]
    public Transform madreTarget1;
    public Transform hijaTarget1;

    public Transform madreTarget2;
    public Transform hijaTarget2;

    [Header("Targets Finales Hija")]
    public Transform hijaTarget3;
    public Transform hijaTarget4;
    public Transform hijaTarget5;

    [Header("Fade Final")]
    public SceneFader sceneFader;
    public string nextSceneName = "Mundo1";

    [Header("Movimiento")]
    public float speed = 2f;
    public float rotateSpeed = 8f;
    public float stopDistance = 0.08f;

    void Start()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // =====================================================
        // NODO 1
        // =====================================================
        yield return new WaitUntil(() => dialogueManager.waitingExternalResume);
        dialogueManager.waitingExternalResume = false;

        yield return new WaitForSeconds(1f);

        StartWalkBoth();

        yield return StartCoroutine(MoveBoth(madreTarget1, hijaTarget1));

        StopWalkBoth();

        yield return new WaitForSeconds(1f);

        // =====================================================
        // NODO 2
        // =====================================================
        dialogueManager.ForceNextNode();

        yield return new WaitUntil(() => dialogueManager.waitingExternalResume);
        dialogueManager.waitingExternalResume = false;

        yield return new WaitForSeconds(1f);

        StartWalkBoth();

        yield return StartCoroutine(MoveBoth(madreTarget2, hijaTarget2));

        StopWalkBoth();

        yield return new WaitForSeconds(1f);

        // =====================================================
        // NODO 3
        // =====================================================
        dialogueManager.ForceNextNode();

        yield return new WaitUntil(() => dialogueManager.waitingExternalResume);
        dialogueManager.waitingExternalResume = false;

        yield return new WaitForSeconds(1f);

        // =====================================================
        // CAMINATA FINAL HIJA
        // =====================================================
        StartWalkHija();

        yield return StartCoroutine(MoveSingle(hija, hijaTarget3));
        yield return StartCoroutine(MoveSingle(hija, hijaTarget4));
        yield return StartCoroutine(MoveSingle(hija, hijaTarget5));

        StopWalkHija();

        yield return new WaitForSeconds(1f);

        // =====================================================
        // FADE FINAL
        // =====================================================
        if (sceneFader != null)
            sceneFader.FadeToScene(nextSceneName);
    }

    void StartWalkBoth()
    {
        madreAnim.SetBool("isMoving", true);
        hijaAnim.SetBool("isMoving", true);

        madreAnim.SetFloat("Horizontal", 0f);
        madreAnim.SetFloat("Vertical", 1f);

        hijaAnim.SetFloat("Horizontal", 0f);
        hijaAnim.SetFloat("Vertical", 1f);
    }

    void StopWalkBoth()
    {
        madreAnim.SetBool("isMoving", false);
        hijaAnim.SetBool("isMoving", false);

        madreAnim.SetFloat("Horizontal", 0f);
        madreAnim.SetFloat("Vertical", -1f);

        hijaAnim.SetFloat("Horizontal", 0f);
        hijaAnim.SetFloat("Vertical", -1f);

        madre.rotation = Quaternion.LookRotation(Vector3.forward);
        hija.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    void StartWalkHija()
    {
        hijaAnim.SetBool("isMoving", true);
        hijaAnim.SetFloat("Horizontal", 0f);
        hijaAnim.SetFloat("Vertical", 1f);
    }

    void StopWalkHija()
    {
        hijaAnim.SetBool("isMoving", false);
        hijaAnim.SetFloat("Horizontal", 0f);
        hijaAnim.SetFloat("Vertical", -1f);

        hija.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    IEnumerator MoveBoth(Transform targetMadre, Transform targetHija)
    {
        bool madreLlego = false;
        bool hijaLlego = false;

        while (!madreLlego || !hijaLlego)
        {
            if (!madreLlego)
                madreLlego = MoveToTarget(madre, targetMadre);

            if (!hijaLlego)
                hijaLlego = MoveToTarget(hija, targetHija);

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator MoveSingle(Transform actor, Transform target)
    {
        bool llego = false;

        while (!llego)
        {
            llego = MoveToTarget(actor, target);
            yield return new WaitForFixedUpdate();
        }
    }

    bool MoveToTarget(Transform actor, Transform targetRef)
    {
        if (actor == null || targetRef == null)
            return true;

        Vector3 targetPos = new Vector3(
            targetRef.position.x,
            actor.position.y,
            targetRef.position.z
        );

        Vector3 currentFlat = new Vector3(actor.position.x, 0f, actor.position.z);
        Vector3 targetFlat = new Vector3(targetPos.x, 0f, targetPos.z);

        float dist = Vector3.Distance(currentFlat, targetFlat);

        if (dist <= stopDistance)
            return true;

        Vector3 dir = targetPos - actor.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            dir.Normalize();

            Quaternion lookRot = Quaternion.LookRotation(dir);

            actor.rotation = Quaternion.Slerp(
                actor.rotation,
                lookRot,
                rotateSpeed * Time.fixedDeltaTime
            );
        }

        actor.position = Vector3.MoveTowards(
            actor.position,
            targetPos,
            speed * Time.fixedDeltaTime
        );

        return false;
    }
}