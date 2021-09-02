using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SetOutlineColor : MonoBehaviour
{
    public int Priority => 0;
    public int TicksPerSecond => 0;
    public Color selectionColor = new Color(1f, 0.5f, 0f, 1f);
    public float flickerRate = 0.2f;
    private float timer = 0f;
    private Color currColor = Color.red;
    public bool pulse = false;

    private void Update()
    {
        if (pulse) {
            timer += Time.deltaTime;
            if (timer > flickerRate) {
                if (currColor == Color.red) {
                currColor = Color.yellow;
                } else {
                    currColor = Color.red;
                }
                timer = 0;
            }
            
        } else {
            timer = 0;
            currColor = Color.red;
        }
    }
}
