//========================== Koo-Koo :D ===================================\\
//========================= Code by Can Ur ================================\\
#include <SoftwareSerial.h>
#include "Laser.h"
#include "Rotary.h"

/* ROTARY ENCODERS */
void doEncoder0();
void doEncoder1();
void doEncoder2();
void doEncoder3();
void doEncoder4();
void doEncoder5();
static float Step = 6.5;
//Interrupt pins on a MEGA:
//2, 3, 18, 19, 20, 21
Rotary r0 = Rotary(1, 2, 4, doEncoder0);
Rotary r1 = Rotary(2, 3, 5, doEncoder1);
Rotary r2 = Rotary(3, 18, 6, doEncoder2);
Rotary r3 = Rotary(4, 19, 7, doEncoder3);
Rotary r4 = Rotary(5, 20, 8, doEncoder4);
Rotary r5 = Rotary(6, 21, 9, doEncoder5);
Rotary rotaryList[] = {r0, r1, r2, r3, r4, r5}; 
int rotaryCount;

/*    LASER   */
Laser l0 = Laser(1, 22);
Laser l1 = Laser(2, 23);
Laser l2 = Laser(3, 24);
Laser l3 = Laser(4, 25);
Laser l4 = Laser(5, 26);
Laser l5 = Laser(6, 27);
Laser laserList[] = {l0, l1, l2, l3, l4, l5};
int laserCount;

void setup() {
  rotaryCount = sizeof(rotaryList) / sizeof(r0);
  laserCount = sizeof(laserList) / sizeof(l0);
   
  Serial.begin(250000);
  while (!Serial);
}

void loop() {
  FetchControls();
}

void FetchControls() {
  String players = "";
  for(int i = 0; i < rotaryCount; i++) players += String(rotaryList[i].encoderPos) + ":" + laserList[i].IsBroken() + "|";
  Serial.println(players);
  Serial.flush();
  delay(42);
}

void doEncoder0() {
  int i = 0;
  rotaryList[i].aState = digitalRead(rotaryList[i].pinA);
  if(rotaryList[i].aState != rotaryList[i].aLastState) {
    if(digitalRead(rotaryList[i].pinB) != rotaryList[i].aState) rotaryList[i].encoderPos += Step;
    else rotaryList[i].encoderPos -= Step;
  }
  rotaryList[i].aLastState = rotaryList[i].aState;
}

void doEncoder1() {
  int i = 1;
  rotaryList[i].aState = digitalRead(rotaryList[i].pinA);
  if(rotaryList[i].aState != rotaryList[i].aLastState) {
    if(digitalRead(rotaryList[i].pinB) != rotaryList[i].aState) rotaryList[i].encoderPos += Step;
    else rotaryList[i].encoderPos -= Step;
  }
  rotaryList[i].aLastState = rotaryList[i].aState;
}

void doEncoder2() {
  int i = 2;
  rotaryList[i].aState = digitalRead(rotaryList[i].pinA);
  if(rotaryList[i].aState != rotaryList[i].aLastState) {
    if(digitalRead(rotaryList[i].pinB) != rotaryList[i].aState) rotaryList[i].encoderPos += Step;
    else rotaryList[i].encoderPos -= Step;
  }
  rotaryList[i].aLastState = rotaryList[i].aState;
}

void doEncoder3() {
  int i = 3;
  rotaryList[i].aState = digitalRead(rotaryList[i].pinA);
  if(rotaryList[i].aState != rotaryList[i].aLastState) {
    if(digitalRead(rotaryList[i].pinB) != rotaryList[i].aState) rotaryList[i].encoderPos += Step;
    else rotaryList[i].encoderPos -= Step;
  }
  rotaryList[i].aLastState = rotaryList[i].aState;
}

void doEncoder4() {
  int i = 4;
  rotaryList[i].aState = digitalRead(rotaryList[i].pinA);
  if(rotaryList[i].aState != rotaryList[i].aLastState) {
    if(digitalRead(rotaryList[i].pinB) != rotaryList[i].aState) rotaryList[i].encoderPos += Step;
    else rotaryList[i].encoderPos -= Step;
  }
  rotaryList[i].aLastState = rotaryList[i].aState;
}

void doEncoder5() {
  int i = 5;
  rotaryList[i].aState = digitalRead(rotaryList[i].pinA);
  if(rotaryList[i].aState != rotaryList[i].aLastState) {
    if(digitalRead(rotaryList[i].pinB) != rotaryList[i].aState) rotaryList[i].encoderPos += Step;
    else rotaryList[i].encoderPos -= Step;
  }
  rotaryList[i].aLastState = rotaryList[i].aState;
}

/*
  int i = 5;
  if(digitalRead(rotaryList[i].pinA) == digitalRead(rotaryList[i].pinB)) rotaryList[i].encoderPos += Step;
  else rotaryList[i].encoderPos -= Step;
*/
