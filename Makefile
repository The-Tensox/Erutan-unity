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
		go run .

proto:
		protoc --go_out=plugins=grpc:. protobuf/protometry/*.proto --go_opt=paths=source_relative
		protoc --go_out=plugins=grpc:. protobuf/*.proto --go_opt=paths=source_relative
























default: dbuild
