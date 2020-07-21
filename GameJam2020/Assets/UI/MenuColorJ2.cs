using UnityEngine;
using UnityEngine.UI;

public class MenuColorJ2 : MonoBehaviour
{
    //image affiché dans le canvas
    public Color colorJ1 = new Color(0.3f, 0f, 1f, 1f);
    public int tier = 1;
    public Sprite[] paletteJ2;  //sprites a afficher sur l'image

    private PlayerControls playerControls;
    private float disablePaletteJ2 = 0f;
    private int[] tierOffset = new int[] { 0, 1, 3 };
    private int currentColor = 0;

    void Start()
    {
        gameObject.GetComponent<Image>().enabled = false;

        playerControls = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerScript>().playerControls;
        playerControls.Player2.Palette.performed += ctx => PaletteTriggerJ2();  //set la condition de trigger sur le bouton de menu de chaque joueur
    }

    private void PaletteTriggerJ2()                 //timer pour desactiver apres activation
    {
        currentColor = (currentColor + 1) % tier;
        gameObject.GetComponent<Image>().sprite = paletteJ2[tierOffset[tier - 1] + currentColor];
        gameObject.GetComponent<Image>().enabled = true;
        disablePaletteJ2 = Time.time + 1f;
    }

    private void Update()
    {
        if (disablePaletteJ2 <= Time.time) { gameObject.GetComponent<Image>().enabled = false; }
    }

}
