Talk2M SDK - 1.2.0.21508
------------------------

Welcome to the Talk2M SDK.

This SDK consists in:

- The M2Web API
- The DataMailbox API (DMWeb)

Talk2M Developer ID
-------------------

All users of Talk2M APIs must register to obtain their own Talk2M Developer ID.
Your Talk2M developer ID can be obtained here:
https://developer.ewon.biz/registration 

M2Web API Section
-----------------

- The M2Web API Reference Guide.

- The M2Web Library source code (including some sample code)
- Experimental M2Web Library PHP code (Not supported. Use with care!)

DataMailbox API Section
-----------------------

- The DataMailbox Reference Guide. 

- The Talk2M Developer ID Request Form. 
  Please fill it and send it back to your eWON distributor to request a 
  Talk2M Developer ID.

- The Talk2M DataMailbox Viewer
  Talk2M DataMailbox Viewer is a software that offers the possibility to 
  generate the needed URL to retrieve the different values of the API requests.
  Instead of writing manually each parameter of the URL, this software helps 
  you by letting you select the request you want to send to the Data Mailbox 
  and shows immediately its result.
  It is useful to check if the request will succeed or not, which information 
  you want to retrieve but most certainly how to write the URL.
  To use the Viewer software, simply double-click on the “DMBoxViewer.exe” 
  file you received when you downloaded the Talk2M DataMailbox SDK.
  
- Samples applications with source code:

 - My Little Historian
   Sample C# console application that retrieves the contents of the DataMailbox
   using the DMWeb syncdata mechanism, outputs the contents in text files (one
   sub-directory per eWON, one text file per tag) and deletes the contents of
   the DataMailbox.
   This sample program shows how to download data using compressed HTTPS in 
   C#, turns it into a dynamic .NET object, browses through its contents and 
   uses the DMWeb transaction mechanism.
   Requirement: Visual Studio 2010+ / .NET 4.0+ / C#
   
 - Talk2M Datamailbox Viewer
   This is the source code of the tool application found in this SDK.
   It demonstrates the creation of a DMWeb URL, including the various options
   of each API call and the download of the DataMailbox contents.
   Requirement: Visual Studio 2010+ / .NET 4.0+ / C# 
  