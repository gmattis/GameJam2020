using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraScript : MonoBehaviour
{
    public Vector3 cameraOffset;
    public float distanceToRoom = 20f;

    private int colorStep = 0;
    private int step = 0;
    private AudioSource audioSource;
    private Bloom bloomLayer;

    private float[] meanHistory = new float[15];

    private void Start()
    {
        cameraOffset = new Vector3(Mathf.Cos(transform.rotation.eulerAngles.x), 1, Mathf.Cos(transform.rotation.eulerAngles.x)).normalized;
        transform.position = CalculateNewPosition();
        
        audioSource = MusicManager.Instance.audioSourceCombat;
        gameObject.GetComponent<PostProcessVolume>().profile.TryGetSettings(out bloomLayer);
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 newPosition = CalculateNewPosition();

        if (!newPosition.Equals(transform.position))
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.075f);
        }

        float[] spectrum = new float[256];
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        float mean = spectrum[1];

        float bloomModifier = mean;
        for (int i = 0; i < meanHistory.Length; i++) { bloomModifier += meanHistory[i]; }
        bloomLayer.intensity.value = 5f * (1f + bloomModifier * 0.25f);

        for (int i = 0; i < meanHistory.Length - 1; i++) { meanHistory[i] = meanHistory[i + 1]; }
        meanHistory[meanHistory.Length - 1] = mean;
    }

    private void FixedUpdate()
    {
        if (colorStep == 0)
        {
            GetComponent<Camera>().backgroundColor = Color.Lerp(Color.red, Color.green, step / 30f);
        }
        else if (colorStep == 1)
        {
            GetComponent<Camera>().backgroundColor = Color.Lerp(Color.green, Color.blue, step / 30f);
        }
        else if (colorStep == 2)
        {
            GetComponent<Camera>().backgroundColor = Color.Lerp(Color.blue, Color.red, step / 30f);
        }
        step += 1;

        if (step == 31)
        {
            step = 0;
            colorStep += 1;

            if (colorStep == 3)
            {
                colorStep = 0;
            }
        }
    }

    private Vector3 CalculateNewPosition()
    {
        Vector3 playerPosition = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
        Vector2 room = new Vector2(Mathf.Floor((playerPosition.x + 10) / 20), Mathf.Floor((playerPosition.z + 10) / 20));
        Vector3 roomCenter = new Vector3(room.x * 20, 0, room.y * 20);
        return roomCenter + cameraOffset * distanceToRoom;
    }
}
