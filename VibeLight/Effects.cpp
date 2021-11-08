#pragma once
#include "Leds.cpp"
#include "Connection.cpp"

class Effects
{
    Leds *strip;
    Connection *comm;

public:
    Effects(Leds *stripref, Connection *commref)
    {
        strip = stripref;
        comm = commref;
    }

    void EffectStarter(String NameOfEffect)
    {

        if (NameOfEffect == "")
        {
            return;
        }
        else if (NameOfEffect == "end")
        {
            strip->AnimateClear();
        }
        else if (NameOfEffect == "rocket")
        {
            Rocket();
        }
        else if (NameOfEffect == "fire")
        {
            Fire();
        }
        else if (NameOfEffect == "gradient")
        {
            Gradient();
        }
        else if (NameOfEffect == "interactive")
        {
            Interactive();
        }
        else if (NameOfEffect == "lightning")
        {
            Lightning();
        }
      
    }

    void Rocket()
    {
        int NumberOfParam = 6;
        int param[NumberOfParam];

        // Kezdoertekek
        param[0] = 5;   // Raketa meret
        param[1] = 128; // Halvanyulas merteke
        param[2] = 70;  // Szín (HUE)
        param[3] = 4;   // Szín eltolás nagysaga
        param[4] = 255; // Kezdo fenyero
        param[5] = 20;  // Kesleltetes

        while (comm->MessageHandler(param, NumberOfParam))
        {
            EVERY_N_MILLIS_I(timer1, param[5])
            {
                static int iDirection = 1;
                static int iPos = 0;

                param[2] += param[3];

                iPos += iDirection;
                if (iPos == (strip->LED_NUM - param[0]) || iPos == 0)
                    iDirection *= -1;

                for (int i = 0; i < param[0]; i++)
                    strip->leds[iPos + i] = CHSV(param[2], 255, param[4]);

                for (int j = 0; j < strip->LED_NUM; j++)
                    if (random(10) > 5)
                        strip->leds[j] = strip->leds[j].fadeToBlackBy(param[1]);
            }
            timer1.setPeriod(param[5]);
        }
    }

    void Fire()
    {
        int NumberOfParam = 8;
        int param[NumberOfParam];

        param[0] = 1;   // Mode, értékek: 1-4, melyik irányból kezdődik a tűz (fent, lent, kétoldalt, kozeprol tukrozve)
        param[1] = 55;  // Cooling, értékek: 1-255,
        param[2] = 120; // Sparkling, értékek: 1-255
        param[3] = 0;   // FirstColorHue, értékek: 1-255
        param[4] = 100; // SecondColorHue, értékek: 1-255
        param[5] = 0;   // ThirdColorHue, értékek: 1-255
        param[6] = 255; // Brightness, értékek: 1-255
        param[7] = 10;  // Delaying, értékek: 1-255

        CRGBPalette16 gPal = CHSVPalette16(CHSV(0, 255, 0), CHSV(param[5], 255, 255), CHSV(param[4], 150, 255), CHSV(param[3], 50, 255));

        while (comm->MessageHandler(param, NumberOfParam))
        {
            EVERY_N_MILLIS_I(timer2, param[7])
            {
                gPal = CHSVPalette16(CHSV(0, 255, 0), CHSV(param[5], 255, param[6]), CHSV(param[4], 150, param[6]), CHSV(param[3], 50, param[6]));

                static byte heat[144];

                for (int i = 0; i < strip->LED_NUM; i++)
                {
                    heat[i] = qsub8(heat[i], random8(0, ((param[1] * 10) / strip->LED_NUM) + 2));
                }

                for (int k = strip->LED_NUM - 3; k > 0; k--)
                {
                    heat[k] = (heat[k - 1] + heat[k - 2] + heat[k - 2]) / 3;
                }

                if (random8() < param[2])
                {
                    int y = random8(7);
                    heat[y] = qadd8(heat[y], random8(160, 255));
                }

                if (param[0] == 1) // fent
                {
                    for (int j = 0; j < strip->LED_NUM; j++)
                    {
                        byte colorindex = scale8(heat[j], 240);
                        strip->leds[j] = ColorFromPalette(gPal, colorindex);
                    }
                }

                else if (param[0] == 2) // lent
                {
                    for (int j = 0; j < strip->LED_NUM; j++)
                    {
                        byte colorindex = scale8(heat[(strip->LED_NUM - 1) - j], 240);
                        strip->leds[j] = ColorFromPalette(gPal, colorindex);
                    }
                }

                else if (param[0] == 3) // kozeprol tukrozve
                {
                    for (int j = (strip->LED_NUM / 2); j < strip->LED_NUM; j++)
                    {
                        byte colorindex = scale8(heat[(j - strip->LED_NUM / 2) + 1], 240);
                        strip->leds[j] = ColorFromPalette(gPal, colorindex);
                    }

                    for (int j = (strip->LED_NUM / 2); j > 0; j--)
                    {
                        byte colorindex = scale8(heat[(((j - (strip->LED_NUM / 2))) * -1) + 1], 240);
                        strip->leds[j] = ColorFromPalette(gPal, colorindex);
                    }
                }

                else if (param[0] == 4) // kétoldalt
                {
                    for (int j = 0; j < (strip->LED_NUM / 2) + 1; j++)
                    {
                        byte colorindex = scale8(heat[j], 240);
                        strip->leds[j] = ColorFromPalette(gPal, colorindex);
                    }

                    for (int j = strip->LED_NUM; j > strip->LED_NUM / 2; j--)
                    {
                        byte colorindex = scale8(heat[strip->LED_NUM - j], 240);
                        strip->leds[j] = ColorFromPalette(gPal, colorindex);
                    }
                }
            }
            timer2.setPeriod(param[7]);
        }
    }

