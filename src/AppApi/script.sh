#!/bin/bash
set -e
/app/efbundle --connection "$CONNECTION"
exec dotnet AppApi.dll
