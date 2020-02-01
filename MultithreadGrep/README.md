# MultithreadGrep

This is a project designed to search a windows file system for specific file types with specific content.

## About

Language: C++.

This project was designed for multi threaded searches by automatically using as many threads as the CPU has logical cores. The threads asynchronously search though every directory on the path specified for the regex supplied, and optionally file extension(s) supplied, and then the results of the search are printed to the console.


## Running

This project is made up of cpp and h file and must be complied to an exe to run. It includes a Visual Studio Solution file and can be opened form there.
