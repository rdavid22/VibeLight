#pragma once
#include "Bluetooth.cpp"
#include "Wifi.cpp"
#include "EEPROMhandler.cpp"
#include "Leds.cpp"

class Connection
{
    int NUM_LED;
    int UDP_PORT;
    String DEVICE_NUM;
    String DEVICE_NAME;
    String MessageForMain = "";

public:
    ///////////////////////////////////
    BluetoothComm *bl;  //bluetooth
    WiFiComm *wifi;     //wifi
    EEPROMhandler *rom; //EEPROM
    Leds* strip;

    Connection(int numled, int udpport, String devicenum, String devicename,Leds* stripref)
    {
        pinMode(0,INPUT);

        strip = stripref;
        NUM_LED = numled;
        UDP_PORT = udpport;
        DEVICE_NUM = devicenum;

        rom = new EEPROMhandler();

        if (rom->GetAliasStored())
        {
            DEVICE_NAME = rom->GetAlias();
        }
        else
        {
            DEVICE_NAME = devicename;
        }


        wifi = new WiFiComm(UDP_PORT);
        bl = new BluetoothComm(DEVICE_NUM);
      
    }
    void Connect()
    {
        bool connected = false;

        String SSID = "";
        String PW = "";

        if (!rom->GetCredentialsStored())
        {
            bl->Start();
            bl->CollectWifiCredentials(SSID, PW);
        }
        else
        {
            SSID = rom->GetSSID();
            PW = rom->GetPW();
        }
        Serial.println(SSID);
        Serial.println(PW);

        while (!connected)
        {
            if (wifi->Connect(SSID, PW))
            {

                if (bl->AlreadyStarted)
                {
                    bl->ApproveWifiConnection();
                    bl->ShutDown();
                    rom->SetSSID(SSID);
                    rom->SetPW(PW);
                    rom->SetCredentialsStored(true);
                }

                connected = true;
            }
            else
            {
                bl->DenyWifiConnection();
                bl->Start();
                bl->CollectWifiCredentials(SSID, PW);
            }
        }

        WiFi.setSleep(false);
    }
    void Reset()
    {
        rom->SetCredentialsStored(false);
        rom->SetAliasStored(false);
        ESP.restart();
    }
    String MainMessageHandler()
    {

        String message;
        if (MessageForMain != "")
        {
            message = MessageForMain;
            MessageForMain = "";
        }
        else
        {
            message = wifi->Read();
        }

        if (message == "helo")
        {
            wifi->WriteToSender("VibelightController" + DEVICE_NUM + "," + wifi->myIpAdress + "," + NUM_LED + "," + DEVICE_NAME);
            return "";
        }
        if (message.startsWith("start:"))
        {
            wifi->WriteToSender("gotit");
            message.remove(0, 6); 
            return message;
        }
        if (message == "end")
        {
            wifi->WriteToSender("gotit");
            return message;
        }
        if (message.startsWith("lamp:"))
        {
           message.remove(0,5);
           strip->Lamp(message);

            return "";
        }
        if (message.startsWith("rename:"))
        {
            wifi->WriteToSender("gotit");
            message.remove(0, 7);
            DEVICE_NAME = message;
            rom->SetAlias(message);
            rom->SetAliasStored(true);
            

            return "";
        }
        if (message.startsWith("reset") || (digitalRead(0) == LOW))
        {
            wifi->WriteToSender("gotit");
            Reset();
            return "";
        }
         if (message.startsWith("brightness:"))
        {
            message.remove(0, 11);
            strip->SetBrightness(message.toInt());
            return "";
        }
        return "";
    }

    bool MessageHandler(int param[], int NumberOfParam)
    {
        String message = wifi->Read();
        if (message == "helo")
        {
            wifi->WriteToSender("VibelightController" + DEVICE_NUM + "," + wifi->myIpAdress + "," + NUM_LED + "," + DEVICE_NAME);
            return true;
        }
        if (message.startsWith("start:"))
        {
            MessageForMain = message;
            return false;
        }
        if (message == "end")
        {
            MessageForMain = message;
            return false;
        }
        if (message.startsWith("param:"))
        {
            message.remove(0, 6);
            for (int i = 0; i < NumberOfParam; i++)
            {
                int currentParam = message.substring(0, message.indexOf(',')).toInt();
                //Serial.println(currentParam);
                if (currentParam != -1)
                {
                    param[i] = currentParam;
                }
                message.remove(0, message.indexOf(',') + 1);
            }
        }
        if (message.startsWith("lamp:"))
        {
            MessageForMain = message;
            return false;
        }
        if (message.startsWith("rename:"))
        {
            wifi->WriteToSender("gotit");
            message.remove(0, 7);
            DEVICE_NAME = message;
            rom->SetAlias(message);
            rom->SetAliasStored(true);

            return true;
        }
        if (message.startsWith("reset")  || (digitalRead(0) == LOW))
        {
            wifi->WriteToSender("gotit");
            strip->Clear();
            Reset();   
            return false;
        }
        if (message.startsWith("brightness:"))
        {
            message.remove(0, 11);
            strip->SetBrightness(message.toInt());
            return true;
        }
        return true;
    }
};
