#!/usr/bin/env sh
set -e
DIRNAME="$(cd "$(dirname "$0")" && pwd)"
java -jar "$DIRNAME/gradle/wrapper/gradle-wrapper.jar" "$@"
