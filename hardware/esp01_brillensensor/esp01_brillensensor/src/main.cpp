
#include "MPU9250.h"
#include <Arduino.h>
#include "SPI.h"
#include <Wire.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>


const char* ssid = "GATEWAY49";
const char* pass = "Ready2start";

const char mqtt_Host[] = "192.168.3.31";
const int mqtt_Port = 1883;
const char* mqtt_User = "user";
const char* mqtt_Password = "user";

WiFiClient net;
PubSubClient client(net);

MPU9250 IMU(Wire,0x68);

int status;


unsigned long lastMillis = 0;





void connect() {
  Serial.print("checking wifi...");
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(1000);
  }

  Serial.println("\nconnected!");
}

void callback(char* topic, byte* payload, int length) {

  Serial.print("Message received in topic: ");
  Serial.print(topic);
  Serial.print("   length is:");
  Serial.println(length);

  Serial.print("Data Received From Broker:");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }

  Serial.println();
  Serial.println("-----------------------");
  Serial.println();

}
float downAcc[3];
void setup() {
  Serial.begin(115200);
  while(!Serial) {}

  WiFi.begin(ssid, pass);

  connect();
  client.setServer(mqtt_Host, mqtt_Port);
  client.setCallback(callback);

  /**while (!client.connected()) {
    Serial.println("Connecting to MQTT...");
    yield();
    if (client.connect("ESP32Client", mqtt_User, mqtt_Password ))
    {

      Serial.println("connected to MQTT broker");

    }
    else
    {

      Serial.print("failed with state ");
      Serial.print(client.state());
      delay(500);

    }
  }**/


  Wire.begin(5,4);
  // serial to display data



  // start communication with IMU 
  status = IMU.begin();
  if (status < 0) {
    Serial.println("IMU initialization unsuccessful");
    Serial.println("Check IMU wiring or try cycling power");
    Serial.print("Status: ");
    Serial.println(status);
    while(1) {}
  }
  // setting the accelerometer full scale range to +/-8G 
  IMU.setAccelRange(MPU9250::ACCEL_RANGE_4G);
  // setting the gyroscope full scale range to +/-500 deg/s
  IMU.setGyroRange(MPU9250::GYRO_RANGE_250DPS);
  // setting DLPF bandwidth to 20 Hz
  IMU.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_5HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU.setSrd(19);
  
  /**downAcc[0]=IMU.getAccelX_mss();
  downAcc[1]=IMU.getAccelY_mss();
  downAcc[2]=IMU.getAccelZ_mss();
  **/
}

//void getStrData(char* buffer) {
//    dtostrf(IMU.getAccelX_mss(), 6, 5, buffer);
//}
/////////////////////////////////////////////////////////////////////////

//!!!!!!!!!!!!!DIE HIER SIND ZUM TUNEN DER ERKENNUNG!!!!!!!!!!!!!!!!!!!!


const int count=4;   //WIE LANGE DIE BEWEGUNG ANHALTEN MUSS
const int triggerCount=30;    //WIE LANGE KEINE NEUEN EVENTS ERKANNT WERDEN
const int threshhold=9;     //WIE STARK DIE BEWEGUNG SEIN MUSS

//X UND Y ACHSE SIND VERTAUSCHT, BITTE NICHT "FIXEN", DAS SOLL SO

/////////////////////////////////////////////////////////////////////////