    void fill_gradient_RGB_loopback(CRGB *leds, uint16_t startpos, CRGB startcolor, uint16_t endpos, CRGB endcolor, int maxLeds)
    {
        int rdistance87;
        int gdistance87;
        int bdistance87;

        rdistance87 = (endcolor.r - startcolor.r) << 7;
        gdistance87 = (endcolor.g - startcolor.g) << 7;
        bdistance87 = (endcolor.b - startcolor.b) << 7;

        uint16_t pixeldistance = endpos - startpos;

        int16_t divisor = pixeldistance ? pixeldistance : 1;

        int rdelta87 = rdistance87 / divisor;
        int gdelta87 = gdistance87 / divisor;
        int bdelta87 = bdistance87 / divisor;

        rdelta87 *= 2;
        gdelta87 *= 2;
        bdelta87 *= 2;

        int r88 = startcolor.r << 8;
        int g88 = startcolor.g << 8;
        int b88 = startcolor.b << 8;

        if (endpos > maxLeds)
        {
            endpos = endpos - maxLeds;
        }

        if (startpos > maxLeds)
        {
            startpos = startpos - maxLeds;
        }

        if (endpos < startpos)
        {
            int iter = 0;
            int innerIter = startpos;

            while (pixeldistance > iter)
            {

                leds[innerIter] = CRGB(r88 >> 8, g88 >> 8, b88 >> 8);

                r88 += rdelta87;
                g88 += gdelta87;
                b88 += bdelta87;

                innerIter++;
                if (innerIter >= maxLeds)
                {

                    innerIter = 0;
                }
                iter++;
            }
        }
        else
        {
            for (uint16_t i = startpos; i <= endpos; i++)
            {
                leds[i] = CRGB(r88 >> 8, g88 >> 8, b88 >> 8);

                r88 += rdelta87;
                g88 += gdelta87;
                b88 += bdelta87;
            }
        }
    }

