using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SideMenuV : MonoBehaviour
{

    public GameObject StartScreen;
    public GameObject PflegeScreen;

    Color lerpedColor = Color.white;

    Color colorCommunication = new Color(0.0177f, 0.5377f, 0.0697f);
    Color colorEntertainment = new Color(0.7924f, 0.7423f, 0.0261f);
    Color colorTraining = new Color(0.3434f, 0.7047f, 0.7830f);
    Color colorHelp = new Color(0.6117f, 0.21157f, 0.1411f);  
    Color colorStartScreen = new Color(0.2264151f, 0.2264151f, 0.2264151f);

    Color startColor;
    Color endColor;
    float colorTime;
    float duration;

    public GameObject lastSelectedButton;
    public int currentPanel;

    //Buttons
    //Startpanel
    public GameObject Communication_Button;
    public GameObject Entertainment_Button;
    public GameObject Training_Button;
    public GameObject Help_Button;

    //Pflegepanel
    public GameObject LeaveHelp_Button;
    public GameObject Toilet_Button;
    public GameObject Pain_Button;
    public GameObject OtherMed_Button;



    //Do Stuffs
    void Start()
    {
        //Communication_Button.GetComponent<Button>().colors.normalColor = new Color(0.5f, 0.5f, 0.5f);
        highlightButton(lastSelectedButton);

        startColor = colorStartScreen;
        endColor = colorStartScreen;
        duration = 1.3f;
    }


    void Update()
    {
        {
            lerpedColor = Color.Lerp(startColor, endColor, colorTime);
            StartScreen.GetComponent<Image>().color = lerpedColor;
            PflegeScreen.GetComponent<Image>().color = lerpedColor;
            if (colorTime < 1){ // while t below the end limit...
                // increment it at the desired rate every update:
                colorTime += Time.deltaTime/duration;
            }else if(colorTime == 1){
                startColor = endColor;
            }
        }

        void recolorAllBackgrounds(Color destinationColor){
            endColor = destinationColor;
            colorTime = 0;
        }


        /*lerpedColor = Color.Lerp(Color.black, green, Mathf.PingPong(Time.time, 1));
        //Debug.Log(lerpedColor);
        Background.GetComponent<Image>().color=lerpedColor;
        BackgroundFollow.GetComponent<Image>().color=lerpedColor;
        //Debug.Log(Background.GetComponent<Image>().color);*/

        //Debug.Log("Penis");
        //Debug.Log(Communication_Button.GetComponent<Button>().colors.normalColor);

        //reaction on inputs my guy
        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            Debug.Log("Up.");
            if(currentPanel == 1){
                switch(lastSelectedButton.name){
                    case "Entertainment_Button":
                        changeButton(Communication_Button);
                        break;
                    case "Training_Button":
                        changeButton(Entertainment_Button);
                        break;
                    case "Help_Button":
                        changeButton(Training_Button);
                        break;
                }
            }else if(currentPanel == 2){
                
                switch(lastSelectedButton.name){
                    case "Toilet_Button":
                        changeButton(LeaveHelp_Button);
                        break;
                    case "Pain_Button":
                        changeButton(Toilet_Button);
                        break;
                    case "OtherMed_Button":
                        changeButton(Pain_Button);
                        break;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            Debug.Log("confirm.");
            switch(lastSelectedButton.name){
                case "Help_Button":
                    PflegeScreen.SetActive(true);
                    recolorAllBackgrounds(colorHelp);
                    changeButton(LeaveHelp_Button);
                    currentPanel = 2;
                    break;
                case "LeaveHelp_Button":
                    PflegeScreen.SetActive(false);
                    recolorAllBackgrounds(colorStartScreen);
                    changeButton(Communication_Button);
                    currentPanel = 1;
                    break;
            }
        }
        if (Input.GetKeyUp(KeyCode.Keypad2))
        {
            Debug.Log("down.");
            if(currentPanel == 1){
                switch(lastSelectedButton.name){
                    case "Communication_Button":
                        changeButton(Entertainment_Button);
                        break;
                    case "Entertainment_Button":
                        changeButton(Training_Button);
                        break;
                    case "Training_Button":
                        changeButton(Help_Button);
                        break;
                }
            }else if(currentPanel == 2){
                switch(lastSelectedButton.name){
                    case "LeaveHelp_Button":
                        changeButton(Toilet_Button);
                        break;
                    case "Toilet_Button":
                        changeButton(Pain_Button);
                        break;
                    case "Pain_Button":
                        changeButton(OtherMed_Button);
                        break;
                }
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