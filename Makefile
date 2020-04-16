UNITY_PROJECT_PATH ?= $(HOME)/Documents/unity/Erutan
NS ?= erutan
VERSION ?= 1.0.0
EDITOR_PATH ?= $(HOME)/Unity/Hub/Editor/2019.2.17f1/Editor/Unity


.PHONY: help build run proto
help:
		@echo ''
		@echo 'Usage: make [TARGET]'
		@echo 'Targets:'
		@echo '  install    	blabla'
		@echo ''


build:
	$(EDITOR_PATH) -quit -batchmode -logFile /tmp/erutan_unity_build.log -projectPath $(UNITY_PROJECT_PATH) \
		-buildLinux64Player $(UNITY_PROJECT_PATH)/Builds/Linux -executeMethod Builds.BuildLinux


run:
	./Builds/Linux/Erutan.x86_64

proto:
	# TODO: fix this need for cd Assets later :)
	cd Assets; protoc --csharp_out=protobuf/protometry protobuf/protometry/*.proto --grpc_out=protobuf/protometry \
		--plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin
	cd Assets; protoc --csharp_out=protobuf protobuf/*.proto --grpc_out=protobuf \
		--plugin=protoc-gen-grpc=/usr/local/bin/grpc_csharp_plugin

default: dbuild
