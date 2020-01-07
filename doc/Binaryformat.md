# Binary Format
The data that goes through the air is a binary transmission with the length of 18 Byte.
It contains two parts, a unique counter and the encrypted content.
## General Format
```elm
schema message {
	counter : uint16_t;
	data : uint8_t[16];
}
```
You need a secret for encrypt the data part in the message. This secret acts like an PSK so it must be 
known to both sides of the communication. This both will be summated together and generate a 
sha2 `crypto = sha2(key + counter);`.

Now we bitwise XOR our data block with the crypto sha2 value. `data = dataraw ^ crypto`. 
We CUT crypto to the length of data.

## Data Section
```elm
schema data {
    name : uint16_t; // [0..1]
    lan : float; // [2..5]
    lon : float; // [6..9]
    height : uint16_t; // [10..11]
    battery : uint8_t; // [12]
    hdop : uint8_t; // [13]
    status : bitfield : uint8_t { // [14]
        message_type : 1 // 0 = regular, 1 = eventbased
        has_time : 1; // 0 = true, 1 = false
        has_date : 1; // 0 = true, 1 = false
        has_fix : 1; // 0 = true, 1 = false
        satelites : 4; // uint5_t
    }
    counter_crc: uint8_t; // [15]
}
```
## Examples
We calculate a crypto and recovery it. For this example we use `key = 0xDEADBEEF;`.
So this is now a very bad key.

### Encryption
#### Increment Counter
```elm 
counter = 0x0001;
```
#### Calculate xorkey
```elm 
key = 0xDEADBEEF;
shakey = sha256(key + counter);
crypto = shakey & 00000000000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF;
crypro = 0xA7631AC2DFCC0C15E920797C8E25D2E7;
```
#### Create Datablock
```elm 
data { 'A','A',55.234,6.9812,123.4,3.5,0.9,0b11111100,0x97 }
data = 0x4141004352050012980604D2785AFC97;
```
#### Calculate message
```elm 
datac = data ^ crypto;
message = counter << 128 | datac;
message = 0x0001E6221A818DC90C0771267DAEF67F2E70;
```

### Decryption
We get `message = 0x0001E6221A818DC90C0771267DAEF67F2E70;` as message.
#### Reading counter
```elm 
counter = message >> 128;
counter = 0x0001;
```
#### Calculating xorkey
```elm 
key = 0xDEADBEEF;
shakey = sha256(key + counter);
crypto = shakey & 00000000000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF;
crypro = 0xA7631AC2DFCC0C15E920797C8E25D2E7;
```
### Decrypting Message
```elm 
datac = message & 0x0000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF;
datac = 0xE6221A818DC90C0771267DAEF67F2E70;
data = datac ^ crypto;
data = 0x4141004352050012980604D2785AFC97;
```
#### Decode Datablock
```elm 
data { 'A','A',55.234,6.9812,123.4,3.5,0.9,0b11111100,0x97 }
```