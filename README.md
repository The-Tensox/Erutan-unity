# Erutan-unity

Simulating darwinian evolution, fully networked allowing several clients to have a 3D vizualisation.

To be used with [the go server](https://github.com/The-Tensox/Erutan-go)

[![Alt text](docs/example.gif)](https://www.youtube.com/watch?v=OElXIRdJFVs)

![Alt text](docs/example2.gif)

**Octree ugly visualisation**
![octree](docs/octree.png)

## Installation

TODO: add grpc installation ...

```bash
export UNITY_PROJECT_PATH="/home/louis/Documents/unity/Erutan"
cd Assets
protoc --csharp_out=protobuf/protometry \
    protobuf/protometry/*.proto --grpc_out=protobuf/protometry \
    --plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin
protoc --csharp_out=protobuf \
    protobuf/*.proto --grpc_out=protobuf \
    --plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin
```

## Build

```bash
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/tools
export PATH=$PATH:$ANDROID_HOME/platform-tools
```

```bash
# Doesn't work
~/Unity/Hub/Editor/2019.2.17f1/Editor/Unity -quit -batchmode -logFile stdout.log -projectPath $UNITY_PROJECT_PATH -buildLinux64Player . -executeMethod Builds.BuildLinux
```


## Run

```bash
./Erutan.x86_64 & ./Erutan.x86_64 && fg
```

## Features

- Stateless front-end
- Render what it receives
- Can requests server for updates (tweak timeframe, spawn objects ...)
- PC optimized atm, soon will do the necessary for targetting mobile, maybe web too
