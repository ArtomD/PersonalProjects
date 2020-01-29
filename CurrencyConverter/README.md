# CurrencyConverter

This is a project designed to compare different world currencies based on real time data as well as use machine learning to recognize various currencies from bills.

## About

Language: Java.

The project had two dropdowns, or spinners, from which to select the current and target currencies. The ratio between the two would then be displayed which was taken live from a public API. 
Additionally you could use image recognition to take photos of bills that would try to set the target spinner automatically.
This project used Firebase to train a model based on three different currencies, Canadian Dollar, Cambodian Riel, and Burmese Kyat.
The Open Camera button would open a fragment that could take and process an image and run it against the model to see if it could detect what currency was being shown. If a currency was detected then it would then select it from the target spinner. 


## Running

To run this project must be complied to an APK. There is a release APK provided in app/release but a new one can be made. The simplest way would to launch and compile it through Android Studio. Testing the image recognition can be done Canadian 5 and 10 Dollar bills, Burmese 100, 200 and 1000 Kyat and Cambodian 100, 500 and 1000 Riel. A sample of test images is provided with the project.
