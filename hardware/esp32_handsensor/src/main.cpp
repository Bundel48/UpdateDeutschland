
#include "MPU9250.h"
#include <Arduino.h>
#include "SPI.h"
#include <Wire.h>
#include <WiFi.h>
#include <ESPmDNS.h>
#include <WiFiUdp.h>
#include <PubSubClient.h>
#include <ArduinoOTA.h>
#include <Tone32.h>
#include <SmartLeds.h>

const char* ssid = "GATEWAY49";
const char* pass = "Ready2start";

const char mqtt_Host[] = "135.125.216.231";

const int mqtt_Port = 1883;
const char* mqtt_User = "recomo";
const char* mqtt_Password = "Recomo123%";
const char* mqtt_Name = "hand";
#define MQTT_PREF "hand"

const char* ota_Name = "hand";
const char* ota_Password = "47110815";

#define LEDPIN 27
#define BUZZPIN 14
#define VIBPIN 12

//!!!!!!!!!!!!!DIE HIER SIND ZUM TUNEN DER ERKENNUNG!!!!!!!!!!!!!!!!!!!!

const int count = 6;   //WIE LANGE DIE BEWEGUNG ANHALTEN MUSS
const int triggerCount = 150;    //WIE LANGE KEINE NEUEN EVENTS ERKANNT WERDEN
const int threshhold = 9;     //WIE STARK DIE BEWEGUNG SEIN MUSS
const int countUp = 6;
const int countRight = 4;

WiFiClient net;
PubSubClient client(net);
SmartLed leds( LED_SK6812, 1, LEDPIN, 0, DoubleBuffer );
MPU9250 IMU(Wire,0x68);

int status;

unsigned long lastMillis = 0;
float downAcc[3];
float angle[3];
float accelInt[3];
float delta[3];
float buffer[3][100];
int trigger = 0;
int counterup = 0;
int counterdown = 0;
int counterleft = 0;
int counterright = 0;
float mittel[3] = {0,0,0};
bool flagInitPhase = true;

void startton() {
  leds[0] = Rgb{ 100, 100, 0 };
  tone(BUZZPIN,440,500,0);
}
void finishton() {
  leds[0] = Rgb{ 0, 100, 0 };
//  tone(BUZZPIN,500,500);
//  tone(BUZZPIN,0,500);
  tone(BUZZPIN,500,200,0);
  tone(BUZZPIN,0,200,0);
  tone(BUZZPIN,500,200,0);
  tone(BUZZPIN,0,200,0);
  tone(BUZZPIN,1000,1000,0);
}
void checkton(int type) { //1 => UP | 2 => DOWN | 3 => LEFT | 4 => RIGHT
  leds[0] = Rgb{ 0, 0, 100 };
  digitalWrite(VIBPIN,HIGH);
  switch (type) {
    case 1:
      tone(BUZZPIN,2500,30,0);
    break;
    case 2:
      tone(BUZZPIN,500,30,0);
    break;
    case 3:
      tone(BUZZPIN,1200,30,0);
    break;
    case 4:
      tone(BUZZPIN,1700,30,0);
    break;
  }
  delay(100);
  leds[0] = Rgb{ 0, 100, 0 };
  digitalWrite(VIBPIN,LOW); 
}
void errorton() {
  leds[0] = Rgb{ 100, 0, 0 };
  tone(BUZZPIN,500,200,0);
  tone(BUZZPIN,0,200,0);
  tone(BUZZPIN,300,600,0);

}

