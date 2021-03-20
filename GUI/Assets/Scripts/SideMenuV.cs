using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SideMenuV : MonoBehaviour
{

    public GameObject Background;
    public GameObject BackgroundFollow;

    Color lerpedColor = Color.white;

    Color green = new Color(0.0177f, 0.5377f, 0.0697f);
    Color yellow = new Color(0.7924f, 0.7423f, 0.0261f);
    Color lightblue = new Color(0.3434f, 0.7047f, 0.7830f);
    Color red = new Color(0.6117f, 0.21157f, 0.1411f);  
    Color black = new Color(0.2264151f, 0.2264151f, 0.2264151f);

    public GameObject lastSelectedButton;

    //Buttons
    public GameObject Communication_Button;
    public GameObject Entertainment_Button;
    public GameObject Training_Button;
    public GameObject Help_Button;



    //Do Stuffs
    void Start()
    {
        //Communication_Button.GetComponent<Button>().colors.normalColor = new Color(0.5f, 0.5f, 0.5f);
        highlightButton(lastSelectedButton);
    }


    void Update()
    {
        /*lerpedColor = Color.Lerp(Color.black, green, Mathf.PingPong(Time.time, 1));
        //Debug.Log(lerpedColor);
        Background.GetComponent<Image>().color=lerpedColor;
        BackgroundFollow.GetComponent<Image>().color=lerpedColor;
        //Debug.Log(Background.GetComponent<Image>().color);*/

        Debug.Log("Penis");
        Debug.Log(Communication_Button.GetComponent<Button>().colors.normalColor);

        //reaction on inputs my guy
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            Debug.Log("Up.");
            switch(lastSelectedButton.name){
                case "Button_Unterhaltung":
                    changeButton(Communication_Button);
                    break;
                case "Button_Training":
                    changeButton(Entertainment_Button);
                    break;
                case "Button_Pflege":
                    changeButton(Training_Button);
                    break;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            Debug.Log("confirm.");
        }
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            Debug.Log("down.");
            switch(lastSelectedButton.name){
                case "Button_Kommunikation":
                    changeButton(Entertainment_Button);
                    break;
                case "Button_Unterhaltung":
                    changeButton(Training_Button);
                    break;
                case "Button_Training":
                    changeButton(Help_Button);
                    break;
            }
        }
    }

    void changeButton(GameObject Button){

        //first unhighlight current Button.
        unhightlightButton(lastSelectedButton);
        lastSelectedButton = Button;
        highlightButton(Button);
    }

    //makes the Button not highlighted
    void unhightlightButton(GameObject Button){
        Debug.Log("Get Bright");
        ColorBlock cb = Button.GetComponent<Button>().colors;
        Color newColor = cb.normalColor;
        newColor.a = 1f;
        cb.normalColor = newColor;
        Button.GetComponent<Button>().colors = cb;
    }

    //makes the Button highlighted
    void highlightButton(GameObject Button){
        Debug.Log("Tango Charlie going dark");
        ColorBlock cb = Button.GetComponent<Button>().colors;
        Color newColor = cb.normalColor;
        newColor.a = 0.5f;
        cb.normalColor = newColor;
        Button.GetComponent<Button>().colors = cb;
    }
}