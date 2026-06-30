using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class NPCInteract : InteractableObject
{
    private static readonly HashSet<string> completedTalkNodes = new HashSet<string>();

    [Header("Yarn")]
    public string talkNode = "Start";

    [Header("References")]
    public PlayerMovement playerMovement;
    public bool isDialogueActive = false;

    [Header("Move UI")]
    public UnityEvent onMoveStart;
    public UnityEvent onMoveEnd;

    private DialogueRunner dialogueRunner;

    void Start()
    {
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }
    }

    public override void Interact()
    {
        if (dialogueRunner.IsDialogueRunning)
        {
            return;
        }

        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        dialogueRunner.onDialogueComplete.AddListener(EndDialogue);
        dialogueRunner.StartDialogue(talkNode);
    }

    [YarnCommand("MoveNPC")]
    public IEnumerator MoveNPC(string waypointName, float speed)
    {
        GameObject waypoint = GameObject.Find(waypointName);
        if (waypoint == null)
        {
            Debug.LogError($"[NPC Move Error] Missing waypoint: {waypointName}");
            yield break;
        }

        onMoveStart?.Invoke();

        Vector3 targetPos = waypoint.transform.position;
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        Debug.Log("NPC move complete");
        onMoveEnd?.Invoke();
    }

    private void EndDialogue()
    {
        completedTalkNodes.Add(talkNode);

        if (playerMovement != null)
        {
            playerMovement.canMove = true;
        }

        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.RemoveListener(EndDialogue);
        }

        Debug.Log("Dialogue complete");
    }

    private void OnDestroy()
    {
        if (dialogueRunner != null)
        {
            dialogueRunner.onDialogueComplete.RemoveListener(EndDialogue);
        }
    }

    public static bool HasCompletedTalkNode(string nodeName)
    {
        return !string.IsNullOrEmpty(nodeName) && completedTalkNodes.Contains(nodeName);
    }
}
