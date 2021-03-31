
#include "MPU9250.h"
#include <Arduino.h>
#include "SPI.h"
#include <Wire.h>
#include <ESP8266WiFi.h>
#include <ESP8266mDNS.h>
#include <PubSubClient.h>
#include <ArduinoOTA.h>
#include <WiFiUdp.h>
//include <Tone.h>

#define buzzPin 3
#define sdaPin 0
#define sclPin 2

boolean test=true;

boolean head=false;

const char* ssid = "GATEWAY49";
const char* pass = "Ready2start";

const char mqtt_Host[] = "135.125.216.231";
const int mqtt_Port = 1883;
const char* mqtt_User = "recomo";
const char* mqtt_Password = "Recomo123%";
const char* mqtt_Name = "brille";

const char* ota_Name = "brille";
const char* ota_Password = "47110815";

//!!!!!!!!!!!!!DIE HIER SIND ZUM TUNEN DER ERKENNUNG!!!!!!!!!!!!!!!!!!!!

const int count = 2;   //WIE LANGE DIE BEWEGUNG ANHALTEN MUSS
const int triggerCount = 150;    //WIE LANGE KEINE NEUEN EVENTS ERKANNT WERDEN
const int threshhold = 1;     //WIE STARK DIE BEWEGUNG SEIN MUSS
unsigned long timeToCheck=10;


WiFiClient net;
PubSubClient client(net);

MPU9250 IMU(Wire,0x68);

int status;
unsigned long updateMillis = 0;
unsigned long lastMillis = 0;
float downAcc[3];


float gyroData[3];
float integral[3];
float pitchRollYaw[3]={0,0,0};
float delta[3];
float buffer[3][100];
int trigger = 0;
int counterup = 0;
int counterdown = 0;
int counterleft = 0;
int counterright = 0;
float mittel[3] = {0,0,0};
float measurement[3]={0,0,0};
bool flagInitPhase = true;

float kalmanGain[3]={0,0,0};
float estimate[3]={10,10,10};
#define M_PI 3.14159265359


void startton() {
  tone(buzzPin,440,500);
}
void finishton() {
//  tone(buzzPin,500,500);
//  tone(buzzPin,0,500);
  tone(buzzPin,500,500);
  tone(buzzPin,0,500);
  tone(buzzPin,500,500);
  tone(buzzPin,0,500);
  tone(buzzPin,1000,1000);
}
void checkton(int type) { //1 => UP | 2 => DOWN | 3 => LEFT | 4 => RIGHT
  switch (type) {
    case 1:
      tone(buzzPin,2500,30);
    break;
    case 2:
      tone(buzzPin,500,30);
    break;
    case 3:
      tone(buzzPin,1200,30);
    break;
    case 4:
      tone(buzzPin,1700,30);
    break;
  }
}
void errorton() {
  tone(buzzPin,500,200);
  tone(buzzPin,0,200);
  tone(buzzPin,300,600);

}

void connect() {
  //startton();
  //Serial.print("checking wifi...");
  while (WiFi.status() != WL_CONNECTED) {
    //Serial.print(".");
    delay(500);
  }
  
  #ifdef test=false

  while (!client.connected()) {
    //Serial.println("Connecting to MQTT...");
    if (client.connect(mqtt_Name, mqtt_User, mqtt_Password)) {
      //Serial.println("connected to MQTT broker");
    }
    else {
      //Serial.print("failed with state ");
      //Serial.print(client.state());
      delay(500);
    }
  }
  #endif
  //Serial.println("\nconnected!");
}

void callback(char* topic, byte* payload, int length) {
  /*
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
  */
}

void kalmanFilter(){};



void complementaryFilter(){
    float pitchAcc, rollAcc;

    pitchRollYaw[0]+=(gyroData[0]/65.536)*(timeToCheck/1000);
    pitchRollYaw[1]-=(gyroData[1]/65.536)*(timeToCheck/1000);

    int forceMagnitudeApprox = abs(measurement[0])+abs(measurement[1])+abs(measurement[2]);
    if(forceMagnitudeApprox>8192 && forceMagnitudeApprox<32768)
    {
      pitchAcc=atan2f(measurement[1],measurement[2])*180/M_PI;
      pitchRollYaw[0]=pitchRollYaw[0]*0.98+pitchAcc*0.02;

      rollAcc=atan2f(measurement[0],measurement[2])*180/M_PI;
      pitchRollYaw[1]=pitchRollYaw[1]*0.98+rollAcc*0.02;
    }
};



