gRPC C# on Unity
========================

EXPERIMENTAL ONLY
-------------
Support of the Unity platform is currently experimental.

PREREQUISITES
-------------

- Unity 2018.3.5f1

BUILD
-------

- Follow instructions in https://github.com/grpc/grpc/tree/master/src/csharp/experimental#unity to obtain the grpc_csharp_unity.zip
  that contains gRPC C# for Unity. Unzip it under `Assets/Plugins` directory.
- Open the `HelloworldUnity.sln` in Unity Editor.
- Build using Unity Editor.

```bash
UNITY_PROJECT_DIR="/home/louis/Documents/unity/Erutan"
protoc -I $UNITY_PROJECT_DIR/Assets/Protos/Realtime --csharp_out=$UNITY_PROJECT_DIR/Assets/Protos/Realtime \
    $UNITY_PROJECT_DIR/Assets/Protos/Realtime/realtime.proto --grpc_out=$UNITY_PROJECT_DIR/Assets/Protos/Realtime \
    --plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin
```

```bash
# MACOS
/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod ProjectBuilder.PerformBuild
```

```bash
./Erutan.x86_64 & ./Erutan.x86_64 && fg
```