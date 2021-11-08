#pragma once

#include <FastLED.h>
//#define FASTLED_ALLOW_INTERRUPTS 0

#define UPDATE_DELAY 1 // ledek szama

static TaskHandle_t Task;

static bool UpdateRunning = false; //hogy ne lehessen 2x inditani

static void ShowStrip(void *pvParameters) //update fuggveny
{
    while (UpdateRunning)
    {
        vTaskDelay(UPDATE_DELAY);
        FastLED.show();
    }
}

class Leds
{

public:
    int LED_NUM;
    CRGB *leds;

    Leds(int lednum)
    {
        LED_NUM = lednum;
        leds = new CRGB[LED_NUM];
        FastLED.addLeds<WS2812B, 5, GRB>(leds, LED_NUM).setCorrection(TypicalLEDStrip);
        FastLED.setBrightness(255);
       
        Clear();
    }

    void StartUpdate()
    {

        if (UpdateRunning)
        {
            return;
        }

        UpdateRunning = true;

        xTaskCreatePinnedToCore(ShowStrip, "Task", 1024, NULL, 1, &Task, 1); // külön szál indítása ledszalag frissitésére 1 es magon

        Blink();
        Clear();
    }
    void StopUpdate()
    {
        UpdateRunning = false;
        vTaskDelete(NULL);
    }
    void Blink()
    {
        for (int i = 0; i < 70; i++)
        {
            for (int j = 0; j < LED_NUM; j++)
            {
                fadeTowardColor(leds[j], CRGB(0, 0, 140), 3);
            }
            delay(8);
        }

        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < LED_NUM; j++)
            {
                fadeTowardColor(leds[j], CRGB(0, 0, 0), 5);
            }
            delay(3);
        }
       
    }
    void Clear()
    {
        for (int i = 0; i < LED_NUM; i++)
        {
            leds[i] = CHSV(0, 0, 0);
        }
    }
    void SetBrightness(int value)
    {
        FastLED.setBrightness(value);
    }
    void AnimateClear() //suimitott elhalványulas
    {
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < LED_NUM; j++)
            {
                fadeTowardColor(leds[j], CRGB(0, 0, 0), 5);
            }
            delay(1);
        }
    }
    CRGB fadeTowardColor(CRGB &cur, const CRGB &target, uint8_t amount)
    {
        nblendU8TowardU8(cur.red, target.red, amount);
        nblendU8TowardU8(cur.green, target.green, amount);
        nblendU8TowardU8(cur.blue, target.blue, amount);
        return cur;
    }
    void fadeTowardColor(CRGB *L, uint16_t N, const CRGB &bgColor, uint8_t fadeAmount)
    {
        for (uint16_t i = 0; i < N; i++)
        {
            fadeTowardColor(L[i], bgColor, fadeAmount);
        }
    }
    void nblendU8TowardU8(uint8_t &cur, const uint8_t target, uint8_t amount)
    {
        if (cur == target)
            return;

        if (cur < target)
        {
            uint8_t delta = target - cur;
            delta = scale8_video(delta, amount);
            cur += delta;
        }
        else
        {
            uint8_t delta = cur - target;
            delta = scale8_video(delta, amount);
            cur -= delta;
        }
    }
    void Lamp(String parameters)
    {
        int NumberOfParam = 3;
        int param[NumberOfParam];

        for (int i = 0; i < NumberOfParam; i++)
        {
            int currentParam = parameters.substring(0, parameters.indexOf(',')).toInt();
            //Serial.println(currentParam);
            if (currentParam != -1)
            {
                param[i] = currentParam;
            }
            parameters.remove(0, parameters.indexOf(',') + 1);
        }

        // param[0] // TargetHue
        // param[1] // TargetSaturation
        // param[2] // TargetBrightness

        CRGB rgb;
        CHSV chsv(param[0], param[1], param[2]);

        hsv2rgb_rainbow(chsv, rgb);

        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < LED_NUM; j++)
            {
                fadeTowardColor(leds[j], rgb, 5);
            }
        }

    }
};