void setup() {
  //Serial.begin(9600);
  //while(!Serial) {}

  pinMode(buzzPin, OUTPUT);
  startton();

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, pass);
#ifdef test=false
  client.setServer(mqtt_Host, mqtt_Port);
  client.setCallback(callback);
#endif
  connect();

  ArduinoOTA.setHostname(ota_Name);
  ArduinoOTA.setPassword(ota_Password);

  ArduinoOTA.onStart([]() {
    String type;
    if (ArduinoOTA.getCommand() == U_FLASH)
      type = "sketch";
    else // U_SPIFFS
      type = "filesystem";

    // NOTE: if updating SPIFFS this would be the place to unmount SPIFFS using SPIFFS.end()
    //Serial.println("Start updating " + type);
  });

  ArduinoOTA.onEnd([]() {
    //Serial.println("\nEnd");
  });

  ArduinoOTA.onProgress([](unsigned int progress, unsigned int total) {
    //Serial.printf("Progress: %u%%\r", (progress / (total / 100)));
  });

  ArduinoOTA.onError([](ota_error_t error) {
    errorton();
    /*
    Serial.printf("Error[%u]: ", error);
    if (error == OTA_AUTH_ERROR) Serial.println("Auth Failed");
    else if (error == OTA_BEGIN_ERROR) Serial.println("Begin Failed");
    else if (error == OTA_CONNECT_ERROR) Serial.println("Connect Failed");
    else if (error == OTA_RECEIVE_ERROR) Serial.println("Receive Failed");
    else if (error == OTA_END_ERROR) Serial.println("End Failed");
    */
  });

  ArduinoOTA.begin();
  //Serial.println("Ready");
  //Serial.print("IP address: ");
  //Serial.println(WiFi.localIP());


  Wire.begin(sdaPin,sclPin);

  // start communication with IMU 
  status = IMU.begin();
  if (status < 0) {
    //Serial.println("IMU initialization unsuccessful");
    //Serial.println("Check IMU wiring or try cycling power");
    //Serial.print("Status: ");
    //Serial.println(status);
    errorton();
    ESP.restart();
  }
  // setting the accelerometer full scale range to +/-4G 
  IMU.setAccelRange(MPU9250::ACCEL_RANGE_4G);
  // setting the gyroscope full scale range to +/-250 deg/s
  IMU.setGyroRange(MPU9250::GYRO_RANGE_250DPS);
  // setting DLPF bandwidth to 5 Hz
  IMU.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_5HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU.setSrd(19);
  finishton();
}

