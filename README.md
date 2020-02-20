# Erutan-unity

Simulating darwinian evolution, fully networked allowing several clients to have a 3D vizualisation.

To be used with [the go server](https://github.com/The-Tensox/Erutan-go)

[![Alt text](docs/example.gif)](https://www.youtube.com/watch?v=OElXIRdJFVs)

![Alt text](docs/example2.gif)

# Installation

TODO: add grpc installation ...

```bash
export UNITY_PROJECT_PATH="/home/louis/Documents/unity/Erutan"
protoc -I $UNITY_PROJECT_PATH/Assets/Protos/Realtime --csharp_out=$UNITY_PROJECT_PATH/Assets/Protos/Realtime \
    $UNITY_PROJECT_PATH/Assets/Protos/Realtime/realtime.proto --grpc_out=$UNITY_PROJECT_PATH/Assets/Protos/Realtime \
    --plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin
```

# Build

```bash
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/tools
export PATH=$PATH:$ANDROID_HOME/platform-tools
```

# Run

```bash
./Erutan.x86_64 & ./Erutan.x86_64 && fg
```
