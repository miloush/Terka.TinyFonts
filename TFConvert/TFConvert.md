TFConvert is the standard way users are used to make fonts on .NET Micro Framework. This application was completely rewritten in managed code for the **TERKA** project. The TFConvert has two parameters, input file and output file.
```
    TFConvert <input file> <output file>
      <input file>  = Font definition file (.fntdef)
      <output file> = Font output file (.tinyfnt)
```

Users need to create a _font definition_ file specifying what should be generated into the font, a plain text file with commands. Listing 1 shows a sample definition importing capital Latin letters and _Small Capitals_ feature. For the complete commands reference, see the user documentation for TFConvert.

```Perl
SelectFont "FN:Arial,HE:12"
#import A-Z
ImportRange 65 90
#import a-z
ImportRange 97 122
#import Small Capitals feature
ImportFeature latn dflt smcp
```
_Listing 1 Sample font definition, letters A-z, small capitals_
