# Erutan-unity

[![Alt text](docs/example.gif)](https://www.youtube.com/watch?v=OElXIRdJFVs)

# WIP - no clear instructions yet

PREREQUISITES
-------------

- Unity 2018.3.5f1

```bash
UNITY_PROJECT_DIR="/home/louis/Documents/unity/Erutan"
protoc -I $UNITY_PROJECT_DIR/Assets/Protos/Realtime --csharp_out=$UNITY_PROJECT_DIR/Assets/Protos/Realtime \
    $UNITY_PROJECT_DIR/Assets/Protos/Realtime/realtime.proto --grpc_out=$UNITY_PROJECT_DIR/Assets/Protos/Realtime \
    --plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin
```

```bash
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/tools
export PATH=$PATH:$ANDROID_HOME/platform-tools
```

```bash
./Erutan.x86_64 & ./Erutan.x86_64 && fg
```