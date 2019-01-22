#!/bin/bash

dotnet run -p web/ &
dotnet run -p api/ &
dotnet run -p consoleClient/ &