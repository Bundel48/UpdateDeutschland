using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SideMenuV : MonoBehaviour
{

    public GameObject Start_Screen;
    public GameObject Help_Screen;
    public GameObject Communication_Screen;
    public GameObject Entertainment_Screen;
    public GameObject Training_Screen;
    public GameObject Success_Screen;

    Color lerpedColor = Color.white;

    Color colorCommunication = new Color(0.6941177f, 0.2941177f, 0.7921569f);
    Color colorEntertainment = new Color(0.9058824f, 0.8431373f, 0.0f);
    Color colorTraining = new Color(0.0f, 0.6235294f, 1.0f);
    Color colorHelp = new Color(1.0f, 0.3215686f, 0.3215686f);  
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

    //Kommunikationspanel
    public GameObject LeaveCommunication_Button;
    public GameObject FirstContact_Button;
    public GameObject SecondContact_Button;
    public GameObject ThirdContact_Button;

    //Unterhaltungspanel
    public GameObject LeaveEntertainment_Button;
    public GameObject FirstEntertainment_Button;
    public GameObject SecondEntertainment_Button;
    public GameObject ThirdEntertainment_Button;

    //Uebungspanel
    public GameObject LeaveTraining_Button;
    public GameObject FirstTraining_Button;
    public GameObject SecondTraining_Button;
    public GameObject ThirdTraining_Button;

    //Successpanel
    public GameObject LeaveSuccess_Button;

    //Animations
    public Animator Start_Animation;
    public Animator Communication_Animation;
    public Animator Help_Animation;
    public Animator Entertainment_Animation;
    public Animator Training_Animation;
    public Animator Success_Animation;




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
            Start_Screen.GetComponent<Image>().color = lerpedColor;
            Communication_Screen.GetComponent<Image>().color = lerpedColor;
            Entertainment_Screen.GetComponent<Image>().color = lerpedColor;
            Training_Screen.GetComponent<Image>().color = lerpedColor;
            Help_Screen.GetComponent<Image>().color = lerpedColor;
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
        if (Input.GetKeyUp(KeyCode.W))
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
            }else if(currentPanel == 3){
                
                switch(lastSelectedButton.name){
                    case "FirstContact_Button":
                        changeButton(LeaveCommunication_Button);
                        break;
                    case "SecondContact_Button":
                        changeButton(FirstContact_Button);
                        break;
                    case "ThirdContact_Button":
                        changeButton(SecondContact_Button);
                        break;
                }
            }else if(currentPanel == 4){
                
                switch(lastSelectedButton.name){
                    case "FirstEntertainment_Button":
                        changeButton(LeaveEntertainment_Button);
                        break;
                    case "SecondEntertainment_Button":
                        changeButton(FirstEntertainment_Button);
                        break;
                    case "ThirdEntertainment_Button":
                        changeButton(SecondEntertainment_Button);
                        break;
                }
            }else if(currentPanel == 5){
                
                switch(lastSelectedButton.name){
                    case "FirstTraining_Button":
                        changeButton(LeaveTraining_Button);
                        break;
                    case "SecondTraining_Button":
                        changeButton(FirstTraining_Button);
                        break;
                    case "ThirdTraining_Button":
                        changeButton(SecondTraining_Button);
                        break;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            Debug.Log("confirm.");
            
            switch (lastSelectedButton.name){
                case "Help_Button":
                    //Help_Screen.SetActive(true);
                    recolorAllBackgrounds(colorHelp);
                    changeButton(LeaveHelp_Button);
                    slidePanelsEntry(Help_Animation);
                    currentPanel = 2;
                    break;
                case "LeaveHelp_Button":
                    //Help_Screen.SetActive(false);
                    recolorAllBackgrounds(colorStartScreen);
                    changeButton(Communication_Button);
                    slidePanelsExit(Help_Animation);
                    currentPanel = 1;
                    break;
                case "Communication_Button":
                    //Communication_Screen.SetActive(true);
                    recolorAllBackgrounds(colorCommunication);
                    changeButton(LeaveCommunication_Button);
                    slidePanelsEntry(Communication_Animation);
                    currentPanel = 3;
                    break;
                case "LeaveCommunication_Button":
                    recolorAllBackgrounds(colorStartScreen);
                    slidePanelsExit(Communication_Animation);
                    changeButton(Communication_Button);
                    //Communication_Screen.SetActive(false);
                    currentPanel = 1;
                    break;
                case "Entertainment_Button":
                   // Entertainment_Screen.SetActive(true);
                    recolorAllBackgrounds(colorEntertainment);
                    changeButton(LeaveEntertainment_Button);
                    slidePanelsEntry(Entertainment_Animation);
                    currentPanel = 4;
                    break;
                case "LeaveEntertainment_Button":
                    //Entertainment_Screen.SetActive(false);
                    recolorAllBackgrounds(colorStartScreen);
                    changeButton(Communication_Button);
                    slidePanelsExit(Entertainment_Animation);
                    currentPanel = 1;
                    break;
                case "Training_Button":
                   // Training_Screen.SetActive(true);
                    recolorAllBackgrounds(colorTraining);
                    changeButton(LeaveTraining_Button);
                    slidePanelsEntry(Training_Animation);
                    currentPanel = 5;
                    break;
                case "LeaveTraining_Button":
                    //Training_Screen.SetActive(false);
                    recolorAllBackgrounds(colorStartScreen);
                    changeButton(Communication_Button);
                    slidePanelsExit(Training_Animation);
                    currentPanel = 1;
                    break;
                 case "LeaveSuccess_Button":
                    recolorAllBackgrounds(colorStartScreen);
                    changeButton(Communication_Button);
                    slidePanelsExit(Success_Animation);
                    currentPanel = 1;
                    break;
                case "Toilet_Button":
                    // Training_Screen.SetActive(true);
                    recolorAllBackgrounds(colorHelp);
                    changeButton(LeaveSuccess_Button);
                    slidePanelsEntry(Success_Animation);
                    currentPanel = 6;
                    break;
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
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
            }else if(currentPanel == 3){
                switch(lastSelectedButton.name){
                    case "LeaveCommunication_Button":
                        changeButton(FirstContact_Button);
                        break;
                    case "FirstContact_Button":
                        changeButton(SecondContact_Button);
                        break;
                    case "SecondContact_Button":
                        changeButton(ThirdContact_Button);
                        break;
                }
            }else if(currentPanel == 4){
                switch(lastSelectedButton.name){
                    case "LeaveEntertainment_Button":
                        changeButton(FirstEntertainment_Button);
                        break;
                    case "FirstEntertainment_Button":
                        changeButton(SecondEntertainment_Button);
                        break;
                    case "SecondEntertainment_Button":
                        changeButton(ThirdEntertainment_Button);
                        break;
                }
            }else if(currentPanel == 5){
                switch(lastSelectedButton.name){
                    case "LeaveTraining_Button":
                        changeButton(FirstTraining_Button);
                        break;
                    case "FirstTraining_Button":
                        changeButton(SecondTraining_Button);
                        break;
                    case "SecondTraining_Button":
                        changeButton(ThirdTraining_Button);
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
        ColorBlock cb = Button.GetComponent<Button>().colors;
        Color newColor = cb.normalColor;
        newColor.a = 1f;
        cb.normalColor = newColor;
        Button.GetComponent<Button>().colors = cb;
    }

    void slidePanelsEntry(Animator cur_Panel_Animation)
    {
        Start_Animation.SetBool("Entry_B", false);   
        cur_Panel_Animation.SetBool("Exit_B", false);
        Start_Animation.SetBool("Exit_B", true);
        cur_Panel_Animation.SetBool("Entry_B", true);
        
    }    
    
    void slidePanelsExit(Animator cur_Panel_Animation)
    {
        Start_Animation.SetBool("Exit_B", false);
        cur_Panel_Animation.SetBool("Entry_B", false);
        Start_Animation.SetBool("Entry_B", true);
        cur_Panel_Animation.SetBool("Exit_B", true);
        
        
    }

    //makes the Button highlighted
    void highlightButton(GameObject Button){
        ColorBlock cb = Button.GetComponent<Button>().colors;
        Color newColor = cb.normalColor;
        newColor.a = 0.5f;
        cb.normalColor = newColor;
        Button.GetComponent<Button>().colors = cb;
    }
}