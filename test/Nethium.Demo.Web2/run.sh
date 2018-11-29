#!/bin/sh

mkdir -p /certs/
cp /certs/*.pem /usr/share/ca-certificates/
update-ca-certificates
dockerize $DOCKERIZE_ARGS $@
