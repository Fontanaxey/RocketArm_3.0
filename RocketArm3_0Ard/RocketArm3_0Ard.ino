#include <Servo.h>
Servo base, s1, s2, s3, set, pinza;
#define TRIG_PIN 9
#define ECHO_PIN 10
String command = "";
/*
base:     0 - 180,  pin: 4
servo 1:  0 - 270,  pin: 5
servo 2:  0 - 180,  pin: 6
servo 3:  0 - 180,  pin: 7
pinza:    90 - 130, pin: 8
     (chiusa - aperta)
*/
void setup()
{
  Serial.begin(9600);
  set.attach(3);
  base.attach(4);
  s1.attach(5);
  s2.attach(6);
  s3.attach(7);
  pinza.attach(8);
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);
}

void loop()
{
  //n();
  //processCommand();
  //getDistance();
  //chiudipinza();
  //set.write(90);
}

void processCommand()
{
  if (Serial.available())
  {
    while (Serial.available() > 5)
      Serial.readStringUntil('\n');
    command = Serial.readStringUntil('\n');
    Serial.println(command);
    if (command.length() > 1)
    {
      char s = command.charAt(0);
      int an = command.substring(1).toInt();
      if (an >= 0 && an <= 180)
      {
        switch (s)
        {
          case 'a':
            base.write(an);
            break;
          case 'b':
            s1.write(an);
            break;
          case 'c':
            s2.write(an);
            break;
          case 'd':
            s3.write(an);
            break;
          case 'e':
            pinza.write(an);
            break;
          case 'f':
            demo1();
            break;
          case 'g':
          int a = 0;
          while (a < 5)
          {
            while (Serial.available() > 5)
              Serial.readStringUntil('\n');
            command = Serial.readStringUntil('\n');
            if (command.length() > 1)
            {
              char s = command.charAt(0);
              int an = command.substring(1).toInt();
              switch (a)
              {
                case 0:
                  base.write(an);
                  break;
                case 1:
                  s1.write(an);
                  break;
                case 2:
                  s2.write(an);
                break;
                case 3:
                  s3.write(an);
                  break;
                case 4:
                  pinza.write(an);
                  break;
              }
            }
            a++;
          }
          break;
        }
      }
    }
    command = "";
  }
}

float getDistance()
{
  digitalWrite(TRIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG_PIN, LOW);
  long duration = pulseIn(ECHO_PIN, HIGH, 30000);
  if (duration == 0)
    return -1;
  float distance = duration * 0.034 / 2;
  Serial.println(distance);
  return distance;
}

void stringipinza()
{
  pinza.write(87);
}

void chiudipinza() //90 chiusa, 130 aperta
{
  float dista = getDistance();
  if (dista > 0 && dista < 10)
    pinza.write(90);
}

void n()
{
  base.write(90);
  s1.write(90);
  s2.write(90);
  s3.write(90);
  chiudipinza();
}

void demo1()
{
  base.write(45);
  s1.write(145);
  s2.write(40);
  s3.write(35);
  chiudipinza();
}