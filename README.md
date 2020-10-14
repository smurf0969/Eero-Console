# Eero-Console
.Net Core Console Application for obtaining Eero network details via their API

This application is based on the source code from [Eero Client](https://github.com/343max/eero-client) (#unofficial barebone client lib for eero router (https://eero.com)) and ported to a C# .Net Core Application to more suit my needs.  
The main focus for me was to access specific devices i.e. Mobile Phones on the Eero Wifi Netaork so as to use the data as a primative Presence Detection system to let my home automation system to be able to parse a simple json text file to asscertain who was at home.  
  
  ## How to use  
  Edit the appsettings.json then run the app.  
  You will be prompted to enter the Validation code sent to your phone or email account.
  The tokens/cookies are saved to file in same directory as the program is running from. (You may want to add some encryption code to want extra piece of mind.)
  
  ### Thanks  
  Many thanks to [Max von Webel](https://github.com/343max) who's work this is based upon.