void loop() {
  #ifdef test=false
  client.loop();
  #endif
  ArduinoOTA.handle();

  if (!client.connected()) {
    //Serial.println("Error, connection MQTT lost!");
    errorton();
    connect();
  }

  // publish a message roughly every second.
  if (millis() - lastMillis > timeToCheck) {
    lastMillis = millis();
    IMU.readSensor();


gyroData[0]=IMU.getGyroX_rads()  * (180 / 3.14);
gyroData[1]=IMU.getGyroY_rads()  * (180 / 3.14);
gyroData[2]=IMU.getGyroZ_rads()  * (180 / 3.14);

measurement[0]=IMU.getAccelX_mss();
measurement[1]=IMU.getAccelY_mss();
measurement[2]=IMU.getAccelZ_mss();

complementaryFilter();












  
    
    
    
    integral[0] = measurement[0];
    integral[1] = measurement[1];
    integral[2] = measurement[2];

    mittel[0] = 0;
    mittel[1] = 0;
    mittel[2] = 0;

    flagInitPhase = false;

    for (int coord = 0; coord < 3; coord++) {
      for (int entry = 99; entry != 0; entry--) {
        if (buffer[coord][entry] != NULL) {
          mittel[coord] += buffer[coord][entry];
        }
        else {
          flagInitPhase = true;
        }
      }
    }

    mittel[0] = mittel[0] / 100;    
    mittel[1] = mittel[1] / 100;  
    mittel[2] = mittel[2] / 100;

    delta[0] = (mittel[0] - integral[0]);
    delta[1] = (mittel[1] - integral[1]);
    delta[2] = (mittel[2] - integral[2]);

    float intensity = sqrt(delta[0] * delta[0] + delta[1] * delta[1] + delta[2] * delta[2]);

    trigger = trigger > 0 ? trigger - 1 : trigger;
    if (intensity > threshhold && trigger == 0) {
      if (abs(delta[2]) > abs(delta[1])) {
        if (delta[2] > 0) {
          counterup += 1;
          //counterdown += -1;
          counterleft += -1;
          counterright += -1;
          if (counterup > count) {
            #ifdef test=false
            client.publish("kopf/Move", "LEFT");
            #else
            Serial.println("Up");
            #endif

            checkton(1); //1 => UP
            //Serial.println("Bewegung oben erkannt!");
            counterup = 0;
            counterdown = 0;
            counterleft = 0;
            counterright = 0;
            trigger = triggerCount;
          }
        }
        else {
          //counterup += -1;
          counterdown += +1;
          counterleft += -1;
          counterright += -1;
          if (counterdown > count) {
            #ifdef test=false
            client.publish("kopf/Move", "RIGHT");
            #else
            Serial.println("Down");
            #endif
            checkton(2); //2 => DOWN
            //Serial.println("Bewegung unten erkannt!");
            counterup = 0;
            counterdown = 0;
            counterleft = 0;
            counterright = 0;
            trigger = triggerCount;
          }
        }
      }
      else {
        if (delta[1] > 0) {
          counterup += -1;
          counterdown += -1;
          //counterleft += -1;
          counterright += 1;
          if (counterright > count) {
            #ifdef test=false
            client.publish("kopf/Move", "UP");
            #else
            Serial.println("Right");
            #endif
            checkton(3); //3 => LEFT
            //Serial.println("Bewegung links erkannt!");
            counterup = 0;
            counterdown = 0;
            counterleft = 0;
            counterright = 0;
            trigger = triggerCount;
          }
        }
        else {
          counterup += -1;
          counterdown += -1;
          counterleft += 1;
          //counterright += -1;
          if (counterleft > count) {
            #ifdef test=false
            client.publish("kopf/Move", "DOWN");
            #else
            Serial.println("Left");
            #endif
            checkton(4); //4 => RIGHT
            //Serial.println("Bewegung rechts erkannt!");
            counterup = 0;
            counterdown = 0;
            counterleft = 0;
            counterright = 0;
            trigger = triggerCount;
          }
        }
      }
      counterup = counterup < 0 ? 0 : counterup;
      counterdown = counterdown < 0 ? 0 : counterdown;
      counterleft = counterleft < 0 ? 0 : counterleft;
      counterright = counterright < 0 ? 0 : counterright;
    }
    else {
      for (int coord = 0; coord < 3; coord++) {
        for (int entry = 99 ; entry != 0; entry--) {
          if (buffer[coord][entry-1] != NULL) {  
            buffer[coord][entry] = buffer[coord][entry-1];
          }
        }
        buffer[coord][0] = integral[coord];
      }
    }

  Serial.print(pitchRollYaw[0]);
  Serial.print(",");
  Serial.print(pitchRollYaw[1]);
  Serial.print(",");
  Serial.println(pitchRollYaw[2]);

  }
  /*
    char buffer[6];
    dtostrf(IMU.getAccelX_mss(), 6, 5, buffer);
    client.publish("kopf/AccelX", buffer);
    dtostrf(IMU.getAccelY_mss(), 6, 5, buffer);
    client.publish("kopf/AccelY", buffer);
    dtostrf(IMU.getAccelZ_mss(), 6, 5, buffer);
    client.publish("kopf/AccelZ", buffer);
    dtostrf(IMU.getGyroX_rads(), 6, 5, buffer);
    client.publish("kopf/GyroX", buffer);
    dtostrf(IMU.getGyroY_rads(), 6, 5, buffer);
    client.publish("kopf/GyroY", buffer);
    dtostrf(IMU.getGyroZ_rads(), 6, 5, buffer);
    client.publish("kopf/GyroZ", buffer);
    */
}