#define ATTESA 25
int spostaBraccio(int posPartenza, int posFinale, int nMotore)
{
  if (posPartenza < posFinale)
  {
    for (int i = posPartenza; i <= posFinale; i++)
    {
      v[nMotore].write(i);
      delay(ATTESA);
    }
  }
  else
  {
    for (int i = posPartenza; i >= posFinale; i--)
    {
      v[nMotore].write(i);
      delay(ATTESA);
    }
  }
  return posFinale;
}
void spostaBraccioInParallelo() 
{
  int max_delta = 0;
  for(int i = 0; i < 6; i++)
    if(abs(posT[i] - posA[i]) > max_delta)
    {
      max_delta = abs(posT[i] - posA[i]);
      idx_max_delta = i;
    }
    
  int gradoInizio = posA[idx_max_delta];
  int gradoTarget = posT[idx_max_delta];
  int vI[6] = {posA[0], posA[1], posA[2], posA[3], posA[4], posA[5]};
  if(gradoInizio < gradoTarget)
  {
    for(int j = gradoInizio; j < gradoTarget; j++)
    {
      for(int i = 0; i <= 5; i++)
        v[i].write(vI[i]);
      for(int i = 0; i < 6; i++)
        if(posA[i] < posT[i])
          vI[i] = posA[i] + (j - posA[idx_max_delta]) / (float)( posT[idx_max_delta] - posA[idx_max_delta]) * (posT[i] - posA[i]);
        else
          vI[i] = posA[i] - (j - posA[idx_max_delta]) / (float)( posT[idx_max_delta] - posA[idx_max_delta]) * (posA[i] - posT[i]);
      delay(ATTESA);
    }
  }
  else
  {
    for(int j = gradoInizio; j > gradoTarget; j--)
    {
      for(int i = 0; i <= 5; i++)
        v[i].write(vI[i]);
      for(int i = 0; i < 6; i++)
        if(posA[i] < posT[i])
          vI[i] = posA[i] - (j - posA[idx_max_delta]) / (float)( posA[idx_max_delta] - posT[idx_max_delta]) * (posT[i] - posA[i]);
        else
          vI[i] = posA[i] + (j - posA[idx_max_delta]) / (float)( posA[idx_max_delta] - posT[idx_max_delta]) * (posA[i] - posT[i]);
      delay(ATTESA);
    }
  }
  for(int i = 0; i < 6; i++)
    posA[i] = posT[i];
}

void spostaBraccioInParalleloDemo()
{
  Servo s1, s2;
  // s1: 15 -> 90
  // s2: 80 -> 100
  int i = 80, j = 15;
  for (j = 15; j < 90; j++, i = 80 + (j - 15) / 75.0f * 20)  // 'i' da 80 a 100
  {
    s1.write(j);
    s2.write(i);
    delay(ATTESA);
  }

  i = 100; j = 15;
  for (j = 15; j < 90; j++, i = 100 - (j - 15) / 75.0f * 20)  // 'i' da 100 a 80
  {
    s1.write(j);
    s2.write(i);
    delay(ATTESA);
  }

  i = 100; j = 90;  
  for (j = 90; j > 15; j--, i = 80 + (j - 15) / 75.0f * 20)  // 'i' da 100 a 80
  {
    s1.write(j);
    s2.write(i);
    delay(ATTESA);
  }

  i = 80; j = 90;
  for (j = 90; j > 15; j--, i = 100 - (j - 15) / 75.0f * 20)  // 'i' da 80 a 100
  {
    s1.write(j);
    s2.write(i);
    delay(ATTESA);
  }


}

/*
for (i = 80, j = 90; j > 15; j--, i = 100 - (j - 15) / 75.0f * 20)  // 'i' da 80 a 100
  {
    s1.write(j);
    s2.write(i);
    delay(ATTESA);
  } 

   for (j = 15; j < 90; j++, i = 80 + (j - 15) / 75.0f * 20)  // 'i' da 80 a 100
  {
    PS.write(i);
    SPS.write(j);
    delay(25);
  }


void BaseToSx() {
  delay(100);
  int i = 90;
  int j;
  for (j = 90; j < 165; j++, i = 100 - (j - 90) / 75.0f * 20)  //i da 100 a 80
  {
    PD.write(i);
    SPS.write(j);
    delay(10);
  }

  for (j = 90; j < 165; j++, i = 80 + (j - 90) / 75.0f * 20)  //i da 80 a 100
  {
    PD.write(i);
    SPS.write(j);
    delay(10);
  }
}*/
