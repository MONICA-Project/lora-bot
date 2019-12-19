# Fraunhofer.Fit.IoT.Bots.LoraBot (Lora-Bot)
<!-- Short description of the project. -->

Program that runs on a raspberry pi and process the Lora traffic from the Lora library. This readme is meant for describing the application.

<!-- A teaser figure may be added here. It is best to keep the figure small (<500KB) and in the same repo -->

## Getting Started
<!-- Instruction to make the project up and running. -->

The project documentation is available on the [Wiki](https://github.com/MONICA-Project/lora-bot/wiki).

## Deployment
<!-- Deployment/Installation instructions. If this is software library, change this section to "Usage" and give usage examples -->

This software can not run in docker, it is made to run on real hardware (raspberry pi) with mono. This came from the [lora](https://github.com/MONICA-Project/lora) library.

## Development
<!-- Developer instructions. -->

* Versioning: Use [SemVer](http://semver.org/) and tag the repository with full version string. E.g. `v1.0.0`

### Prerequisite
This projects depends on some librarys:

#### Internal
* BlubbFish.Utils ([Utils](http://git.blubbfish.net/vs_utils/Utils))
* BlubbFish.Utils.IoT ([Utils-IoT](http://git.blubbfish.net/vs_utils/Utils-IoT))
* BlubbFish.Utils.IoT.Bots ([Bot-Utils](http://git.blubbfish.net/vs_utils/Bot-Utils))
* BlubbFish.Utils.IoT.Connector.Data.Mqtt ([ConnectorDataMqtt](http://git.blubbfish.net/vs_utils/ConnectorDataMqtt))
* BlubbFish.Utils.IoT.Interfaces ([Iot-Interfaces](http://git.blubbfish.net/vs_utils/Iot-Interfaces))
* Fraunhofer.Fit.Iot.Lora ([Lora](https://github.com/MONICA-Project/lora))

#### External
* litjson
* M2Mqtt
* Unosquare Swan
* Unosquare RaspberryIO
* Unosquare WiringPI


### Test

The only way to test this programm is to run it on real hardware.


### Build

Please build it with Visual Studio.

To install dotnet on a Raspi:
* [Download .NET Core Runtime 3.1.0](https://dotnet.microsoft.com/download/dotnet-core/3.1) for ARM32
```bash
wget [download link]
sudo mkdir /usr/share/dotnet
sudo tar zxf dotnet-runtime-3.1.0-linux-arm.tar.gz -C /usr/share/dotnet
rm dotnet-runtime-3.1.0-linux-arm.tar.gz
sudo ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet 
```

## Contributing
Contributions are welcome. 

Please fork, make your changes, and submit a pull request. For major changes, please open an issue first and discuss it with the other authors.

## Affiliation
![MONICA](https://github.com/MONICA-Project/template/raw/master/monica.png)  
This work is supported by the European Commission through the [MONICA H2020 PROJECT](https://www.monica-project.eu) under grant agreement No 732350.
