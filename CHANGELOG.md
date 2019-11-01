1.1.0 Update Scral addresses
1.2.0 Run Module Events in threads so that one Module can not block others, TXTOut now appends to the logfile
1.3.0 Scral now get its config from configfile, lora now want to get battery as [0-9].[0-9]{2} value
1.4.0 Adding Debugmode for finetuning Lora-Trackers
1.4.1 Remove old Wirelesscode and Rename some Classes
1.5.0 Send over Mqtt the new status items and refactoring
1.5.1 Dependencies in debian Packet cleaned
1.6.0 Implement Height in LoraBot
1.6.1 Fixing parsing bug with linebreaks in Lora
1.6.2 Adding a test for LoraBinary
1.7.0 Adding IC800A Lora-Reciever
1.7.1 Fixing binary data transmission & fixing Scral Plugin
1.7.2 Update to local librarys
1.7.3 Parsing new Status format and Panic Package
1.8.0 Add field that indicates when the last gps position was recieved, change all times to UTC
1.8.1 Add Hostname to MQTT, so you can see from witch device the data is recieved
1.8.2 Bugfix, create also an event for sending normal loradata when update panic