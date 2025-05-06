#include <Servo.h>
Servo base, s1, s2, s3, s4, set, pinza, v[5];
int posA[5]{ 90, 135, 90, 90, 90 }, posT[5]{};
#define BASE 0
#define S1 1
#define S2 2
#define S3 3
#define PINZA 4
#define TRIG_PIN 9
#define ECHO_PIN 10
String command = "";
bool save_mode = true; 
/*
base:     0 - 180,  pin: 4
servo 1:  0 - 270,  pin: 5
servo 2:  0 - 180,  pin: 6
servo 3:  0 - 180,  pin: 7
pinza:    90 - 130, pin: 8
     (chiusa - aperta)
*/

/*
 *  Char map 
 *  a: base
 *  b: s1
 *  c: s2
 *  d: s3
 *  e: apri
 *  f: chiudi
 *  g: demo1
 *  h: s4 (braccio2)
 *  i: modalità trackbar
 *  l: modalità grid
 *  m: fine grid
 */

void setup()
{
  Serial.begin(9600);
  v[BASE].attach(4);
  v[S1].attach(5);
  v[S2].attach(6);
  v[S3].attach(7);
  v[PINZA].attach(8);
  s4.attach(9);   //servo per braccio 2
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);
}

/*
  TODO:
    - 
*/

void loop()
{
  //readcmd();
  trackbarscroll();
}

void readcmd()
{
  if(Serial.available())
  {
    char mode = Serial.peek();

    if(mode == 'l')
      save_mode = false;

    if(save_mode)
      trackbarscroll();
    else
      processGrid();
  }
}

void processGrid()
{
  while(Serial.available())
  {
    for(int i = 0; i < 5; i++)
    {
      char cmd = Serial.read();
      int cmdInt = Serial.parseInt();
      posT[cmd - 97] = cmdInt;
    }
    spostaBraccioInParallelo();
    Serial.readStringUntil("\n");
    if(Serial.peek() == 'm')
    {
      Serial.read();
      save_mode = true;
      break;
    }
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

void chiudipinza()  //90 chiusa, 130 aperta
{
  v[PINZA].write(90);
  posA[PINZA] = posT[PINZA] = 90;
}

void apripinza()  //90 chiusa, 130 aperta
{
  v[PINZA].write(130);
  posA[PINZA] = posT[PINZA] = 130;
}

void n()
{ 
  v[BASE].write(90);
  v[S1].write(90);
  v[S2].write(90);
  v[S3].write(90);
  chiudipinza();
}

void demo1() 
{
  apripinza();
  posT[BASE] = 45;
  posT[S1] = 200;
  posT[S2] = 0;
  posT[S3] = 25;
  spostaBraccioInParallelo();
  chiudipinza();
}

void trackbarscroll()
{
    while (Serial.available() > 5)
      Serial.readStringUntil('\n');
    delay(10);
    String cmd = Serial.readStringUntil('\n');
    Serial.print(cmd);
    if (cmd.length())
    {
      char s = cmd.charAt(0);
      int an = cmd.substring(1).toInt();
      switch (s)
      {
        case 'a':
          if (an >= 0 && an <= 180)
          {
            v[BASE].write(an);
            posA[BASE] = posT[BASE] = an;
          }
        break;
        case 'b':
          if (an >= 0 && an <= 270)
          {
            v[S1].write(an);
            posA[S1] = posT[S1] = an;
          }
        break;
        case 'c':
          if (an >= 0 && an <= 180)
          {
            v[S2].write(an);
            posA[S2] = posT[S2] = an;
          }
        break;
        case 'd':
          if(an >= 0 && an <= 180)
          {
            v[S3].write(an);
            posA[S3] = posT[S3] = an;
          }
        break;
        case 'e':
          apripinza();
        break;
        case 'f':
          chiudipinza();
        break;
        case 'g':
          demo1();
        break;
      }
    }
    cmd = "";

}
/*
float L1 = 100.0; // mm
float L2 = 100.0;         //lunghezze snodi da definire
float x = getDistance(); // distanza misurata
float z = 0; // oggetto a terra

float d = sqrt(sq(x) + sq(z));
float cos_angle2 = (sq(d) - sq(L1) - sq(L2)) / (2 * L1 * L2);
if (cos_angle2 < -1 || cos_angle2 > 1) return; // punto non raggiungibile

float theta2 = acos(cos_angle2);
float k1 = L1 + L2 * cos(theta2);
float k2 = L2 * sin(theta2);
float theta1 = atan2(z, x) - atan2(k2, k1);
float theta3 = -(theta1 + theta2);

// conversione in gradi
theta1 = degrees(theta1);
theta2 = degrees(theta2);
theta3 = degrees(theta3);

// conversione per servomotori
// s1 ha ingranaggio 1:3, quindi moltiplichiamo per 3
s1.write(theta1 * 3);
s2.write(theta2);
s3.write(theta3);

*/