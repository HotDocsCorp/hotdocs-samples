
Setting Up
----------------------------
1. Open up the Web.config file in the project and look at the appSettings section
1. Set the AppAddress setting to the address this example will be running on (example: http://localhost:52948) no '/' at the end
2. Set the HostAddress setting  to the address of the web api controller (example: http://localhost:52948/api/hdcs)
3. Set the PackageId setting  to be the unique guid assigned to the package. (This should be known when the package is uploaded)
4. Optionally set the AnswerFileLocation to the phsyical path of an answerfile
5. Set the DocumentOutputLocation to the location you want the documents to be output, you also have to specify the name of the file with no extension(example:C:\user\bob\thedocument)


Running The Application
----------------------------
1. When you run the application in visual studio the first page you should be taken to is the interview
2. The interview should be built from all the details passed into the interview settings (line 36 in the HomeController.cs)
3. The settings and template are then passed to the new service we have created in the SDK.
4. The interview should now appear in your browser of choice.
5. After filling in the interview click finish the page should refresh and your documents should be written to the DocumentOutputLocation