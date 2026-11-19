using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public GoalZone Goal;
public void Restart()
    {
        SceneManager.LoadScene("GameScene");
        Goal.HasWon = false;
    }
}
