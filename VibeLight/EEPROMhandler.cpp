#pragma once
#include <EEPROM.h>

class EEPROMhandler
{
    void writeStringToEEPROM(int addrOffset, const String &strToWrite)
    {
        byte len = strToWrite.length();
        EEPROM.write(addrOffset, len);
        for (int i = 0; i < len; i++)
        {
            EEPROM.write(addrOffset + 1 + i, strToWrite[i]);
        }
        EEPROM.commit();
    }
    String readStringFromEEPROM(int addrOffset)
    {
        int newStrLen = EEPROM.read(addrOffset);
        char data[newStrLen + 1];
        for (int i = 0; i < newStrLen; i++)
        {
            data[i] = EEPROM.read(addrOffset + 1 + i);
        }
        data[newStrLen] = '\0';
        return String(data);
    }

public:
    EEPROMhandler()
    {
        EEPROM.begin(512);
    }

    bool GetCredentialsStored() //Taroltuk e mar a wifi adatokat
    {
        if (EEPROM.read(200) == 1)
        {
            return true;
        }
      return false;      
    }
    bool GetAliasStored() //Taroltuk e mar a nev adatokat
    {
        if (EEPROM.read(400) == 1)
        {        

            return true;
        }
      return false;      
    }
    String GetSSID()
    {
        return readStringFromEEPROM(0); //Wifi ssid kiolvasasa
    }
    
    String GetPW()
    {
        return readStringFromEEPROM(100); //Wifi jelszo kiolvasasa
    }

    void SetSSID(String SSID)
    {
        writeStringToEEPROM(0, SSID); //Wifi ssid felulirasa
    }

    void SetPW(String PW) //Wifi jelszo felulirasa
    {
        writeStringToEEPROM(100, PW);
    }
    void SetAlias(String alias) //Nev felulirasa
    {
       
        writeStringToEEPROM(300, alias);
    }
    String GetAlias() //Nev kiolvasasa
    {
        return readStringFromEEPROM(300);
        
        
    }

    void SetCredentialsStored(bool stored) //ha ture akkor a wifi adatok el lettek tarolva ha nem akkor nem
    {
        if(stored){ EEPROM.write(200, 1);}
        else{ EEPROM.write(200, 0);}
        EEPROM.commit();
    }
    void SetAliasStored(bool stored) //ha true akkor a nev el lett tarolva ha nem akkor nem
    {
        if(stored){ EEPROM.write(400, 1);}
        else{ EEPROM.write(400, 0);}
        EEPROM.commit();
    }
};

