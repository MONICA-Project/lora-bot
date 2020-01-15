# Binary Format
The data that goes through the air is a binary transmission with the length of 18 Byte.
It contains two parts, a unique counter and the encrypted content.
## General Format
```elm
schema message {
    counter : uint16_t;
    name : uint16_t;
    data : uint8_t[14];
}
```
You need a secret for encrypt the data part in the message. This secret acts like an PSK so it must be 
known to both sides of the communication. This both will be summated together and generate a 
sha2 `crypto = sha2(key << 32 | counter << 16 | name);`.

Now we bitwise XOR our data block with the crypto sha2 value. `data = dataraw ^ crypto`. 
We CUT crypto to the length of data and use the last bytes, in this case the last 14 bytes.

## Data Section
```elm
schema data {
    lan : float; // [0..3]
    lon : float; // [4..7]
    height : uint16_t; // [8..9]
    battery : uint8_t; // [10]
    hdop : uint8_t; // [11]
    status : bitfield : uint8_t { // [12]
        message_type : 1 // 0 = regular, 1 = eventbased
        has_time : 1; // 0 = true, 1 = false
        has_date : 1; // 0 = true, 1 = false
        has_fix : 1; // 0 = true, 1 = false
        satelites : 4; // uint5_t
    }
    sha: uint8_t; // [13]
}
```
* lan: Latitude coded as 4 Byte value.
* lon: Longitude coded as 4 Byte value.
* height: Height * 10; coded as 2 Byte value (so resolution is 10 cm).
* battery: (Battery_Voltage * 100)-230; coded as 1 Byte (so resolution is 0.01V from 2.3V to 4.85V, fits for one LiPo Cell).
* hdop: Hdop * 10; coded as 1 Byte (so resolution is 0.1, reach from 0.0 to 25.5).
* message_type: If the message was send regular, like to faar distance or timebased use 0, if it was userintended like pressing a button use 1 (it will be handled as "panic" message).
* has_time: If the gps module provides a valid timestamp.
* has_date: If the gps module provides a valid date.
* has_fix: If the gps module say there is a gps fix.
* satelites: How many satelites the gps module see (reach from 0 to 15).
* sha: calculate a SHA256 value above the first 17 Byte of the complete packet and use the first byte, so you can maybe see if it was manipulated.
## Examples
We calculate a crypto and recovery it. For this example we use `key = 0xDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEF;`.
So this is now a very bad key.

### Encryption
#### Using counter and name
```elm 
counter = 0x0000;
name = 0x4141;
```
#### Calculate xorkey
```elm 
key = 0xDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEF;
shakey = sha256(key << 32 | counter << 16 | name);
crypto = shakey & 0x000000000000000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFF;
crypro = 0xA6480EEAE95F35B8EEF0F7156358;
```
#### Create Datablock
```elm 
data { 55.234,6.9812,123.4,3.5,0.9,0b11111100,0x03 }
data = 0x004352050012980604D2785AFC03;
```
#### Calculate message
```elm 
datac = data ^ crypto;
datac = 0xA60B5CEFE94DADBEEA228F4F9FFE;
message = counter << 128 | name<<112 | datac;
message = 0x00014141A60B5CEFE94DADBEEA228F4F9FF5B;
```

### Decryption
We get `message = 0x00014141A60B5CEFE94DADBEEA228F4F9F5B;` as message.
#### Reading counter and name
```elm 
counter = message >> 128;
counter = 0x0001;
name = message >> 112;
name = name & 0x0000FFFF;
name = 0x4141;
```
#### Calculating xorkey
```elm 
key = 0xDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEFDEADBEEF;
shakey = sha256(key << 32 | counter << 16 | name);
crypto = shakey & 0x000000000000000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFF;
crypro = 0xA6480EEAE95F35B8EEF0F7156358;
```
### Decrypting Message
```elm 
datac = message & 0x00000000FFFFFFFFFFFFFFFFFFFFFFFFFFFF;
datac = 0xA60B5CEFE94DADBEEA228F4F9F5B;
data = datac ^ crypto;
data = 0x004352050012980604D2785AFC03;
```
#### Decode Datablock
```elm 
data { 55.234,6.9812,123.4,3.5,0.9,0b11111100,0x03 }
```