SET PATH_TO_APP=%~dp0..\Valuator\
SET PATH_TO_RANK_CALCULATOR=%~dp0..\RankCalculator\
SET PATH_TO_EVENTS_LOGGER=%~dp0..\EventsLogger\

cd %PATH_TO_APP%
dotnet build                                                               

cd %PATH_TO_RANK_CALCULATOR%
dotnet build

cd %PATH_TO_EVENTS_LOGGER%
dotnet build