    void Gradient()
    {
        int NumberOfParam = 12;
        int param[NumberOfParam];
        int pushPosition = strip->LED_NUM / 2;

        param[0] = 130; // color 1 (HUE)
        param[1] = 4;  // color 2 (HUE)
        param[2] = 0;  // color 3 (HUE)
        param[3] = 0;  // color 4 (HUE)
        param[4] = 0;  // color 5 (HUE)
        param[5] = 0;   // color 6 (HUE)
        param[6] = 0;   // color 7 (HUE)
        param[7] = 0;   // color 8 (HUE)

        param[8] = 255;  // Saturation
        param[9] = 2;    // Direction
        param[10] = 255; // Brigtness
        param[11] = 5;   // Delaying

        while (comm->MessageHandler(param, NumberOfParam))
        {
            int ColorNumber = 0;

            for (int i = 0; i < 8; i++)
            {
                if (param[i] != 0)
                {
                    ColorNumber++;
                }
            }

            int Colors[ColorNumber];

            int counter = 0;

             for (int i = 0; i < 8; i++)
            {
                if (param[i] != 0)
                {
                    Colors[counter] = param[i];
                    counter++;
                }
            }


            int divided = strip->LED_NUM / ColorNumber;

            EVERY_N_MILLIS_I(timer4, param[11])
            {
                for (int i = 1; i <= ColorNumber; i++)
                {
                    if (i < ColorNumber)
                    {
                        CHSV temphsv(Colors[i - 1], param[8], param[10]);
                        CHSV secondtemphsv(Colors[i], param[8], param[10]);
                        CRGB convertedrgb;
                        CRGB secondconvertedrgb;
                        hsv2rgb_rainbow(temphsv, convertedrgb);
                        hsv2rgb_rainbow(secondtemphsv, secondconvertedrgb);
                        fill_gradient_RGB_loopback(strip->leds, divided * (i - 1) + pushPosition, CRGB(convertedrgb.r, convertedrgb.g, convertedrgb.b), divided * i + pushPosition, CRGB(secondconvertedrgb.r, secondconvertedrgb.g, secondconvertedrgb.b), strip->LED_NUM);
                    }
                }

                CHSV temphsv(Colors[ColorNumber - 1], param[8], param[10]);
                CHSV secondtemphsv(Colors[0], param[8], param[10]);
                CRGB convertedrgb;
                CRGB secondconvertedrgb;
                hsv2rgb_rainbow(temphsv, convertedrgb);
                hsv2rgb_rainbow(secondtemphsv, secondconvertedrgb);

                fill_gradient_RGB_loopback(strip->leds, divided * (ColorNumber - 1) + pushPosition, CRGB(convertedrgb.r, convertedrgb.g, convertedrgb.b), divided * ColorNumber + pushPosition, CRGB(secondconvertedrgb.r, secondconvertedrgb.g, secondconvertedrgb.b), strip->LED_NUM);

                if (param[9] == 2)
                {
                    pushPosition++;
                    if (pushPosition == strip->LED_NUM - 1)
                    {
                        pushPosition = 0;
                    }
                }

                else if (param[9] == 1)
                {
                    pushPosition--;
                    if (pushPosition == 0)
                    {
                        pushPosition = strip->LED_NUM - 1;
                    }
                }
            }
            timer4.setPeriod(param[11]);
        }
    }

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

