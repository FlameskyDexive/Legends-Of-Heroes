# Unity simple CSV/object serialiser

## Purpose

I find it useful to be able to expose game parameters and other tweakables via
CSV when prototyping so that non-coders can easily open them in a spreadsheet
tool to experiment with game design without having to use Unity.

I also like to serialize values in a type safe manner, directly from/to my
objects; much like `JsonUtility` works but with friendlier CSV (and no nested
objects).

This class works with public fields on C# objects, and definitely works with
fields which are of type `string`, `int`, `float`, `double`, and `enum` (value
names are used in the CSV). Other types will also work so long as they have
a `TypeConverter` available for parsing and implement `ToString`.

## How to use

Let's say we have an object with public fields, e.g.

```c#
public class MyObject {
    public string Name;
    public int Level;
    public float Dps;
    public enum Colour {
        Red = 1,
        Green = 2,
        Blue = 3,
        Black = 4,
        Purple = 15
    }
    public Colour ShirtColour;
}
```

### Single instance per file

If you want to store a single instance of an object in a file, for example game
settings which change difficulty etc, then fields of an object are stored
per line.

#### Saving:
```c#
var obj = new MyObject("Steve", 20, 1002.5f, MyObject.Colour.Red);
Sinbad.CsvUtil.SaveObject(obj, "filename.csv");
```

This will create a CSV file which looks like this:
```
Name,Steve
Level,20
Dps,1002.5
ShirtColour,Red
```

Notice that for a single instance this defaults to one field per line which is
convenient for editing.

#### Loading:

You could load that same CSV file back into an object instance:

```c#
var obj = new MyObject();
Sinbad.CsvUtil.LoadObject("filename.csv", obj);
```

The CSV file can include additional columns if you want, for example if you have
a notes column to explain what a setting does. `CsvUtil` will ignore any column
after the first 2 if you want to do this.

Also, if you want to include a header row in the CSV you can, so long as the
row is prefixed with `#`. E.g. an alternative CSV for the above could be:

```
#Field,#Value,#Notes
Name,Steve,The character name
Level,20,The level of the character
Dps,1002.5,Damager per second
ShirtColour,Red,Colour of the fabric covering the torso
```

This would load back in exactly the same way but would be more descriptive for
someone editing the CSV in Excel for example.

### Multiple instances per file

If you want to store many instances of an object in one file, for example
weapon tables, you can do that too; in this case there is one instance per
line and a header row indicates the field names.

#### Saving:

```c#
var objs = new List<MyObject>() {
    new MyObject("Steve", 20, 1002.5f, MyObject.Colour.Red);
    new MyObject("Batman", 12, 600.6f, MyObject.Colour.Black);
    new MyObject("Peewee Herman", 1, -2f, MyObject.Colour.Purple),
};
Sinbad.CsvUtil.SaveObjects(objs, "filename.csv);
```

The CSV created from this would be:
```
Name,Level,Dps,ShirtColour
Steve,20,1002.5,Red
Batman,12,600.6,Black
Peewee Herman,1,-2,Purple
```

#### Loading:

```c#
var objs = Sinbad.CsvUtil.LoadObjects<MyObject>("filename.csv")
```

Again if you want to you can include additional columns in the CSV which are
ignored during import, if you wanted to add comments. Simply prefix the header
of the column to be ignored with `#` and that column won't be imported, e.g.

```
Name,Level,Dps,ShirtColour,#Notes
Steve,20,1002.5,Red,Probably OP
Batman,12,600.6,Black,Who are you
Peewee Herman,1,-2,Purple,wat
```

In this case the last column will be ignored when loading that CSV.

### Embedded commas

If there are any commas embedded in the strings then they're quoted before being
written, and those quotes are dealt with on import too.

## Tests

Tests are included in this library and if you're using a recent version of
Unity they'll show up in the Editor Test Runner.

## License (MIT)

Copyright (c) 2017 Steve Streeting

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Acknowledgements

The regex for CSV splitting comes from [CSVReader](http://wiki.unity3d.com/index.php?title=CSVReader)
on the Unity wiki.

