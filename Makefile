UNITY_PROJECT_PATH ?= $(HOME)/Documents/unity/Erutan
NS ?= erutan
VERSION ?= 1.0.0
EDITOR_PATH ?= $(HOME)/Unity/Hub/Editor/2019.2.17f1/Editor/Unity
IMAGE_NAME ?= erutan-unity
CONTAINER_NAME ?= erutan-unity
CONTAINER_INSTANCE ?= default
GRPC_CSHARP_PLUGIN_PATH ?= /usr/local/bin


.PHONY: help build build-headless run proto docker-build docker-run
help:
		@echo ''
		@echo 'Usage: make [TARGET]'
		@echo 'Targets:'
		@echo '  install    	blabla'
		@echo ''


build:
	rm -rf Builds/Linux
	$(EDITOR_PATH) -batchmode -quit -logFile /tmp/erutan_unity_build.log -projectPath $(UNITY_PROJECT_PATH) \
		-buildLinux64Player $(UNITY_PROJECT_PATH)/Builds/Linux -executeMethod Editor.Builds.BuildLinux \
		-silent-crashes -headless 

build-headless:
	rm -rf Builds/Linux
	$(EDITOR_PATH) -batchmode -quit -logFile /tmp/erutan_unity_build.log -projectPath $(UNITY_PROJECT_PATH) \
		-buildLinux64Player $(UNITY_PROJECT_PATH)/Builds/Linux -executeMethod Editor.Builds.BuildLinuxHeadless \
		-silent-crashes -headless


run:
	./Builds/Linux/Erutan.x86_64

proto:
	# TODO: fix this need for cd Assets later :)
	cd Assets; protoc --csharp_out=protobuf/protometry protobuf/protometry/*.proto --grpc_out=protobuf/protometry \
		--plugin=protoc-gen-grpc=$(GRPC_CSHARP_PLUGIN_PATH)/grpc_csharp_plugin
	cd Assets; protoc --csharp_out=protobuf protobuf/*.proto --grpc_out=protobuf \
		--plugin=protoc-gen-grpc=$(GRPC_CSHARP_PLUGIN_PATH)/grpc_csharp_plugin

docker-build:
	# FIXME: make build always say "failed to build" (but succeed) it break the make pipeline
	# also u need to press enter -.-
	# make proto
	# make build
	docker build -t $(NS)/$(IMAGE_NAME):$(VERSION) -f Dockerfile .

docker-run:
	docker run --rm --name $(CONTAINER_NAME)-$(CONTAINER_INSTANCE) $(PORTS) $(VOLUMES) $(ENV) $(NS)/$(IMAGE_NAME):$(VERSION)

default: build
