# Changelog

## 1.8.3

### New Features

* Add an Output for Panic Events

### Bugfixes

* Implement the new ConnectorDataMqtt

### Changes

* Remove Scral and the Configfile, because its an own Project

## 1.8.2

### New Features

### Bugfixes

* Bugfix, create also an event for sending normal loradata when update panic

### Changes

## 1.8.1

### New Features

* Add Hostname to MQTT, so you can see from witch device the data is recieved

### Bugfixes

### Changes

## 1.8.0

### New Features

* Add field that indicates when the last gps position was recieved, change all times to UTC

### Bugfixes

### Changes

## 1.7.3

### New Features

* Parsing new Status format and Panic Package

### Bugfixes

### Changes

## 1.7.2

### New Features

### Bugfixes

### Changes

* Update to local librarys

## 1.7.1

### New Features

* Fixing binary data transmission & fixing Scral Plugin

### Bugfixes

### Changes

## 1.7.0

### New Features

* Adding IC800A Lora-Reciever

### Bugfixes

### Changes

## 1.6.2

### New Features

### Bugfixes

### Changes

* Adding a test for LoraBinary

## 1.6.1

### New Features

### Bugfixes

* Fixing parsing bug with linebreaks in Lora

### Changes

## 1.6.0

### New Features

* Implement Height in LoraBot

### Bugfixes

### Changes

## 1.5.1

### New Features

### Bugfixes

### Changes

* Dependencies in debian Packet cleaned

## 1.5.0

### New Features

* Send over Mqtt the new status items and refactoring

### Bugfixes

### Changes

## 1.4.1

### New Features

### Bugfixes

### Changes

* Remove old Wirelesscode and Rename some Classes

## 1.4.0

### New Features

* Adding Debugmode for finetuning Lora-Trackers

### Bugfixes

### Changes

## 1.3.0

### New Features

### Bugfixes

### Changes

* Scral now get its config from configfile, lora now want to get battery as [0-9].[0-9]{2} value

## 1.2.0

### New Features

### Bugfixes

### Changes

* Run Module Events in threads so that one Module can not block others, TXTOut now appends to the logfile

## 1.1.0.0

### New Features

### Bugfixes

### Changes

* Update Scral addresses

## 1.0.0.0

### New Features

* First working Version

### Bugfixes

### Changes

