#!/bin/sh
# -*- sh -*-

xbuild ./2015_CompetitionScoring.csproj
if [ $? = 0 ]; then
	mono ./bin/Debug/2015_CompetitionScoring.exe
fi
