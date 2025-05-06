int trapano = 13;
void setup()
{
  Serial.begin(9600);
  pinMode(trapano, OUTPUT);
}

void loop()
{
  if(Serial.available())
  {
    int c = Serial.read();
    if( c == 'a')
      digitalWrite(trapano, HIGH);
    if(c == 'b')
      digitalWrite(trapano, LOW);
  }
}