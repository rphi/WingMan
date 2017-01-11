/*
 * Firmware for Arduino Uno to be used with
 * WingMan for EOS. 
 * 
 * Data format: 7 bytes per message
 * First 6 bytes, 8 bit encoded fader values (f0 - f5)
 * from A0 - A5 on Arduino
 * Remaining byte, 8 binary inputs for buttons on pins
 * 2 - 9 on Arduino
 * 
 * Device ID: ARDUINOUNO-0001
 * NOTE: these ids need to be unique for the
 * ConfigLibrary to work correctly
 */

// bytes to store fader values and buttons
// byte d[20] = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}

byte inByte;

bool changed = false;

void setup() {
  pinMode(2, INPUT_PULLUP);
  pinMode(3, INPUT_PULLUP);
  pinMode(4, INPUT_PULLUP);
  pinMode(5, INPUT_PULLUP);
  pinMode(6, INPUT_PULLUP);
  pinMode(7, INPUT_PULLUP);
  pinMode(8, INPUT_PULLUP);
  pinMode(9, INPUT_PULLUP);
  Serial.begin(115200);
  delay(1000);
}

void loop() {
  if (Serial.available() > 0) {
    // get incoming byte:
    inByte = Serial.read();
    switch (inByte){
      case B01010101: // ascii 'U'
        sendData();
        break;
      case B00111111: // ascii '?'
        Serial.print("WINGMAN P:ARDUINOUNO V:0.0.1 F:16 B:32 I:7c74c2c2-62ec-495c-8e0f-cf8877be322f");
        break;
      default:
        break;
    }
  }
}

void sendData(){
  byte d[20] = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,0};
  d[0] = map(analogRead(A0), 0, 1023, 0, 255);
  d[1] = map(analogRead(A1), 0, 1023, 0, 255);
  d[2] = map(analogRead(A2), 0, 1023, 0, 255);
  d[3] = map(analogRead(A3), 0, 1023, 0, 255);
  d[4] = map(analogRead(A4), 0, 1023, 0, 255);
  d[5] = map(analogRead(A5), 0, 1023, 0, 255);
  bitWrite(d[19], 0, digitalRead(2));
  bitWrite(d[19], 1, digitalRead(3));
  bitWrite(d[19], 2, digitalRead(4));
  bitWrite(d[19], 3, digitalRead(5));
  bitWrite(d[19], 4, digitalRead(6));
  bitWrite(d[19], 5, digitalRead(7));
  bitWrite(d[19], 6, digitalRead(8));
  bitWrite(d[19], 7, digitalRead(9));
  Serial.write(d,20);
}

