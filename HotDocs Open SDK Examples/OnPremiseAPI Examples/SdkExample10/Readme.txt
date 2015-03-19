This will test assembling a document. The assembled document (output.docx) will be found in the bin/Debug folder and should contain the text ‘Hello World’.
To test the on premise web API replace the hardcoded host address passed into the OnPremiseServices class at line 21 with your relevant address.
In order for this to be successful the package must exist in the TempFiles folder in the on premise web API solution. 
It will be a package named ‘HelloWorld.hdpkg’ within a folder named by the package id ‘7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2’ unless
you alter these hardcoded values in the GetTemplate() method.
