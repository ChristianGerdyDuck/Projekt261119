using TMPro;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    [SerializeField]
    private int KeysRequired;
    private int KeyCollected; 
    [SerializeField]
    private int StarsRequired;
    private int StarCollected;
    public TextMeshProUGUI KeyText;
    public TextMeshProUGUI StarText;
    public bool HasEnoughKeys => KeyCollected >= KeysRequired;
    public bool HasEnoughStars => StarCollected >= StarsRequired;
    private void Start() => Refresh();
    public void AddCollectibleKey()
    {
        KeyCollected++;
        Refresh();
    }
    public void AddCollectibleStar()
    {
        StarCollected++;
        Refresh();
    }
    private void Refresh()
    {
        KeyText.text = $"Keys Collected: {KeyCollected}/{KeysRequired}";
        StarText.text = $"Stars Collected: {StarCollected}/{StarsRequired}";
    }
}
