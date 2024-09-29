#!/bin/bash
dotnet dev-certs https
dotnet new tool-manifest
dotnet tool install csharpier