void connect() {
  startton();
  Serial.print("checking wifi...");
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);
  }
  while (!client.connected()) {
    Serial.println("Connecting to MQTT...");

    if (client.connect(mqtt_Name, mqtt_User, mqtt_Password)) {
      Serial.println("connected to MQTT broker");
    }
    else {
      Serial.print("failed with state ");
      Serial.print(client.state());
      delay(500);
    }
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

void setup() {
  Serial.begin(115200);
  while(!Serial) {}
  pinMode(VIBPIN, OUTPUT);

  WiFi.begin(ssid, pass);
  client.setServer(mqtt_Host, mqtt_Port);
  client.setCallback(callback);

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
    Serial.println("Start updating " + type);
  });

  ArduinoOTA.onEnd([]() {
    Serial.println("\nEnd");
  });

  ArduinoOTA.onProgress([](unsigned int progress, unsigned int total) {
    Serial.printf("Progress: %u%%\r", (progress / (total / 100)));
  });

  ArduinoOTA.onError([](ota_error_t error) {
    errorton();
    Serial.printf("Error[%u]: ", error);
    if (error == OTA_AUTH_ERROR) Serial.println("Auth Failed");
    else if (error == OTA_BEGIN_ERROR) Serial.println("Begin Failed");
    else if (error == OTA_CONNECT_ERROR) Serial.println("Connect Failed");
    else if (error == OTA_RECEIVE_ERROR) Serial.println("Receive Failed");
    else if (error == OTA_END_ERROR) Serial.println("End Failed");
    
  });

  ArduinoOTA.begin();
  Serial.println("Ready");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  Wire.begin(16,4);
  // serial to display data

  // start communication with IMU 
  status = IMU.begin();
  if (status < 0) {
    Serial.println("IMU initialization unsuccessful");
    Serial.println("Check IMU wiring or try cycling power");
    Serial.print("Status: ");
    Serial.println(status);
    errorton();
    ESP.restart();
  }
  // setting the accelerometer full scale range to +/-8G 
  IMU.setAccelRange(MPU9250::ACCEL_RANGE_4G);
  // setting the gyroscope full scale range to +/-500 deg/s
  IMU.setGyroRange(MPU9250::GYRO_RANGE_250DPS);
  // setting DLPF bandwidth to 20 Hz
  IMU.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_10HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU.setSrd(19);
  finishton();
}

///////////////////////////////////////////////////////////////////////
//  X UND Y ACHSE SIND VERTAUSCHT, BITTE NICHT "FIXEN", DAS SOLL SO  //
///////////////////////////////////////////////////////////////////////

void loop() {
  client.loop();
  ArduinoOTA.handle();

  if (!client.connected()) {
    //Serial.println("Error, connection MQTT lost!");
    errorton();
    connect();
  }
  IMU.readSensor();
  
  // publish a message roughly every second.
  if (millis() - lastMillis > 10) {
    lastMillis = millis();
    
    IMU.readSensor();
    angle[0] = (angle[0] + IMU.getGyroX_rads() * 0.1 * 180 / 3.14); 
    angle[1] = (angle[1] + IMU.getGyroY_rads() * 0.1 * 180 / 3.14);
    angle[2] = (angle[2] + IMU.getGyroZ_rads() * 0.1 * 180 / 3.14);
    
    accelInt[0] = IMU.getAccelX_mss();
    accelInt[1] = IMU.getAccelY_mss();
    accelInt[2] = IMU.getAccelZ_mss();

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

    delta[0] = (mittel[0] - accelInt[0]);
    delta[1] = (mittel[1] - accelInt[1]);
    delta[2] = (mittel[2] - accelInt[2]);

    float intensity = sqrt(delta[0] * delta[0] + delta[1] * delta[1] + delta[2] * delta[2]);

    trigger = trigger > 0 ? trigger - 1 : trigger;
    if (!flagInitPhase && intensity > threshhold && trigger == 0) {
      if (abs(delta[2]) > abs(delta[1])) {
        if (delta[2] > 0) {
          counterup += 1;
          //counterdown += -1;
          counterleft += -1;
          counterright += -1;
          if (counterup > countUp) {
            client.publish("hand/Move", "UP");
            Serial.println("Bewegung oben erkannt!");
            checkton(1); //1 => UP
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
            client.publish("hand/Move", "DOWN");
            Serial.println("Bewegung unten erkannt!");
            checkton(2); //2 => DOWN
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
          if (counterright > countRight) {
            client.publish("hand/Move", "RIGHT");
            Serial.println("Bewegung rechts erkannt!");
            checkton(4); //4 => RIGHT
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
            client.publish("hand/Move", "LEFT");
            Serial.println("Bewegung links erkannt!");
            checkton(3); //3 => LEFT
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
        buffer[coord][0] = accelInt[coord];
      }
    }
  }
  /*
    char buffer[6];
    dtostrf(IMU.getAccelX_mss(), 6, 5, buffer);
    client.publish("/"+mqtt_Pref+"/AccelX", buffer);
    dtostrf(IMU.getAccelY_mss(), 6, 5, buffer);
    client.publish("/"+mqtt_Pref+"/AccelY", buffer);
    dtostrf(IMU.getAccelZ_mss(), 6, 5, buffer);
    client.publish("/"+mqtt_Pref+"/AccelZ", buffer);
    dtostrf(IMU.getGyroX_rads(), 6, 5, buffer);
    client.publish("/"+mqtt_Pref+"/GyroX", buffer);
    dtostrf(IMU.getGyroY_rads(), 6, 5, buffer);
    client.publish("/"+mqtt_Pref+"/GyroY", buffer);
    dtostrf(IMU.getGyroZ_rads(), 6, 5, buffer);
    client.publish("/"+mqtt_Pref+"/GyroZ", buffer);
  */
}
