#pragma once
#include "BluetoothSerial.h"

class BluetoothComm
{

    BluetoothSerial SerialBT;

    String message = "";
    // Bluetooth setup
    String DEVICE_NUM;

#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

public:

    bool AlreadyStarted = false;

    BluetoothComm(String devicenum)
    {
        DEVICE_NUM = devicenum;
        pinMode(OUTPUT,2);
        digitalWrite(2,LOW);
    }
    void Start()
    {   
       
        if (!AlreadyStarted)
        {
            Serial.println("bl starting");
            digitalWrite(2,HIGH);
            SerialBT.end();
          
            SerialBT.begin("VibelightController" + DEVICE_NUM);
           
            AlreadyStarted = true;
            Serial.println("bl started");
        }
    }
    void ApproveWifiConnection()
    {
      delay(1000);
      SerialBT.println("connected");
      delay(2000);
    }
     void DenyWifiConnection()
    {
      
      SerialBT.println("failed");
   
    }

    bool CollectWifiCredentials(String &SSID, String &PW)
    {
        Serial.println("credential collect started");

        String tempSSID = "";
        String tempPW = "";

        bool gotCredentials = false;
        bool gotSSID = false;
        bool gotPW = false;

        while (!gotCredentials)
        {
            if (SerialBT.available())
            {
                message = SerialBT.readString();
            }
            if (message.startsWith("ssid:"))
            {
                gotSSID = true;

                Serial.write("got SSID!");
                SerialBT.println("got SSID!"); // xamarin varja ezt az uzenetet
                message.remove(0, 5);
                tempSSID = message;
            }
            if (message.startsWith("pw:"))
            {
                gotPW = true;

                Serial.write("got PW!");
                SerialBT.println("got PW!"); // xamarin varja ezt az uzenetet
                message.remove(0, 3);
                tempPW = message;
            }
            if (gotSSID && gotPW)
            {
                String WIFI_SSID = "";

                for (int i = 0; i <= tempSSID.length(); i++)
                {
                    WIFI_SSID = WIFI_SSID + tempSSID[i];
                }
                String WIFI_PASS = "";
                for (int i = 0; i <= tempPW.length(); i++)
                {
                    WIFI_PASS = WIFI_PASS + tempPW[i];
                }
                SSID = WIFI_SSID;
                PW = WIFI_PASS;

                gotCredentials = true;
                 Serial.println("credential collect ended true");
                return true;
            }
        }
         Serial.println("credential collect ended false");
        return false;
    }
    void ShutDown()
    {
        digitalWrite(2,LOW);
        SerialBT.end();
        AlreadyStarted = false;
    }
};
