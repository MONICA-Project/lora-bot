# Binary Format
The data that goes through the air is a binary transmission with the length of 18 Byte.
It contains two parts, a unique counter and the encrypted content.
```elm
schema message {
	counter : uint16_t;
	data : uint8_t[16];
}
```
You need a secret for encrypt the data part in the message. This secret acts like an PSK so it must be 
known to both sides of the communication. Here we assume `key = 0xDEADBEEF; counter = 0x0001`. 
This both will be summated together and generate a sha2 `crypto = sha2(key + counter);`.

Now we bitwise XOR our data block with the crypto sha2 value. `data = dataraw ^ crypto`. 
We CUT crypto to the length of data.

```elm
schema data {
	name : uint16_t; // [0..1]
	lan : float; // [2..5]
	lon : float; // [6..9]
	height : uint16_t; // [10..11]
	battery : uint8_t; // [12]
	hdop : uint8_t; // [13]
	status : bitfield : uint8_t { // [14]
		has_time : 1; // 0 = true, 1 = false
		has_date : 1; // 0 = true, 1 = false
		has_fix : 1; // 0 = true, 1 = false
		satelites : 5; // uint5_t
	}
	counter_crc: uint8_t; // [15]
}
```
As Example:  
`crypro = 0xa7631ac2dfcc0c15e920797c8e25d2e7`  
```elm 
dataraw { 'A','A',55.234,6.9812,123.4,3.5,0.9,0b11101100,0x97 }
```
`dataraw = 0x4141004352050012980604D2785AEC97`  
`data =    0xE6221A818DC90C0771267DAEF67F3E70`