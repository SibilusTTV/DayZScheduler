# DayZScheduler

## Introduction
This is a simple small program that spawned out of the desire to make a version of BEC that is cross-platform.
Currently it can only create a restart schedule and the corresponding messages for any ArmA server, but there will be more features in the future.

## Starting
if you want to start the DayZScheduler, then first download the latest release for your platform and unzip it. After that make sure to use "chmod +x DayZScheduler" on it, if you are on Linux.
On Linux you want to start the program by using this command "./DayZScheduler" and on Windows with this command ".\DayZScheduler".
By default it will use the Config.json. If you want to use another config when starting, you can add "-config [YOURCONFIGNAME].json" as an argument behind it, but it needs to be in the Config folder.

## Functionality
When you first start up the program, it will create a basic config that you definitely should change in the future based on your preferences or circumstances.
The scheduler will be created automatically based upon your settings in the Config.json. The file in the config folder isn't read and only serves the purpose of showing what exactly the scheduler is going to do.
If you need customMessages or commands, please enter them in the Config.json in the CustomMessages array. The default config will have a single message in that array that say that you should join the Discord.

## Disclaimer
This is open-source for a reason. I do this as a passion project, because I couldn't find any alternative on the internet. If you have any suggestions or questions, feel free to ask me on my Discord or wherever else you can find me. Other than that, I hope that you are enjoying my program.
