
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

void setup() {
  Serial.begin(9600);
  while(!Serial) {}

  WiFi.begin(ssid, pass);

  connect();
  client.setServer(mqtt_Host, mqtt_Port);
  client.setCallback(callback);

  while (!client.connected()) {
    Serial.println("Connecting to MQTT...");

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
  }


  Wire.begin(0,2);
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
  IMU.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_20HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU.setSrd(19);
  
}

//void getStrData(char* buffer) {
//    dtostrf(IMU.getAccelX_mss(), 6, 5, buffer);
//}

void loop() {
  client.loop();

  if (!client.connected()) {
    Serial.println("error");
    //connect();
  }


  delay(10);  // <- fixes some issues with WiFi stability
  
  // publish a message roughly every second.
  if (millis() - lastMillis > 100) {
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