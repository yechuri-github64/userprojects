#!/usr/bin/env sh
# Placeholder Gradle wrapper script
if [ -z "$JAVA_HOME" ]; then
  echo "JAVA_HOME is not set"
fi
set -e
BASEDIR=$(dirname "$0")
"$BASEDIR/gradle/wrapper/gradle-wrapper.jar" "$@"
