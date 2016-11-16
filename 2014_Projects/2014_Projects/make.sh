#!/bin/sh
# -*- sh -*-

xbuild ./2014_Projects.csproj
if [ $? = 0 ]; then
	mono ./bin/Debug/2014_Projects.exe
fi
