# FindWifesPhone
For some reason whoever designs woman clothes has decided women don't need pockets. Consequently, my wife can never find her phone!
This project provides a class library that will log into iCloud and perform a find my phone for a given devices name. Simply provide the service your iCloud username and password, and the name of the device (as shown in iCloud). 

##Known issues
* If you have multiple devices with the same name, only the first device I find will be located.
* You will get an e-mail every time you find the phone. I'm sure persisting a cookie will fix this, but for V1 this will do :)

##Project Structure
The code you probably want is located in [/FindWifesPhone.Library/FindPhoneService.cs](/FindWifesPhone.Library/FindPhoneService.cs). This contains all the code for communicating with iCloud

##Test windows application
I have included a test windows application here [/FindWifesPhone](/FindWifesPhone) to just ensure the code works correctly for you. Simply fill the form, click find and hopefully your phone will go crazy.

##Hosted windows service
I wanted to create a webserivce that I could post to from an arduino/using tasker on android whenever I wanted to find a phone. [This project](/FindWifePhoneHost) is a command prompt application which can easily be installed as a service.

Simply configure the app.config file and set the BaseUrl to an publically accessible url, then configure the username, password and the default device and run the exe. You can now make a post to http://url/findphone and a find my phone request will be made to the default configued device (you can append a device name after the find phone to search for a different device e.g. http://url/findphone/Ross's iPhone).

If everything is working you can install the command prompt as a service by simply running the exe with the paramaters "install start" after it. Use the help argument to see the list of options available.
