# BatcherLauncher

This is a project designed to launch multiple applications asynchronously based on a set order provided by a text file.

## About

Language: C++.
This was a class project to parse a text file and launch the applications specified in it in groups. Each application in a group would be launched asynchronously and when all are completed the next group would be launched. The applications to launch would be specified in a text file which must be specified as the argument used when launching the main application.

The text file used to launch the applications has each application in a newline delimited format with specific instruction for that application launch in comma delimited format. 
The exact format is as follows:
[Group number], [Application name], [Application arguments]


## Running

This project is made up of cpp and h file and must be complied to an exe to run. It includes a Visual Studio Solution file and can be opened form there. If the text file used to launch the application is not in the root directory the full path to it must be specified. The same is true for applications being launched unless they are on the PATH variable.
