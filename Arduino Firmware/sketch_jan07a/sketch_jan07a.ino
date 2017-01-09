/*
 * Firmware for Arduino Uno to be used with
 * WingMan for EOS. 
 * 
 * Data format: 7 bytes per message
 * First 6 bytes, 8 bit encoded fader values (f0 - f5)
 * from A0 - A5 on Arduino
 * Remaining byte, 8 binary inputs for buttons on pins
 * 2 - 9 on Arduino
 */

// bytes to store fader values and buttons
//byte d[6] = {0,0,0,0,0,0,B00000000}

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
        Serial.print("WINGMAN P:ARDUINOUNO V:0.0.1 F:6 B:8");
        break;
      default:
        break;
    }
  }
}

void sendData(){
  byte d[6];
  d[0] = map(analogRead(A0), 0, 1023, 0, 255);
  d[1] = map(analogRead(A1), 0, 1023, 0, 255);
  d[2] = map(analogRead(A2), 0, 1023, 0, 255);
  d[3] = map(analogRead(A3), 0, 1023, 0, 255);
  d[4] = map(analogRead(A4), 0, 1023, 0, 255);
  d[5] = map(analogRead(A5), 0, 1023, 0, 255);
  bitWrite(d[6], 0, digitalRead(2));
  bitWrite(d[6], 1, digitalRead(3));
  bitWrite(d[6], 2, digitalRead(4));
  bitWrite(d[6], 3, digitalRead(5));
  bitWrite(d[6], 4, digitalRead(6));
  bitWrite(d[6], 5, digitalRead(7));
  bitWrite(d[6], 6, digitalRead(8));
  bitWrite(d[6], 7, digitalRead(9));
  Serial.write(d,7);
}

