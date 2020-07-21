using UnityEngine;
using UnityEngine.UI;

public class MenuColor : MonoBehaviour
{
    //image affiché dans le canvas
    public Color colorJ1=new Color(1f, 0f, 0.4f, 1f);
    public int tier = 1;
    public Sprite[] paletteJ1;  //sprites a afficher sur l'image

    private PlayerControls playerControls;
    private float disablePaletteJ1 = 0f;
    private int[] tierOffset = new int[] { 0, 1, 3 };
    private int currentColor = 0;

    void Start()
    {
        gameObject.GetComponent<Image>().enabled = false;

        playerControls = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerScript>().playerControls;
        playerControls.Player1.Palette.performed += ctx => PaletteTriggerJ1();  //set la condition de trigger sur le bouton de menu de chaque joueur
    }

    private void PaletteTriggerJ1()                 //timer pour desactiver apres activation
    {
        currentColor = (currentColor + 1) % tier;
        gameObject.GetComponent<Image>().sprite = paletteJ1[tierOffset[tier - 1] + currentColor];
        gameObject.GetComponent<Image>().enabled = true;
        disablePaletteJ1 = Time.time + 1f;
    }

    private void Update()
    {
        if (disablePaletteJ1 <= Time.time) { gameObject.GetComponent<Image>().enabled = false; }
    }

}
