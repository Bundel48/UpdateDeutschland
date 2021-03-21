
#include "MPU9250.h"
#include <Arduino.h>
#include "SPI.h"
#include <Wire.h>
#include <ESP8266WiFi.h>
#include <ESP8266mDNS.h>
#include <PubSubClient.h>
#include <Buzzer.h>
#include <ArduinoOTA.h>
#include <WiFiUdp.h>

const char* ssid = "GATEWAY49";
const char* pass = "Ready2start";

const char mqtt_Host[] = "135.125.216.231";
const int mqtt_Port = 1883;
const char* mqtt_User = "recomo";
const char* mqtt_Password = "Recomo123%";

WiFiClient net;
PubSubClient client(net);
Buzzer buzzer(3);

MPU9250 IMU(Wire,0x68);

int status;

unsigned long updateMillis = 0;

unsigned long lastMillis = 0;

void startton() {
  buzzer.begin(0);

  buzzer.fastDistortion(NOTE_C3, NOTE_C5);
  buzzer.fastDistortion(NOTE_C5, NOTE_C3);
  
  buzzer.end(1000);
}

void connect() {
  //startton();
  //Serial.print("checking wifi...");
  while (WiFi.status() != WL_CONNECTED) {
    //Serial.print(".");
    delay(500);
  }
  while (!client.connected()) {
    //Serial.println("Connecting to MQTT...");

    if (client.connect("SensBrille", mqtt_User, mqtt_Password ))
    {
      //Serial.println("connected to MQTT broker");
    }
    else
    {
      //Serial.print("failed with state ");
      //Serial.print(client.state());
      delay(500);
    }
  }
  //Serial.println("\nconnected!");
}

void callback(char* topic, byte* payload, int length) {

  //Serial.print("Message received in topic: ");
  //Serial.print(topic);
  //Serial.print("   length is:");
  //Serial.println(length);

  //Serial.print("Data Received From Broker:");
  for (int i = 0; i < length; i++) {
    //Serial.print((char)payload[i]);
  }

  //Serial.println();
  //Serial.println("-----------------------");
  //Serial.println();

}

//void float2Bytes(char bytes_temp[4],float float_variable){ 
//  memcpy(bytes_temp, (unsigned char*) (&float_variable), 4);
//}

void setup() {
  //Serial.begin(9600);
  //while(!Serial) {}

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, pass);

  client.setServer(mqtt_Host, mqtt_Port);
  client.setCallback(callback);

  connect();

  ArduinoOTA.setHostname("brille");

  ArduinoOTA.setPassword("47110815");

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


  Wire.begin(0,2);

  // start communication with IMU 
  status = IMU.begin();
  if (status < 0) {
    //Serial.println("IMU initialization unsuccessful");
    //Serial.println("Check IMU wiring or try cycling power");
    //Serial.print("Status: ");
    //Serial.println(status);
    ESP.restart();
  }
  // setting the accelerometer full scale range to +/-8G 
  IMU.setAccelRange(MPU9250::ACCEL_RANGE_4G);
  // setting the gyroscope full scale range to +/-500 deg/s
  IMU.setGyroRange(MPU9250::GYRO_RANGE_250DPS);
  // setting DLPF bandwidth to 20 Hz
  IMU.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_20HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU.setSrd(19);
}

void loop() {
  client.loop();
  ArduinoOTA.handle();

  if (!client.connected()) {
    //Serial.println("lost connection!");
    connect();
  }


  delay(10);  // <- fixes some issues with WiFi stability
  
  // publish a message roughly every second.
  if (millis() - lastMillis > 50) {
    lastMillis = millis();
    IMU.readSensor();

    char buffer[6];
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

    //client.publish("/IP", WiFi.localIP());

    //IMU.readSensor(); // read the sensor
    // display the data
    /*
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
    Serial.println("\t");
*/
    updateMillis = millis(); // - lastMillis;
    itoa(updateMillis, buffer, 6);
    client.publish("/Update", buffer);

    //Serial.print(IMU.getMagX_uT(),6);
    //Serial.print("\t");
    //Serial.print(IMU.getMagY_uT(),6);
    //Serial.print("\t");
    //Serial.print(IMU.getMagZ_uT(),6);
    //Serial.print("\t");
    //Serial.println(IMU.getTemperature_C(),6);
    //char buffer[7];
    //getStrData(buffer);
    //client.publish("/AccelX", String(1234));
    
  }

}