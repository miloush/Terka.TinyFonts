This sub-repository is part of the **TERKA** project providing text input & advanced text rendering for .NET Micro Framework.

# TinyFont Tools

[![Apache 2.0](https://img.shields.io/badge/license-Apache%202.0-green.svg)](LICENSE)

There are two ways how to generate tiny fonts in .NET Micro Framework. Using the _TFConvert_ console application, and using the new _TinyFont Builder_ library.

#### 3.4.1.	Using TFConvert
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

The TFConvert binary is located in the `Release` directory; source code is available in `Source\CLR\Tools\TinyFonts`.

### 3.4.2.	Using TinyFont Builder
The TFConvert calls the TinyFont Builder library to do the core work. Users are welcome to call it directly, should they need any advanced features or custom modifications to the generated font.
```C#
string fonts = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
GlyphTypeface arial = new GlyphTypeface(new Uri(Path.Combine(fonts, "arial.ttf")));

TinyFontBuilder builder = new TinyFontBuilder(arial, 15);
builder.OpenTypeCompiler = new OpenTypeCompiler();
builder.ImportFeature("dflt", "latn", "smcp");

TinyFont font = builder.Build("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
font.Save("arial.tinyfnt");
```
_Listing 2 Sample font builder usage, letters A-z, small capitals_

The code equivalent to Listing 1 using the library directly is shown in Listing 2. Note that users need to resolve the input font file themselves and provide a feature compiler. The **TERKA** compiler is available in a separate assembly. All binaries are located in the `Release` directory; source code in `Source\CLR\Tools\TinyFonts`.

### 3.4.3.	Using the generated file

The generated file can be used in .NET Micro Framework as a resource file.

1.	Create a new Micro Framework Window Application.
2.	Open `Resources.resx`.
3.	Use _Add Existing File_ command under _Add Resource_.
4.	Locate the `arial.tinyfnt` generated above and add it.
5.	Open the `Program.cs` and update
```C#
text.Font = Resources.GetFont(Resources.FontResources.small);
```
to
```C#
text.Font = Resources.GetFont(Resources.FontResources.arial);
```
6.	To see the imported feature, add the following line:
```C#
text.TextFeatures = new[] { TextFeature.SmallCapitals };
```
7.	Run the project.  
