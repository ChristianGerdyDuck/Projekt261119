using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NpcDialogue : MonoBehaviour
{
    [Header("Scene References")]
    public Transform Player;
    public float TalkDistance = 2f;
    public GameObject Prompt;
    public GameObject Panel;
    public TextMeshProUGUI NpcText;
    public TextMeshProUGUI ChoicesText;
    [SerializeField] private Transform cameraLookTarget;

    [Header("Dialogue")]
    [TextArea] public string Greeting = "Hello, can I help you?";
    public string Choice1 = "What should I do?";
    [TextArea] public string Reply1 = "Gather the keys and stars!";
    public string Choice2 = "Got any tips?";
    [TextArea] public string Reply2 = "Look around for hidden objects.";
    private State state;
    public ThirdPersonMotor DialogueMotor;
    public Vector3 CameraLookPosition => cameraLookTarget != null
        ? cameraLookTarget.position
        : transform.position + Vector3.up * 1.5f;
    private void Awake()
    {
        Close();
        Prompt.SetActive(false);
    }
    private void Update()
    {
        bool near = Vector3.Distance(Player.position, transform.position) <= TalkDistance;

        if (!near && state != State.Closed)
            Close();

        Keyboard board = Keyboard.current;

        if (near && board != null)
            ReadInput(board);

        Prompt.SetActive(near && state == State.Closed);
    }
    private void ReadInput(Keyboard board)
    {
        switch (state)
        {
            case State.Closed:
                if (board.eKey.wasPressedThisFrame)
                    Open();
                break;

            case State.Choosing:
                if (board.digit1Key.wasPressedThisFrame)
                    ShowReply(Reply1);
                else if (board.digit2Key.wasPressedThisFrame)
                    ShowReply(Reply2);
                break;
            case State.Reply:
                if (board.eKey.wasPressedThisFrame)
                    Close();
                break;
            default: break;
        }
    }
    private void Open()
    {
        if (!DialogueMotor.TryBeginDialogue(this))
            return;

        state = State.Choosing;
        NpcText.text = Greeting;
        ChoicesText.text = $"1      {Choice1}\n2      {Choice2}";
        ChoicesText.gameObject.SetActive(true);
        Panel.SetActive(true);
    }
    private void ShowReply(string reply)
    {
        state = State.Reply;
        NpcText.text = reply;
        ChoicesText.text = "E      Close conversation";
    }
    private void Close()
    {
        state = State.Closed;

        if (DialogueMotor != null)
            DialogueMotor.EndDialogue(this);
        Panel.SetActive(false);
    }

}
public enum State
{
    Closed, Choosing, Reply
}
