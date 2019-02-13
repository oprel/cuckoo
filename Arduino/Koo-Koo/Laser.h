class Laser {
 public:
  Laser(int num, int pin);
  int IsBroken();

 private:
  int pin;
  int num;
};

Laser::Laser(int num, int pin) {
  this->pin = pin;
  this->num = num;
  pinMode(pin, INPUT_PULLUP);
}

int Laser::IsBroken() {
  return digitalRead(pin);
}

