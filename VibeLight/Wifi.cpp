#pragma once

// Wifi & IDE
#include <WiFi.h>
#include <WiFiUdp.h>

class WiFiComm
{
    WiFiUDP UDP;
    int UDP_PORT;

    char replyPacket[255];
    char packet[255];

public:
    String senderIp = ""; // legutobbi
    String myIpAdress = "";

    WiFiComm(int port)
    {
        
        UDP_PORT = port;
        WiFi.begin();
        WiFi.setSleep(true); //130 pingrol 3 ra :)
        UDP.begin(UDP_PORT);
    }

    bool Connect(String SSID, String PW)
    {
        for (int i = 0; i < 3; i++) // 3 probalkozas
        {
            int waiting = 0;
            WiFi.disconnect();
            WiFi.begin(SSID.c_str(), PW.c_str()); 
            delay(200);
            while (WiFi.status() != WL_CONNECTED) // várunk a sikeres csatlakozásra timeouttal // megbizhatatlan hulladek
            {                                     // hibasan igaz erteket ad neha

                delay(90);
                Serial.print(".");
                waiting++;
                if (waiting > 40)
                {
                    break;
                }
            }

            if (WiFi.status() == WL_CONNECTED)
            {
                if (waiting = 0 && i == 0) // a megbizhatatlan wifi status miatt kell mert amikor hibasan ad igazat akkor azonnal adja amikor meg waitin es i 0
                {
                    WiFi.disconnect();
                }
                else
                {
                    myIpAdress = WiFi.localIP().toString();
                    Serial.println();
                    Serial.print("Csatlakozva! en IP cimem: ");
                    Serial.println(WiFi.localIP());

                    return true;
                }

                
            }
        }
        return false;
    }
    String Read()
    {
        int packetSize = UDP.parsePacket();
        if (packetSize)
        {
            int len = UDP.read(packet, 255);
            if (len > 0)
            {
                packet[len] = '\0';
            }
            senderIp = UDP.remoteIP().toString();
            return String(packet);
        }
        return "";
    }
    void WriteToSender(String message)
    {
        int hossz = message.length();
        UDP.beginPacket(UDP.remoteIP(), UDP_PORT);
        int i = 0;
        while (i < hossz)
        {
            UDP.write((uint8_t)message[i++]);
        }
        UDP.endPacket();
    }
};
