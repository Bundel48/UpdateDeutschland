using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Setup : MonoBehaviour
{
    public GameObject cardObj;
    public GameObject backgroundPanel;
    public Card currentCard;

    public Texture BlueCard;
    public Texture RedCard;
    public Texture GreenCard;
    public Texture YellowCard;
    public Texture BlackCard;
    public Texture UnoCard;

    public Texture Retour;
    public Texture Punish;
    public Texture Hold;
    public Texture ChangeColorCard;

    public Color32 blueCardColor;
    public Color32 RedCardColor;
    public Color32 GreenCardColor;
    public Color32 YellowCardColor;
    public Color32 BlackCardColor;

    public List<Card> cardStack = new List<Card>();
    public List<Card> playerCards = new List<Card>();
    public List<Card> computerCards = new List<Card>();
    public List<Card> usedCards = new List<Card>();

    float playerCarddrawerStartX = 121f;
    float playerCarddrawerStartY = 189f;

    public int CurrentSelectedCard;
    public int CurrentCard;

    public int cardCounter;

    //TODO
    //Karten verteilen
    void Start()
    {
        cardCounter = 0;
        makeCards();
        shuffleCards();

        drawCards();

        for(int playerShower = 0; playerShower < playerCards.Count; playerShower++){
            float calcWidth = 1000 / (playerCards.Count + 1);
            loadCard(playerCards[playerShower], playerCarddrawerStartX + (calcWidth * playerShower), playerCarddrawerStartY);
        }
        CurrentSelectedCard = -1;
        CurrentCard = 0;

        makeBoard();

        logCards(playerCards);
        factorInRules();
    }

    public void makeCards(){
        for(int colorCounter = 1; colorCounter < 5; colorCounter++){
            for(int numberCounter = 0; numberCounter < 15; numberCounter++){
                Card card = new Card(numberCounter, colorCounter);
                cardStack.Add(card);
                cardStack.Add(card);
            }
        }
    }

    public void shuffleCards(){
        for(int shuffle = 0; shuffle < 1000; shuffle ++){
            int pos1 = Random.Range(0, cardStack.Count-1);
            int pos2 = Random.Range(0, cardStack.Count-1);

            Card saving = cardStack[pos1];
            cardStack[pos1] = cardStack[pos2];
            cardStack[pos2] = saving;
        }
    }

    public void logCards(List<Card> cardStacks){
        foreach(Card cardShower in cardStacks){
            Debug.Log(cardShower.cardNumber + " in color " + cardShower.cardColor + " in card " + cardShower.cardObject);
        }
    }

    public void drawCards(){
        for(int drawcounter = 0; drawcounter < 7; drawcounter++){
            playerCards.Add(cardStack[0]);
            cardStack.RemoveAt(0);  
            computerCards.Add(cardStack[0]);
            cardStack.RemoveAt(0);  
        }
    }

    public void loadCard(Card currentcard, float posX, float posY) { //does all the setup for loading. Used if card doesn't need a specific position		
		GameObject temp = Instantiate (cardObj);
        temp.name = ("card" + cardCounter);
        cardCounter++;
        switch(currentcard.cardColor){
            case 1:
                temp.GetComponent<RawImage> ().texture = RedCard;
                temp.transform.GetChild(1).GetComponent<Text>().color = RedCardColor;
                break;
            case 2:
                temp.GetComponent<RawImage> ().texture = BlueCard;
                temp.transform.GetChild(1).GetComponent<Text>().color = blueCardColor;
                break;
            case 3:
                temp.GetComponent<RawImage> ().texture = GreenCard;
                temp.transform.GetChild(1).GetComponent<Text>().color = GreenCardColor;
                break;
            case 4:
                temp.GetComponent<RawImage> ().texture = YellowCard;
                temp.transform.GetChild(1).GetComponent<Text>().color = YellowCardColor;
                break;
        }

        temp.transform.position = new Vector3(posX,posY,0);
		temp.transform.localScale = new Vector3 (1, 1, 1);

        if(currentcard.cardNumber <10){
            temp.transform.GetChild(0).GetComponent<Text>().text = currentcard.cardNumber.ToString();
            temp.transform.GetChild(1).GetComponent<Text>().text = currentcard.cardNumber.ToString();
            temp.transform.GetChild(2).GetComponent<Text>().text = currentcard.cardNumber.ToString();
        }else if(currentcard.cardNumber == 10){
            temp.transform.GetChild(0).GetComponent<Text>().text = "+2";
            temp.transform.GetChild(1).GetComponent<Text>().text = "";
            temp.transform.GetChild(2).GetComponent<Text>().text = "+2";


            temp.transform.GetChild(3).GetComponent<RawImage>().texture = Punish;
            GameObject currentProblem = temp.transform.gameObject;
            temp.transform.GetChild(3).gameObject.SetActive(true);

        }else if(currentcard.cardNumber == 11){
            temp.transform.GetChild(0).GetComponent<Text>().text = "";
            temp.transform.GetChild(1).GetComponent<Text>().text = "";
            temp.transform.GetChild(2).GetComponent<Text>().text = "";


            temp.transform.GetChild(3).GetComponent<RawImage>().texture = Retour;
            GameObject currentProblem = temp.transform.gameObject;
            temp.transform.GetChild(3).gameObject.SetActive(true);
        }else if(currentcard.cardNumber == 12){
            temp.transform.GetChild(0).GetComponent<Text>().text = "";
            temp.transform.GetChild(1).GetComponent<Text>().text = "";
            temp.transform.GetChild(2).GetComponent<Text>().text = "";


            temp.transform.GetChild(3).GetComponent<RawImage>().texture = Hold;
            GameObject currentProblem = temp.transform.gameObject;
            temp.transform.GetChild(3).gameObject.SetActive(true);

        }else if(currentcard.cardNumber == 13){

            temp.transform.GetChild(0).GetComponent<Text>().text = "";
            temp.transform.GetChild(1).GetComponent<Text>().text = "";
            temp.transform.GetChild(2).GetComponent<Text>().text = "";


            temp.transform.GetChild(3).GetComponent<RawImage>().texture = ChangeColorCard;
            GameObject currentProblem = temp.transform.gameObject;
            temp.transform.GetChild(3).gameObject.SetActive(true);

        }else if(currentcard.cardNumber == 14){

            temp.transform.GetChild(0).GetComponent<Text>().text = "+4";
            temp.transform.GetChild(1).GetComponent<Text>().text = "";
            temp.transform.GetChild(2).GetComponent<Text>().text = "+4";


            temp.transform.GetChild(3).GetComponent<RawImage>().texture = ChangeColorCard;
            GameObject currentProblem = temp.transform.gameObject;
            temp.transform.GetChild(3).gameObject.SetActive(true);

        }else{
            temp.transform.GetChild(0).GetComponent<Text>().text = "F";
            temp.transform.GetChild(1).GetComponent<Text>().text = "F";
            temp.transform.GetChild(2).GetComponent<Text>().text = "F";
        }

		temp.transform.SetParent(backgroundPanel.transform);

        currentcard.cardObject = temp;
    }

    public void makeBoard(){
        
        GameObject temp = Instantiate (cardObj);
        temp.name = ("BackwardsCard");

        temp.GetComponent<RawImage> ().texture = UnoCard;
        temp.transform.GetChild(1).GetComponent<Text>().color = YellowCardColor;

        temp.transform.GetChild(0).GetComponent<Text>().text = "";
        temp.transform.GetChild(1).GetComponent<Text>().text = "";
        temp.transform.GetChild(2).GetComponent<Text>().text = "";
        
        temp.transform.position = new Vector3(playerCarddrawerStartX,700,0);
		temp.transform.localScale = new Vector3 (1, 1, 1);
		temp.transform.SetParent(backgroundPanel.transform);

        usedCards.Add(cardStack[0]);
        loadCard(usedCards[usedCards.Count - 1], playerCarddrawerStartX + 250, 700);
        usedCards[usedCards.Count-1].cardObject.transform.SetAsFirstSibling();
        cardStack.RemoveAt(0);

    }

    public void displayTopCard(){
        loadCard(usedCards[usedCards.Count-1], playerCarddrawerStartX + 250, 700);
    }

    public void repositionPlayerCards(){
        for(int playerShower = 0; playerShower < playerCards.Count; playerShower++){
            float calcWidth = 1000 / (playerCards.Count + 1);
            playerCards[playerShower].cardObject.transform.position = new Vector3(playerCarddrawerStartX + (calcWidth * playerShower), playerCarddrawerStartY, 0);
        }
    }



    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Keypad6)){
            CurrentCard++;
            if(CurrentCard > playerCards.Count - 1){
                CurrentCard = playerCards.Count - 1;
            }
           Debug.Log(CurrentCard);


        }else if (Input.GetKeyUp(KeyCode.Keypad5)){
            Debug.Log("wut");
            if(checkForAllowance(usedCards[usedCards.Count-1], playerCards[CurrentCard])){
                makeMove(CurrentCard);
            }


        }else if (Input.GetKeyUp(KeyCode.Keypad4)){
            CurrentCard--;
            if(CurrentCard < 0){
                CurrentCard = 0;
            }
            Debug.Log(CurrentCard);
        }

        CardSelector();
        repositionPlayerCards();
        factorInRules();
    }    

    public void CardSelector(){
        if(CurrentSelectedCard != CurrentCard){
            Debug.Log("My Anaconda dont want none, unless you got " + CurrentCard + ", hun!");
            Debug.Log(playerCards[CurrentCard].cardObject);
            //playerCards[CurrentSelectedCard].cardObject.transform.GetChild(3).gameObject.SetActive(false);
            //playerCards[CurrentCard].cardObject.transform.GetChild(3).gameObject.SetActive(true);
            playerCards[CurrentCard].cardObject.transform.SetAsLastSibling();
            CurrentSelectedCard = CurrentCard;
        }
    }

    public bool checkForAllowance(Card stackCard, Card playerCard){
        if(stackCard.cardNumber == playerCard.cardNumber || playerCard.cardNumber >= 13){
            return true;
        }else if(stackCard.cardColor == playerCard.cardColor && playerCard.cardNumber < 13){
            return true;
        }
        return false;
    }

    public void factorInRules(){
        for(int factorChecker = 0; factorChecker < playerCards.Count; factorChecker++){
            if(checkForAllowance(usedCards[usedCards.Count-1], playerCards[factorChecker])){
                playerCards[factorChecker].cardObject.transform.GetChild(4).gameObject.SetActive(false);
            }else{
                playerCards[factorChecker].cardObject.transform.GetChild(4).gameObject.SetActive(true);
            }
        }
    }

    public void makeMove(int chosenCard){
        int cardType = playerCards[chosenCard].cardNumber;
        if(cardType < 10){
            usedCards.Add(playerCards[chosenCard]);
            Destroy(playerCards[chosenCard].cardObject);
            playerCards.RemoveAt(chosenCard);
            displayTopCard();
            repositionPlayerCards();
            makeAIMove();
        }
    }

    public void makeAIMove(){
        Debug.Log("AI nowwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww" + usedCards.Count);
        List<int> possible = new List<int>();
        for(int computerCardcounter = 0; computerCardcounter < computerCards.Count; computerCardcounter++){
            if(checkForAllowance(usedCards[usedCards.Count-1], computerCards[computerCardcounter])){
                possible.Add(computerCardcounter);
            }
        }

        if(possible.Count > 0){
            int chosenCard= Random.Range(0, possible.Count);

            Debug.Log("playing possible Card number: " + chosenCard);

            int cardType = computerCards[chosenCard].cardNumber;
            if(cardType < 10){
                usedCards.Add(playerCards[chosenCard]);
                Destroy(playerCards[chosenCard].cardObject);
                playerCards.RemoveAt(chosenCard);
                displayTopCard();
                repositionPlayerCards();
            }
        }else{
            Debug.Log("no options");
            Debug.Log(computerCards.Count);
            computerCards.Add(cardStack[0]);
            cardStack.RemoveAt(0);  
        }
            Debug.Log(computerCards.Count + "             " +  usedCards.Count);
    }
}