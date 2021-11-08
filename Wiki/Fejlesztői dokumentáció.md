# Fejlesztői dokumentáció

# Tervezési fázisok & felmerülő problémák a fejlesztés során
## Hardver
- Megfelelő board kiválasztása (ESP-32 Devkit v1, Bluetooth, WiFi)
- LED szalag kiválasztása (WS2812-B)
- Áramkör megtervezése
- IDE kiválasztása (VS, VSCode - Platformio)
- Megfelelő platform kiválasztása (Android, IOS)
- Programozási nyelv és keretrendszer kiválasztása (C#, C++, Xamarin)
- Használni kívánt szenzorok kiválasztása (Orientáció, Gyorsulás, Mikrofon)
- Fejlesztési mérföldkövek meghatározása

## Szoftver
- ESP: Kliens és szerver oldali UDP kommunikáció megtervezése (Több eszköz kezelése, Wifi)
- ESP: Kommunikáció megvalósítása a board és a telefon között (WiFi, Bluetooth)
- ESP: Effektek megtervezése és implementálása
- ESP: Adattárolás megtervezése és megvalósítása (Flash - EEPROM)
- ESP: LED-szalag vezérlésének megvalósítása
- Android, IOS: Kliens és szerver oldali UDP kommunikáció megtervezése (Több eszköz kezelése, Wifi)

- Android, IOS: Kommunikáció megvalósítása a telefon és a board között (WiFi, Bluetooth)
- Android, IOS: UI megtervezése
- Android, IOS: Behind-code elkészítése a UI felülethez
- Android, IOS: Natív funkciók elkészítése
- Android, IOS: Valósidejű paraméterátadás az effektekhez (UDP)
- Android, IOS: Szenzorok implementálása
> Az IOS még nem lett implementálva!

# Áramköri rajz
![image](https://kepkuldes.com/images/f4386fbcf2f829d0da276b56f36f131d.jpg)
# Felhasznált elemek
- ESP32 Board Devkit v1.
- WS2812-B LED strip
- 5V, 2A adapter
- Okostelefon

# Hardver specifikációk
## ESP
- Dual Core 240Mhz Processor
- 4MB Flash EEPROM
- 520Kb SRAM
- WiFi (802.11 g/b/n) module
- Bluetooth (BLE, Serial) module
- Capacitive Sensing GPIO's

## LED 
- 144 LED / m
- Individually addressable leds

## Devices
- Huawei P10
- Huawei MediaPad T3 7.0
- Samsung Galaxy A10
- Samsung Galaxy S10

# Szoftver specifikációk

## Visual Studio Community Edition (2021)
- LiveShare
- Nuget Packages:
- XamarinAndroidService (v. 1.0)
- XamarinEssentials (v. 1.6.1)
- XamarinForms (v. 5.0.0.2012)

## Visual Studio Code
- PlatformIO Extension
- FASTLed library
- WiFi, WiFi UDP library
- EEPROM library
- BluetoothSerial library

# Rendszerkövetelmények
> _Az itt látható rendszerkövetelmények a fejlesztés során felhasznált eszközök alapján lettek meghatározva_

 _Minimum:_
- OS: Android 7.0.0 +
- CPU: 1.3 GHz, 4 magos
- RAM: 2GB
- Storage: 50 MB szabad hely
- Screen: 600 x 1024
- Microphone, Orientation & Acceleration sensor

_Recommended:_
- OS: Android 10.0.0 +
- CPU: 4GHz, 8 magos
- RAM: 4GB
- Storage: 50 szabad hely
- Screen: FullHD, 1080 x 1920
- Microphone, Orientation & Acceleration sensor

# Fejlesztett kódok részletezése (csak a lényeges részleteket tartalmazza!)
## ESP forráskód részletek
#### main.cs
Inicializálja a ledszalagot, amelynek paramétere a ledek mennyisége, majd a megadott paraméterekkel külön szálon frissíti a képkockákat (strip->StartUpdate());.
```c++
void setup()
{
    Serial.begin(9600);
    strip = new Leds(NUM_LED);
    comm = new Connection(NUM_LED,UDP_PORT,DEVICE_NUM,DEVICE_NAME,strip);
    effect = new Effects(strip,comm);
    strip->StartUpdate(); // Frissítgetés indítasa és teszt felvillanás
    comm->Connect(); //teljes csatlakozas Connection.cpp ben
}
```
Elindul a folytonos üzenethallgatás a fő szálon.
```c++
void loop() //main
{
   StartEffect = comm->MainMessageHandler();
   effect->EffectStarter(StartEffect);
}
```
#### Leds.cpp
A ShowStrip() a ledszalag frissítéséért felelős metódus. Azért kell külön szálon indítani, mert a FastLED.Show() túl sok időt vesz igénybe a fő szálon való futattáshoz.
```c++
static void ShowStrip(void *pvParameters) //update fuggveny
{
    while (UpdateRunning)
    {
        vTaskDelay(UPDATE_DELAY);
        FastLED.show();
    }
}
```
### Connection.cpp
Inicializálja a kommunikációhoz szükséges osztályokat és kiolvassa az EEPROM-ból az indításhoz szükséges paramétereket.
```c++
public:
    BluetoothComm *bl;  // Bluetooth
    WiFiComm *wifi;     // WiFi
    EEPROMhandler *rom; // EEPROM
    Leds* strip;

    Connection(int numled,int udpport,String devicenum,String devicename,Leds* stripref)
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
```
A Connect() függvény kezeli a Bluetoothra és Wifire való csatlakozáshoz szükséges metódusokat.
```c++
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
```
### Bluetooth.cpp
A CollectWifiCredentials() függvény várja az "ssid:" és a "pw:"-vel kezdődő üzeneteket. Ha megkapja, eltárolja az SSID és a PW nevű változóban.
```c++
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
                SerialBT.println("got SSID!");
                message.remove(0, 5);
                tempSSID = message;
            }
            if (message.startsWith("pw:"))
            {
                gotPW = true;
                Serial.write("got PW!");
                SerialBT.println("got PW!");
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
```
### Wifi.cpp
A Connect() a megadott paraméterek alapján megpróbál csatlakozni a wifire. Sikeres csatlakozás esertén true értékkel tér vissza.
```c++
    bool Connect(String SSID, String PW)
    {
        for (int i = 0; i < 3; i++) // 3 probalkozas
        {
            int waiting = 0;
            WiFi.disconnect();
            WiFi.begin(SSID.c_str(), PW.c_str()); 
            delay(200);
            while (WiFi.status() != WL_CONNECTED) // várunk a sikeres csatlakozásra timeouttal // 
            {
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
```
UDP üzenetek küldésére és fogadására való függvények:
```c++
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
```
EEPROM-ba való írás és olvasás.
Működése: A writeStringToEEPROM() függvény a paraméter alapján kapott memóriacímbe írja az átadott string hosszát. Ezáltal a readStringFromEEPROM() függvény tudja hány karaktert kell kiolvasni az adott memóriacímtől számítva.
```c++
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
```
### Effects.cpp
Az egyik effekt bemutatása. Az Interactive() effekt lényege, hogy szabadon állíthassuk a ledszalagon felvillanó ledek mennyiségét, pozícióját, szaturációját és fényerejét.

Működése: Inicializáljuk a függvény működéséhez szükséges változókat és a csúszóátlaghoz szükséges tömböket. A csúszóátlag számítás a darabosan beérkező üzenetek kisimításhoz szükséges. Milliszekundumonként történik egy iteráció, melyben a beérkező paramétereket tároljuk el a paraméterekhez tartozó tömbben, majd átlagot számolunk a tömb elemeivel. A kód többi része a megjelenítésért felelős. A paraméterátadás folyamatosan történik a hálózaton keresztül UDP segítségével.
```c++
    void Interactive()
    {
        int smothingValue = 55;
        int delaying = 1;

        int cycle = 0;

        int hueArray[smothingValue];
        int satArray[smothingValue];
        int briArray[smothingValue];
        int posArray[smothingValue];
        int sizArray[smothingValue];

        int smothedHue = 0;
        int smothedSat = 0;
        int smothedBri = 0;
        int smothedPos = 0;
        int smothedSiz = 0;

        int diff = 10000 / strip->LED_NUM; // ezzel kell osztani hogy visszakapjuk a led szamot
        int truePosition = 0;
        int trueSize = 0;
        bool clearing = true;

        int NumberOfParam = 5;
        int param[NumberOfParam];
        bool stay = true;

        param[0] = 130;  // Hue
        param[1] = 255;  // Saturation
        param[2] = 255;  // Brightness
        param[3] = 5000; // Position
        param[4] = 5000; // Size

        while (stay)
        {
            EVERY_N_MILLIS_I(mainTimer, delaying)
            {
                stay = comm->MessageHandler(param, NumberOfParam);

                if (cycle == smothingValue)
                {
                    cycle = 0;
                }

                hueArray[cycle] = param[0];
                satArray[cycle] = param[1];
                briArray[cycle] = param[2];
                posArray[cycle] = param[3];
                sizArray[cycle] = param[4];

                // Sliding AVG
                smothedHue = 0;
                smothedSat = 0;
                smothedBri = 0;
                smothedPos = 0;
                smothedSiz = 0;

                for (int i = 0; i < smothingValue; i++)
                {
                    smothedHue += hueArray[i];
                    smothedSat += satArray[i];
                    smothedBri += briArray[i];
                    smothedPos += posArray[i];
                    smothedSiz += sizArray[i];
                }

                smothedHue /= smothingValue;
                smothedSat /= smothingValue;
                smothedBri /= smothingValue;
                smothedPos /= smothingValue;
                smothedSiz /= smothingValue;
                cycle++;

                truePosition = smothedPos / diff;
                trueSize = smothedSiz / diff;

                if (clearing)
                {
                    for (int i = 0; i < strip->LED_NUM; i++)
                    {
                        if ((i >= (truePosition - trueSize / 2)) && (i <= (truePosition + trueSize / 2)))
                        {
                            strip->leds[i] = CHSV(smothedHue, smothedSat, smothedBri);
                        }
                        else
                        {
                            strip->leds[i] = CHSV(smothedHue, smothedSat, 0);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < strip->LED_NUM; i++)
                    {
                        if ((i >= (truePosition - trueSize / 2)) && (i <= (truePosition + trueSize / 2)))
                        {
                            strip->leds[i] = CHSV(smothedHue, smothedSat, smothedBri);
                        }
                    }
                }
            }
            mainTimer.setPeriod(delaying);
        }
    }
```

## Xamarin.Android és Xamarin.Forms forráskód részletek:
### App.xaml.cs
Az olyan osztályokat, melyekre a program teljes futása alatt szükség van, statikusan deklaráljuk a fájlban. 
```cs
    public partial class App : Application
    {
        public static List<DeviceList> Controllers;
        public static DeviceList SelectedDevice;
        public static MessageHandler Message;
        public static MicrophoneHandler Mic;
        
        public App()
        {
            InitializeComponent();

            Controllers = new List<DeviceList>(); // Osszes VibelightController
            SelectedDevice = new DeviceList();    // Jelenleg kivalasztott VibelightController
            Message = new MessageHandler();       // Uzenet kuldes / eszkoz bekereses
            Mic = new MicrophoneHandler();        // Mikrofon kezelesere leszarmaztatott osztaly
                    
            Controllers =  Message.DiscoverDevices();

            if (Controllers.Count > 0)
            {
                SelectedDevice = Controllers[0];
                MainPage = new NavigationPage(new TabbedPages.Effects()); 
            }
            
            else
            {
                MainPage = new NavigationPage(new SplashScreen());
            }
        }

        protected async override void OnStart()
        {
            await Mic.Initialize();
        }
    }
```
### DeviceList.cs
A DeviceList class az mikrokontrollerek tárolásáért felel. Ezen eszközökről alapvető információkat tárolunk, melyeket később felhasználunk.
```cs
    public class DeviceList
    {
        public string Device;
        public string Alias;
        public string IP;
        public string Led;
        public string ListID;
        public DeviceList(string deviceName, string aliasName, string ipAddress, string ledNumber, string listId)
        {
            Device = deviceName;
            Alias = aliasName;
            IP = ipAddress;
            Led = ledNumber;
            ListID = listId;
        }
        public DeviceList()
        {
          
        }
    }
```
### UDP.cs
A BroadcastAsyncAndGetResponse() a hálózaton található mikrokontrollerek összegyűjtését segíti a MessageHandler osztálynak.
Működése: Létrehozunk egy aszinkron függvényt, hogy ne blokkoljuk az UI felületet. Ezen aszinkron függvény visszatérési értéke az eszközök adatai string típusként. Két paramétert vár, a broadcastolandó üzenet és az időtúllépés. A board mint UDP kliens a 'helo' üzenetet várja. A korábban már deklarált byte[] típusú bufferbe várjuk az adatokat, melyeket később stringkonverzióval átalakítunk a megfelelő alakra. A maximum lehetséges eltárolható eszköz 100 db. Ezt a for ciklusban található max.iterációval tudjuk módosítani.
```cs
public async Task<string> BroadcastAsyncAndGetResponse(string message, int ListenTimeout)
{
            string responses = "";
            await Task.Run(() =>
            {
                byte[] send_buffer = Encoding.ASCII.GetBytes(message);

                Socket.SendTo(send_buffer, new IPEndPoint(IPAddress.Broadcast, Port));

                try
                {
                    for (int i = 0; i < 100; i++) // legfeljebb 100 uzenet és ListenTimeout nyi ido utan kilep
                    {
                        using (UdpClient udpClient = new UdpClient())
                        {
                            udpClient.Client.ReceiveTimeout = ListenTimeout; // legutobbi uzenet utan legfeljebb mennyit varjon
                            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
                            IPEndPoint endpoint = new IPEndPoint(0, 0);
                            byte[] recv_buffer = udpClient.Receive(ref endpoint);
                            responses += Encoding.UTF8.GetString(recv_buffer) + '\n';
                        }
                    }
                }
                catch (Exception EX) { Console.WriteLine(EX); }
            });
            return responses;
}
```

### MessageHandler.cs
Működése: A korábban létrehozott DeviceList típust várja paraméterként, ezáltal tudjuk, hogy hova kell küldeni a message paramétert. Ez egy aszinkron metódus, mely nem blokkolja a főszálat, sikeres üzenetküldés esetén true értékkel tér vissza. A lehetséges üzenettípusoknak megfelelően kezeli a visszaigazolást.
```cs
        public async Task<bool> MessageSendAsync(DeviceList device, string message)
        {
            if (device == null) { return false; }

            if (message.StartsWith("start:") || message.StartsWith("end") || message.StartsWith("rename:") || message.StartsWith("reset"))
            {
                return await ValidationSendAsync(device.IP, message, 3000);
            }

            else
            {
                await SendAsync(device.IP, message);
                return true;
            }
        }
```

### AndroidMicrophone.cs
Inicializálja a MediaRecorder osztályt a mikrofonhasználathoz. A GetAmplitude két lekérés közötti legmagasabb amplitúdót adja vissza.
```cs
        public async Task StartMicrophoneAsync()
        {
            if (AlreadyStarted || !InitializedSuccesfully) { return; }
            AlreadyStarted = true;

            await Task.Run(() =>
            {
                MicrophoneRec = new MediaRecorder();
                MicrophoneRec.SetAudioSource(Source);
                MicrophoneRec.SetOutputFormat(Output);
                MicrophoneRec.SetAudioEncoder(Encoder);
                MicrophoneRec.SetOutputFile("/dev/null");
                MicrophoneRec.Prepare();
                MicrophoneRec.Start();
            });
        }
        public int GetAmplitude()
        {
            if (!AlreadyStarted) { return 0; }

            int Amplitude = MicrophoneRec.MaxAmplitude;
            // 0-10000 integer value
            return Amplitude / 2;
        }
```

### NativeCodes.cs
Összeköttetés az AndroidMicrophone és a Xamarin.Forms kóddal DependencyService-n keresztül az IAndroidMicrophone interfész használatával. A továbbiakban nem térünk ki az összeköttetésekre.
```cs
    public interface IAndroidMicrophone
    {
        Task<bool> InitializeMicrophoneAsync();
        Task StartMicrophoneAsync();
        Task StopMicrophoneAsync();
        int GetAmplitude();
        bool IsInitialized();
        bool IsStarted();
    }

public class Microphone
    {
        public async Task Start()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                await DependencyService.Get<IAndroidMicrophone>().StartMicrophoneAsync();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                // IOS CODE
            }
        }
}
```

### MicrophoneHandler.cs
A StartReading() függvény paraméterként vár egy üzenetet. Kezdésként leellenőrzi, hogy inicializálva lett -e már a Microphone objektum, vagy hogy éppen olvasás történik -e. Ha semelyik feltétel sem teljesül, létrejön egy Foreground Service, mely a mikrofon aktiválásakor indul el és egészen addig fut amíg a felhasználó le nem állítja. Megtörténik a megfelelő értékek kiszámítása MicrophoneSensitivity függvény segítségével (dinamikusan állítja az érzékenységet). Miután megvannak a megfelelő értékeink, az üzenet elküldésre kerül az ESP felé, hogy valós időben módosítsa a ledek tulajdonságait.
```cs
        public async void StartReading(string message)
        {
            if (!IsInitialized() || Reading) { return; }

            simpleService = await SimpleServiceManager.Start<MyService>("Effect Running", "", options);

            long SmoothedValue = 0;
            double SensitivityValue = 20;
            string FormattedSend = "";
            long cycle = 0;
            int[] ValueArray = new int[SmoothingValue];

            Reading = true;

            await Start();

            await Task.Run(async () =>
            {
                while (Reading)
                {
                    await Task.Delay(RefreshRate);
                    FormattedSend = "";
                    string send = message;
                    SmoothedValue = 0;
                    if (cycle == SmoothingValue)
                    {
                        cycle = 0;
                    }
                    for (int i = 0; i < SmoothingValue; i++)
                    {
                        SmoothedValue = SmoothedValue + ValueArray[i];
                    }
                    SmoothedValue = SmoothedValue / SmoothingValue;
                    ValueArray[cycle] = Convert.ToInt32(Convert.ToDouble(Read()) * SensitivityValue);
                    FormattedSend = "";

                    MicrophoneSensitivity(ref SensitivityValue, ref SmoothedValue);
                    while (send.Contains("["))
                    {
                        long minimum = Convert.ToInt64(send.Substring(send.IndexOf("[") + 1, send.IndexOf("_") - send.IndexOf("[") - 1));
                        long maximum = Convert.ToInt64(send.Substring(send.IndexOf("_") + 1, send.IndexOf("]") - send.IndexOf("_") - 1));
                        long difference = Math.Abs(maximum - minimum);
                        long finalValue = 0;
                        if (maximum > minimum)
                        {
                            finalValue = Convert.ToInt64(minimum + (Convert.ToDouble(SmoothedValue) / 10000 * Convert.ToDouble(difference)));
                        }
                        else
                        {
                            finalValue = Convert.ToInt64(minimum - (Convert.ToDouble(SmoothedValue) / 10000 * Convert.ToDouble(difference)));
                        }
                        FormattedSend += send.Substring(0, send.IndexOf("[")) + Convert.ToString(finalValue);

                        send = send.Remove(0, send.IndexOf("]") + 1);
                        if (!send.Contains("["))
                        {
                            FormattedSend += send;
                        }
                    }
                    // Console.WriteLine(FormattedSend)
                    cycle++;
                    App.Message.MessageSendAsync(App.SelectedDevice, FormattedSend);
                }
            });
            simpleService.StopService(removeNotification: true);
            await Stop();
            
        private void MicrophoneSensitivity(ref double sensitivity, ref long amplitude)
        {
            if (amplitude != 0)
            {
                long OverFlowValue = amplitude;

                TargetSensitivity = (TargetAmplitude / Convert.ToDouble(amplitude)) * sensitivity;

                if (amplitude > 10000)
                {
                    amplitude = 10000;
                }
                double AddingValue = Math.Abs(sensitivity - TargetSensitivity);
                AddingValue = AddingValue * 0.001;

                if (sensitivity > TargetSensitivity && sensitivity > 1)
                {
                    sensitivity = sensitivity - AddingValue;
                }
                else if (sensitivity < 100)
                {
                    sensitivity = sensitivity + AddingValue;
                }
            }
        }
        }
```


### ConnectionPage.cs
Csatlakozás gonmb: Kattintáskor a kiválasztott eszközre próbál csatlakozni a bluetooth.SendCredentialsAndGetState függvény, mely ESP oldali sikeres wifi csatlakozás esetén visszatér egy true értékkel. Ebben az esetben lefut a DiscoverDevicesAsync függvény, mely egy DeviceList  típusú listát ad vissza a jelenleg hálózaton lévő eszközökről. Ezután megtörténik az oldalváltás.
```cs
        private async void Connect_Clicked(object sender, EventArgs e)
        {
            await Task.WhenAll(
           PageAnimation.ButtonPress(Connect, 100),
           PageAnimation.Unload(MainLayout, 10)
          );
            Loading.Opacity = 0;
            Loading.IsVisible = true;
            Loading.Text = "Connecting..";
            Loading.TranslationY = -20;

            await Task.WhenAll(
            Loading.TranslateTo(0, 0, 150, Easing.SinInOut),
            Loading.FadeTo(1, 150, Easing.SinInOut));

            if (await bluetooth.SendCredentialsAndGetState(Ssid.Text, Password.Text, 100))
            {
                await Task.Delay(6000);
                await Task.WhenAll(
               Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
               Loading.FadeTo(0, 150, Easing.SinInOut)

                );
                Loading.Text = "Connected";
                await Task.WhenAll(
               Loading.TranslateTo(0, 0, 150, Easing.SinInOut),
               Loading.FadeTo(1, 150, Easing.SinInOut)

                );

                App.Controllers = await App.Message.DiscoverDevicesAsync();

                if (App.Controllers.Count > 0)
                {
                    App.SelectedDevice = App.Controllers[0];
                }
                else
                {
                    App.SelectedDevice = null;
                }

                await Task.WhenAll(
                 Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
                 Loading.FadeTo(0, 150, Easing.SinInOut)
                  );

                 await Navigation.PushAsync(new TabbedPages.Effects(), false);
                this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                GC.Collect();
            }
            else
            {
                await Task.WhenAll(
                      Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
                      Loading.FadeTo(0, 150, Easing.SinInOut)
                       );
                Loading.Text = "Failed";
                await Task.WhenAll(
               Loading.TranslateTo(0, 0, 150, Easing.SinInOut),
               Loading.FadeTo(1, 150, Easing.SinInOut)
             );
                await Task.WhenAll(
                await Task.WhenAll(
                 Loading.TranslateTo(0, -20, 150, Easing.SinInOut),
                 Loading.FadeTo(0, 150, Easing.SinInOut)
                 );
                PageAnimation.Load(MainLayout, 70);
            }
        }
```
### BluetoothAndroid.cs
Üzenetek küldése és fogadása Bluetooth-on keresztül.
```cs
        public async Task Send(string message)
        {
            byte[] toSendArray = new byte[100];
            for (int i = 0; i < message.Length; i++)
            {
                toSendArray[i] = Convert.ToByte(message[i]);
            }
            await MainSocket.OutputStream.WriteAsync(toSendArray, 0, message.Length);
        }

        public async Task<string> Read()
        {
            var Reader = new InputStreamReader(MainSocket.InputStream);
            var Buffer = new BufferedReader(Reader);

            return await Buffer.ReadLineAsync(); 
        }
```

### Fire.cs
Az OrientationSensorChanged() függvény a szenzorból olvassa ki az adatokat. A Microphone_Clicked() event elindítja a mikrofon figyelését.
```cs
async void OrientationSensorChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading;
            int Xaxis = Convert.ToInt32(data.Orientation.X  * 500);
            Xaxis = Math.Abs(Xaxis-300);
            Console.WriteLine(Xaxis);
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1," + Xaxis + ",80,-1,-1,-1,-1,-1");
        }
        
        private async void Microphone_Clicked(object sender, EventArgs e)
        {
            if (OrientationSensorToggled || MicrophoneToggled) { Cooling.IsEnabled = true; Sparkling.IsEnabled = true; Hue1.IsEnabled = true; Hue2.IsEnabled = true; App.Mic.StopReading(); MicrophoneToggled = false; Microphone.BackgroundColor = Color.FromHex("414141"); return; }
            MicrophoneToggled = true;
            Microphone.BackgroundColor = Color.FromHex("645566");

            Cooling.IsEnabled = false;
            Sparkling.IsEnabled = false;
            Hue1.IsEnabled = false;
            Hue2.IsEnabled = false;

            App.Mic.StartReading("param:-1,[400_0],[0_255],-1,[0_255],[0_255],-1,-1");
        }
```