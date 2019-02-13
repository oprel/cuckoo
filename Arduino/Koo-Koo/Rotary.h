class Rotary {
  public:
    int pinA, pinB;
    volatile int encoderPos;
    volatile int aState, aLastState;
    int num;
    Rotary(int num, int A, int B, void* func);
};

Rotary::Rotary(int num, int A, int B, void* onChange) {
  this->num = num;
  this->pinA = A;
  this->pinB = B;
  pinMode(pinA, INPUT_PULLUP);
  pinMode(pinB, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(pinA), onChange, CHANGE);
}

