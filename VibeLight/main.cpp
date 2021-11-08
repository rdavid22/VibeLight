#include "Arduino.h"
#include "Connection.cpp"
#include "Effects.cpp"
// 
// FO PARAMETEREK

 const int NUM_LED = 144;
 const String DEVICE_NUM = "#00001";
 String DEVICE_NAME = DEVICE_NUM; // bban az esetben lesz ez a neve ha nincs flashben tarolva nev
 const int UDP_PORT = 4210;

///////////////////////////////////
String StartEffect = "";
Leds *strip;        //ledszalag
Connection *comm;   //wifi,bluettoh,eeprom
Effects *effect;    //effektek
void setup()        // csatlakozás
{
    Serial.begin(9600);

    strip = new Leds(NUM_LED);
    comm = new Connection(NUM_LED,UDP_PORT,DEVICE_NUM,DEVICE_NAME,strip);
    effect = new Effects(strip,comm);


    strip->StartUpdate(); //frissitgetes inditasa és teszt felvillanás
    comm->Connect(); //teljes csatlakozas Connection.cpp ben

}
void loop() //main
{
   
   StartEffect = comm->MainMessageHandler();
   effect->EffectStarter(StartEffect);

 
}