float angle[3];
float accelInt[3];
float delta[3];
float buffer[3][100];
int trigger=0;
int counterup=0;
int counterdown=0;
int counterleft=0;
int counterright=0;
float mittel[3]={0,0,0};
void loop() {
  client.loop();

 /** if (!client.connected()) {
    Serial.println("error");
    //connect();
  }**/
  IMU.readSensor();
  



  delay(10);  // <- fixes some issues with WiFi stability
  
  // publish a message roughly every second.
  if (millis() - lastMillis > 10) {
    lastMillis = millis();
    
    IMU.readSensor();
    angle[0]=(angle[0]+IMU.getGyroX_rads()*0.1*180/3.14); 
    angle[1]=(angle[1]+IMU.getGyroY_rads()*0.1*180/3.14);
    angle[2]=(angle[2]+IMU.getGyroZ_rads()*0.1*180/3.14);
    
    accelInt[0]=IMU.getAccelX_mss();
    accelInt[1]=IMU.getAccelY_mss();
    accelInt[2]=IMU.getAccelZ_mss();

  //Serial.print("x:");
  //Serial.println(accelInt[0]);  
//Serial.print("y:");
//Serial.println(accelInt[1]);
//Serial.print("z:");
//Serial.println(accelInt[2]);  

  mittel[0]=0;
  mittel[1]=0;
  mittel[2]=0;


    for(int coord=0;coord<3;coord++){
      for(int entry=99;entry!=0;entry--){
        if(buffer[coord][entry]!=NULL){
          mittel[coord]+=buffer[coord][entry];

        }
      }
    }
mittel[0]=mittel[0]/100;    
mittel[1]=mittel[1]/100;  
mittel[2]=mittel[2]/100;
    /**accelInt[0]=accelInt[0]*0.9+IMU.getAccelX_mss()*0.1;
    accelInt[1]=accelInt[1]*0.9+IMU.getAccelY_mss()*0.1;
    accelInt[2]=accelInt[2]*0.9+IMU.getAccelZ_mss()*0.1;
**/

    delta[0]=(mittel[0]-accelInt[0]);
    delta[1]=(mittel[1]-accelInt[1]);
    delta[2]=(mittel[2]-accelInt[2]);
     
  //Serial.print("x:");
  //Serial.println(delta[0]);  
  //Serial.print("y:");
  //Serial.println(delta[1]);
  //Serial.print("z:");
  //Serial.println(delta[2]); 
     
    float intensity=sqrt(delta[0]*delta[0]+delta[1]*delta[1]+delta[2]*delta[2]);
  
    /**for(int i=2;i>=0;i--){
      downAcc[(i+1)%3]=downAcc[(i+1)%3]*cos(angle[i])-downAcc[(i+2)%3]*sin(angle[i]);
      downAcc[(i+2)%3]=downAcc[(i+2)%3]*cos(angle[i])+downAcc[(i+1)%3]*sin(angle[i]);
    }**/
/**
    Serial.print("deg");
    for(float element:angle){
      Serial.println(element);
    }
    Serial.print("acc");
    for(float element:accelInt){
      Serial.println(element);
    }
    Serial.println("down ");
    for(float element:downAcc){
      Serial.println(element);
    }**/
    
  trigger=trigger>0?trigger-1:trigger;
   if(intensity>threshhold&&trigger==0){
     if(abs(delta[2])>abs(delta[1])){
       if(delta[2]>0){
         //Serial.println("u");
         counterup+=1;
         counterdown+=-1;
         counterleft+=-1;
         counterright+=-1;
         if(counterup>count){
           Serial.println("UP");
           counterup=0;
           counterdown=0;
           counterleft=0;
           counterright=0;
           trigger=triggerCount;
         }

       }
       else{
         //Serial.println("d");
         counterup+=-1;
         counterdown+=+1;
         counterleft+=-1;
         counterright+=-1;
         if(counterdown>count){
           Serial.println("DOWN");
           counterup=0;
           counterdown=0;
           counterleft=0;
           counterright=0;
           trigger=triggerCount;
         }
       }
     }
     else{
       if(delta[1]>0){
         //Serial.println("r");
         counterup+=-1;
         counterdown+=-1;
         counterleft+=-1;
         counterright+=1;
         if(counterright>count){
            Serial.println("LEFT");
            counterup=0;
            counterdown=0;
            counterleft=0;
            counterright=0;
            trigger=triggerCount;
         }
       }
       else{
         //Serial.println("l");
         counterup+=-1;
         counterdown+=-1;
         counterleft+=1;
         counterright+=-1;
         if(counterleft>count){
            Serial.println("RIGHT");
            counterup=0;
            counterdown=0;
            counterleft=0;
            counterright=0;
            trigger=triggerCount;
         }
       }
         
     }
         counterup=counterup<0?0:counterup;
         counterdown=counterdown<0?0:counterdown;
         counterleft=counterleft<0?0:counterleft;
         counterright=counterright<0?0:counterright;
     //Serial.println(delta[0]);
//     Serial.println(delta[1]);
     //Serial.println(delta[2]);

      //Serial.println(mittel[0]);
      //Serial.println(mittel[1]);
     //Serial.println(mittel[2]);
     
   }else{
     for(int coord=0;coord<3;coord++){
      for(int entry=99;entry!=0;entry--){
        if(buffer[coord][entry-1]!=NULL){  
          buffer[coord][entry]=buffer[coord][entry-1];
        }
      }
      buffer[coord][0]=accelInt[coord];
    }
   }
    /**char buffer[6];
    dtostrf(IMU.getAccelX_mss(), 6, 5, buffer);
    client.publish("/AccelX", buffer);
    dtostrf(IMU.getAccelY_mss(), 6, 5, buffer);
    client.publish("/AccelY", buffer);
    dtostrf(IMU.getAccelZ_mss(), 6, 5, buffer);
    client.publish("/AccelZ", buffer);
    dtostrf(IMU.getGyroX_rads(), 6, 5, buffer);
    client.publish("/GyroX", buffer);
    dtostrf(IMU.getGyroY_rads(), 6, 5, buffer);
    client.publish("/GyroY", buffer);
    dtostrf(IMU.getGyroZ_rads(), 6, 5, buffer);
    client.publish("/GyroZ", buffer);
    /*
    IMU.readSensor(); // read the sensor

    // display the data
    Serial.print(IMU.getAccelX_mss(),6);
    Serial.print("\t");
    Serial.print(IMU.getAccelY_mss(),6);
    Serial.print("\t");
    Serial.print(IMU.getAccelZ_mss(),6);
    Serial.print("\t");
    Serial.print(IMU.getGyroX_rads(),6);
    Serial.print("\t");
    Serial.print(IMU.getGyroY_rads(),6);
    Serial.print("\t");
    Serial.print(IMU.getGyroZ_rads(),6);
    Serial.print("\t");
    Serial.print(IMU.getMagX_uT(),6);
    Serial.print("\t");
    Serial.print(IMU.getMagY_uT(),6);
    Serial.print("\t");
    Serial.print(IMU.getMagZ_uT(),6);
    Serial.print("\t");
    Serial.println(IMU.getTemperature_C(),6);
    */
    //char buffer[7];
    //getStrData(buffer);
    //client.publish("/AccelX", String(1234));
    
  }

}