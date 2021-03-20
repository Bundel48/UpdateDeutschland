/*
Advanced_I2C.ino
Brian R Taylor
brian.taylor@bolderflight.com

Copyright (c) 2017 Bolder Flight Systems

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

#include <Wire.h>
#include "MPU9250.h"

#define SDA1 21
#define SCL1 22

#define SDA2 17
#define SCL2 16

#define SDA3 18
#define SCL3 5

TwoWire I2Cone = TwoWire(0);
TwoWire I2Ctwo = TwoWire(1);
TwoWire I2Cthre = TwoWire(2);

// an MPU9250 object with the MPU-9250 sensor on I2C bus 0 with address 0x68
MPU9250 IMU1(I2Cone,0x68);
MPU9250 IMU2(I2Ctwo,0x68);
MPU9250 IMU3(I2Ctwo,0x68);

int status1, status2, status3;

void setup() {

  I2Cone.begin(SDA1,SCL1,400000); // SDA pin 21, SCL pin 22 TTGO TQ
  I2Ctwo.begin(SDA2,SCL2,400000); // SDA2 pin 17, SCL2 pin 16 
  I2Cthre.begin(SDA3,SCL3,400000); // SDA2 pin 17, SCL2 pin 16 
  // serial to display data
  Serial.begin(9600);
  while(!Serial) {}

  // start communication with IMU 
  status1 = IMU1.begin();
  if (status1 < 0) {
    Serial.println("IMU1 initialization unsuccessful");
    Serial.println("Check IMU wiring or try cycling power");
    Serial.print("Status: ");
    Serial.println(status1);
    while(1) {}
  }
  status2 = IMU2.begin();
  if (status2 < 0) {
    Serial.println("IMU2 initialization unsuccessful");
    Serial.println("Check IMU wiring or try cycling power");
    Serial.print("Status: ");
    Serial.println(status2);
    while(1) {}
  }
  status3 = IMU3.begin();
  if (status3 < 0) {
    Serial.println("IMU2 initialization unsuccessful");
    Serial.println("Check IMU wiring or try cycling power");
    Serial.print("Status: ");
    Serial.println(status3);
    while(1) {}
  }
  // setting the accelerometer full scale range to +/-8G 
  IMU1.setAccelRange(MPU9250::ACCEL_RANGE_2G);
  // setting the gyroscope full scale range to +/-500 deg/s
  IMU1.setGyroRange(MPU9250::GYRO_RANGE_250DPS);
  // setting DLPF bandwidth to 20 Hz
  IMU1.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_20HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU1.setSrd(19);

  // setting the accelerometer full scale range to +/-8G 
  IMU2.setAccelRange(MPU9250::ACCEL_RANGE_2G);
  // setting the gyroscope full scale range to +/-500 deg/s
  IMU2.setGyroRange(MPU9250::GYRO_RANGE_250DPS);
  // setting DLPF bandwidth to 20 Hz
  IMU2.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_20HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU2.setSrd(19);

  // setting the accelerometer full scale range to +/-8G 
  IMU3.setAccelRange(MPU9250::ACCEL_RANGE_2G);
  // setting the gyroscope full scale range to +/-500 deg/s
  IMU3.setGyroRange(MPU9250::GYRO_RANGE_250DPS);
  // setting DLPF bandwidth to 20 Hz
  IMU3.setDlpfBandwidth(MPU9250::DLPF_BANDWIDTH_20HZ);
  // setting SRD to 19 for a 50 Hz update rate
  IMU3.setSrd(19);
}

void loop() {
  // read the sensor
  IMU1.readSensor();
  IMU2.readSensor();
  IMU3.readSensor();
  // display the data

/*
  Serial.print("1=>");
  Serial.print(IMU1.getAccelX_mss(),6);
  Serial.print("\t");
  Serial.print(IMU1.getAccelY_mss(),6);
  Serial.print("\t");
  Serial.print(IMU1.getAccelZ_mss(),6);
  Serial.print("\t");
  Serial.print(IMU1.getGyroX_rads(),6);
  Serial.print("\t");
  Serial.print(IMU1.getGyroY_rads(),6);
  Serial.print("\t");
  Serial.print(IMU1.getGyroZ_rads(),6);
  Serial.println("\t");
*/
/*
  Serial.print("2=>");
  Serial.print(IMU2.getAccelX_mss(),6);
  Serial.print("\t");
  Serial.print(IMU2.getAccelY_mss(),6);
  Serial.print("\t");
  Serial.print(IMU2.getAccelZ_mss(),6);
  Serial.print("\t");
  Serial.print(IMU2.getGyroX_rads(),6);
  Serial.print("\t");
  Serial.print(IMU2.getGyroY_rads(),6);
  Serial.print("\t");
  Serial.print(IMU2.getGyroZ_rads(),6);
  Serial.println("\t");
*/
  Serial.print("3=>");
  Serial.print(IMU3.getAccelX_mss(),6);
  Serial.print("\t");
  Serial.print(IMU3.getAccelY_mss(),6);
  Serial.print("\t");
  Serial.print(IMU3.getAccelZ_mss(),6);
  Serial.print("\t");
  Serial.print(IMU3.getGyroX_rads(),6);
  Serial.print("\t");
  Serial.print(IMU3.getGyroY_rads(),6);
  Serial.print("\t");
  Serial.print(IMU3.getGyroZ_rads(),6);
  Serial.println("\t");

  delay(200);
}