FROM ubuntu:18.04

COPY Builds/Linux/* ./

# Prometheus metrics
EXPOSE 35556

# Obviously this wont work, need to either build headless, either turn on X11 or something
CMD ./Erutan.x86_64