    void Lightning()
    {
        int NumberOfParam = 2;
        int param[NumberOfParam];

        param[0] = 10; // Chance of lightning
        param[1] = 0;  // Zeus: 1 = true, 0 = false

        int delaying = 50;
        int fromHue = 150;
        int toHue = 230;
        int directionHue = random8(fromHue, toHue);
        int now = 0;
        int prevTime = 0;
        int prevClock = 0;
        int mainHue = 190;
        int mainBrightness = 0;
        bool increasing = true;
        int lightningGenerated = false;
        int lightningStartIndex = 0;
        int lightningEndIndex = 0;
        int lightningMaxBrightness = 0;
        int lightningCurrentBrightness = mainBrightness;
        int lightningDecreasingBrightnessSpeed = 0;
        int lightningIncreasingBrightnessSpeed = 0;
        int lightningCurrentStep = 0;
        int lightningHue = 0;
        int lightningAddSaturation = 0;
        int lightningFadeDirection = 0;
        int lightningLength = 0;
        bool lightningTypeFar = false;
        bool zeus = false;
        while (comm->MessageHandler(param, NumberOfParam))
        {
            if (param[1] == 1)
            {
                zeus = true;
                param[1] = 0;
            }
            if (now - prevTime > 300 || zeus)
            {
                prevTime = now;
                int randomChance = random(param[0]);
                if (randomChance == 0 || zeus)
                {
                    zeus = false;
                    if (!lightningGenerated)
                    {

                        int lightningTypeChance = random8(5);
                        if (lightningTypeChance == 2)
                        {
                            lightningTypeFar = true;
                            increasing = true;
                            lightningGenerated = true;
                            lightningStartIndex = 0;
                            lightningEndIndex = strip->LED_NUM;
                            lightningMaxBrightness = 60;
                            lightningIncreasingBrightnessSpeed = random8(10, 30);
                            lightningDecreasingBrightnessSpeed = random8(40, 50);
                            lightningHue = random8(128, 193);
                            lightningAddSaturation = 0;
                        }
                        else
                        {
                            lightningTypeFar = false;
                            increasing = true;
                            lightningGenerated = true;
                            lightningStartIndex = random16(5, strip->LED_NUM);
                            lightningEndIndex = random16(lightningStartIndex, strip->LED_NUM - 5);
                            lightningMaxBrightness = random8(230, 255);
                            lightningIncreasingBrightnessSpeed = random8(2, 5);
                            lightningDecreasingBrightnessSpeed = random8(10, 15);
                            lightningHue = random8(128, 193);
                            int chanceOfColor = random8(4);
                            if (chanceOfColor == 2)
                            {
                                lightningAddSaturation = random8(120);
                            }
                            else
                            {
                                lightningAddSaturation = 0;
                            }
                            lightningFadeDirection = random8(6);
                        }
                    }
                }
            }

            for (int i = 0; i < strip->LED_NUM; i++) // 1DB frame
            {
                if (lightningGenerated)
                {
                    if (i >= lightningStartIndex && i <= lightningEndIndex)
                    {

                        if (!increasing && !lightningTypeFar)
                        {
                            lightningLength = lightningEndIndex - lightningStartIndex;

                            if (lightningFadeDirection == 1)
                            {
                                strip->leds[i] = CHSV(lightningHue, lightningAddSaturation, lightningCurrentBrightness - (i - lightningLength));
                            }
                            else
                            {
                                strip->leds[i] = CHSV(lightningHue, lightningAddSaturation, lightningCurrentBrightness);
                            }
                        }
                        else
                        {
                            strip->leds[i] = CHSV(lightningHue, lightningAddSaturation, lightningCurrentBrightness);
                        }
                    }
                    else
                    {
                        strip->leds[i] = CHSV(mainHue, 130, mainBrightness);
                    }
                }
                else
                {
                    strip->leds[i] = CHSV(mainHue, 130, mainBrightness);
                }
            }

            if (lightningGenerated)
            {
                if ((lightningMaxBrightness != lightningCurrentBrightness) && increasing)
                {
                    if (lightningCurrentStep == lightningIncreasingBrightnessSpeed)
                    {
                        if (lightningMaxBrightness > lightningCurrentBrightness)
                        {
                            lightningCurrentBrightness++;
                        }
                        lightningCurrentStep = 0;
                    }
                    else if (lightningCurrentStep < lightningIncreasingBrightnessSpeed)
                    {
                        lightningCurrentStep++;
                    }
                }
                else
                {
                    increasing = false;
                    if (mainBrightness != lightningCurrentBrightness)
                    {
                        if (lightningCurrentStep == lightningDecreasingBrightnessSpeed)
                        {
                            if (mainBrightness < lightningCurrentBrightness)
                            {
                                lightningCurrentBrightness--;
                            }
                            lightningCurrentStep = 0;
                        }

                        else if (lightningCurrentStep < lightningDecreasingBrightnessSpeed)
                        {
                            lightningCurrentStep++;
                        }
                    }
                    else
                    {
                        int chance = random8(2);
                        if (chance == 1)
                        {
                            increasing = true;
                            if (!lightningTypeFar)
                            {
                                lightningIncreasingBrightnessSpeed = random8(2, 3);
                                lightningDecreasingBrightnessSpeed = random8(1, 5);
                                lightningHue = random8(128, 193);
                                int chanceOfNewLocation = random(2);
                                if (chanceOfNewLocation == 1)
                                {
                                    lightningStartIndex = random16(5, strip->LED_NUM);
                                    lightningEndIndex = random16(lightningStartIndex, strip->LED_NUM - 5);
                                }
                            }
                            else
                            {
                                lightningIncreasingBrightnessSpeed = random8(5, 15);
                                lightningDecreasingBrightnessSpeed = random8(15, 25);
                            }
                        }
                        else
                        {
                            lightningGenerated = false;
                        }
                    }
                }
            }

            if (now - prevClock > 50)
            {
                prevClock = now;
                if (mainHue == directionHue)
                {
                    directionHue = random8(fromHue, toHue);
                }
                else if (mainHue > directionHue)
                {
                    mainHue--;
                }
                else if (mainHue < directionHue)
                {
                    mainHue++;
                }
            }
            delayMicroseconds(delaying);
            now = millis();
        }
    }

   
};